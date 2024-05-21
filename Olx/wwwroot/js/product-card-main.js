$(document).ready(function () {
    let cityDiv = $('.city');
    getCityNameByRefIndex(cityDiv.data('ref'), cityDiv);
    $('#favorite').on('click', function () {
        let productId = $(this).data('id');
        let favorite = $('#favorite');
        console.log(productId);
        $.ajax({
            url: '/Product/ToggleFavorite',
            type: 'POST',
            data: { id: productId },
            success: function (data) {
                if (data.isFavorite) {
                    favorite.addClass('active');
                } else {
                    favorite.removeClass('active');
                }
            }
        });
    });
    $('.collapse-toggler').on('click', function () {
        let collapseId = $(this).data('toggle-id');
        let collapse = $(`#${collapseId}`);
        let collapsed = $(this).data('collapsed');
        console.log(collapse, collapsed)
        if (!collapsed) {
            $(collapse).addClass('hidden');
            $(this).data('collapsed', true);
        } else {
            $(collapse).removeClass('hidden');
            $(this).data('collapsed', false);
        }
    });
});