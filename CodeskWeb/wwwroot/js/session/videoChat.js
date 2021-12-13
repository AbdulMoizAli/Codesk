﻿export default function initializeVideoChat() {
    const videoList = document.querySelector('.video-list ul');
    const peers = {};
    const peer = new Peer();

    let selfPeerId = undefined;

    navigator.mediaDevices.getUserMedia({
        video: true,
        audio: true
    }).then(async stream => {
        toggleVideoStream(stream);
        toggleAudioStream(stream);

        const { li, div, video } = getVideoElements();
        video.muted = true;

        addVideoStream(li, div, video, stream);

        peer.on('call', call => {
            call.answer(stream);
            const { li, div, video } = getVideoElements();
            call.on('stream', userStream => addVideoStream(li, div, video, userStream));
            call.on('close', () => video.parentElement.parentElement.remove());

            peer.on('connection', peerConnection => {
                peerConnection.on('data', userId => peers[userId] = call);
            });
        });

        hubConnection.on('ReceivePeerId', (peerId, userId) => connectToNewPeer(peerId, stream, userId));

        controlMediaStream(stream);

        if (selfPeerId)
            await hubConnection.invoke('SendPeerId', selfPeerId, sessionKey);
    });

    hubConnection.on('CloseVideoCall', userId => {
        if (peers[userId]) {
            peers[userId].close();
            delete peers[userId];
        }
    });

    peer.on('open', peerId => selfPeerId = peerId);

    function connectToNewPeer(peerId, stream, userId) {
        const call = peer.call(peerId, stream);

        const { li, div, video } = getVideoElements();

        call.on('stream', userStream => addVideoStream(li, div, video, userStream));
        call.on('close', () => video.parentElement.parentElement.remove());

        const peerConnection = peer.connect(peerId);

        peerConnection.on('open', () => {
            peerConnection.send(sessionUsers[0].userId);
        });

        peers[userId] = call;
    }

    function getVideoElements() {
        return {
            li: document.createElement('li'),
            div: document.createElement('div'),
            video: document.createElement('video')
        };
    }

    function addVideoStream(li, div, video, stream) {
        video.srcObject = stream;
        video.addEventListener('loadedmetadata', () => video.play());

        li.className = 'video-item';
        div.className = 'video-box';
        video.className = 'z-depth-2';
        video.setAttribute('oncontextmenu', 'return false;');

        div.append(video);
        li.append(div);
        videoList.append(li);
    }

    function toggleVideoStream(stream) {
        stream.getVideoTracks()[0].enabled = !(stream.getVideoTracks()[0].enabled);
    }

    function toggleAudioStream(stream) {
        stream.getAudioTracks()[0].enabled = !(stream.getAudioTracks()[0].enabled);
    }

    function controlMediaStream(stream) {
        $('#camera').click(function () {
            toggleVideoStream(stream);

            $(this).find('div#center-div').toggleClass('hide');

            const $icon = $(this).find('i');

            if ($icon.text() === 'videocam_off')
                $icon.text('videocam');
            else if ($icon.text() === 'videocam')
                $icon.text('videocam_off');
        });

        $('#microphone').click(function () {
            toggleAudioStream(stream);

            $(this).find('div#center-div').toggleClass('hide');

            const $icon = $(this).find('i');

            if ($icon.text() === 'mic_off')
                $icon.text('mic');
            else if ($icon.text() === 'mic')
                $icon.text('mic_off');
        });
    }
}