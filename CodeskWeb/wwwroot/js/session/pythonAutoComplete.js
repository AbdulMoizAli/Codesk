export default function getPythonProposals(monacoLanguages) {
    return [
        {
            label: 'print',
            kind: monacoLanguages.CompletionItemKind.Snippet,
            documentation: 'The print() function prints the specified message to the screen, or other standard output device',
            detail: 'Print Statement',
            insertText: 'print(${1:message})$0',
            insertTextRules: monacoLanguages.CompletionItemInsertTextRule.InsertAsSnippet
        },
        {
            label: 'def',
            kind: monacoLanguages.CompletionItemKind.Keyword,
            detail: 'def',
            insertText: 'def'
        },
        {
            label: 'def',
            kind: monacoLanguages.CompletionItemKind.Snippet,
            documentation: 'Defines a new function',
            detail: 'Function Statement',
            insertText: 'def ${1:name}(${2:params}):\n\t$0',
            insertTextRules: monacoLanguages.CompletionItemInsertTextRule.InsertAsSnippet
        },
        {
            label: 'class',
            kind: monacoLanguages.CompletionItemKind.Keyword,
            detail: 'class',
            insertText: 'class'
        },
        {
            label: 'class',
            kind: monacoLanguages.CompletionItemKind.Snippet,
            documentation: 'Defines a new class',
            detail: 'Class Statement',
            insertText: 'class ${1:name}:\n\t$0',
            insertTextRules: monacoLanguages.CompletionItemInsertTextRule.InsertAsSnippet
        },
        {
            label: 'for',
            kind: monacoLanguages.CompletionItemKind.Keyword,
            detail: 'for',
            insertText: 'for'
        },
        {
            label: 'for',
            kind: monacoLanguages.CompletionItemKind.Snippet,
            documentation: 'For loop is used for iterating over a sequence (a list, a tuple, a dictionary, a set, or a string)',
            detail: 'For Loop Statement',
            insertText: 'for ${1:item} in ${2:collection}:\n\t$0',
            insertTextRules: monacoLanguages.CompletionItemInsertTextRule.InsertAsSnippet
        },
        {
            label: 'while',
            kind: monacoLanguages.CompletionItemKind.Keyword,
            detail: 'while',
            insertText: 'while'
        },
        {
            label: 'while',
            kind: monacoLanguages.CompletionItemKind.Snippet,
            documentation: 'A while loop will continue until the statement is false',
            detail: 'While Loop Statement',
            insertText: 'while ${1:condition}:\n\t$0',
            insertTextRules: monacoLanguages.CompletionItemInsertTextRule.InsertAsSnippet
        },
        {
            label: 'try',
            kind: monacoLanguages.CompletionItemKind.Keyword,
            detail: 'try',
            insertText: 'try'
        },
        {
            label: 'except',
            kind: monacoLanguages.CompletionItemKind.Keyword,
            detail: 'except',
            insertText: 'except'
        },
        {
            label: 'finally',
            kind: monacoLanguages.CompletionItemKind.Keyword,
            detail: 'finally',
            insertText: 'finally'
        },
        {
            label: 'try/except',
            kind: monacoLanguages.CompletionItemKind.Snippet,
            documentation: 'Try a block of code, and decide what to to if it raises an error\nThe try keyword is used in try...except blocks. It defines a block of code test if it contains any errors\nYou can define different blocks for different error types, and blocks to execute if nothing went wrong',
            detail: 'Try/Except Statement',
            insertText: 'try:\n\t$1\nexcept:\n\t$2',
            insertTextRules: monacoLanguages.CompletionItemInsertTextRule.InsertAsSnippet
        },
        {
            label: 'try/except/finally',
            kind: monacoLanguages.CompletionItemKind.Snippet,
            documentation: 'Try a block of code, and decide what to to if it raises an error\nThe finally block will always be executed, no matter if the try block raises an error or not\nThe finally keyword is used in try...except blocks. It defines a block of code to run when the try...except...else block is final',
            detail: 'Try/Except/Finally Statement',
            insertText: 'try:\n\t$1\nexcept:\n\t$2\nfinally:\n\t$3',
            insertTextRules: monacoLanguages.CompletionItemInsertTextRule.InsertAsSnippet
        },
        {
            label: 'if',
            kind: monacoLanguages.CompletionItemKind.Keyword,
            detail: 'if',
            insertText: 'if'
        },
        {
            label: 'elif',
            kind: monacoLanguages.CompletionItemKind.Keyword,
            detail: 'elif',
            insertText: 'elif'
        },
        {
            label: 'else',
            kind: monacoLanguages.CompletionItemKind.Keyword,
            detail: 'else',
            insertText: 'else'
        },
        {
            label: 'if',
            kind: monacoLanguages.CompletionItemKind.Snippet,
            documentation: 'The if keyword is used to create conditional statements (if statements), and allows you to execute a block of code only if a condition is True',
            detail: 'IF Statement',
            insertText: 'if ${1:condition}:\n\t$0',
            insertTextRules: monacoLanguages.CompletionItemInsertTextRule.InsertAsSnippet
        },
        {
            label: 'elif',
            kind: monacoLanguages.CompletionItemKind.Snippet,
            documentation: 'The elif keyword is used in conditional statements (if statements), and is short for else if',
            detail: 'ELIF Statement',
            insertText: 'elif ${1:condition}:\n\t$0',
            insertTextRules: monacoLanguages.CompletionItemInsertTextRule.InsertAsSnippet
        },
        {
            label: 'else',
            kind: monacoLanguages.CompletionItemKind.Snippet,
            documentation: 'The else keyword is used in conditional statements (if statements), and decides what to do if the condition is False',
            detail: 'ELSE Statement',
            insertText: 'else:\n\t$1',
            insertTextRules: monacoLanguages.CompletionItemInsertTextRule.InsertAsSnippet
        },
        {
            label: 'if/else',
            kind: monacoLanguages.CompletionItemKind.Snippet,
            documentation: 'if/else block',
            detail: 'IF/ELSE Statement',
            insertText: 'if ${1:condition}:\n\t$2\nelse:\n\t$3',
            insertTextRules: monacoLanguages.CompletionItemInsertTextRule.InsertAsSnippet
        },
        {
            label: 'if/elif',
            kind: monacoLanguages.CompletionItemKind.Snippet,
            documentation: 'if/elif block',
            detail: 'IF/ELIF Statement',
            insertText: 'if ${1:condition1}:\n\t$2\nelif ${3:condition2}:\n\t$4',
            insertTextRules: monacoLanguages.CompletionItemInsertTextRule.InsertAsSnippet
        },
        {
            label: 'if/elif/else',
            kind: monacoLanguages.CompletionItemKind.Snippet,
            documentation: 'if/elif/else block',
            detail: 'IF/ELIF/ELSE Statement',
            insertText: 'if ${1:condition1}:\n\t$2\nelif ${3:condition2}:\n\t$4\nelse:\n\t$5',
            insertTextRules: monacoLanguages.CompletionItemInsertTextRule.InsertAsSnippet
        },
        {
            label: 'and',
            kind: monacoLanguages.CompletionItemKind.Keyword,
            detail: 'and',
            insertText: 'and'
        },
        {
            label: 'as',
            kind: monacoLanguages.CompletionItemKind.Keyword,
            detail: 'as',
            insertText: 'as'
        },
        {
            label: 'assert',
            kind: monacoLanguages.CompletionItemKind.Keyword,
            detail: 'assert',
            insertText: 'assert'
        },
        {
            label: 'break',
            kind: monacoLanguages.CompletionItemKind.Keyword,
            detail: 'break',
            insertText: 'break'
        },
        {
            label: 'continue',
            kind: monacoLanguages.CompletionItemKind.Keyword,
            detail: 'continue',
            insertText: 'continue'
        },
        {
            label: 'del',
            kind: monacoLanguages.CompletionItemKind.Keyword,
            detail: 'del',
            insertText: 'del'
        },
        {
            label: 'False',
            kind: monacoLanguages.CompletionItemKind.Keyword,
            detail: 'False',
            insertText: 'False'
        },
        {
            label: 'from',
            kind: monacoLanguages.CompletionItemKind.Keyword,
            detail: 'from',
            insertText: 'from'
        },
        {
            label: 'global',
            kind: monacoLanguages.CompletionItemKind.Keyword,
            detail: 'global',
            insertText: 'global'
        },
        {
            label: 'import',
            kind: monacoLanguages.CompletionItemKind.Keyword,
            detail: 'import',
            insertText: 'import'
        },
        {
            label: 'in',
            kind: monacoLanguages.CompletionItemKind.Keyword,
            detail: 'in',
            insertText: 'in'
        },
        {
            label: 'is',
            kind: monacoLanguages.CompletionItemKind.Keyword,
            detail: 'is',
            insertText: 'is'
        },
        {
            label: 'lambda',
            kind: monacoLanguages.CompletionItemKind.Keyword,
            detail: 'lambda',
            insertText: 'lambda'
        },
        {
            label: 'None',
            kind: monacoLanguages.CompletionItemKind.Keyword,
            detail: 'None',
            insertText: 'None'
        },
        {
            label: 'nonlocal',
            kind: monacoLanguages.CompletionItemKind.Keyword,
            detail: 'nonlocal',
            insertText: 'nonlocal'
        },
        {
            label: 'not',
            kind: monacoLanguages.CompletionItemKind.Keyword,
            detail: 'not',
            insertText: 'not'
        },
        {
            label: 'or',
            kind: monacoLanguages.CompletionItemKind.Keyword,
            detail: 'or',
            insertText: 'or'
        },
        {
            label: 'pass',
            kind: monacoLanguages.CompletionItemKind.Keyword,
            detail: 'pass',
            insertText: 'pass'
        },
        {
            label: 'raise',
            kind: monacoLanguages.CompletionItemKind.Keyword,
            detail: 'raise',
            insertText: 'raise'
        },
        {
            label: 'return',
            kind: monacoLanguages.CompletionItemKind.Keyword,
            detail: 'return',
            insertText: 'return'
        },
        {
            label: 'True',
            kind: monacoLanguages.CompletionItemKind.Keyword,
            detail: 'True',
            insertText: 'True'
        },
        {
            label: 'with',
            kind: monacoLanguages.CompletionItemKind.Keyword,
            detail: 'with',
            insertText: 'with'
        },
        {
            label: 'yield',
            kind: monacoLanguages.CompletionItemKind.Keyword,
            detail: 'yield',
            insertText: 'yield'
        }
    ];
}