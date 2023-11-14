

//delete Product
function deleteProduct(id) {
    Swal.fire({
        title: 'Bạn có chắc chắn muốn xóa?',
        text: 'Hành động này không thể hoàn tác!',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Xóa'
    }).then((result) => {
        if (result.isConfirmed) {
            // Gọi controller để xóa khi người dùng xác nhận
            $.ajax({
                type: 'POST',
                url: '/Admin/Product/Delete/' + id,
                success: function (response) {
                    if (response.success) {
                        // Hiển thị thông báo thành công
                        Swal.fire('Xóa thành công!', '', 'success')
                            .then(() => {
                                // Chuyển đến trang Index sau khi xóa
                                location.href = '/Admin/Product/Index';
                            });
                    } else {
                        // Hiển thị thông báo lỗi
                        Swal.fire('Không thể xóa loại sản phẩm này !', '', 'error');
                    }
                },
                error: function () {
                    // Xử lý lỗi nếu cần
                    Swal.fire('Có lỗi xảy ra!', '', 'error');
                }
            });
        }
    });
}

//delete Category
function deleteCategory(id) {
    Swal.fire({
        title: 'Bạn có chắc chắn muốn xóa?',
        text: 'Hành động này không thể hoàn tác!',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Xóa'
    }).then((result) => {
        if (result.isConfirmed) {
            // Gọi controller để xóa khi người dùng xác nhận
            $.ajax({
                type: 'POST',
                url: '/Admin/Category/Delete/' + id,
                success: function (response) {
                    if (response.success) {
                        // Hiển thị thông báo thành công
                        Swal.fire('Xóa thành công!', '', 'success')
                            .then(() => {
                                // Chuyển đến trang Index sau khi xóa
                                location.href = '/Admin/Category/Index';
                            });
                    } else {
                        // Hiển thị thông báo lỗi
                        Swal.fire('Không thể xóa loại sản phẩm này !', '', 'error');
                    }
                },
                error: function () {
                    // Xử lý lỗi nếu cần
                    Swal.fire('Có lỗi xảy ra!', '', 'error');
                }
            });
        }
    });
}


//delete Brand

function deleteBrand(id) {
    Swal.fire({
        title: 'Bạn có chắc chắn muốn xóa?',
        text: 'Hành động này không thể hoàn tác!',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Xóa'
    }).then((result) => {
        if (result.isConfirmed) {
            // Gọi controller để xóa khi người dùng xác nhận
            $.ajax({
                type: 'POST',
                url: '/Admin/Brand/Delete/' + id,
                success: function (response) {
                    if (response.success) {
                        // Hiển thị thông báo thành công
                        Swal.fire('Xóa thành công!', '', 'success')
                            .then(() => {
                                // Chuyển đến trang Index sau khi xóa
                                location.href = '/Admin/Brand/Index';
                            });
                    } else {
                        // Hiển thị thông báo lỗi
                        Swal.fire('Không thể xóa hãng sản phẩm này !', '', 'error');
                    }
                },
                error: function () {
                    // Xử lý lỗi nếu cần
                    Swal.fire('Có lỗi xảy ra!', '', 'error');
                }
            });
        }
    });
}
function validateAndSubmitCategory() {
    var form = document.getElementById('createFormCategory');
    var inputElement = form.elements['TenLoai'];
    var spanElement = document.querySelector('[data-valmsg-for="TenLoai"]');

    // Kiểm tra trùng tên loại bằng Ajax
    $.ajax({
        type: 'GET',
        url: '/Admin/Category/IsTenLoaiExists?tenLoai=' + inputElement.value,
        success: function (response) {
            if (response.exists) {
                // Hiển thị thông báo lỗi nếu tên loại đã tồn tại
                spanElement.textContent = 'Tên loại sản phẩm đã tồn tại.';
                spanElement.style.display = 'block';
                inputElement.focus();
            } else {
                // Nếu không có lỗi, gọi Ajax để tạo mới loại sản phẩm
                $.ajax({
                    type: 'POST',
                    url: '/Admin/Category/Create',
                    data: { TenLoai: inputElement.value },
                    success: function (response) {
                        console.log("Thêm Loại sản phẩm thành công");

                        const Toast = Swal.mixin({
                            toast: true,
                            position: "top-end",
                            showConfirmButton: false,
                            timer: 1800,
                            timerProgressBar: true,
                            didOpen: (toast) => {
                                toast.onmouseenter = Swal.stopTimer;
                                toast.onmouseleave = Swal.resumeTimer;
                            }
                        });

                        Toast.fire({
                            icon: "success",
                            title: "Thêm Loại sản phẩm thành công"
                        });

                        new Promise(resolve => setTimeout(resolve, 1800))
                            .then(() => {
                                window.location.href = "/Admin/Category/Index";
                            });
                    },
                    error: function () {
                        // Xử lý lỗi nếu cần
                        spanElement.textContent = 'Có lỗi xảy ra khi thêm loại sản phẩm.';
                        spanElement.style.display = 'block';
                        inputElement.focus();
                    }
                });
            }
        },
        error: function () {
            // Xử lý lỗi nếu cần
            spanElement.textContent = 'Có lỗi xảy ra khi kiểm tra trùng tên loại.';
            spanElement.style.display = 'block';
            inputElement.focus();
        }
    });
}


function validateEditBrand() {
    var form = document.getElementById('EditFormBrand');
    var ngaySanXuatInput = form.elements['NgaySanXuat'];
    var ngaySanXuatValidationError = document.querySelector('[data-valmsg-for="NgaySanXuat"]');

    var currentDate = new Date();
    var selectedDate = new Date(ngaySanXuatInput.value);

    if (!ngaySanXuatInput.value || selectedDate > currentDate) {
        showValidationError(ngaySanXuatValidationError, "Vui lòng chọn ngày sản xuất hợp lệ.");
        return;
    } else {
        hideValidationError(ngaySanXuatValidationError);
    }

    // Lấy dữ liệu từ form và chuyển đổi thành đối tượng JSON
    var formData = new FormData(form);
    var jsonObject = {};
    formData.forEach((value, key) => {
        jsonObject[key] = value;
    });

    // Thực hiện AJAX để chỉnh sửa brand
    $.ajax({
        type: 'POST',
        url: '/Admin/Brand/Edit',
        data: JSON.stringify(jsonObject),
        contentType: 'application/json',
        success: function (response) {
            const Toast = Swal.mixin({
                toast: true,
                position: 'top-end',
                showConfirmButton: false,
                timer: 1800,
                timerProgressBar: true,
                didOpen: (toast) => {
                    toast.onmouseenter = Swal.stopTimer;
                    toast.onmouseleave = Swal.resumeTimer;
                }
            });

            Toast.fire({
                icon: 'success',
                title: 'Chỉnh sửa Hãng sản  phẩm thành công'
            });

            new Promise(resolve => setTimeout(resolve, 1800))
                .then(() => {
                    window.location.href = '/Admin/Brand/Index';
                });
        },
        error: function (error) {
            // Xử lý lỗi
            console.log('Lỗi khi chỉnh sửa brand', error);
        }
    });
}


function validateEditCategory() {
    var form = document.getElementById('EditFormCategory');
    var ngaySanXuatInput = form.elements['TenLoai'];
    var ngaySanXuatValidationError = document.querySelector('[data-valmsg-for="TenLoai"]');

    if (!ngaySanXuatInput.value) {
        showValidationError(ngaySanXuatValidationError, "Nhập Tên Loại Sản phẩm.");
        return;
    } else {
        hideValidationError(ngaySanXuatValidationError);
    }

    // Lấy dữ liệu từ form và chuyển đổi thành đối tượng JSON
    var formData = new FormData(form);
    var jsonObject = {};
    formData.forEach((value, key) => {
        jsonObject[key] = value;
    });

    // Thực hiện AJAX để chỉnh sửa brand
    $.ajax({
        type: 'POST',
        url: '/Admin/Category/Edit',
        data: JSON.stringify(jsonObject),
        contentType: 'application/json',
        success: function (response) {
            const Toast = Swal.mixin({
                toast: true,
                position: 'top-end',
                showConfirmButton: false,
                timer: 1800,
                timerProgressBar: true,
                didOpen: (toast) => {
                    toast.onmouseenter = Swal.stopTimer;
                    toast.onmouseleave = Swal.resumeTimer;
                }
            });

            Toast.fire({
                icon: 'success',
                title: 'Chỉnh sửa Loại sản  phẩm thành công'
            });

            new Promise(resolve => setTimeout(resolve, 1800))
                .then(() => {
                    window.location.href = '/Admin/Category/Index';
                });
        },
        error: function (error) {
            // Xử lý lỗi
            console.log('Lỗi khi chỉnh sửa brand', error);
        }
    });
}


function validateAndSubmitBrand() {
    var form = document.getElementById('createFormBrand');
    var tenHangInput = form.elements['TenHang'];
    var xuatXuInput = form.elements['XuatXu'];
    var ngaySanXuatInput = form.elements['NgaySanXuat'];
    var tenHangValidationError = document.querySelector('[data-valmsg-for="TenHang"]');
    var xuatXuValidationError = document.querySelector('[data-valmsg-for="XuatXu"]');
    var ngaySanXuatValidationError = document.querySelector('[data-valmsg-for="NgaySanXuat"]');

    // Kiểm tra trường "Tên hãng"
    if (!tenHangInput.value) {
        showValidationError(tenHangValidationError, "Vui lòng nhập tên hãng.");
        return;
    } else {
        hideValidationError(tenHangValidationError);
    }

    // Kiểm tra trùng tên và gọi Ajax để thêm mới nếu mọi thứ hợp lệ
    $.ajax({
        type: "GET",
        url: "/Admin/Brand/IsTenHangExists",
        data: { tenHang: tenHangInput.value },
        success: function (result) {
            if (result.exists) {
                showValidationError(tenHangValidationError, "Tên hãng đã tồn tại. Vui lòng chọn tên khác.");
            } else {
                hideValidationError(tenHangValidationError);

                // Kiểm tra trường "Nơi sản xuất"
                if (!xuatXuInput.value) {
                    showValidationError(xuatXuValidationError, "Vui lòng nhập nơi sản xuất.");
                    return;
                } else {
                    hideValidationError(xuatXuValidationError);
                }

                // Kiểm tra trường "Ngày sản xuất"
                var currentDate = new Date();
                var selectedDate = new Date(ngaySanXuatInput.value);
                if (!ngaySanXuatInput.value || selectedDate > currentDate) {
                    showValidationError(ngaySanXuatValidationError, "Vui lòng chọn ngày sản xuất hợp lệ.");
                    return;
                } else {
                    hideValidationError(ngaySanXuatValidationError);
                }

                // Gọi Ajax để thêm mới hãng sản xuất
                $.ajax({
                    type: 'POST',
                    url: '/Admin/Brand/Create',
                    data: {
                        TenHang: tenHangInput.value,
                        XuatXu: xuatXuInput.value,
                        NgaySanXuat: ngaySanXuatInput.value
                    },
                    success: function (response) {
                        console.log("Thêm Hãng sản phẩm thành công");

                        const Toast = Swal.mixin({
                            toast: true,
                            position: "top-end",
                            showConfirmButton: false,
                            timer: 1800,
                            timerProgressBar: true,
                            didOpen: (toast) => {
                                toast.onmouseenter = Swal.stopTimer;
                                toast.onmouseleave = Swal.resumeTimer;
                            }
                        });

                        Toast.fire({
                            icon: "success",
                            title: "Thêm Hãng sản phẩm thành công"
                        });

                        new Promise(resolve => setTimeout(resolve, 1800))
                            .then(() => {
                                window.location.href = "/Admin/Brand/Index";
                            });
                    },
                    error: function () {
                        // Xử lý lỗi nếu cần
                        handleAjaxError('Có lỗi xảy ra khi thêm hãng sản xuất.', tenHangValidationError, tenHangInput);
                    }
                });
            }
        },
        error: function () {
            alert("Đã xảy ra lỗi khi kiểm tra tên hãng.");
        }
    });
}


function validateAndSubmitProduct() {
    var form = document.getElementById('createFormProduct');
    var tenSanPhamInput = form.elements['TenSanPham'];
    var giaInput = form.elements['Gia'];
    var giaGiamInput = form.elements['GiaGiam'];
    var imageInput = form.elements['imageFile'];
    var thongTinSanPhamInput = form.elements['ThongTinSanPham'];

    var tenSanPhamValidationError = document.querySelector('[data-valmsg-for="TenSanPham"]');
    var giaValidationError = document.querySelector('[data-valmsg-for="Gia"]');
    var giaGiamValidationError = document.querySelector('[data-valmsg-for="GiaGiam"]');
    var imageValidationError = document.querySelector('[data-valmsg-for="Image"]');
    var thongTinSanPhamValidationError = document.querySelector('[data-valmsg-for="ThongTinSanPham"]');

    // Kiểm tra trường "Tên sản phẩm"
    if (!tenSanPhamInput.value) {
        showValidationError(tenSanPhamValidationError, "Vui lòng nhập tên sản phẩm.");
        return;
    } else {
        hideValidationError(tenSanPhamValidationError);
    }

    // Kiểm tra trùng tên sản phẩm
    $.ajax({
        type: "GET",
        url: "/Admin/Product/IsTenSanPhamExists",
        data: { tenSanPham: tenSanPhamInput.value },
        success: function (result) {
            if (result.exists) {
                showValidationError(tenSanPhamValidationError, "Tên sản phẩm đã tồn tại. Vui lòng chọn tên khác.");
            } else {
                hideValidationError(tenSanPhamValidationError);

                // Tiếp tục kiểm tra các trường khác nếu tên sản phẩm không trùng
                // Kiểm tra trường "Giá"
                if (!giaInput.value) {
                    showValidationError(giaValidationError, "Vui lòng nhập giá sản phẩm.");
                    return;
                } else {
                    hideValidationError(giaValidationError);
                }

                // Kiểm tra trường "Giá giảm"
                if (!giaGiamInput.value) {
                    showValidationError(giaGiamValidationError, "Vui lòng nhập giá giảm sản phẩm.");
                    return;
                } else if (parseFloat(giaGiamInput.value) >= parseFloat(giaInput.value)) {
                    showValidationError(giaGiamValidationError, "Giá giảm phải nhỏ hơn giá gốc.");
                    return;
                } else {
                    hideValidationError(giaGiamValidationError);
                }

                // Kiểm tra trường "Ảnh Sản Phẩm"
                if (!imageInput.files || imageInput.files.length === 0) {
                    showValidationError(imageValidationError, "Vui lòng chọn ảnh sản phẩm.");
                    return;
                } else {
                    hideValidationError(imageValidationError);
                }

                // Kiểm tra trường "Thông Tin Sản Phẩm"
                if (!thongTinSanPhamInput.value) {
                    showValidationError(thongTinSanPhamValidationError, "Vui lòng nhập thông tin sản phẩm.");
                    return;
                } else {
                    hideValidationError(thongTinSanPhamValidationError);
                }

                // Nếu mọi thứ hợp lệ, tiếp tục gửi dữ liệu lên server và thực hiện các bước xử lý khác
                var giaDecimal = parseFloat(giaInput.value.replace(/\D/g, ''));
                var giaGiamDecimal = parseFloat(giaGiamInput.value.replace(/\D/g, ''));

                if (isNaN(giaDecimal) || isNaN(giaGiamDecimal)) {
                    // Hiển thị lỗi và ngăn chặn việc gửi dữ liệu nếu có lỗi
                    showValidationError(giaValidationError, "Giá hoặc giảm giá không hợp lệ.");
                    return;
                }

                var formData = new FormData(form);
                formData.set("Gia", giaDecimal.toString());
                formData.set("GiaGiam", giaGiamDecimal.toString());

                $.ajax({
                    type: "POST",
                    url: "/Admin/Product/Create",
                    data: formData,
                    processData: false,
                    contentType: false,
                    success: function (response) {
                        console.log("Thêm sản phẩm thành công");

                        const Toast = Swal.mixin({
                            toast: true,
                            position: "top-end",
                            showConfirmButton: false,
                            timer: 1800,
                            timerProgressBar: true,
                            didOpen: (toast) => {
                                toast.onmouseenter = Swal.stopTimer;
                                toast.onmouseleave = Swal.resumeTimer;
                            }
                        });

                        Toast.fire({
                            icon: "success",
                            title: "Thêm sản phẩm thành công"
                        });

                        new Promise(resolve => setTimeout(resolve, 1800))
                            .then(() => {
                                window.location.href = "/Admin/Product/Index";
                            });
                    },

                    error: function (error) {
                        console.log("Lỗi khi thêm sản phẩm", error);
                    }
                });
            }
        },
        error: function () {
            alert("Đã xảy ra lỗi khi kiểm tra tên sản phẩm.");
        }
    });
}


function validateAndEditProduct() {
    var form = document.getElementById('editFormProduct');
    var formData = new FormData(form);

    // Additional data
    var giaInput = form.elements['Gia'];
    var giaGiamInput = form.elements['GiaGiam'];

    var giaValidationError = document.querySelector('[data-valmsg-for="Gia"]');
    var giaGiamValidationError = document.querySelector('[data-valmsg-for="GiaGiam"]');

    if (!giaInput.value) {
        showValidationError(giaValidationError, "Vui lòng nhập giá sản phẩm.");
        return;
    } else {
        hideValidationError(giaValidationError);
    }

    if (!giaGiamInput.value) {
        showValidationError(giaGiamValidationError, "Vui lòng nhập giá giảm sản phẩm.");
        return;
    }

    // Validate if giaGiam is less than gia
    var giaDecimal = parseFloat(giaInput.value.replace(/\D/g, ''));
    var giaGiamDecimal = parseFloat(giaGiamInput.value.replace(/\D/g, ''));

    if (isNaN(giaDecimal) || isNaN(giaGiamDecimal)) {
        showValidationError(giaValidationError, 'Giá hoặc giảm giá không hợp lệ.');
        return;
    }

    if (giaGiamDecimal >= giaDecimal) {
        showValidationError(giaGiamValidationError, 'Giá giảm phải nhỏ hơn giá gốc.');
        return;
    } else {
        hideValidationError(giaGiamValidationError);
    }

    formData.set('Gia', giaDecimal.toString());
    formData.set('GiaGiam', giaGiamDecimal.toString());

    // Convert FormData to JSON
    var jsonObject = {};
    formData.forEach((value, key) => {
        jsonObject[key] = value;
    });

    // Thực hiện AJAX để chỉnh sửa sản phẩm
    $.ajax({
        type: 'POST',
        url: '/Admin/Product/Edit',
        data: JSON.stringify(jsonObject),
        contentType: 'application/json',
        success: function (response) {
            // Handle success
            showSuccessToast();
        },
        error: function (error) {
            // Handle error
            console.log('Lỗi khi chỉnh sửa sản phẩm', error);
        }
    });
}


function showError(element, message) {
    element.textContent = message;
    element.style.display = 'block';
}

function showSuccessToast() {
    const Toast = Swal.mixin({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 1800,
        timerProgressBar: true,
        didOpen: (toast) => {
            toast.onmouseenter = Swal.stopTimer;
            toast.onmouseleave = Swal.resumeTimer;
        }
    });

    Toast.fire({
        icon: 'success',
        title: 'Chỉnh sửa sản phẩm thành công'
    });

    new Promise(resolve => setTimeout(resolve, 1800))
        .then(() => {
            window.location.href = '/Admin/Product/Index';
        });
}




function showValidationError(element, message) {
    element.textContent = message;
    element.style.display = 'block';
}

function hideValidationError(element) {
    element.textContent = '';
    element.style.display = 'none';
}
