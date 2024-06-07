$(document).ready(() => {
    $('.search-result > a').click(e => {
        if (e.target.tagName.toLowerCase() === 'button'
            || e.target.tagName.toLowerCase() === 'svg'
            || e.target.tagName.toLowerCase() === 'path') {
            e.preventDefault();
        }
    });
    $('.like-button').click(e => {
        const id = $(e.target).closest('.search-result').data('id');
        $.post('/Product/ToggleFavorite', { id }, function (data) {
            console.log(data)
            let svg = $(`.search-result[data-id="${id}"] .like-button svg`);
            console.log(svg);
            if (data.isFavorite) {
                svg.attr('fill', '#FB6C6C');
                svg.find('path').attr('stroke', 'none');
            } else {
                svg.attr('fill', 'none');
                svg.find('path').attr('stroke', '#353535');
                svg.find('path').attr('stroke-width', '2');
            }
        });
    });
    $('.shipment-button').click(e => {
        const id = $(e.target).closest('.search-result').data('id');
        window.location.href = `/delivery/${id}`;
    });
    $('.message-button').on('click', function (e) {
        e.preventDefault();
        const id = $(e.target).closest('.search-result').data('id');
        window.location.href = `/Chats/Chat?id=${id}`;
    });
    $('.order-button').on('click', function (e) {
        e.preventDefault();
        const id = $(e.target).closest('.search-result').data('id');
        window.location.href = `/delivery/${id}`;
    });
});