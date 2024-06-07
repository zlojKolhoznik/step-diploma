$(document).ready(() => {
    $('.group input').focus(changeInputState);
    $('.group input').change(changeInputState);
    $('.group input').focusout(changeInputState);
    $('.group input').each((i, e) => changeInputState({ target: e }));
});

function changeInputState(e) {
    let hideLabel = e.target.value.length > 0 || e.type === 'focus';
    console.log($(e.target).siblings('label').first());
    $(e.target).siblings('label').first().css('display', hideLabel ? 'none' : 'block');
}