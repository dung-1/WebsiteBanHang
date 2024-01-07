var carousel = document.getElementById('carouselExampleCrossfade');
var interval = 3000;

function carouselNext() {
    var currentSlide = carousel.querySelector('.carousel-item.active');
    var nextSlide = currentSlide.nextElementSibling || carousel.querySelector('.carousel-item:first-child');

    currentSlide.classList.remove('active');
    nextSlide.classList.add('active');
}

setInterval(carouselNext, interval);
$(document).ready(function () {
    $(".updatecartitem").click(function () {
        var productId = $(this).data("productid");
        var quantity = $("#quantity-" + productId).val();

        // Kiểm tra số lượng nhập vào
        if (quantity < 1) {
            Swal.fire("Cập nhật thất bại", "Vui lòng nhập số lượng ít nhất là 1.", "error");
            return; // Dừng lại nếu số lượng không hợp lệ
        }

        if (quantity < 1) {
            quantity = 3; // Thay 3 bằng giá trị mặc định bạn muốn giữ nguyên
        }

        // Gọi AJAX để cập nhật số lượng
        $.ajax({
            url: "/Cart/UpdateCartItem",
            method: "POST",
            data: { productId: productId, quantity: quantity },
            success: function (data) {
                // Cập nhật thành công, hiển thị thông báo
                Swal.fire("Cập nhật thành công", "", "success").then(function () {
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
        url: "/Home/ListCategory",
        dataType: "json",
        success: function (data) {
            var categoryDropdown = $("#categoryDropdown");
            categoryDropdown.empty();

            $.each(data, function (index, item) {
                // Thêm một thuộc tính data-category-id để lưu ID của danh mục
                categoryDropdown.append(`<li><a class="dropdown-item" href="#" data-category-id="${item.id}">${item.tenLoai}</a></li>`);
            });

            // Thêm sự kiện click cho mỗi mục danh sách
            categoryDropdown.on('click', 'a', function (e) {
                e.preventDefault(); // Ngăn chặn hành động mặc định của thẻ a
                var categoryId = $(this).data('category-id');
                redirectToCategoryPage(categoryId);
            });

        },
        error: function () {
            console.log("Lỗi khi tải danh mục sản phẩm.");
        }
    });
}

function redirectToCategoryPage(categoryId) {
    // Chuyển hướng đến trang Category/Index với ID của danh mục được chọn
    window.location.href = `/Category/Index/${categoryId}`;
}

// Gọi hàm để tải danh mục sản phẩm khi trang web được tải
loadCategories();


$(document).ready(function () {
    $(".addToCartButton").click(function () {
        var productId = $(this).data("product-id");

        $.ajax({
            type: "POST",
            url: "/Cart/AddToCart",
            data: { id: productId },
            success: function (result) {
                const Toast = Swal.mixin({
                    toast: true,
                    position: "top-end",
                    showConfirmButton: false,
                    timer: 1500,
                    timerProgressBar: true,
                    didOpen: (toast) => {
                        toast.onmouseenter = Swal.stopTimer;
                        toast.onmouseleave = Swal.resumeTimer;
                    }
                });
                Toast.fire({
                    icon: "success",
                    title: "Thêm sản phẩm vào giỏ hàng thành công!"
                }).then(function () {
                    location.reload();
                });
            },
            error: function () {
                // Xử lý lỗi nếu có
                Swal.fire("Lỗi", "Bạn cần phải đăng nhập .", "warning").then(function () {
                    window.location.href = "/Login/account/Login";
                });
            }
        }); 
    });
});

$(document).ready(function () {
    $(".checkLoginAndNavigateToCart").click(function () {
        $.ajax({
            type: "GET",
            url: "/Cart/Index", // Đặt URL kiểm tra đăng nhập tại đây
            success: function (isLoggedIn) {
                if (isLoggedIn) {
                    window.location.href = "/Cart/Index";
                } 
            },
            error: function () {
                // Người dùng chưa đăng nhập, hiển thị thông báo và chuyển hướng đến trang đăng nhập
                Swal.fire("Thông báo", "Bạn cần phải đăng nhập .", "warning").then(function () {
                    window.location.href = "/Login/account/Login";
                });
            }
        });
    });
});
$(document).ready(function () {
    $(".checkLoginAndNavigateToAccout").click(function () {
        $.ajax({
            type: "GET",
            url: "/CustomerInfo/AccountInfo", // Đặt URL kiểm tra đăng nhập tại đây
            success: function (isLoggedIn) {
                if (isLoggedIn) {
                    window.location.href = "/CustomerInfo/AccountInfo";
                }
            },
            error: function () {
                // Người dùng chưa đăng nhập, hiển thị thông báo và chuyển hướng đến trang đăng nhập
                Swal.fire("Thông báo", "Bạn cần phải đăng nhập ", "warning").then(function () {
                    window.location.href = "/Login/account/Login";
                });
            }
        });
    });
}); $(document).ready(function () {
    $(".checkLoginAndNavigateToBill").click(function () {
        $.ajax({
            type: "GET",
            url: "/CustomerOrder/Index", // Đặt URL kiểm tra đăng nhập tại đây
            success: function (isLoggedIn) {
                if (isLoggedIn) {
                    window.location.href = "/CustomerOrder/Index";
                }
            },
            error: function () {
                // Người dùng chưa đăng nhập, hiển thị thông báo và chuyển hướng đến trang đăng nhập
                Swal.fire("Thông báo", "Bạn cần phải đăng nhập.", "warning").then(function () {
                    window.location.href = "/Login/account/Login";
                });
            }
        });
    });
});

