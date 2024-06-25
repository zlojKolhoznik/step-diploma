$(document).ready(() => {
    $('#deleteAccount').on('click', e => {
        let message = `Якщо ви видалите свій обліковий запис, ви втратите всі оголошення, відгуки та інші дані ` +
         `вашого облікового запису. Ці дані НЕМОЖЛИВО буде відновити. Ви впевнені, що хочете видалити свій обліковий запис?`;
        let deleteAccount = confirm(message);
        if (!deleteAccount) {
            e.preventDefault();
        }
    });
    
    $('.profile-picture').on('click', e => {
        $('input[name=profilePicture]').trigger('click');
    });
    
    $('input[name=profilePicture]').on('change', e => {
        let file = e.target.files[0];
        let reader = new FileReader();
        reader.onload = e => {
            $('.profile-picture').attr('src', e.target.result);
        };
        reader.readAsDataURL(file);
    });
    
    $('#settingsForm').on('submit', e => {
        let password = $('input[name=currentPassword]').val();
        let newPassword = $('input[name=newPassword]').val();
        let confirmPassword = $('input[name=confirmPassword]').val();
        if (!password && !newPassword && !confirmPassword) {
            return;
        }
        if (password === newPassword) {
            alert('Новий пароль не може співпадати з поточним паролем');
            e.preventDefault();  
            return;
        } 
        if (newPassword !== confirmPassword) {
            alert('Паролі не співпадають');
            e.preventDefault();
        }
    });
});