$(document).ready(() => {
    (async () => {
        $('#video-panel').sidenav({ edge: 'right' });

        window.addEventListener('beforeunload', e => {
            e.preventDefault();
            e.returnValue = '';
        });

        $('#copy-session-key').click(() => {
            navigator.clipboard.writeText($('#session-key-chip').text());
            M.toast({ html: 'Copied to clipboard' });
        });

        $('#participant-search').keyup(function () {
            const query = $(this).val().trim().toLowerCase();

            if (!query) {
                showUsers(sessionUsers);
                return;
            }

            const filteredUsers = sessionUsers.filter(user => user.userName.toLowerCase().includes(query));

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
                markup += `<li class="participant"><a class="blue-text text-darken-4" data-userid="${user.userId}">${user.userName}</a></li>`;
            }

            $list.html(markup);
        }

        function addUser(user) {
            sessionUsers.push(user);
            $('.participant-list ul').append(`<li class="participant"><a class="blue-text text-darken-4" data-userid="${user.userId}">${user.userName}</a></li>`);
        }

        function removeUser(user) {
            sessionUsers = sessionUsers.filter(x => x.userId !== user.userId);
            $('.participant-list ul').find(`li a[data-userid="${user.userId}"]`).parent().remove();
        }

        hubConnection.on('ReceiveNewSessionInfo', (user, _sessionKey) => {
            sessionKey = _sessionKey;
            addUser(user);

            $('#session-key-chip').text(_sessionKey);
        });

        hubConnection.on('ReceiveJoinSessionInfo', users => {
            sessionUsers = users;
            showUsers(users);
        });

        hubConnection.on('AddUser', user => {
            addUser(user);
        });

        hubConnection.on('RemoveUser', user => {
            removeUser(user);
        });

        hubConnection.on('NotifyUser', notification => {
            M.toast({ html: notification });
        });

        await hubConnection.start();

        if ($('#session-type').val() === 'new') {
            await hubConnection.invoke('CreateSession');
        }
        else if ($('#session-type').val() === 'join') {
            const userName = $('#session-username').val();
            const _sessionKey = $('#session-key').val();

            sessionKey = _sessionKey;
            $('#session-key-chip').text(_sessionKey);

            await hubConnection.invoke('JoinSession', userName, _sessionKey);
        }
    })();
});