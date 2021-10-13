$(document).ready(() => {
    (async () => {
        $('#video-panel').sidenav({ edge: 'right' });

        const sessionUsers = [];
        let sessionKey = '';

        $('#participant-search').keyup(function () {
            const query = $(this).val().trim().toLowerCase();

            if (!query) {
                showUsers(sessionUsers);
                return;
            }

            const filteredUsers = sessionUsers.filter(name => name.toLowerCase().includes(query));

            showUsers(filteredUsers);
        });

        function showUsers(users) {
            const $list = $('.participant-list ul');

            if (users.length === 0) {
                $list.html('<li class="participant center"><a class="blue-grey-text">No Participant</a></li>');
                return;
            }

            let markup = '';

            for (const user of users) {
                markup += `<li class="participant"><a class="blue-text">${user}</a></li>`;
            }

            $list.html(markup);
        }

        function addUser(userName) {
            sessionUsers.push(userName);
            $('.participant-list ul').append(`<li class="participant"><a class="blue-text">${userName}</a></li>`);
        }

        const connection = new signalR.HubConnectionBuilder().withUrl('/sessionHub').build();

        connection.on('ReceiveNewSessionInfo', (userName, _sessionKey) => {
            sessionKey = _sessionKey;
            addUser(userName);

            $('#session-key-chip').text(_sessionKey);
        });

        connection.on('ReceiveJoinSessionInfo', userNames => {
            sessionUsers = userNames;
            showUsers(userNames);
        });

        connection.on('AddNewUserName', userName => {
            addUser(userName);
        });

        connection.on('NotifyUser', notification => {
            M.toast({ html: notification })
        });

        await connection.start();

        if ($('#session-type').attr('data-type') === 'new') {
            await connection.invoke('CreateSession');
        }
        else if ($('#session-type').attr('data-type') === 'join') {
            const userName = $('#session-username').val();
            const _sessionKey = $('#session-key').val();

            sessionKey = _sessionKey;
            $('#session-key-chip').text(_sessionKey);

            await connection.invoke('JoinSession', userName, _sessionKey);
        }
    })();
});