import initializeVideoChat from '../../js/session/videoChat.js';

$(document).ready(async () => {
    $.LoadingOverlay('show');

    await hubConnection.start();

    $('#session-time').attr('data-badge-caption', getCurrentDateTime());

    configureAccessRight();

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
        M.toast({ html: '<i class="material-icons left">check</i> Copied to clipboard', classes: 'rounded' });
    });

    $('#participant-search').keyup(function () {
        const query = $(this).val().trim().toLowerCase();

        $('.participant').filter(function () {
            $(this).toggle($(this).first().text().toLowerCase().indexOf(query) > -1);
        });

        if (!$('.participant').is(':visible'))
            $('.participant-list ul').append('<li class="participant center"><a class="blue-grey-text">NO PARTICIPANT</a></li>');
        else
            $('.participant-list ul li:contains("NO PARTICIPANT")').remove();
    });

    function showUsers(users) {
        const $list = $('.participant-list ul');

        if (users.length === 0) {
            $list.html('<li class="participant center"><a class="blue-grey-text">NO PARTICIPANT</a></li>');
            return;
        }

        let markup = '';

        for (const user of users) {
            markup += `<li class="participant"><a class="blue-text text-darken-4" data-userid="${user.UserId}">${user.UserName.toUpperCase()}</a></li>`;
        }

        $list.html(markup);
    }

    function askUserBeforeUnload(e) {
        e.preventDefault();
        e.returnValue = '';
    }

    function configureAccessRight() {
        $('.participant-list').delegate('a[data-usertype="participant"]', 'click', async function () {
            const $icon = $(this).find('i');

            if ($icon.text() === 'speaker_notes')
                $icon.text('speaker_notes_off')
            else if ($icon.text() === 'speaker_notes_off')
                $icon.text('speaker_notes');

            const userId = $(this).data('userid');
            await hubConnection.invoke('ToggleWriteAccess', sessionKey, userId);
        });
    }

    function manageTasks() {
        $('#task-form').submit(async function (e) {
            e.preventDefault();

            const $taskName = $(this).find('#task-name');
            const $taskDescription = $(this).find('#task-description');

            const model = {
                taskName: $taskName.val(),
                taskDescription: $taskDescription.val()
            };

            let url = '';

            if ($(this).attr('data-optype') === 'create')
                url = `/WorkSpace/SessionTask/CreateTask?sessionKey=${sessionKey}`;
            else if ($(this).attr('data-optype') === 'update') {
                url = '/WorkSpace/SessionTask/UpdateTask';
                model['taskId'] = $(this).attr('data-taskid');
            }

            $.LoadingOverlay('show');

            const response = await fetch(url, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(model)
            });

            $.LoadingOverlay('hide');

            if (response.status !== 200) {
                showAlert('Error', 'something went wrong while creating the task', true, 'OK');
                return;
            }

            if ($(this).attr('data-optype') === 'create') {
                const data = await response.json();

                const row = `
                    <tr data-rowid="${data.taskId}">
                        <td>${$taskName.val()}</td>
                        <td>${$taskDescription.val()}</td>
                        <td>
                            <a style="cursor: pointer;" class="edit-task blue-text text-lighten-1 tooltipped" data-position="left" data-tooltip="Edit"><i class="material-icons">edit</i></a>
                            <a style="cursor: pointer;" class="delete-task red-text text-lighten-1 tooltipped" data-position="right" data-tooltip="Delete"><i class="material-icons">delete</i></a>
                        </td>
                    </tr>
                `;

                const $tasksTable = $('#tasks-table');

                if ($tasksTable.attr('data-isempty') === 'true') {
                    $tasksTable.find('tbody').html(row);
                    $tasksTable.attr('data-isempty', false);
                }
                else {
                    $tasksTable.find('tbody').append(row);
                }

                $('.tooltipped').tooltip();
            }
            else if ($(this).attr('data-optype') === 'update') {
                const $tr = $(`#tasks-table tbody tr[data-rowid="${$(this).attr('data-taskid')}"]`);

                $tr.children().eq(0).text($taskName.val());
                $tr.children().eq(1).text($taskDescription.val())

                $(this)
                    .attr('data-optype', 'create')
                    .removeAttr('data-taskid')
                    .find('button')
                    .attr('data-tooltip', 'Create')
                    .find('i').text('add');
            }

            $taskName.val('');
            $taskDescription.val('');

            M.textareaAutoResize($taskDescription);

            $taskName.next().removeClass('active');
            $taskDescription.next().removeClass('active');

        });

        $(document).on('click', '.delete-task', function () {
            const $tr = $(this).closest('tr');

            showActionAlert('Are you sure?', 'You won\'t be able to revert this!', false, 'Cancel', 'Delete', async () => {
                $.LoadingOverlay('show');

                const response = await fetch(`/WorkSpace/SessionTask/DeleteTask?taskId=${$tr.attr('data-rowid')}`, {
                    method: 'POST'
                });

                $.LoadingOverlay('hide');

                if (response.status !== 200) {
                    showAlert('Error', 'something went wrong while deleting the task', true, 'OK');
                    return;
                }

                $tr.remove();

                if ($('#tasks-table tbody tr').length === 0) {
                    const row = `
                        <tr>
                            <td></td>
                            <td class="center-align">There are currently no tasks available</td>
                            <td></td>
                        </tr>
                    `;

                    $('#tasks-table tbody').html(row);
                }
            });
        });

        $(document).on('click', '.edit-task', function () {
            const tr = $(this).closest('tr');

            const $taskName = $('#task-name');
            const $taskDescription = $('#task-description');

            $taskName.val(tr.children().eq(0).text());
            $taskDescription.val(tr.children().eq(1).text());

            M.textareaAutoResize($taskDescription);

            $taskName.next().addClass('active');
            $taskDescription.next().addClass('active');

            $('#task-form')
                .attr('data-optype', 'update')
                .attr('data-taskid', tr.attr('data-rowid'))
                .find('button')
                .attr('data-tooltip', 'Update')
                .find('i').text('edit');
        });
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

        let icon = '';
        let iconClass = '';
        let userType = ''

        if ($('#session-type').val() === 'new' && sessionUsers.length > 1) {
            icon = '<i class="material-icons">speaker_notes_off</i>';
            iconClass = 'waves-effect';
            userType = 'participant';
        }

        $('.participant-list ul').append(`<li class="participant"><a class="blue-text text-darken-4 ${iconClass}" data-userid="${user.UserId}" data-usertype="${userType}">${icon} ${user.UserName.toUpperCase()}</a></li>`);
    }

    function removeUser(user) {
        sessionUsers = sessionUsers.filter(x => x.UserId !== user.UserId);
        $('.participant-list ul').find(`li a[data-userid="${user.UserId}"]`).parent().remove();
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
        M.toast({ html: notification, classes: 'rounded' });
    });

    hubConnection.on('EndSession', async () => {
        $.LoadingOverlay('show');
        await hubConnection.stop();
        leaveSession();
    });

    hubConnection.on('UpdateUserId', (prevUserId, currentUserId) => {
        $('.participant-list ul')
            .find(`li a[data-userid="${prevUserId}"]`)
            .attr('data-userid', currentUserId);

        const index = sessionUsers.findIndex(user => user.UserId === prevUserId);

        if (index !== -1)
            sessionUsers[index].UserId = currentUserId;
    });

    if ($('#session-type').val() === 'new') {
        await hubConnection.invoke('CreateSession');
        manageTasks();
    }
    else if ($('#session-type').val() === 'join') {
        const userName = $('#session-username').val();
        const _sessionKey = $('#session-key').val();

        sessionKey = _sessionKey;
        $('#session-key-chip').text(_sessionKey);

        await hubConnection.invoke('JoinSession', userName, _sessionKey);
    }

    $.LoadingOverlay('hide');

    initializeVideoChat();
});