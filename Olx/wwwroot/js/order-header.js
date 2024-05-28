$(document).ready(function () {
    const cityId = $('#productCityId').val();
    console.log($('#productCityId'));
    const cityElement = $('#city');
    getCityNameByRefIndex(cityId, cityElement);
});