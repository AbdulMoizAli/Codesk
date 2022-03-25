$(document).ready(() => {
    $('#signin-failed').click(function () {
        showAlert('Sign in failed', $(this).attr('data-errorMessage'), false, 'OK');
    });

    $('#signin-failed').trigger('click');

    const rememberMe = localStorage.getItem('rememberMe');

    if (rememberMe)
        $('#RememberMe').attr('checked', rememberMe === 'true');

    $('#RememberMe').change(function () {
        const value = $(this).is(':checked');
        localStorage.setItem('rememberMe', value);
    });
});