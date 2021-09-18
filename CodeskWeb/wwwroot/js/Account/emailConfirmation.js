$(document).ready(() => {
    $('#loader').hide();

    $('#send-email').click(() => {
        const email = $('#user-email').attr('data-userEmail');
        showActionAlert('Confirm', `Activation link will be sent to "${email}"`, false, 'Cancel', 'Proceed', async () => {
            M.toast({ html: 'Sending email...' });
            $('#loader').show();

            const response = await fetch('/Users/Email/SendAccountConfirmationEmail');

            if (response.status === 200)
                M.toast({ html: 'Email sent' });
            else
                M.toast({ html: 'Something went wrong while sending email' });

            $('#loader').hide();
        });
    });
});