export default function getPythonProposals(monacoLanguages) {
    return [
        {
            label: 'print',
            kind: monacoLanguages.CompletionItemKind.Function,
            documentation: 'The print() function prints the specified message to the screen, or other standard output device',
            detail: 'print(message)',
            insertText: 'print(${1:message})$0',
            insertTextRules: monacoLanguages.CompletionItemInsertTextRule.InsertAsSnippet
        },
        {
            label: 'abs',
            kind: monacoLanguages.CompletionItemKind.Function,
            documentation: 'Return the absolute value of a number. The argument may be an integer, a floating point number, or an object implementing __abs__(). If the argument is a complex number, its magnitude is returned.',
            detail: 'abs(x)',
            insertText: 'abs(${1:x})$0',
            insertTextRules: monacoLanguages.CompletionItemInsertTextRule.InsertAsSnippet
        },
        {
            label: 'aiter',
            kind: monacoLanguages.CompletionItemKind.Function,
            documentation: 'Return an asynchronous iterator for an asynchronous iterable. Equivalent to calling x.__aiter__().\n\naiter(x) itself has an __aiter__() method that returns x, so aiter(aiter(x)) is the same as aiter(x).',
            detail: 'aiter(async_iterable)',
            insertText: 'aiter(${1:async_iterable})$0',
            insertTextRules: monacoLanguages.CompletionItemInsertTextRule.InsertAsSnippet
        },
        {
            label: 'all',
            kind: monacoLanguages.CompletionItemKind.Function,
            documentation: 'Return True if all elements of the iterable are true (or if the iterable is empty).',
            detail: 'all(iterable)',
            insertText: 'all(${1:iterable})$0',
            insertTextRules: monacoLanguages.CompletionItemInsertTextRule.InsertAsSnippet
        },
        {
            label: 'any',
            kind: monacoLanguages.CompletionItemKind.Function,
            documentation: 'Return True if any element of the iterable is true. If the iterable is empty, return False.',
            detail: 'any(iterable)',
            insertText: 'any(${1:iterable})$0',
            insertTextRules: monacoLanguages.CompletionItemInsertTextRule.InsertAsSnippet
        },
        {
            label: 'ascii',
            kind: monacoLanguages.CompletionItemKind.Function,
            documentation: 'As repr(), return a string containing a printable representation of an object, but escape the non-ASCII characters in the string returned by repr() using \\x, \\u, or \U escapes. This generates a string similar to that returned by repr() in Python 2.',
            detail: 'ascii(object)',
            insertText: 'ascii(${1:object})$0',
            insertTextRules: monacoLanguages.CompletionItemInsertTextRule.InsertAsSnippet
        },
        {
            label: 'bin',
            kind: monacoLanguages.CompletionItemKind.Function,
            documentation: 'Convert an integer number to a binary string prefixed with “0b”. The result is a valid Python expression. If x is not a Python int object, it has to define an __index__() method that returns an integer.',
            detail: 'bin(x)',
            insertText: 'bin(${1:x})$0',
            insertTextRules: monacoLanguages.CompletionItemInsertTextRule.InsertAsSnippet
        },
        {
            label: 'bool',
            kind: monacoLanguages.CompletionItemKind.Function,
            documentation: 'Return a Boolean value, i.e. one of True or False. x is converted using the standard truth testing procedure. If x is false or omitted, this returns False; otherwise, it returns True. The bool class is a subclass of int (see Numeric Types — int, float, complex). It cannot be subclassed further. Its only instances are False and True (see Boolean Values).',
            detail: 'class bool([x])',
            insertText: 'bool(${1:x})$0',
            insertTextRules: monacoLanguages.CompletionItemInsertTextRule.InsertAsSnippet
        },
        {
            label: 'chr',
            kind: monacoLanguages.CompletionItemKind.Function,
            documentation: "Return the string representing a character whose Unicode code point is the integer i. For example, chr(97) returns the string 'a', while chr(8364) returns the string '€'. This is the inverse of ord().\n\nThe valid range for the argument is from 0 through 1,114,111 (0x10FFFF in base 16). ValueError will be raised if i is outside that range.",
            detail: 'chr(i)',
            insertText: 'chr(${1:i})$0',
            insertTextRules: monacoLanguages.CompletionItemInsertTextRule.InsertAsSnippet
        },
        {
            label: 'complex',
            kind: monacoLanguages.CompletionItemKind.Function,
            documentation: "Return a complex number with the value real + imag*1j or convert a string or number to a complex number. If the first parameter is a string, it will be interpreted as a complex number and the function must be called without a second parameter. The second parameter can never be a string. Each argument may be any numeric type (including complex). If imag is omitted, it defaults to zero and the constructor serves as a numeric conversion like int and float. If both arguments are omitted, returns 0j.",
            detail: 'class complex([real[, imag]])',
            insertText: 'complex(${1:x})$0',
            insertTextRules: monacoLanguages.CompletionItemInsertTextRule.InsertAsSnippet
        },
        {
            label: 'delattr',
            kind: monacoLanguages.CompletionItemKind.Function,
            documentation: "This is a relative of setattr(). The arguments are an object and a string. The string must be the name of one of the object’s attributes. The function deletes the named attribute, provided the object allows it. For example, delattr(x, 'foobar') is equivalent to del x.foobar.",
            detail: 'delattr(obj, name)',
            insertText: 'delattr(${1:obj}, ${2:name})$0',
            insertTextRules: monacoLanguages.CompletionItemInsertTextRule.InsertAsSnippet
        },
        {
            label: 'dict',
            kind: monacoLanguages.CompletionItemKind.Function,
            documentation: "Create a new dictionary. The dict object is the dictionary class. See dict and Mapping Types — dict for documentation about this class.",
            detail: 'class dict(**kwarg)',
            insertText: 'dict(${1:**kwarg})$0',
            insertTextRules: monacoLanguages.CompletionItemInsertTextRule.InsertAsSnippet
        },
        {
            label: 'dir',
            kind: monacoLanguages.CompletionItemKind.Function,
            documentation: "Without arguments, return the list of names in the current local scope. With an argument, attempt to return a list of valid attributes for that object.",
            detail: 'dir([obj])',
            insertText: 'dir(${1:[obj]})$0',
            insertTextRules: monacoLanguages.CompletionItemInsertTextRule.InsertAsSnippet
        },
        {
            label: 'divmod',
            kind: monacoLanguages.CompletionItemKind.Function,
            documentation: "Take two (non-complex) numbers as arguments and return a pair of numbers consisting of their quotient and remainder when using integer division. With mixed operand types, the rules for binary arithmetic operators apply. For integers, the result is the same as (a // b, a % b). For floating point numbers the result is (q, a % b), where q is usually math.floor(a / b) but may be 1 less than that. In any case q * b + a % b is very close to a, if a % b is non-zero it has the same sign as b, and 0 <= abs(a % b) < abs(b).",
            detail: 'divmod(a, b)',
            insertText: 'divmod(${1:a}, ${2:b})$0',
            insertTextRules: monacoLanguages.CompletionItemInsertTextRule.InsertAsSnippet
        },
        {
            label: 'float',
            kind: monacoLanguages.CompletionItemKind.Function,
            documentation: "Return a floating point number constructed from a number or string x.",
            detail: 'class float([x])',
            insertText: 'float(${1:x})$0',
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