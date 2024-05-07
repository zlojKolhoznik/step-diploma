$(document).ready(() => {
    $('.group input').focus(changeInputState);
    $('.group input').change(changeInputState);
    $('.group input').focusout(changeInputState);
    $('.search-location').change(e => {
        let value = $(e.target).val();
        console.log('change')
        if (value.length < 3) {
            $('.suggestions').empty();
            $('.suggestions').css('display', 'none');
            return;
        }
        let suggestions = $('.suggestions');
        suggestions.css('display', 'block');
        suggestions.append('<div style="color: var(--muted-color)">Завантаження підказок...</div>');
        console.log('here');
        $.ajax({
            url: '/Filter/CitySuggestions?q=' + value,
            type: 'GET',
            success: data => {
                suggestions.empty();
                if (data.length === 0) {
                    suggestions.append('<div style="color: var(--red-color)">Жодної підказки не знайдено</div>');
                }
                $.each(data, (index, value) => {
                    suggestions.append('<li class="suggestion" data-ref="' + value.id + '">' + value.name + '</li>')
                        .children().last().append('<span class="region">' + value.region + '</span>');
                });
                $('.suggestion').on('click', e => {
                    let target = $(e.target);
                    if (target.hasClass('region')) {
                        target = target.parent();
                    }
                    target.children('.region').remove();
                    let city = target.text();
                    $('input[name="loc"]').val($(e.target).data('ref'));
                    $('input[name="l"]').val(city);
                    $('.suggestions').empty();
                });
            },
            error: function (msg) {
                console.log(msg);
            }
        });
    });
});

function changeInputState(e) {
    let hideLabel = e.target.value.length > 0 || e.type === 'focus';
    console.log($(e.target).siblings('label').first());
    $(e.target).siblings('label').first().css('display', hideLabel ? 'none' : 'block');
}