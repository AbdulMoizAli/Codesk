$(document).ready(() => {
    $('.tooltipped').tooltip();
    $('.sidenav').sidenav();
    $('.modal').modal({
        dismissible: false,
        onOpenEnd: () => {
            $('#language-input').focus();
        }
    });
    $('select').formSelect();
    $('.dropdown-trigger').dropdown({ coverTrigger: false });
    $('#video-panel').sidenav({ edge: 'right' });
});

function showAlert(title = null, message, dark, okText) {
    new duDialog(
        title,
        message,
        {
            dark,
            okText
        }
    );
}

function showActionAlert(title = null, message, dark, cancelText, okText, callback) {
    return new duDialog(
        title,
        message,
        {
            buttons: duDialog.OK_CANCEL,
            dark,
            cancelText,
            okText,
            callbacks: {
                okClick: function () {
                    this.hide();
                    callback();
                }
            }
        }
    );
}

$.fn.exchangePositionWith = function (selector) {
    var other = $(selector);
    this.after(other.clone());
    other.after(this).remove();
};