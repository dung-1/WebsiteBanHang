// Modal create Brand
$(document).on("click", ".category_create", function (e) {
    e.preventDefault()
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
    e.preventDefault()
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
    e.preventDefault()
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
    e.preventDefault()
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
    e.preventDefault()
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
    e.preventDefault()
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