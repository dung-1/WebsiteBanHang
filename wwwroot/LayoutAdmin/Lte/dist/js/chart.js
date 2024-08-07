
function getRandomColor(excludeColors) {
    var letters = '0123456789ABCDEF'.split('');
    var color;
    do {
        color = '#';
        for (var i = 0; i < 6; i++) {
            color += letters[Math.floor(Math.random() * 16)];
        }
    } while (excludeColors.includes(color));
    return color;
}

function getColorList(labelCount) {
    var predefinedColors = [
        '#FF5733', '#33FF57', '#3357FF', '#F333FF', '#33FFF3',
        '#FF33A1', '#FF8C33', '#FF3333', '#33FFBD', '#FF333D'
    ];

    var colors = [];
    for (var i = 0; i < labelCount; i++) {
        if (i < predefinedColors.length) {
            colors.push(predefinedColors[i]);
        } else {
            colors.push(getRandomColor(predefinedColors));
        }
    }
    return colors;
}

function renderSalesChart(labels, data) {
    var ctx = document.getElementById('salesChart').getContext('2d');
    var colors = getColorList(labels.length);
    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Doanh thu tháng',
                data: data,
                backgroundColor: colors,
                borderColor: colors,
                borderWidth: 1,
                //barThickness: 30
            }]
        },
        options: {
            responsive: false,
            plugins: {
                title: {
                    display: true,
                    text: 'Biểu đồ Doanh thu theo Tháng'
                },
                tooltip: {
                    callbacks: {
                        title: function (tooltipItems) {
                            return 'Tháng: ' + labels[tooltipItems[0].dataIndex];
                        },
                        label: function (tooltipItem) {
                            return 'Doanh thu: ' + data[tooltipItem.dataIndex].toLocaleString() + ' VND';
                        },
                        //afterLabel: function () {
                        //    // Thêm thông tin bổ sung ở đây
                        //    return 'Thông tin bổ sung';
                        //}
                    },
                    displayColors: true // Không hiển thị màu của dataset trong tooltip
                },
                datalabels: {
                    display: true,
                    color: 'black',
                    anchor: 'end',
                    align: 'top',
                    formatter: (value) => {
                        return value.toLocaleString() + ' VND';
                    }
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
        },
        plugins: [ChartDataLabels]
    });
}

function renderStatusChart(labels, chartData) {
    var ctx = document.getElementById('statusChart').getContext('2d');
    var colors = getColorList(labels.length);
    var datasets = chartData.map((item, index) => ({
        label: item.status,
        data: item.data,
        fill: false,
        borderColor: colors[index],
        backgroundColor: colors[index],
        tension: 0.1
    }));
    new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: datasets
        },
        options: {
            maintainAspectRatio: false,
            responsive: false,
            plugins: {
                title: {
                    display: true,
                    text: 'Biểu đồ Số lượng Sản phẩm theo Trạng thái'
                },
                tooltip: {
                    callbacks: {
                        title: function (tooltipItems) {
                            return 'Tháng: ' + labels[tooltipItems[0].dataIndex];
                        },
                        label: function (tooltipItem) {
                            var dataset = tooltipItem.dataset;
                            var currentValue = dataset.data[tooltipItem.dataIndex];
                            return dataset.label + ': ' + currentValue.toLocaleString();
                        }
                    },
                    displayColors: true
                },
                datalabels: {
                    display: true,
                    color: 'black',
                    anchor: 'end',
                    align: 'top',
                    formatter: (value) => {
                        return value.toLocaleString();
                    }
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
        },
        plugins: [ChartDataLabels]
    });
}

function renderCategoryPieChart(labels, data) {
    var ctx = document.getElementById('categoryPieChart').getContext('2d');
    var colors = getColorList(labels.length);
    var backgroundColors = colors;

    new Chart(ctx, {
        type: 'pie',
        data: {
            labels: labels,
            datasets: [{
                label: 'Phần trăm sản phẩm tồn kho theo danh mục',
                data: data,
                backgroundColor: backgroundColors,
                borderWidth: 1
            }]
        },
        options: {
            plugins: {
                title: {
                    display: true,
                    text: 'Biểu đồ Phần trăm Sản phẩm Tồn kho theo Danh mục'
                },
                datalabels: {
                    display: true,
                    color: 'white',
                    formatter: (value, context) => {
                        let sum = 0;
                        let dataArr = context.chart.data.datasets[0].data;
                        dataArr.map(data => {
                            sum += data;
                        });
                        let percentage = (value * 100 / sum).toFixed(2) + "%";
                        return percentage;
                    }
                },
                tooltip: {
                    callbacks: {
                        label: function (tooltipItem) {
                            var data = tooltipItem.chart.data.datasets[0].data;
                            var total = data.reduce((sum, value) => sum + value, 0);
                            var currentValue = data[tooltipItem.dataIndex];
                            var percentage = (currentValue / total * 100).toFixed(2);
                            return `${tooltipItem.label}: ${currentValue.toLocaleString()} (${percentage}%)`;
                        }
                    },
                    displayColors: true
                }
            }
        },
        plugins: [ChartDataLabels]
    });
}

function renderProductBarChart(labels, data) {

    var ctx = document.getElementById('productBarChart').getContext('2d');
    var colors = getColorList(labels.length);
    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Số lượng Sản phẩm Tồn kho',
                data: data,
                backgroundColor: colors,
                borderColor: colors,
                borderWidth: 1,
                barThickness: 30
            }]
        },
        options: {
            responsive: false,
            plugins: {
                title: {
                    display: true,
                    text: 'Biểu đồ Số lượng Sản phẩm Tồn kho'
                },
                tooltip: {
                    callbacks: {
                        title: function (tooltipItems) {
                            return 'Sản phẩm: ' + labels[tooltipItems[0].dataIndex];
                        },
                        label: function (tooltipItem) {
                            return 'Số lượng: ' + data[tooltipItem.dataIndex].toLocaleString();
                        },
                        //afterLabel: function () {
                        //    // Thêm thông tin bổ sung ở đây
                        //    return 'Thông tin bổ sung';
                        //}
                    },
                    displayColors: true // Không hiển thị màu của dataset trong tooltip
                },
                datalabels: {
                    display: true,
                    color: 'black',
                    anchor: 'end',
                    align: 'top',
                    formatter: (value) => {
                        return value.toLocaleString();
                    }
                }
            },
            scales: {
                x: {
                    title: {
                        display: true,
                        text: 'Sản phẩm'
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
        },
        plugins: [ChartDataLabels]
    });
}

