$(document).ready(() => {
    addThemeStylesheet();

    require.config({
        paths: { vs: '/lib/monaco-editor/min/vs' },
    });

    const requirePaths = ['vs/editor/editor.main'];

    require(requirePaths, createEditor);

    async function createEditor() {
        const editorDiv = document.querySelector('#code-editor');

        const monacoEditor = monaco.editor;
        //const monacoLanguages = monaco.languages;

        const codeEditor = monacoEditor.create(editorDiv, getEditorOptions());
        codeEditor.focus();

        await configureEditorSettings(monacoEditor, codeEditor);

        bindEditorContentChangeEvent(codeEditor);
    }

    function getEditorOptions() {
        return {
            language: 'plaintext',
            value: 'select a language of your choice from settings and start coding... 🙂',
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

    function bindEditorContentChangeEvent(codeEditor) {
        codeEditor.onKeyUp(async () => {
            const editorContent = codeEditor.getValue();
            await hubConnection.invoke('SendEditorContent', editorContent, sessionKey);
        });

        hubConnection.on('ReceiveEditorContent', editorContent => codeEditor.setValue(editorContent));
    }
});