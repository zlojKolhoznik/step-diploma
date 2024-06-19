$(document).ready(() => {
    $('.slider').on('click', e => {
        let inp = $(e.target.closest('.switch')).find('input')[0];
        $(inp).trigger('click');
    });
    
    $('.switch input').on('change', e => {
        let inp = $(e.target);
        console.log(inp);
        let id = inp.data('id');
        let url = '/Product/ToggleAutorenewing/' + id;
        $.post(url, res => {
            console.log(res);
        });
    });
    
    $('.order-by').on('change', e => {
        let orderBy = $(e.target).val();
        let url = window.location.href;
        if (url.endsWith('publications')) {
            url += '?orderBy=' + orderBy;
        } else if (url.includes('orderBy')) {
            url = url.replace(/orderBy=[^&]+/, 'orderBy=' + orderBy);
        } else if (url.includes('?')) {
            url += '&orderBy=' + orderBy;
        } else {
            url += '?orderBy=' + orderBy;
        }
        window.location.href = url;
    });
    
    $('form button[type="submit"].action-button').on('click', e => {
        if (!confirm('Цю дію неможливо скасувати. Ви впевнені що хочете видалити оголошення?')) {
            e.preventDefault();
        }
    });
});