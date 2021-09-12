$(document).ready(() => {
    $('.tooltipped').tooltip();
    $('.dropdown-trigger').dropdown();
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