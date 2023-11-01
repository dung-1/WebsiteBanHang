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

// Sử dụng hàm này để kiểm tra có lỗi trước khi submit
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
