// Modal View Order
$(document).on("click", ".View-Order", function (e) {

    let id = $(this).data("id")
    $.ajax({
        url: "/Admin/Billorder/View?id=" + id,// Đường dẫn đến API của bạn
        type: "GET",
        dataType: "html", // Đặt kiểu dữ liệu trả về
        success: function (data) {
            $('#View_Order').find('.modal-content').html(data)
            $('#View_Order').modal('show');
        }
    })
});
// Modal Edit Inventory
$(document).on("click", ".edit-inventory", function (e) {

    let id = $(this).data("id")
    $.ajax({
        url: "/Admin/Inventory/Edit?id=" + id,// Đường dẫn đến API của bạn
        type: "GET",
        dataType: "html", // Đặt kiểu dữ liệu trả về
        success: function (data) {
            $('#edit-inventory-modal').find('.modal-content').html(data)
            $('#edit-inventory-modal').modal('show');
        }
    })
});

// Modal create Order
$(document).on("click", ".edit-OderDetail", function (e) {

    let id = $(this).data("id")
    $.ajax({
        url: "/Admin/Order/Edit?id=" + id,// Đường dẫn đến API của bạn
        type: "GET",
        dataType: "html", // Đặt kiểu dữ liệu trả về
        success: function (data) {
            $('#edit-Order-modal').find('.modal-content').html(data)
            $('#edit-Order-modal').modal('show');
        }
    })
});
// Modal create inventory
$(document).on("click", ".inventory_create", function (e) {

    $.ajax({
        url: "/Admin/Inventory/Create",// Đường dẫn đến API của bạn
        type: "GET",
        dataType: "html", // Đặt kiểu dữ liệu trả về
        success: function (data) {
            $('#Inventory_Create').find('.modal-content').html(data)
            $('#Inventory_Create').modal('show');
        }
    })
});
// Modal create Brand
$(document).on("click", ".category_create", function (e) {
    
    $.ajax({
        url: "/Admin/Category/Create",// Đường dẫn đến API của bạn
        type: "GET",
        dataType: "html", // Đặt kiểu dữ liệu trả về
        success: function (data) {
            $('#Category_Create').find('.modal-content').html(data)
            $('#Category_Create').modal('show');
        }
    })
});
// Modal Edit Category
$(document).on("click", ".edit-category", function (e) {
    
    let id = $(this).data("id")
    $.ajax({
        url: "/Admin/Category/Edit?id=" + id,// Đường dẫn đến API của bạn
        type: "GET",
        dataType: "html", // Đặt kiểu dữ liệu trả về
        success: function (data) {
            $('#edit-category-modal').find('.modal-content').html(data)
            $('#edit-category-modal').modal('show');
        }
    })
});
// Modal create Brand
$(document).on("click", ".create-brand", function (e) {
    
    $.ajax({
        url: "/Admin/Brand/Create" ,// Đường dẫn đến API của bạn
        type: "GET",
        dataType: "html", // Đặt kiểu dữ liệu trả về
        success: function (data) {
            $('#Create_Brand').find('.modal-content').html(data)
            $('#Create_Brand').modal('show');
            fetchCountries();
        }
    })
});
// Modal Edit Category
$(document).on("click", ".edit-brand", function (e) {
    
    let id = $(this).data("id")
    $.ajax({
        url: "/Admin/Brand/Edit?id=" + id,// Đường dẫn đến API của bạn
        type: "GET",
        dataType: "html", // Đặt kiểu dữ liệu trả về
        success: function (data) {
            $('#edit-brand-modal').find('.modal-content').html(data)
            $('#edit-brand-modal').modal('show');
        }
    })
});
// Modal create Brand
$(document).on("click", ".create-produt", function (e) {
    
    $.ajax({
        url: "/Admin/Product/Create",// Đường dẫn đến API của bạn
        type: "GET",
        dataType: "html", // Đặt kiểu dữ liệu trả về
        success: function (data) {
            $('#Create_Product').find('.modal-content').html(data)
            $('#Create_Product').modal('show');
        }
    })
});
// Modal Edit Category
$(document).on("click", ".edit-produt", function (e) {
    
    let id = $(this).data("id")
    $.ajax({
        url: "/Admin/Product/Edit?id=" + id,// Đường dẫn đến API của bạn
        type: "GET",
        dataType: "html", // Đặt kiểu dữ liệu trả về
        success: function (data) {
            $('#edit-produt-modal').find('.modal-content').html(data)
            $('#edit-produt-modal').modal('show');
        }
    })
});
// Modal Create User
$(document).on("click", ".create-user", function (e) {

    $.ajax({
        url: "/Admin/User/Create",// Đường dẫn đến API của bạn
        type: "GET",
        dataType: "html", // Đặt kiểu dữ liệu trả về
        success: function (data) {
            $('#Create_User').find('.modal-content').html(data)
            $('#Create_User').modal('show');
        }
    })
});
// Modal Edit User
$(document).on("click", ".edit-user", function (e) {

    let id = $(this).data("id")
    $.ajax({
        url: "/Admin/User/Edit?id=" + id,// Đường dẫn đến API của bạn
        type: "GET",
        dataType: "html", // Đặt kiểu dữ liệu trả về
        success: function (data) {
            $('#edit-user-modal').find('.modal-content').html(data)
            $('#edit-user-modal').modal('show');
        }
    })
});
// Modal Edit Customer
$(document).on("click", ".edit-customer", function (e) {

    let id = $(this).data("id")
    $.ajax({
        url: "/Admin/Customer/Edit?id=" + id,// Đường dẫn đến API của bạn
        type: "GET",
        dataType: "html", // Đặt kiểu dữ liệu trả về
        success: function (data) {
            $('#edit-customer-modal').find('.modal-content').html(data)
            $('#edit-customer-modal').modal('show');
        }
    })
});




// Modal create Categoryposts
$(document).on("click", ".category_post_create", function (e) {

    $.ajax({
        url: "/Admin/CategoryPost/Create",// Đường dẫn đến API của bạn
        type: "GET",
        dataType: "html", // Đặt kiểu dữ liệu trả về
        success: function (data) {
            $('#Category_Post_Create').find('.modal-content').html(data)
            $('#Category_Post_Create').modal('show');
        }
    })
});


// Modal Edit Category
$(document).on("click", ".edit-post-category", function (e) {

    let id = $(this).data("id")
    $.ajax({
        url: "/Admin/CategoryPost/Edit?id=" + id,// Đường dẫn đến API của bạn
        type: "GET",
        dataType: "html", // Đặt kiểu dữ liệu trả về
        success: function (data) {
            $('#edit-category-post-modal').find('.modal-content').html(data)
            $('#edit-category-post-modal').modal('show');
        }
    })
});


function formatCurrency(input) {
    // Lấy giá trị nhập vào và loại bỏ tất cả các ký tự không phải số
    var value = input.value.replace(/\D/g, '');

    // Định dạng giá trị thành chuỗi số có dấu phân cách
    if (value) {
        var formattedValue = new Intl.NumberFormat('vi-VN').format(parseInt(value));

        // Gán giá trị đã định dạng vào ô input
        input.value = formattedValue;
    }
}
function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}

