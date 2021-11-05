$(document).ready(() => {
    const videoList = document.querySelector('.video-list ul');

    navigator.mediaDevices.getUserMedia({
        video: true,
        audio: true
    }).then(stream => {
        const li = document.createElement('li');
        li.className = 'video-item';

        const div = document.createElement('div');
        div.className = 'video-box';

        const video = document.createElement('video');

        video.muted = true;
        video.srcObject = stream;
        video.addEventListener('loadedmetadata', () => video.play());

        div.append(video);
        li.append(div);

        videoList.append(li);
    });
});