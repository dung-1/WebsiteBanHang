var emailInput = document.getElementById('inputEmail');
var emailError = document.getElementById('emailError');
var fullnameInput = document.getElementById('inputName');
var fullnameError = document.getElementById('fullnameError');
var phoneNumberInput = document.getElementById('inputPhoneNumber');
var phoneNumberError = document.getElementById('phoneNumberError');
var addressInput = document.getElementById('inputAddress');
var addressError = document.getElementById('addressError');
var passwordInput = document.getElementById('inputPassword');
var passwordError = document.getElementById('passwordError');

var hasErrors = false; // Biến kiểm tra có lỗi hay không

emailInput.addEventListener('keyup', function () {
    validateField(emailInput, emailError, validateEmail, "Email không hợp lệ");
});

fullnameInput.addEventListener('keyup', function () {
    validateField(fullnameInput, fullnameError, validateFullname, "Họ tên phải có ít nhất 8 ký tự");
});

phoneNumberInput.addEventListener('keyup', function () {
    validateField(phoneNumberInput, phoneNumberError, validatePhoneNumber, "Số điện thoại không hợp lệ");
});

addressInput.addEventListener('keyup', function () {
    validateField(addressInput, addressError, validateAddress, "Địa chỉ không được để trống");
});

passwordInput.addEventListener('keyup', function () {
    validateField(passwordInput, passwordError, validatePassword, "Mật khẩu phải có ít nhất 6 ký tự");
});

function validateField(input, errorElement, validationFunction, errorMessage) {
    var value = input.value.trim();
    if (validationFunction(value)) {
        errorElement.textContent = "";
    } else {
        errorElement.textContent = errorMessage;
        hasErrors = true; // Có lỗi
    }
}

function validateEmail(email) {
    if (!validateEmailFormat(email)) {
        return false;
    } else {
        checkEmailAvailability(email);
        return true;
    }
}

function validateFullname(fullname) {
    return fullname.length >= 8;
}

function validatePhoneNumber(phoneNumber) {
    return /^[0-9]{10}$/.test(phoneNumber) && isValidPhoneNumber(phoneNumber);
}

function validateAddress(address) {
    return address.trim() !== "";
}

function validatePassword(password) {
    return password.length >= 6;
}

function validateEmailFormat(email) {
    var emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zAZ.]{2,4}$/;
    return emailPattern.test(email);
}

function checkEmailAvailability(email) {
    $.ajax({
        type: 'POST',
        url: '/Login/Account/CheckEmail', // Sử dụng đường dẫn đầy đủ
        data: { email: email },
        success: function (result) {
            if (!result) {
                emailError.textContent = "Email đã tồn tại, vui lòng chọn email khác";
                hasErrors = true; // Có lỗi
            } else {
                emailError.textContent = "";
            }
        },
        error: function () {
            emailError.textContent = "Lỗi xảy ra khi kiểm tra email";
            hasErrors = true; // Có lỗi
        }
    });
}



function isValidPhoneNumber(phoneNumber) {
    var allowedPrefixes = ['032', '033', '034', '035', '036', '037', '038', '039'];
    var prefix = phoneNumber.substring(0, 3);
    return allowedPrefixes.includes(prefix);
}

function clearErrors() {
    var errorElements = document.getElementsByClassName("text-danger");
    for (var i = 0; i < errorElements.length; i++) {
        errorElements[i].textContent = "";
    }
}
function validateForm() {
    hasErrors = false; // Đặt lại biến hasErrors thành false
    clearErrors(); // Xóa thông báo lỗi hiện tại
    // Gọi các hàm kiểm tra trường input
    validateField(emailInput, emailError, validateEmail, "Email không hợp lệ");
    validateField(fullnameInput, fullnameError, validateFullname, "Họ tên phải có ít nhất 8 ký tự");
    validateField(phoneNumberInput, phoneNumberError, validatePhoneNumber, "Số điện thoại không hợp lệ");
    validateField(addressInput, addressError, validateAddress, "Địa chỉ không được để trống");
    validateField(passwordInput, passwordError, validatePassword, "Mật khẩu phải có ít nhất 6 ký tự");
    // Trả về true nếu không có lỗi
    return !hasErrors;
}


// Sử dụng hàm này để kiểm tra có lỗi trước khi submit
$(document).ready(function () {
    $('form').on('submit', function (e) {
        e.preventDefault(); // Ngăn chặn form submit mặc định
        if (validateForm()) { // Nếu form hợp lệ
            submitForm();
        }
    });
});

function submitForm() {
    var formData = $('form').serialize(); // Lấy dữ liệu form

    $.ajax({
        url: '/Login/Account/formSignUp',
        type: 'POST',
        data: formData,
        beforeSend: function () {
            // Hiển thị loading nếu cần
            showLoading();
        },
        success: function (response) {
            if (response.success) {
                Swal.fire({
                    title: 'Đăng ký thành công!',
                    text: response.message,
                    icon: 'success',
                    confirmButtonText: 'OK'
                }).then((result) => {
                    if (result.isConfirmed) {
                        window.location.href = '/Login/Account/Check_Verification';
                    }
                });
            } else {
                Swal.fire({
                    title: 'Đăng ký không thành công',
                    text: response.message,
                    icon: 'error',
                    confirmButtonText: 'OK'
                });
            }
        },
        error: function (xhr, status, error) {
            // Xử lý khi có lỗi kết nối
            Swal.fire({
                title: 'Lỗi',
                text: 'Không thể kết nối đến server. Vui lòng thử lại sau.',
                icon: 'error',
                confirmButtonText: 'OK'
            });
        },
        complete: function () {
            // Ẩn loading nếu cần
            hideLoading();
        }
    });
}
function showLoading() {
    $('#loading-spinner').show();
}

function hideLoading() {
    $('#loading-spinner').hide();
}
function buttonForgotPassword() {
    var data = $("#ForgotPasswordForm").serialize();
    showLoading();
    $.ajax({
        url: '/Login/account/forgot-password',
        type: 'POST',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        data: data,
        success: function (result) {
            if (result.success) {
                Swal.fire('Gửi thông tin thành công', 'Vui lòng kiểm tra Email!', 'success')
                    .then(() => {
                        location.href = '/Login/account/forgot-password';
                    });
            } else {
                Swal.fire('Lỗi', result.message || 'Có lỗi xảy ra khi gửi email!', 'error');
            }
        },
        error: function (xhr) {
            Swal.fire('Lỗi', xhr.responseText || 'Không thể xử lý yêu cầu!', 'error');
        },
        complete: function () {
            hideLoading();
        }
    });
}

function buttonResetPassword() {
    var data = $("#ResetPasswordForm").serialize();
    showLoading(); // Hiển thị hiệu ứng loading
    $.ajax({
        url: '/Login/account/reset-password', // Đường dẫn đến action ResetPassword trong controller
        type: 'POST',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        data: data,
        success: function (result) {

            if (result.success) {
                Swal.fire('Thành công', 'Mật khẩu của bạn đã được đặt lại thành công!', 'success')
                    .then(() => {
                        location.href = '/Login/account/Login'; 
                    });
            } else {
                Swal.fire('Lỗi', result.message || 'Có lỗi xảy ra khi gửi email!', 'error');
            }
        },
        error: function (xhr) {
            Swal.fire('Lỗi', xhr.responseText || 'Có lỗi xảy ra trong quá trình đặt lại mật khẩu!', 'error');
        },
        complete: function () {
            hideLoading(); // Ẩn hiệu ứng loading
        }
    });
}

function buttonCheckVerificationForm() {
    var data = $("#CheckVerificationForm").serialize();
    showLoading(); 
    $.ajax({
        url: '/Login/account/formCheck_Verification', 
        type: 'POST',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        data: data,
        success: function (result) {

            if (result) {
                Swal.fire('Thành công', 'Kích hoạt khoản thành công!', 'success')
                    .then(() => {
                        location.href = '/Login/account/Login';
                    });
            } else {
                Swal.fire('Lỗi', result.message || 'Có lỗi xảy ra khi kích hoạt khoản!', 'error');
            }
        },
        error: function (xhr) {
            Swal.fire('Lỗi', xhr.responseText || 'Có lỗi xảy ra trong quá trình kích hoạt khoản!', 'error');
        },
        complete: function () {
            hideLoading();
        }
    });
}

function buttonLoginForm() {
    var formData = $('#LoginForm').serialize();
    $.ajax({
        url: '/Login/Account/Login',
        type: 'POST',
        data: formData,
        success: function (response) {
            if (response.success) {
                Swal.fire({
                    title: 'Đăng nhập thành công!',
                    text: 'Bạn sẽ được chuyển hướng đến trang chủ.',
                    icon: 'success',
                    confirmButtonText: 'OK'
                }).then((result) => {
                    if (result.isConfirmed) {
                        window.location.href = response.redirectUrl;
                    }
                });
            } else {
                Swal.fire({
                    title: 'Đăng nhập thất bại',
                    text: response.message || 'Email hoặc mật khẩu không chính xác.',
                    icon: 'error',
                    confirmButtonText: 'OK'
                });
            }
        },
        error: function (xhr, status, error) {
            Swal.fire({
                title: 'Lỗi',
                text: 'Đã xảy ra lỗi khi đăng nhập. Vui lòng thử lại sau.',
                icon: 'error',
                confirmButtonText: 'OK'
            });
        }
    });
}

