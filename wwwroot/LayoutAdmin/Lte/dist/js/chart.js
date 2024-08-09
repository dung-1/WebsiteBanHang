
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
                    text: 'Biểu đồ doanh thu theo tháng'
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
                    anchor: 'center',
                    align: 'center',
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
                    text: 'Biểu đồ số lượng sản phẩm theo trạng thái'
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

function renderProductBarChart(labels, data, productDetails) {

    // Kết hợp labels, data, và productDetails vào một mảng đối tượng
    var combinedData = labels.map((label, index) => {
        return {
            label: label,
            data: data[index],
            details: productDetails[index]
        };
    });

    // Sắp xếp mảng theo số lượng tồn kho từ cao đến thấp
    combinedData.sort((a, b) => b.data - a.data);

    // Tách lại các labels, data, và productDetails sau khi sắp xếp
    labels = combinedData.map(item => item.label);
    data = combinedData.map(item => item.data);
    productDetails = combinedData.map(item => item.details);

    var ctx = document.getElementById('productBarChart').getContext('2d');
    var colors = getColorList(labels.length);
    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Số lượng tồn kho theo hãng sản phẩm',
                data: data,
                backgroundColor: colors,
                borderColor: colors,
                borderWidth: 1,
            }]
        },
        options: {
            responsive: false,
            plugins: {
                title: {
                    display: true,
                    text: 'Biểu đồ số lượng tồn kho theo hãng sản phẩm'
                },
                tooltip: {
                    callbacks: {
                        title: function (tooltipItems) {
                            var brandName = labels[tooltipItems[0].dataIndex];
                            return 'Hãng: ' + brandName;
                        },
                        label: function (tooltipItem) {
                            var brandName = labels[tooltipItem.dataIndex];
                            var details = productDetails[tooltipItem.dataIndex].map(function (item) {
                                return '  ' + item.productName + ': ' + item.quantity.toLocaleString();
                            });
                            return details;
                        },
                        labelTextColor: function (context) {
                            return 'white';
                        }
                    },
                    bodyFont: {
                        size: 14
                    },
                    displayColors: false
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
                        text: 'Hãng sản phẩm'
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

function renderPostCategoryChart(labels, data) {
    var ctx = document.getElementById('postCategoryChart').getContext('2d');
    var colors = getColorList(labels.length);
    var chart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Tổng số bài viết theo từng danh mục',
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
                    text: 'Biểu đồ tổng số bài viết theo từng danh mục'
                },
                tooltip: {
                    callbacks: {
                        title: function (tooltipItems) {
                            return 'Thể loại: ' + labels[tooltipItems[0].dataIndex];
                        },
                        label: function (tooltipItem) {
                            return data[tooltipItem.dataIndex].toLocaleString() + ' bài viết';
                        },
                    },
                    displayColors: false
                },
                datalabels: {
                    display: true,
                    color: 'black',
                    anchor: 'center',
                    align: 'center',
                    formatter: (value) => {
                        return value.toLocaleString() + ' Bài viết';
                    }
                }
            },
            scales: {
                x: {
                    title: {
                        display: true,
                        text: 'Thể loại bài viết'
                    }
                },
                y: {
                    title: {
                        display: true,
                        text: 'Số lượng bài viết'
                    },
                    beginAtZero: true
                }
            },
            onClick: function (event, elements) {
                if (elements.length > 0) {
                    var index = elements[0].index;
                    var selectedCategory = labels[index];
                    updatePostCategoryViewCountChart(selectedCategory);
                }
            }
        },
        plugins: [ChartDataLabels]
    });
}

function renderPostCategoryViewCountChart(labels, data, category) {
    var ctx = document.getElementById('postCategoryviewcountChart').getContext('2d');
    var colors = getColorList(labels.length);
    var shortenedLabels = labels.map(label => label.length > 3 ? label.substring(0, 3) + '...' : label);

    if (window.viewCountChart) {
        window.viewCountChart.destroy(); // Xóa biểu đồ cũ nếu tồn tại
    }

    window.viewCountChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: shortenedLabels,
            datasets: [{
                label: 'Số lượt xem của từng bài viết thuộc danh mục ' + category.toLowerCase(),
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
                    text: 'Biểu đồ thống kê số lượt xem của từng bài viết thuộc danh mục ' + category.toLowerCase()
                },
                tooltip: {
                    callbacks: {
                        title: function (tooltipItems) {
                            return 'Bài viết: ' + labels[tooltipItems[0].dataIndex];
                        },
                        label: function (tooltipItem) {
                            return data[tooltipItem.dataIndex].toLocaleString() + ' lượt xem';
                        },
                    },
                    displayColors: false
                },
                datalabels: {
                    display: true,
                    color: 'black',
                    anchor: 'center',
                    align: 'center',
                    formatter: (value) => {
                        return value.toLocaleString() + ' Lượt xem';
                    }
                }
            },
            scales: {
                x: {
                    title: {
                        display: true,
                        text: 'Tên bài viết'
                    }
                },
                y: {
                    title: {
                        display: true,
                        text: 'Số lượng lượt xem'
                    },
                    beginAtZero: true
                }
            }
        },
        plugins: [ChartDataLabels]
    });
}

function updatePostCategoryViewCountChart(selectedCategory) {
    fetchPostViewCountData(selectedCategory).then(response => {
        var postNames = response.map(p => p.postName);
        var postViewCounts = response.map(p => p.viewCount);

        renderPostCategoryViewCountChart(postNames, postViewCounts, selectedCategory);
    });
}

function fetchPostViewCountData(category) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: '/admin/homeadmin/api/GetPostViewCounts',
            type: 'GET',
            data: { category: category },
            success: function (data) {
                resolve(data);
            },
            error: function (err) {
                reject(err);
            }
        });
    });
}
