//delete Customer
function deleteCustomer(id) {
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
                url: '/Admin/Customer/Delete/' + id,
                success: function (response) {
                    if (response.success) {
                        // Hiển thị thông báo thành công
                        Swal.fire('Xóa thành công!', '', 'success')
                            .then(() => {
                                // Chuyển đến trang Index sau khi xóa
                                location.href = '/Admin/Customer/Index';
                            });
                    } else {
                        // Hiển thị thông báo lỗi
                        Swal.fire('Không thể xóa Khách Hàng này !', '', 'error');
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
//delete User
function deleteUser(id) {
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
                url: '/Admin/User/Delete/' + id,
                success: function (response) {
                    if (response.success) {
                        // Hiển thị thông báo thành công
                        Swal.fire('Xóa thành công!', '', 'success')
                            .then(() => {
                                // Chuyển đến trang Index sau khi xóa
                                location.href = '/Admin/User/Index';
                            });
                    } else {
                        // Hiển thị thông báo lỗi
                        Swal.fire('Không thể xóa người dùng này !', '', 'error');
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
//delete Inventory
function deleteInventory(id) {
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
                url: '/Admin/Inventory/Delete/' + id,
                success: function (response) {
                    if (response.success) {
                        // Hiển thị thông báo thành công
                        Swal.fire('Xóa thành công!', '', 'success')
                            .then(() => {
                                // Chuyển đến trang Index sau khi xóa
                                location.href = '/Admin/Inventory/Index';
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



//uuuuuuuuuuu
function validateAndSubmitUser() {
    var form = document.getElementById('createFormUser');
    var emailInput = form.elements['Email'];
    var matKhauInput = form.elements['MatKhau'];
    var hoTenInput = form.elements['HoTen'];
    var soDienThoaiInput = form.elements['SoDienThoai'];
    var diaChiInput = form.elements['DiaChi'];

    var emailValidationError = document.querySelector('[data-valmsg-for="Email"]');
    var matKhauValidationError = document.querySelector('[data-valmsg-for="MatKhau"]');
    var hoTenValidationError = document.querySelector('[data-valmsg-for="HoTen"]');
    var soDienThoaiValidationError = document.querySelector('[data-valmsg-for="SoDienThoai"]');
    var diaChiValidationError = document.querySelector('[data-valmsg-for="DiaChi"]');

    // Kiểm tra trường "Email"
    if (!emailInput.value) {
        showValidationError(emailValidationError, "Vui lòng nhập địa chỉ email.");
        return;
    } else if (!isValidEmail(emailInput.value)) {
        showValidationError(emailValidationError, "Địa chỉ email không hợp lệ.");
        return;
    } else {
        hideValidationError(emailValidationError);
    }

    // Kiểm tra trùng email
    $.ajax({
        type: "GET",
        url: "/Admin/User/IsEmailExists",
        data: { Email: emailInput.value },
        success: function (result) {
            if (result.exists) {
                showValidationError(emailValidationError, "Địa chỉ email đã tồn tại. Vui lòng chọn email khác.");
            } else {
                hideValidationError(emailValidationError);

                // Tiếp tục kiểm tra các trường khác nếu email không trùng

                // Kiểm tra trường "Mật khẩu"
                if (!matKhauInput.value) {
                    showValidationError(matKhauValidationError, "Vui lòng nhập mật khẩu.");
                    return;
                } else if (matKhauInput.value.length < 6 || !hasSpecialCharacter(matKhauInput.value)) {
                    showValidationError(matKhauValidationError, "Mật khẩu phải có ít nhất 6 ký tự và chứa ít nhất một ký tự đặc biệt.");
                    return;
                } else {
                    hideValidationError(matKhauValidationError);
                }

                // Kiểm tra trường "Họ tên"
                if (!hoTenInput.value) {
                    showValidationError(hoTenValidationError, "Vui lòng nhập họ tên.");
                    return;
                } else if (hoTenInput.value.length < 8) {
                    showValidationError(hoTenValidationError, "Họ tên phải có ít nhất 8 ký tự.");
                    return;
                } else {
                    hideValidationError(hoTenValidationError);
                }

                // Kiểm tra trường "Số điện thoại"
                if (!soDienThoaiInput.value) {
                    showValidationError(soDienThoaiValidationError, "Vui lòng nhập số điện thoại.");
                    return;
                } else if (!isValidPhoneNumber(soDienThoaiInput.value)) {
                    showValidationError(soDienThoaiValidationError, "Số điện thoại không hợp lệ.");
                    return;
                } else {
                    hideValidationError(soDienThoaiValidationError);
                }

                // Kiểm tra trường "Địa chỉ"
                if (!diaChiInput.value) {
                    showValidationError(diaChiValidationError, "Vui lòng nhập địa chỉ.");
                    return;
                } else {
                    hideValidationError(diaChiValidationError);
                }

                // Nếu mọi thứ hợp lệ, tiếp tục gửi dữ liệu lên server và thực hiện các bước xử lý khác
                var formData = new FormData(form);

                $.ajax({
                    type: "POST",
                    url: "/Admin/User/Create",
                    data: formData,
                    processData: false,
                    contentType: false,
                    success: function (response) {
                        console.log("Thêm người dùng thành công");

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
                            title: "Thêm người dùng thành công"
                        });

                        new Promise(resolve => setTimeout(resolve, 1800))
                            .then(() => {
                                window.location.href = "/Admin/User/Index";
                            });
                    },

                    error: function (error) {
                        console.log("Lỗi khi thêm người dùng", error);
                    }
                });
            }
        },
        error: function () {
            alert("Đã xảy ra lỗi khi kiểm tra địa chỉ email.");
        }
    });
}

// Hàm kiểm tra định dạng email
function isValidEmail(email) {
    // Bạn có thể thay đổi biểu thức chính quy để kiểm tra theo định dạng cụ thể
    var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}

// Hàm kiểm tra mật khẩu có ít nhất 6 ký tự và chứa ít nhất một ký tự đặc biệt
function hasSpecialCharacter(password) {
    var specialCharacterRegex = /[!@#$%^&*(),.?":{}|<>]/;
    return password.length >= 6 && specialCharacterRegex.test(password);
}

// Hàm kiểm tra số điện thoại có đúng định dạng không (10 số và bắt đầu từ 032-039)
function isValidPhoneNumber(phoneNumber) {
    var phoneRegex = /^(032|033|034|035|036|037|038|039)\d{7}$/;
    return phoneRegex.test(phoneNumber);
}


function validateAndEditUser() {
    var form = document.getElementById('EditFormUser');
    var emailInput = form.elements['Email'];
    var hoTenInput = form.elements['HoTen'];
    var soDienThoaiInput = form.elements['SoDienThoai'];
    var diaChiInput = form.elements['DiaChi'];

    var emailValidationError = document.querySelector('[data-valmsg-for="Email"]');
    var hoTenValidationError = document.querySelector('[data-valmsg-for="HoTen"]');
    var soDienThoaiValidationError = document.querySelector('[data-valmsg-for="SoDienThoai"]');
    var diaChiValidationError = document.querySelector('[data-valmsg-for="DiaChi"]');

    // Kiểm tra trường "Email"
    if (!emailInput.value) {
        showValidationError(emailValidationError, "Vui lòng nhập địa chỉ email.");
        return;
    } else if (!isValidEmail(emailInput.value)) {
        showValidationError(emailValidationError, "Địa chỉ email không hợp lệ.");
        return;
    } else {
        hideValidationError(emailValidationError);
    }
    // Kiểm tra trường "Họ tên"
    if (!hoTenInput.value) {
        showValidationError(hoTenValidationError, "Vui lòng nhập họ tên.");
        return;
    } else if (hoTenInput.value.length < 8) {
        showValidationError(hoTenValidationError, "Họ tên phải có ít nhất 8 ký tự.");
        return;
    } else {
        hideValidationError(hoTenValidationError);
    }

    // Kiểm tra trường "Số điện thoại"
    if (!soDienThoaiInput.value) {
        showValidationError(soDienThoaiValidationError, "Vui lòng nhập số điện thoại.");
        return;
    } else if (!isValidPhoneNumber(soDienThoaiInput.value)) {
        showValidationError(soDienThoaiValidationError, "Số điện thoại không hợp lệ.");
        return;
    } else {
        hideValidationError(soDienThoaiValidationError);
    }

    // Kiểm tra trường "Địa chỉ"
    if (!diaChiInput.value) {
        showValidationError(diaChiValidationError, "Vui lòng nhập địa chỉ.");
        return;
    } else {
        hideValidationError(diaChiValidationError);
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
        url: '/Admin/User/Edit',
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
                title: 'Chỉnh sửa Người dùng thành công'
            });

            new Promise(resolve => setTimeout(resolve, 1800))
                .then(() => {
                    window.location.href = '/Admin/User/Index';
                });
        },
        error: function (error) {
            // Xử lý lỗi
            console.log('Lỗi khi chỉnh sửa brand', error);
        }
    });
}

//uuuuuuuuuuuuuuuuuuu
function validateAndEditCustomer() {
    var form = document.getElementById('EditFormCustomer');
    var emailInput = form.elements['Email'];
    var hoTenInput = form.elements['HoTen'];
    var soDienThoaiInput = form.elements['SoDienThoai'];
    var diaChiInput = form.elements['DiaChi'];

    var emailValidationError = document.querySelector('[data-valmsg-for="Email"]');
    var hoTenValidationError = document.querySelector('[data-valmsg-for="HoTen"]');
    var soDienThoaiValidationError = document.querySelector('[data-valmsg-for="SoDienThoai"]');
    var diaChiValidationError = document.querySelector('[data-valmsg-for="DiaChi"]');

    // Kiểm tra trường "Email"
    if (!emailInput.value) {
        showValidationError(emailValidationError, "Vui lòng nhập địa chỉ email.");
        return;
    } else if (!isValidEmail(emailInput.value)) {
        showValidationError(emailValidationError, "Địa chỉ email không hợp lệ.");
        return;
    } else {
        hideValidationError(emailValidationError);
    }
    // Kiểm tra trường "Họ tên"
    if (!hoTenInput.value) {
        showValidationError(hoTenValidationError, "Vui lòng nhập họ tên.");
        return;
    } else if (hoTenInput.value.length < 8) {
        showValidationError(hoTenValidationError, "Họ tên phải có ít nhất 8 ký tự.");
        return;
    } else {
        hideValidationError(hoTenValidationError);
    }
    // Kiểm tra trường "Số điện thoại"
    if (!soDienThoaiInput.value) {
        showValidationError(soDienThoaiValidationError, "Vui lòng nhập số điện thoại.");
        return;
    } else if (!isValidPhoneNumber(soDienThoaiInput.value)) {
        showValidationError(soDienThoaiValidationError, "Số điện thoại không hợp lệ.");
        return;
    } else {
        hideValidationError(soDienThoaiValidationError);
    }

    // Kiểm tra trường "Địa chỉ"
    if (!diaChiInput.value) {
        showValidationError(diaChiValidationError, "Vui lòng nhập địa chỉ.");
        return;
    } else {
        hideValidationError(diaChiValidationError);
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
        url: '/Admin/Customer/Edit',
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
                title: 'Chỉnh sửa khách hàng thành công'
            });

            new Promise(resolve => setTimeout(resolve, 1800))
                .then(() => {
                    window.location.href = '/Admin/Customer/Index';
                });
        },
        error: function (error) {
            // Xử lý lỗi
            console.log('Lỗi khi chỉnh sửa brand', error);
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



function validateAndSubmitInventori() {
    var form = document.getElementById('createFormInventory');
    var productIdSelect = form.elements['ProductId'];
    var ngayNhapInput = form.elements['NgayNhap'];
    var soLuongInput = form.elements['SoLuong'];
    var ngayNhapValidationError = document.querySelector('[data-valmsg-for="NgayNhap"]');
    var soLuongValidationError = document.querySelector('[data-valmsg-for="SoLuong"]');


    // Kiểm tra trường "Ngày Nhập"
    var currentDate = new Date();
    var selectedDate = new Date(ngayNhapInput.value);
    if (!ngayNhapInput.value || selectedDate > currentDate) {
        showValidationError(ngayNhapValidationError, "Vui lòng chọn ngày nhập hợp lệ.");
        return;
    } else {
        hideValidationError(ngayNhapValidationError);
    }

    // Kiểm tra trường "Số Lượng"
    if (!soLuongInput.value || isNaN(soLuongInput.value) || parseInt(soLuongInput.value) <= 0) {
        showValidationError(soLuongValidationError, "Vui lòng nhập số lượng hợp lệ.");
        return;
    } else {
        hideValidationError(soLuongValidationError);
    }

    // Gọi Ajax để thêm mới thông tin kho
    $.ajax({
        type: 'POST',
        url: '/Admin/Inventory/Create',
        data: {
            ProductId: productIdSelect.value,
            NgayNhap: ngayNhapInput.value,
            SoLuong: soLuongInput.value
        },
        success: function (response) {
            console.log("Thêm thông tin kho thành công");

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
                title: "Thêm thông tin kho thành công"
            });

            new Promise(resolve => setTimeout(resolve, 1800))
                .then(() => {
                    window.location.href = "/Admin/Inventory/Index";
                });
        },
        error: function () {
            // Xử lý lỗi nếu cần
            handleAjaxError('Có lỗi xảy ra khi thêm thông tin kho.', ngayNhapValidationError, ngayNhapInput);
        }
    });
}


function validateEditInventori() {
    var form = document.getElementById('EditFormInventory');
    var productIdSelect = form.elements['ProductId'];
    var ngayNhapInput = form.elements['NgayNhap'];
    var soLuongInput = form.elements['SoLuong'];
    var ngayNhapValidationError = document.querySelector('[data-valmsg-for="NgayNhap"]');
    var soLuongValidationError = document.querySelector('[data-valmsg-for="SoLuong"]');


    // Kiểm tra trường "Ngày Nhập"
    var currentDate = new Date();
    var selectedDate = new Date(ngayNhapInput.value);
    if (!ngayNhapInput.value || selectedDate > currentDate) {
        showValidationError(ngayNhapValidationError, "Vui lòng chọn ngày nhập hợp lệ.");
        return;
    } else {
        hideValidationError(ngayNhapValidationError);
    }

    // Kiểm tra trường "Số Lượng"
    if (!soLuongInput.value || isNaN(soLuongInput.value) || parseInt(soLuongInput.value) <= 0) {
        showValidationError(soLuongValidationError, "Vui lòng nhập số lượng hợp lệ.");
        return;
    } else {
        hideValidationError(soLuongValidationError);
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
        url: '/Admin/Inventory/Edit',
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
                    window.location.href = '/Admin/Inventory/Index';
                });
        },
        error: function (error) {
            // Xử lý lỗi
            console.log('Lỗi khi chỉnh sửa brand', error);
        }
    });
}

// Thêm sản phẩm
function validateAndSubmitProduct() {
    var form = document.getElementById('createFormProduct');
    var tenSanPhamInput = form.elements['TenSanPham'];
    var giaNhapInput = form.elements['GiaNhap'];
    var giaBanInput = form.elements['GiaBan'];
    var giaGiamInput = form.elements['GiaGiam'];
    var imageInput = form.elements['imageFile'];
    var thongTinSanPhamInput = form.elements['ThongTinSanPham'];

    var tenSanPhamValidationError = document.querySelector('[data-valmsg-for="TenSanPham"]');
    var giaNhapValidationError = document.querySelector('[data-valmsg-for="GiaNhap"]');
    var giaBanValidationError = document.querySelector('[data-valmsg-for="GiaBan"]');
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
                // Kiểm tra trường "Giá Nhập"
                if (!giaNhapInput.value) {
                    showValidationError(giaNhapValidationError, "Vui lòng nhập giá nhập sản phẩm.");
                    return;
                } else {
                    hideValidationError(giaNhapValidationError);
                }

                // Kiểm tra trường "Giá Bán"
                if (!giaBanInput.value) {
                    showValidationError(giaBanValidationError, "Vui lòng nhập giá bán sản phẩm.");
                    return;
                } else {
                    hideValidationError(giaBanValidationError);
                }

                // Kiểm tra trường "Giá Giảm"
                 if (isNaN(giaGiamInput.value) || giaGiamInput.value < 0 || giaGiamInput.value > 100) {
                    showValidationError(giaGiamValidationError, "Vui lòng nhập một số từ 0 đến 100.");
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
                var editorData = CKEDITOR.instances['ThongTinSanPham'].getData();
                thongTinSanPhamInput.value = editorData;
                // Kiểm tra trường "Thông Tin Sản Phẩm"
                if (!thongTinSanPhamInput.value) {
                    showValidationError(thongTinSanPhamValidationError, "Vui lòng nhập thông tin sản phẩm.");
                    return;
                } else {
                    hideValidationError(thongTinSanPhamValidationError);
                }

                // Nếu mọi thứ hợp lệ, tiếp tục gửi dữ liệu lên server và thực hiện các bước xử lý khác
                var giaNhapDecimal = parseFloat(giaNhapInput.value.replace(/\D/g, ''));
                var giaBanDecimal = parseFloat(giaBanInput.value.replace(/\D/g, ''));

                if (isNaN(giaNhapDecimal) || isNaN(giaBanDecimal)) {
                    // Hiển thị lỗi và ngăn chặn việc gửi dữ liệu nếu có lỗi
                    showValidationError(giaBanValidationError, "Giá hoặc giảm giá không hợp lệ.");
                    return;
                }

                var formData = new FormData(form);
                formData.set("GiaNhap", giaNhapDecimal.toString());
                formData.set("GiaBan", giaBanDecimal.toString());

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


// Sửa sản phẩm
function validateAndEditProduct() {
    var form = document.getElementById('editFormProduct');
    var formData = new FormData(form);
    // Additional data
    var tenSanPhamInput = form.elements['TenSanPham'];
    var giaNhapInput = form.elements['GiaNhap'];
    var giaBanInput = form.elements['GiaBan'];
    var giaGiamInput = form.elements['GiaGiam'];
    var imageInput = form.elements['imageFile'];
    var thongTinSanPhamInput = form.elements['ThongTinSanPham'];

    var tenSanPhamValidationError = document.querySelector('[data-valmsg-for="TenSanPham"]');
    var giaNhapValidationError = document.querySelector('[data-valmsg-for="GiaNhap"]');
    var giaBanValidationError = document.querySelector('[data-valmsg-for="GiaBan"]');
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

    // Gửi Ajax để kiểm tra trùng tên sản phẩm
    $.ajax({
        type: "GET",
        url: "/Admin/Product/IsTenSanPhamExist",
        data: { tenSanPham: tenSanPhamInput.value, currentProductId: formData.get('Id') },
        success: function (result) {
            if (result.exists) {
                // Hiển thị lỗi nếu tên sản phẩm đã tồn tại
                showValidationError(tenSanPhamValidationError, "Tên sản phẩm đã tồn tại. Vui lòng chọn tên khác.");
            } else {
                // Ẩn lỗi nếu tên sản phẩm không trùng
                hideValidationError(tenSanPhamValidationError);

                // Kiểm tra trường "Giá bán"
                if (!giaBanInput.value) {
                    showValidationError(giaBanValidationError, "Vui lòng nhập giá bán sản phẩm.");
                    return;
                } else {
                    hideValidationError(giaBanValidationError);
                }

                // Kiểm tra trường "Giá nhập"
                if (!giaNhapInput.value) {
                    showValidationError(giaNhapValidationError, "Vui lòng nhập giá nhập sản phẩm.");
                    return;
                }
                // Kiểm tra trường "Giá giảm"

                if (isNaN(giaGiamInput.value) || giaGiamInput.value < 0 || giaGiamInput.value > 100) {
                    showValidationError(giaGiamValidationError, "Vui lòng nhập một số từ 0 đến 100.");
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
                var editorData = CKEDITOR.instances['ThongTinSanPham'].getData();
                thongTinSanPhamInput.value = editorData;
                // Kiểm tra trường "Thông Tin Sản Phẩm"
                if (!thongTinSanPhamInput.value) {
                    showValidationError(thongTinSanPhamValidationError, "Vui lòng nhập thông tin sản phẩm.");
                    return;
                } else {
                    hideValidationError(thongTinSanPhamValidationError);
                }
                // Validate if giaGiam is less than gia
                var giaNhapDecimal = parseFloat(giaNhapInput.value.replace(/\D/g, ''));
                var giaBanDecimal = parseFloat(giaBanInput.value.replace(/\D/g, ''));
                    
                if (isNaN(giaNhapDecimal) || isNaN(giaBanDecimal)) {
                    showValidationError(giaBanValidationError, 'Giá hoặc giảm giá không hợp lệ.');
                    return;
                }
                formData.set("GiaNhap", giaNhapDecimal.toString());
                formData.set("GiaBan", giaBanDecimal.toString());

                // Thực hiện AJAX để chỉnh sửa sản phẩm
                $.ajax({
                    type: 'POST',
                    url: '/Admin/Product/Edit',
                    data: formData,
                    processData: false,
                    contentType: false,
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
        },
        error: function () {
            // Hiển thị thông báo lỗi khi kiểm tra tên sản phẩm
            alert("Đã xảy ra lỗi khi kiểm tra tên sản phẩm.");
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
