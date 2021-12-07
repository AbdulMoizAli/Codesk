export default function initializeVideoChat() {
    const videoList = document.querySelector('.video-list ul');
    const peers = {};
    const peer = new Peer(undefined, {
        config: {
            iceServers: [
                { url: 'stun:stun01.sipphone.com' },
                { url: 'stun:stun.ekiga.net' },
                { url: 'stun:stun.fwdnet.net' },
                { url: 'stun:stun.ideasip.com' },
                { url: 'stun:stun.iptel.org' },
                { url: 'stun:stun.rixtelecom.se' },
                { url: 'stun:stun.schlund.de' },
                { url: 'stun:stun.l.google.com:19302' },
                { url: 'stun:stun1.l.google.com:19302' },
                { url: 'stun:stun2.l.google.com:19302' },
                { url: 'stun:stun3.l.google.com:19302' },
                { url: 'stun:stun4.l.google.com:19302' },
                { url: 'stun:stunserver.org' },
                { url: 'stun:stun.softjoys.com' },
                { url: 'stun:stun.voiparound.com' },
                { url: 'stun:stun.voipbuster.com' },
                { url: 'stun:stun.voipstunt.com' },
                { url: 'stun:stun.voxgratia.org' },
                { url: 'stun:stun.xten.com' },
                {
                    url: 'turn:numb.viagenie.ca',
                    credential: 'muazkh',
                    username: 'webrtc@live.com'
                },
                {
                    url: 'turn:192.158.29.39:3478?transport=udp',
                    credential: 'JZEOEt2V3Qb0y27GRntt2u2PAYA=',
                    username: '28224511:1379330808'
                },
                {
                    url: 'turn:192.158.29.39:3478?transport=tcp',
                    credential: 'JZEOEt2V3Qb0y27GRntt2u2PAYA=',
                    username: '28224511:1379330808'
                }
            ]
        }
    });

    navigator.mediaDevices.getUserMedia({
        video: true,
        audio: true
    }).then(stream => {
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
    });

    hubConnection.on('CloseVideoCall', userId => {
        if (peers[userId]) {
            peers[userId].close();
            delete peers[userId];
        }
    });

    peer.on('open', async peerId => await hubConnection.invoke('SendPeerId', peerId, sessionKey));

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