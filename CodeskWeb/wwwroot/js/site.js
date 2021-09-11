$(document).ready(() => {
    $('.tooltipped').tooltip();

    $('form').submit(function (e) {
        const form = $(this);

        if (form.valid()) {
            const submitBtn = form.find('button[type=submit]');
            submitBtn.children(':first-child').attr('class', 'fas fa-sync fa-spin');
            submitBtn.attr('disabled', true);
        }
    });
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