﻿function addToCart(id) {
    $.ajax({
        type: "POST",
        url: "/Cart/AddToCart/" + id,
        success: function (response) {
            // Xử lý kết quả trả về nếu cần thiết
            // Ví dụ: cập nhật giao diện người dùng

            // Chuyển đến trang giỏ hàng
            window.location.href = "/Cart/Index";
        },
        error: function (xhr, status, error) {
            // Xử lý lỗi nếu có
        }
    });
}
$(document).ready(function () {
    $(".updatecartitem").click(function () {
        var productId = $(this).data("productid");
        var quantity = $("#quantity-" + productId).val();

        // Gọi AJAX để cập nhật số lượng
        $.ajax({
            url: "/Cart/UpdateCartItem",
            method: "POST",
            data: { productId: productId, quantity: quantity },
            success: function (data) {
                // Cập nhật thành công, hiển thị thông báo
                Swal.fire("cập nhật thành công", "", "success").then(function () {
                    // Sau khi người dùng đóng thông báo, làm điều gì đó (nếu cần)
                    // Ví dụ: Reload lại trang giỏ hàng
                    location.reload();
                });
            },
            error: function () {
                // Xử lý lỗi nếu có
                Swal.fire("Lỗi", "Không thể cập nhật sản phẩm.", "error");
            }
        });
    });
});




$(".delete-item").on("click", function (e) {
    e.preventDefault(); // Ngăn chặn hành động mặc định của liên kết

    var link = $(this).attr("href");

    Swal.fire({
        title: "Xác nhận xóa sản phẩm?",
        text: "Bạn có chắc muốn xóa sản phẩm này khỏi giỏ hàng?",
        icon: "error",
        showCancelButton: true,
        confirmButtonText: "Xóa",
        cancelButtonText: "Hủy",
    }).then((result) => {
        if (result.isConfirmed) {
            // Gọi AJAX để xóa sản phẩm
            $.ajax({
                url: link,
                method: "POST",
                success: function (data) {
                    // Hiển thị thông báo xóa thành công
                    Swal.fire("Xóa thành công", "", "success").then(function () {
                        // Sau khi người dùng đóng thông báo, làm điều gì đó (nếu cần)
                        // Ví dụ: Reload lại trang giỏ hàng
                        location.reload();
                    });
                },
                error: function () {
                    // Xử lý lỗi nếu có
                    Swal.fire("Lỗi", "Không thể xóa sản phẩm.", "error");
                }
            });
        }
    });
});
$(document).on("click", ".create-checkout", function (e) {

    $.ajax({
        url: "/Cart/Checkoutcart",// Đường dẫn đến API của bạn
        type: "GET",
        dataType: "html", // Đặt kiểu dữ liệu trả về
        success: function (data) {
            $('#Create_checkout').find('.modal-content').html(data)
            $('#Create_checkout').modal('show');
        }
    })
});
// updateCartItemCount.js

function updateCartItemCount() {
    $.ajax({
        type: "GET",
        url: "/Cart/GetCartItemCount",
        success: function (data) {
            $("#cartItemCount").text(data);
        },
        error: function () {
            console.log("Lỗi khi lấy số lượng sản phẩm trong giỏ hàng.");
        }
    });
}

// Gọi hàm cập nhật số lượng sản phẩm trong giỏ hàng khi trang web được tải
updateCartItemCount();



// Tạo hàm để gọi action ListCategory và hiển thị danh mục sản phẩm
// Tạo hàm để gọi action ListCategory và hiển thị danh mục sản phẩm
function loadCategories() {
    $.ajax({
        type: "GET",
        url: "/Home/ListCategory", // Thay thế 'ControllerName' bằng tên thật của controller
        dataType: "json",
        success: function (data) {
            var categoryDropdown = $("#categoryDropdown");
            categoryDropdown.empty();
            $.each(data, function (index, item) {
                categoryDropdown.append(`<li><a class="dropdown-item" href="#">${item.tenLoai}</a></li>`);
            });
            //categoryDropdown.append(`<li><hr class="dropdown-divider"></li>`);
        //    categoryDropdown.append(`<li><a class="dropdown-item" href="#">Tất cả danh mục</a></li>`);
        },
        error: function () {
            console.log("Lỗi khi tải danh mục sản phẩm.");
        }
    });
}

// Gọi hàm để tải danh mục sản phẩm khi trang web được tải
loadCategories();


