$(document).ready(function () {
    $('input[type="radio"]').on("input", function (e) {
        const id = e.target.id;
        console.log(id);
        switch (id) {
            case "novaPostWarehouse":
            case "meestWarehouse":
                $('.form-group:has(input[name="ReceiverWarehouse"])').removeClass('hidden');
                $('.form-group:has(input[name="ReceiverAddress"])').addClass('hidden');
                $('.form-group:has(input[name="ReceiverZipCode"])').addClass('hidden');
                break;
            case "novaPostCourier":
            case "meestCourier":
            case "ukrposhtaWarehouse":
                $('.form-group:has(input[name="ReceiverWarehouse"])').addClass('hidden');
                $('.form-group:has(input[name="ReceiverAddress"])').removeClass('hidden');
                $('.form-group:has(input[name="ReceiverZipCode"])').removeClass('hidden');
                break;
                
        }
    });
});