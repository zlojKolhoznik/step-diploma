$(document).ready(() => {
    $('.dropdown-toggler').click(() => {
        let dropdown = $('.drop-menu')[0];
        if (dropdown.classList.contains('hidden')) {
            dropdown.classList.remove('hidden');
            $('#downArrow').css('transform', 'rotate(180deg)');
        } else {
            dropdown.classList.add('hidden');
            $('#downArrow').css('transform', 'rotate(0deg)');
        }
    });
}); // change for merge commit, will remove later