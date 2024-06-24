
// URL của API REST Countries
const apiURL = 'https://restcountries.com/v3.1/all';

// Hàm để lấy danh sách các quốc gia
async function fetchCountries() {
    try {
        const response = await fetch(apiURL);
        const countries = await response.json();
        return countries;
    } catch (error) {
        console.error('Error fetching countries:', error);
    }
}

// Hàm để thêm các quốc gia vào ô select
async function populateCountries() {
    const selectElement = document.getElementById('XuatXu');
    if (!selectElement) {
        console.error('Select element not found!');
        return;
    }

    const countries = await fetchCountries();

    if (countries) {
        // Sắp xếp các quốc gia theo bảng chữ cái
        countries.sort((a, b) => {
            const nameA = a.name.common.toUpperCase(); // Bỏ qua chữ hoa/thường
            const nameB = b.name.common.toUpperCase(); // Bỏ qua chữ hoa/thường
            if (nameA < nameB) {
                return -1;
            }
            if (nameA > nameB) {
                return 1;
            }
            return 0;
        });

        // Thêm các quốc gia vào ô select
        countries.forEach(country => {
            const option = document.createElement('option');
            option.value = country.cca2; // Sử dụng mã quốc gia (ISO 3166-1 alpha-2)
            option.textContent = country.name.common; // Tên quốc gia
            selectElement.appendChild(option);
        });
    }
}