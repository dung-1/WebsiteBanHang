function updateChart() {
    var selectedYear = document.getElementById('selectedYear').value;
    window.location.href = "/admin/homeadmin?selectedYear=" + selectedYear;
}

function renderSalesChart(labels, data) {
    var ctx = document.getElementById('salesChart').getContext('2d');
    function getRandomColor() {
        var letters = '0123456789ABCDEF'.split('');
        var color = '#';
        for (var i = 0; i < 6; i++) {
            color += letters[Math.floor(Math.random() * 16)];
        }
        return color;
    }
    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Doanh thu tháng',
                data: data,
                backgroundColor: getRandomColor(),
                borderColor: getRandomColor(),
                borderWidth: 1
            }]
        },
        options: {
            plugins: {
                title: {
                    display: true,
                    text: 'Biểu đồ Doanh thu theo Tháng'
                }
            },
            scales: {
                x: {
                    title: {
                        display: true,
                        text: 'Tháng'
                    }
                },
                y: {
                    title: {
                        display: true,
                        text: 'Doanh thu (VND)'
                    },
                    beginAtZero: true
                }
            }
        }
    });
}


function renderStatusChart(labels, chartData) {
    var datasets = chartData.map(item => ({
        label: item.status,
        data: item.data,
        fill: false,
        borderColor: getRandomColor(),
        tension: 0.1
    }));

    function getRandomColor() {
        var letters = '0123456789ABCDEF'.split('');
        var color = '#';
        for (var i = 0; i < 6; i++) {
            color += letters[Math.floor(Math.random() * 16)];
        }
        return color;
    }

    var ctx = document.getElementById('statusChart').getContext('2d');
    new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: datasets
        },
        options: {
            plugins: {
                title: {
                    display: true,
                    text: 'Biểu đồ Số lượng Sản phẩm theo Trạng thái'
                }
            },
            scales: {
                x: {
                    title: {
                        display: true,
                        text: 'Tháng'
                    }
                },
                y: {
                    title: {
                        display: true,
                        text: 'Số lượng'
                    },
                    beginAtZero: true
                }
            }
        }
    });
}
