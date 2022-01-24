$(document).ready(() => {
    $('a.activator').click(function () {
        const type = $(this).data('type');
        $(this).parent().prev().find(`.${type}`).show().siblings().hide();
    });
});