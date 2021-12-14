import getPythonProposals from '../../js/session/pythonAutoComplete.js';

$(document).ready(() => {
    addThemeStylesheet();

    require.config({
        paths: { vs: '/lib/monaco-editor/min/vs' },
    });

    const requirePaths = ['vs/editor/editor.main'];

    require(requirePaths, createEditor);

    function getEditorOptions(editorContent) {
        return {
            language: 'plaintext',
            value: editorContent ? editorContent : 'select a language of your choice from settings and start coding... 🙂',
            scrollBeyondLastLine: false,
            theme: $('#theme-select').val(),
            cursorStyle: $('#cursor-select').val(),
            cursorBlinking: $('#blinking-select').val(),
            fontWeight: $('#font-weight-select').val(),
            fontSize: $('#font-size-input').val(),
            tabSize: $('#tab-size-input').val(),
            lineNumbers: $('#line-numbers-switch').is(':checked') ? 'on' : 'off',
            wordWrap: $('#word-wrap-switch').is(':checked') ? 'on' : 'off'
        };
    }

    async function createEditor() {
        $.LoadingOverlay('show');

        const editorDiv = document.querySelector('#code-editor');

        const monacoEditor = monaco.editor;
        const monacoLanguages = monaco.languages;

        const editorContent = $('#editor-content').text();

        const codeEditor = monacoEditor.create(editorDiv, getEditorOptions(editorContent));
        codeEditor.focus();

        await configureEditorSettings(monacoEditor, codeEditor);

        configureLanguageAutoComplete(monacoLanguages, 'python', getPythonProposals);

        bindEditorContentChangeEvent(codeEditor);

        $.LoadingOverlay('hide');
    }

    async function configureEditorLanguages(monacoEditor, codeEditor) {
        const response = await fetch('/assets/session/languages/languages.json');
        const data = await response.json();

        $('#language-input').autocomplete({
            data,
            limit: 6,
            onAutocomplete: value => monacoEditor.setModelLanguage(codeEditor.getModel(), value)
        });
    }

    async function saveUserEditorSetting(settingId, settingValue) {
        if ($('#user-authenticated').val() === 'no')
            return;

        const url = `/WorkSpace/Session/SaveUserEditorSetting?settingId=${settingId}&settingValue=${settingValue}`;
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

    async function timeoutFunction() {
        typing = false;
        await hubConnection.invoke('StoppedTyping', sessionKey);
    }

    function bindEditorContentChangeEvent(codeEditor) {
        const ignoreKeys = [
            15, 16, 17, 18, 9, 2, 8, 4, 5, 6, 11, 12, 59, 60, 61, 62, 63, 64,
            65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 14, 13, 19
        ];

        codeEditor.onKeyUp(async e => {
            if (ignoreKeys.includes(e.keyCode))
                return;

            const editorContent = codeEditor.getValue();
            await hubConnection.invoke('SendEditorContent', editorContent, sessionKey);

            if (typing == false) {
                typing = true
                await hubConnection.invoke('StartedTyping', sessionKey);
                timeout = setTimeout(timeoutFunction, 1500);
            } else {
                clearTimeout(timeout);
                timeout = setTimeout(timeoutFunction, 1500);
            }
        });

        hubConnection.on('ReceiveEditorContent', editorContent => codeEditor.setValue(editorContent));

        hubConnection.on('StartTypingIndication', userId => {
            $('.participant-list ul').find(`li a[data-userid="${userId}"]`)
                .prepend('<span class="new badge green lighten-1 pulse" data-badge-caption="Typing..."></span>')
                .parent().exchangePositionWith('.participant-list ul li:eq(1)');
      
        });

        hubConnection.on('StopTypingIndication', userId => {
            $('.participant-list ul').find(`li a[data-userid="${userId}"]`).find('span.badge').remove();
        });
    }
});