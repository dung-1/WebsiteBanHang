$(document).ready(function () {
    $(".updatecartitem").click(function (event) {
        event.preventDefault();
        var productid = $(this).attr("data-productid");
        var quantity = $("#quantity-" + productid).val();
        $.ajax({
            type: "POST",
            url: "@Url.RouteUrl("updatecart")",
            data: {
                productid: productid,
                quantity: quantity
            },
            success: function (result) {
                window.location.href = "@Url.RouteUrl("Cart")";
            }
        });
    });
});
