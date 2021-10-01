$(document).ready(() => {
    require.config({
        paths: { vs: '/lib/monaco-editor/min/vs' },
    });

    const requirePaths = ['vs/editor/editor.main'];

    require(requirePaths, createEditor);

    async function createEditor() {
        const editorDiv = document.querySelector('#code-editor');

        const monacoEditor = monaco.editor;
        //const monacoLanguages = monaco.languages;

        const codeEditor = monacoEditor.create(editorDiv, {
            value: 'select language of your choice from settings and start coding... 🙂',
            scrollBeyondLastLine: false,
        });

        codeEditor.focus();

        await configureEditorSettings(monacoEditor, codeEditor);
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

    async function configureEditorSettings(monacoEditor, codeEditor) {
        await configureEditorLanguages(monacoEditor, codeEditor);

        $('#theme-select').change(function () {
            codeEditor.updateOptions({ theme: $(this).val() });
        });

        $('#cursor-select').change(function () {
            codeEditor.updateOptions({ cursorStyle: $(this).val() });
        });

        $('#blinking-select').change(function () {
            codeEditor.updateOptions({ cursorBlinking: $(this).val() });
        });

        $('#font-weight-select').change(function () {
            codeEditor.updateOptions({ fontWeight: $(this).val() });
        });

        $('#font-size-input').change(function () {
            codeEditor.updateOptions({ fontSize: $(this).val() });
        });

        $('#tab-size-input').change(function () {
            codeEditor.updateOptions({ tabSize: $(this).val() });
        });

        $('#line-numbers-switch').change(function () {
            const value = $(this).is(':checked') ? 'on' : 'off';
            codeEditor.updateOptions({ lineNumbers: value });
        });

        $('#word-wrap-switch').change(function () {
            const value = $(this).is(':checked') ? 'on' : 'off';
            codeEditor.updateOptions({ wordWrap: value });
        });
    }
});