
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

function formatCurrency(input) {
    var value = input.value.replace(/\D/g, '');
    if (value) {
        var formattedValue = new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(parseInt(value));
        input.value = formattedValue;
    }
}