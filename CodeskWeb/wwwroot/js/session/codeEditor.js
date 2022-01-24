import getPythonProposals from '../../js/session/pythonAutoComplete.js';

let sessionCurrentFile = undefined;

$('#file-title').hide();

$(document).ready(() => {
    $.LoadingOverlay('show');

    addThemeStylesheet();

    require.config({
        paths: { vs: '/lib/monaco-editor/min/vs' },
    });

    const requirePaths = ['vs/editor/editor.main'];

    require(requirePaths, createEditor);

    function getEditorOptions() {
        return {
            language: 'plaintext',
            value: $('#session-type').val() === 'new' ? 'select a language of your choice from settings and start coding... 🙂' : $('#editor-content').text(),
            scrollBeyondLastLine: false,
            theme: $('#theme-select').val(),
            cursorStyle: $('#cursor-select').val(),
            cursorBlinking: $('#blinking-select').val(),
            fontWeight: $('#font-weight-select').val(),
            fontSize: $('#font-size-input').val(),
            tabSize: $('#tab-size-input').val(),
            lineNumbers: $('#line-numbers-switch').is(':checked') ? 'on' : 'off',
            wordWrap: $('#word-wrap-switch').is(':checked') ? 'on' : 'off',
            readOnly: $('#session-type').val() === 'join' ? true : false
        };
    }

    async function createEditor() {
        const editorDiv = document.querySelector('#code-editor');

        const monacoEditor = monaco.editor;
        const monacoLanguages = monaco.languages;

        const codeEditor = monacoEditor.create(editorDiv, getEditorOptions());
        codeEditor.focus();

        await configureEditorSettings(monacoEditor, codeEditor);

        configureEditorSettingsReset();

        configureLanguageAutoComplete(monacoLanguages, 'python', getPythonProposals);

        bindEditorContentChangeEvent(codeEditor);

        configureAccessWrite(codeEditor);

        $.LoadingOverlay('hide');
    }

    async function configureEditorLanguages(monacoEditor, codeEditor) {
        const response = await fetch('/assets/session/languages/languages.json');
        const data = await response.json();

        $('#language-input').autocomplete({
            data,
            limit: 6,
            onAutocomplete: async value => {
                if (value === codeEditor.getModel().getLanguageIdentifier().language)
                    return;

                monacoEditor.setModelLanguage(codeEditor.getModel(), value)

                if ($('#session-type').val() !== 'new' || !(['csharp', 'cpp', 'python'].includes(value))) {
                    $('#file-title').hide();
                    return;
                }

                $.LoadingOverlay('show');

                sessionCurrentFile = await hubConnection.invoke('CreateSessionFile', value, sessionKey);

                $('#file-title')
                    .val(sessionCurrentFile.FileTitle)
                    .width($('#file-title-placeholder')
                        .text(sessionCurrentFile.FileTitle)
                        .width())
                    .show();

                const url = `/WorkSpace/SessionFile/GetFileContent?filePath=${sessionCurrentFile.FilePath}`;
                const response = await fetch(url, { method: 'POST' });

                if (response.status !== 200)
                    showAlert('Error', 'something went wrong while fetching the file content', true, 'OK');

                const data = await response.text();

                codeEditor.setValue(data);

                $.LoadingOverlay('hide');
            }
        });
    }

    async function saveUserEditorSetting(settingId, settingValue) {
        if ($('#user-authenticated').val() === 'no')
            return;

        const url = `/WorkSpace/Editor/SaveUserEditorSetting?settingId=${settingId}&settingValue=${settingValue}`;
        const response = await fetch(url, { method: 'POST' });

        if (response.status !== 200)
            showAlert('Error', 'something went wrong', true, 'OK');
    }

    function addThemeStylesheet() {
        $('#editor-theme').remove();

        const stylesheetPath = $('#theme-select').find(':selected').attr('data-stylesheet-path');

        if (stylesheetPath === '#')
            return;

        const cssLink = $('<link>').attr('rel', 'stylesheet').attr('href', stylesheetPath).attr('id', 'editor-theme');
        $('head').append(cssLink);
    }

    async function configureEditorSettings(monacoEditor, codeEditor) {
        await configureEditorLanguages(monacoEditor, codeEditor);

        $('#theme-select').change(async function () {
            addThemeStylesheet();
            codeEditor.updateOptions({ theme: $(this).val() });

            await saveUserEditorSetting($(this).attr('data-setting-id'), $(this).val());
        });

        $('#cursor-select').change(async function () {
            codeEditor.updateOptions({ cursorStyle: $(this).val() });

            await saveUserEditorSetting($(this).attr('data-setting-id'), $(this).val());
        });

        $('#blinking-select').change(async function () {
            codeEditor.updateOptions({ cursorBlinking: $(this).val() });

            await saveUserEditorSetting($(this).attr('data-setting-id'), $(this).val());
        });

        $('#font-weight-select').change(async function () {
            codeEditor.updateOptions({ fontWeight: $(this).val() });

            await saveUserEditorSetting($(this).attr('data-setting-id'), $(this).val());
        });

        $('#font-size-input').change(async function () {
            codeEditor.updateOptions({ fontSize: $(this).val() });

            await saveUserEditorSetting($(this).attr('data-setting-id'), $(this).val());
        });

        $('#tab-size-input').change(async function () {
            codeEditor.updateOptions({ tabSize: $(this).val() });

            await saveUserEditorSetting($(this).attr('data-setting-id'), $(this).val());
        });

        $('#line-numbers-switch').change(async function () {
            const value = $(this).is(':checked') ? 'on' : 'off';
            codeEditor.updateOptions({ lineNumbers: value });

            await saveUserEditorSetting($(this).attr('data-setting-id'), value);
        });

        $('#word-wrap-switch').change(async function () {
            const value = $(this).is(':checked') ? 'on' : 'off';
            codeEditor.updateOptions({ wordWrap: value });

            await saveUserEditorSetting($(this).attr('data-setting-id'), value);
        });
    }

    function configureEditorSettingsReset() {
        const $userAuthenticated = $('#user-authenticated');

        const $themeSelect = $('#theme-select');
        const $cursorSelect = $('#cursor-select');
        const $blinkingSelect = $('#blinking-select');
        const $fontWeightSelect = $('#font-weight-select');
        const $fontSizeInput = $('#font-size-input');
        const $tabSizeInput = $('#tab-size-input');
        const $lineNumberSwitch = $('#line-numbers-switch');
        const $wordWrapSwitch = $('#word-wrap-switch');

        $('#settings-reset-btn').click(async () => {
            let flag = false;

            if ($userAuthenticated.val() === 'yes') {
                $userAuthenticated.val('no');
                flag = true;
            }

            $themeSelect.val('vs').change();
            $cursorSelect.val('line').change();
            $blinkingSelect.val('blink').change();
            $fontWeightSelect.val('400').change();
            $fontSizeInput.val(null).change();
            $tabSizeInput.val(null).change();
            $lineNumberSwitch.prop('checked', true).change();
            $wordWrapSwitch.prop('checked', false).change();

            $('select').formSelect();

            if (flag) {
                $userAuthenticated.val('yes')

                const url = '/WorkSpace/Editor/ResetEditorSettings';
                const response = await fetch(url, { method: 'POST' });

                if (response.status !== 200)
                    showAlert('Error', 'something went wrong', true, 'OK');
            }
        });
    }

    function configureLanguageAutoComplete(monacoLanguages, language, getProposals) {
        monacoLanguages.registerCompletionItemProvider(language, {
            provideCompletionItems: () => {
                return {
                    suggestions: getProposals(monacoLanguages)
                };
            }
        });
    }

    let typing = false;
    let timeout = undefined;
    let fileSaveTimeout = undefined;

    async function timeoutFunction() {
        typing = false;
        await hubConnection.invoke('StoppedTyping', sessionKey);
    }

    async function configTypingIndication() {
        if (typing == false) {
            typing = true
            await hubConnection.invoke('StartedTyping', sessionKey);
            timeout = setTimeout(timeoutFunction, 1000);
        } else {
            clearTimeout(timeout);
            timeout = setTimeout(timeoutFunction, 1000);
        }
    }

    function configSessionFileUpdate(codeEditor) {
        if (!sessionCurrentFile) return;

        if (fileSaveTimeout) clearTimeout(fileSaveTimeout);

        fileSaveTimeout = setTimeout(async () => {
            const url = `/WorkSpace/SessionFile/UpdateFileContent?filePath=${sessionCurrentFile.FilePath}&sessionKey=${sessionKey}`;
            const response = await fetch(url,
                {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(codeEditor.getValue())
                });

            if (response.status !== 200)
                showAlert('Error', 'something went wrong while saving the file', true, 'OK');
        }, 3000);
    }

    async function broadcastEditorContent(data, codeEditor) {
        const change = data.changes[0];

        const editorContent = JSON.stringify({
            range: change.range,
            text: change.text
        });

        await hubConnection.invoke('SendEditorContent', editorContent, sessionKey);

        configTypingIndication();

        configSessionFileUpdate(codeEditor);
    }

    function bindEditorContentChangeEvent(codeEditor) {
        let disposable = codeEditor.onDidChangeModelContent(data => broadcastEditorContent(data, codeEditor));

        hubConnection.on('ReceiveEditorContent', editorContent => {
            const data = JSON.parse(editorContent);

            disposable.dispose();

            const isReadOnly = codeEditor.getOption(80);

            if (isReadOnly)
                codeEditor.updateOptions({ readOnly: false });

            codeEditor.executeEdits(undefined, [{
                forceMoveMarkers: true,
                range: data.range,
                text: data.text
            }]);

            disposable = codeEditor.onDidChangeModelContent(data => broadcastEditorContent(data, codeEditor));

            if (isReadOnly)
                codeEditor.updateOptions({ readOnly: true });

            configSessionFileUpdate(codeEditor);
        });

        hubConnection.on('StartTypingIndication', userId => {
            $('.participant-list ul').find(`li a[data-userid="${userId}"]`)
                .prepend('<span class="new badge green lighten-1 pulse" data-badge-caption="Typing..."></span>')
                .parent().exchangePositionWith('.participant-list ul li:eq(1)');
        });

        hubConnection.on('StopTypingIndication', userId => {
            $('.participant-list ul').find(`li a[data-userid="${userId}"]`).find('span.badge').remove();
        });
    }

    function configureAccessWrite(codeEditor) {
        hubConnection.on('ToggleEditorReadOnly', isReadOnly => codeEditor.updateOptions({ readOnly: isReadOnly }));

        const messageContribution = codeEditor.getContribution('editor.contrib.messageController');
        codeEditor.onDidAttemptReadOnlyEdit(() => {
            messageContribution.showMessage('Please request write access from the session host.', codeEditor.getPosition());
            //messageContribution.dispose();
        });
    }

    $('#file-title')
        .on('input', function () {
            let rectifiedName = '';

            for (const char of $(this).val()) {
                if (!((/[a-zA-Z]/).test(char) || !isNaN(char) || [' ', '(', ')', '_', '-', ',', '.'].includes(char)))
                    continue;

                rectifiedName += char;
            }

            $(this).val(rectifiedName);

            $(this).width($('#file-title-placeholder').text($(this).val()).width());
        })
        .change(async function () {
            if (!sessionCurrentFile)
                return;

            if (!$(this).val()) {
                $(this).width($('#file-title-placeholder').text("Untitled File").width());
                $(this).val('Untitled File');
            }

            sessionCurrentFile.FileTitle = $(this).val();

            const url = `/WorkSpace/SessionFile/UpdateFileTitle?fileId=${sessionCurrentFile.FileId}&fileTitle=${sessionCurrentFile.FileTitle}`;
            const response = await fetch(url, { method: 'POST' });

            if (response.status !== 200)
                showAlert('Error', 'something went wrong while renaming the file', true, 'OK');
        });
});