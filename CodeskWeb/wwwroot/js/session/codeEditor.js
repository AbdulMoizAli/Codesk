import getPythonProposals from '../../js/session/pythonAutoComplete.js';
import getCsharpProposals from '../../js/session/csharpAutoComplete.js';
import getCppProposals from '../../js/session/cppAutoComplete.js';

let sessionCurrentFile = undefined;
let codingMode = 'public';
let hasWriteAccess = $('#session-type').val() === 'new' ? true : false;

$('#file-title').hide();

$(document).ready(() => {
    $.LoadingOverlay('show');

    addThemeStylesheet();

    require.config({
        paths: { vs: '/lib/monaco-editor/min/vs' },
    });

    const requirePaths = ['vs/editor/editor.main'];

    require(requirePaths, initialize);

    function getEditorOptions() {
        return {
            language: 'plaintext',
            value: $('#session-type').val() === 'new' ? 'select a language of your choice from settings and start coding... 💻' : $('#editor-content').text(),
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

    async function initialize() {
        const editorDiv = document.querySelector('#code-editor');

        const monacoEditor = monaco.editor;
        const monacoLanguages = monaco.languages;

        const codeEditor = monacoEditor.create(editorDiv, getEditorOptions());
        codeEditor.focus();

        await configureEditorSettings(monacoEditor, codeEditor);

        configureEditorSettingsReset();

        configureLanguageAutoComplete(monacoLanguages, 'python', getPythonProposals);
        configureLanguageAutoComplete(monacoLanguages, 'csharp', getCsharpProposals);
        configureLanguageAutoComplete(monacoLanguages, 'cpp', getCppProposals);

        bindEditorContentChangeEvent(codeEditor);

        configureAccessWrite(codeEditor);

        configureCodingMode(codeEditor);

        configureCodeExecution(codeEditor);

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

                $('#language-logo').attr('src', `/assets/session/languages/logos/${value}.svg`);

                monacoEditor.setModelLanguage(codeEditor.getModel(), value);

                if ($('#session-type').val() !== 'new' || !(['csharp', 'cpp', 'python'].includes(value))) {
                    $('#file-title').hide();
                    return;
                }

                $.LoadingOverlay('show');

                await hubConnection.invoke('SetCodingLanguage', sessionKey, value);

                const url = `/WorkSpace/SessionFile/CreateSessionFile?fileType=${value}&sessionKey=${sessionKey}&connectionId=${hubConnection.connection.connectionId}`;
                const response = await fetch(url, { method: 'POST' });

                if (response.status !== 200)
                    showAlert('Error', 'Something went wrong while creating the file', true, 'OK');
                else {
                    const data = await response.json();

                    sessionCurrentFile = data.sessionCurrentFile;

                    $('#file-title')
                    .val(sessionCurrentFile.fileTitle)
                    .width($('#file-title-placeholder')
                        .text(sessionCurrentFile.fileTitle)
                        .width())
                        .show();

                    codeEditor.setValue(data.fileContent);
                }

                $.LoadingOverlay('hide');
            }
        });

        hubConnection.on('SetEditorLanguage', language => {
            $('#language-input').val(language);
            $('#language-logo').attr('src', `/assets/session/languages/logos/${language}.svg`);
            monacoEditor.setModelLanguage(codeEditor.getModel(), language);
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
            const url = `/WorkSpace/SessionFile/UpdateFileContent?filePath=${sessionCurrentFile.filePath}&sessionKey=${sessionKey}`;
            const response = await fetch(url, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(codeEditor.getValue())
            });

            if (response.status !== 200)
                showAlert('Error', 'something went wrong while saving the file', true, 'OK');
        }, 3000);
    }

    async function broadcastEditorContent(data, codeEditor) {
        if (codingMode === 'private') {
            configSessionFileUpdate(codeEditor);
            return;
        }

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
        hubConnection.on('ToggleEditorReadOnly', isReadOnly => {
            codeEditor.updateOptions({ readOnly: isReadOnly })
            hasWriteAccess = !isReadOnly;
        });

        const messageContribution = codeEditor.getContribution('editor.contrib.messageController');
        codeEditor.onDidAttemptReadOnlyEdit(() => {
            messageContribution.showMessage('Please request write access from the session host.', codeEditor.getPosition());
            //messageContribution.dispose();
        });
    }

    function configureCodingMode(codeEditor) {
        $('#coding-mode').click(async function () {
            const badge = $(this).children('span.badge');
            const currentMode = badge.attr('data-badge-caption');

            let mode = currentMode === 'Public' ? true : false;

            const response = await fetch(`/WorkSpace/Editor/SetCodingMode?mode=${mode}&sessionKey=${sessionKey}&userId=${sessionUsers[0].UserId}`);

            if (response.status !== 200) {
                showAlert('Error', 'something went wrong while processing the request', true, 'OK');
                return;
            }

            if (mode) {
                if (!hasWriteAccess)
                    codeEditor.updateOptions({ readOnly: false });

                codingMode = 'private';
                badge.attr('data-badge-caption', 'Private');
                $(this).children('i').text('lock');

                M.toast({ html: '<i class="material-icons left">lock</i> Private mode enabled', classes: 'rounded' });
            }
            else {
                // TODO: fetch latest content of the session's current file to show in the editor.
                if (!hasWriteAccess)
                    codeEditor.updateOptions({ readOnly: true });

                codingMode = 'public';
                badge.attr('data-badge-caption', 'Public');
                $(this).children('i').text('no_encryption');

                M.toast({ html: '<i class="material-icons left">no_encryption</i> Private mode disabled', classes: 'rounded' });
            }
        });
    }

    function configureCodeExecution(codeEditor) {
        let isOutputBox = false;
        let outputBox = null;

        $('#code-execute-btn').click(async function () {
            $(this)
                .addClass('btn-large')
                .attr('disabled', true)
                .find('i.material-icons')
                .addClass('hide')
                .next()
                .removeClass('hide');

            const language = codeEditor.getModel().getLanguageIdentifier().language;

            const response = await fetch(`/WorkSpace/CodeExecution/Execute?language=${language}`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(codeEditor.getValue())
            });

            if (response.status !== 200)
                showAlert('Error', 'something went wrong while proccessing the request', true, 'OK');
            else {
                const data = await response.json();

                $('#program-output').text(data.output);
                $('#cpu-time').text(data.cpuTime ? data.cpuTime : '0');
                $('#memory').text(data.memory ? (parseFloat(data.memory) / 1024).toFixed(2) : '0');

                if (!isOutputBox) {
                    outputBox = new WinBox('Output', {
                        index: 999,
                        root: document.body,
                        background: "#5c6bc0",
                        x: 'center',
                        y: 'center',
                        width: 800,
                        height: 400,
                        mount: document.querySelector('#output-box-markup').firstElementChild,
                        onclose: () => {
                            isOutputBox = false
                            outputBox = null;
                        }
                    });

                    isOutputBox = true;
                }
                else {
                    outputBox.minimize(false);
                }
            }

            $(this)
                .removeClass('btn-large')
                .attr('disabled', false)
                .find('i.material-icons')
                .removeClass('hide')
                .next()
                .addClass('hide');
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

            sessionCurrentFile.fileTitle = $(this).val();

            const url = `/WorkSpace/SessionFile/UpdateFileTitle?fileId=${sessionCurrentFile.fileId}&fileTitle=${sessionCurrentFile.fileTitle}`;
            const response = await fetch(url, { method: 'POST' });

            if (response.status !== 200)
                showAlert('Error', 'something went wrong while renaming the file', true, 'OK');
        });
});