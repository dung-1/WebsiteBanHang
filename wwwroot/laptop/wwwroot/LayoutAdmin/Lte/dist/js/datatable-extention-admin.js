$(document).ready(function () {
    loadData();
});

function loadData() {
    $('#example1').DataTable({
        // Optional functionalities (uncomment if needed)
        responsive: true, // Enables responsive behavior
        lengthChange: true, // Disables changing number of items per page
        autoWidth: true, // Disables automatic column width adjustment
        language: {
            // Vietnamese localization (adjust if needed)
            "sProcessing": "Đang xử lý...",
            "sLengthMenu": "Xem _MENU_ mục",
            "sZeroRecords": "Không tìm thấy dòng nào phù hợp",
            "sInfo": "Đang xem từ _START_ đến _END_ trong tổng số _TOTAL_ mục",
            "sInfoEmpty": "Đang xem 0 đến 0 trong tổng số 0 mục",
            "sInfoFiltered": "(được lọc từ _MAX_ mục)",
            "sInfoPostFix": "",
            "sSearch": "Tìm kiếm:",
            "sUrl": "",
            "oPaginate": {
                "sFirst": "Đầu",
                "sPrevious": "Trước",
                "sNext": "Tiếp",
                "sLast": "Cuối"
            }
        },
        // Disables default sorting (uncomment if needed)
        aaSorting: []
    });
}