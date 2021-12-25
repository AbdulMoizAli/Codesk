import initializeVideoChat from '../../js/session/videoChat.js';

$(document).ready(async () => {
    $.LoadingOverlay('show');

    await hubConnection.start();

    $('#session-time').attr('data-badge-caption', getCurrentDateTime());

    addEventListener('beforeunload', askUserBeforeUnload);

    $('#end-btn').click(async () => {
        $.LoadingOverlay('show');
        await hubConnection.invoke('EndSessionForAll', getCurrentDateTime(), sessionKey);
        leaveSession();
    });

    $('#leave-btn').click(() => {
        $.LoadingOverlay('show');
        leaveSession();
    });

    $('#copy-session-key').click(() => {
        navigator.clipboard.writeText($('#session-key-chip').text());
        M.toast({ html: '<i class="material-icons left">check</i> Copied to clipboard' });
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
            $list.html('<li class="participant center"><a class="blue-grey-text">NO PARTICIPANT</a></li>');
            return;
        }

        let markup = '';

        for (const user of users) {
            markup += `<li class="participant"><a class="blue-text text-darken-4" data-userid="${user.userId}">${user.userName.toUpperCase()}</a></li>`;
        }

        $list.html(markup);
    }

    function askUserBeforeUnload(e) {
        e.preventDefault();
        e.returnValue = '';
    }

    function leaveSession() {
        removeEventListener('beforeunload', askUserBeforeUnload);

        if ($('#user-authenticated').val() === 'yes')
            location.replace('/Home/Dashboard');
        else if ($('#user-authenticated').val() === 'no')
            location.replace('/');
    }

    function getCurrentDateTime() {
        return new Date().toLocaleDateString('en-US', {
            day: '2-digit',
            month: 'short',
            year: 'numeric',
            weekday: 'short',
            hour: '2-digit',
            minute: '2-digit',
        });
    }

    function addUser(user) {
        sessionUsers.push(user);
        $('.participant-list ul').append(`<li class="participant"><a class="blue-text text-darken-4" data-userid="${user.userId}">${user.userName.toUpperCase()}</a></li>`);
    }

    function removeUser(user) {
        sessionUsers = sessionUsers.filter(x => x.userId !== user.userId);
        $('.participant-list ul').find(`li a[data-userid="${user.userId}"]`).parent().remove();
    }

    function updateParticipantCount() {
        $('#no-of-participants').text(sessionUsers.length);
    }

    hubConnection.on('ReceiveNewSessionInfo', async (user, _sessionKey) => {
        sessionKey = _sessionKey;
        addUser(user);
        updateParticipantCount();

        $('#session-key-chip').text(_sessionKey);

        const url = `/WorkSpace/Session/SaveSession?startDateTime=${getCurrentDateTime()}&sessionKey=${_sessionKey}`;
        const response = await fetch(url, { method: 'POST' });

        if (response.status !== 200)
            showAlert('Error', 'something went wrong while creating the session', true, 'OK');
    });

    hubConnection.on('ReceiveJoinSessionInfo', async users => {
        sessionUsers = users;
        showUsers(users);
        updateParticipantCount();

        const url = `/WorkSpace/Session/SaveParticipant?userName=${$('#session-username').val()}&sessionKey=${$('#session-key').val()}`;
        const response = await fetch(url, { method: 'POST' });

        if (response.status !== 200)
            showAlert('Error', 'something went wrong while joining the session', true, 'OK');
    });

    hubConnection.on('AddUser', user => {
        addUser(user);
        updateParticipantCount();
    });

    hubConnection.on('RemoveUser', user => {
        removeUser(user);
        updateParticipantCount();
    });

    hubConnection.on('NotifyUser', notification => {
        M.toast({ html: notification });
    });

    hubConnection.on('EndSession', async () => {
        $.LoadingOverlay('show');
        await hubConnection.stop();
        leaveSession();
    });

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

    $.LoadingOverlay('hide');

    initializevideochat();
});