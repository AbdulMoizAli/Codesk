$(document).ready(() => {
    const videoList = document.querySelector('.video-list ul');
    const peers = {};
    const peer = new Peer();

    navigator.mediaDevices.getUserMedia({
        video: true,
        audio: true
    }).then(stream => {
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

        div.append(video);
        li.append(div);
        videoList.append(li);
    }
});