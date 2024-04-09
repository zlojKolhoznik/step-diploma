function deletePhotoHandler(e) {
    e.preventDefault();
    let container = $(e.target).parent().parent();
    if (e.target.tagName === 'I') {
        container = container.parent(); // if the button is clicked, the target is the icon, so we need to go one level up
    }
    let panel = container.parent();
    container.remove();
    let newPhotoContainer = $('<div class="photo-container"></div>');
    newPhotoContainer.append('<input type="file" name="photos" accept="image/*"/>');
    if (panel.children('.photo-input-container').length === 0) {
        newPhotoContainer.removeClass('photo-container').addClass('photo-input-container');
        newPhotoContainer.append('<img src="/assets/icons/free-icon-camera-7708970.png"/>');
        newPhotoContainer.on('click', (e) => {
            $(e.target).children('input').trigger('click');
        });
        newPhotoContainer.children('img').on('click', (e) => {
            $(e.target).parent().trigger('click');
        });
        newPhotoContainer.children('input').on('change', photoUploadHandler);
    } else {
        newPhotoContainer.append('<img src="/assets/icons/free-icon-gallery-964053.png"/>');
    }
    newPhotoContainer.append('<div class="delete-panel"><button class="delete-button"><i class="bi bi-trash3"></i></button></div>');
    newPhotoContainer.children('.delete-panel').children('.delete-button').on('click', deletePhotoHandler);
    panel.append(newPhotoContainer);
}

function photoUploadHandler(e) {
    let input = e.target;
    for (let file of input.files){
        let reader = new FileReader();
        reader.onload = (e) => {
            let container = $(input).parent().next();
            let img = $(input).siblings('img');
            img.attr('src', e.target.result);
            img.attr('style', 'width: 98px; height: 98px; margin: 0; border-radius: 5px;');
            $(input).parent().removeClass('photo-input-container').addClass('photo-container has-photo');
            $(input).parent().on('click', null);
            $(input).siblings('img').on('click', null);
            container.removeClass('photo-container').addClass('photo-input-container');
            container.children('img').attr('src', '/assets/icons/free-icon-camera-7708970.png');
            container.on('click', (e) => {
                $(e.target).children('input').trigger('click');
            });
            container.children('img').on('click', (e) => {
                $(e.target).parent().trigger('click');
            });
            container.children('input').on('change', photoUploadHandler);
        }
        reader.readAsDataURL(file);
    }
}

function suggestionClickHandler(e) {
    let target = $(e.target);
    if (target.hasClass('region')) {
        target = target.parent();
    }
    target.children('.region').remove();
    let city = target.text();
    $('input[name="City"]').val($(e.target).data('ref'));
    $('input[name="city-name"]').val(city);
    $('.suggestions').empty();
}

function getCityNameByRef(ref) {
    $.ajax({
        url: '/Filter/CityNameByRef/' + ref,
        type: 'GET',
        success: data => {
            $('input[name="city-name"]').val(data);
        },
        error: function (msg) {
            console.log(msg);
        }
    });
}

function cityInputChanged(e) {
    let city = $(e.target).val();
    $('input[name="City"]').val('');
    if (city.length < 3) {
        $('.suggestions').empty();
        return;
    }
    $.ajax({
        url: '/Filter/CitySuggestions?q=' + city,
        type: 'GET',
        success: data => {
            let suggestions = $('.suggestions');
            suggestions.empty();
            if (data.length === 0) {
                suggestions.append('<div class="text-danger">No suggestions found</div>');
            }
            $.each(data, (index, value) => {
                suggestions.append('<li class="suggestion" data-ref="' + value.id + '">' + value.name + '</li>')
                    .children().last().append('<span class="region">' + value.region + '</span>');
            });
            $('.suggestion').on('click', suggestionClickHandler);
        },
        error: function (msg) {
            console.log(msg);
        }
    });
}

function getCityNameByRefIndex(ref, element) {
    $.ajax({
        url: '/Filter/CityNameByRef/' + ref,
        type: 'GET',
        success: data => {
            $(element).text(data);
        },
        error: function (msg) {
            console.log(msg);
        }
    });
}