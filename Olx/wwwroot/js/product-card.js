$(document).ready(function () {
    $('.cityName').each((index, element) => {
        const ref = $(element).data('ref');
        getCityNameByRefIndex(ref, element);
    });
    $('.product-card > a').click(e => {
        if (e.target.tagName.toLowerCase() === 'button'
            || e.target.tagName.toLowerCase() === 'svg'
            || e.target.tagName.toLowerCase() === 'path') {
            e.preventDefault();
        }
    });
    $('.like-button').click(e => {
        const id = $(e.target).closest('.product-card').data('id');
        $.post('/Product/ToggleFavorite', { id }, function (data) {
            console.log(data)
            let svg = $(`.product-card[data-id="${id}"] .like-button svg`);
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
});