$(document).ready(() => {
    let isChatBox = false;
    let chatBox = null;

    $('#chat-box').click(() => {
        if (!isChatBox) {
            chatBox = new WinBox('Chat', {
                index: 999,
                class: ['no-max', 'no-full', 'no-resize'],
                root: document.body,
                background: "#5c6bc0",
                border: 4,
                x: 'center',
                y: 'center',
                width: 500,
                height: 600,
                mount: document.querySelector('#chat-box-markup').firstElementChild,
                onclose: () => {
                    isChatBox = false
                    chatBox = null;
                }
            });

            isChatBox = true;
            scrollBottom();
        }
        else {
            chatBox.minimize(false);
        }

        $('#chat-message-input').focus();
    });

    $('.chat-message form').submit(async function (e) {
        e.preventDefault();

        const $messageInput = $(this).find('#chat-message-input');
        const messageText = $messageInput.val().trim();

        if (!messageText)
            return;

        displayMessage('Me', messageText, 1);

        scrollBottom();

        $messageInput.val('');

        await hubConnection.invoke('SendMessage', messageText, sessionKey);
    });

    function getCurrentTime() {
        return new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
    }

    function scrollBottom() {
        const chatHistory = document.querySelector('.chat-history');
        chatHistory.scrollTop = chatHistory.scrollHeight;
    }

    function displayMessage(senderName, messageText, type = 0) {
        let first = '';
        let second = 'my-message';
        let third = '';

        if (type === 1) {
            first = 'right-align';
            second = 'other-message';
            third = 'right';
        }

        const markup = `
            <li class="clearfix">
                <div class="message-data ${first}">
                    <span id="sender-name">${senderName}</span>
                    <span class="message-data-time">${getCurrentTime()}</span>
                </div>
                <div class="message ${second} ${third}">${messageText}</div>
            </li>
        `;

        $('.chat-history ul').append(markup);
    }

    hubConnection.on('ReceiveMessage', (message, userName) => {
        displayMessage(userName, message);
    });
});