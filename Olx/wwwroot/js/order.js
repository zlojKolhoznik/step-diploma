$(document).ready(function () {
    $('input[name="deliveryMethod"]').on("input", function (e) {
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
    $('input[name="paymentMethod"]').on("input", function (e) {
        const id = e.target.id;
        console.log(id);
        switch (id) {
            case "byCard":
            case "prepayment":
                $('.form-group:has(input[name="cardNumber"])').removeClass('hidden');
                $('.form-group:has(input[name="cardExpirationDate"])').removeClass('hidden');
                $('.form-group:has(input[name="cardCvv"])').removeClass('hidden');
                break;
            case "epay":
                $('.form-group:has(input[name="cardNumber"])').addClass('hidden');
                $('.form-group:has(input[name="cardExpirationDate"])').addClass('hidden');
                $('.form-group:has(input[name="cardCvv"])').addClass('hidden');
                break;
            case "inWarehouse":
                $('.form-group:has(input[name="cardNumber"])').addClass('hidden');
                $('.form-group:has(input[name="cardExpirationDate"])').addClass('hidden');
                $('.form-group:has(input[name="cardCvv"])').addClass('hidden');
                break;
        }
        const price = e.target.data('price');
        $('#orderButton').val(`Сплатити ${price} грн.`);
    });
});