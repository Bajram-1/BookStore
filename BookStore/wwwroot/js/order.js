var dataTable;

$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("inprocess")) {
        loadDataTable("inprocess");
    } else {
        if (url.includes("completed")) {
            loadDataTable("completed");
        } else {
            if (url.includes("pending")) {
                loadDataTable("pending");
            } else {
                if (url.includes("approved")) {
                    loadDataTable("approved");
                } else {
                    if (url.includes("cancelled")) {
                        loadDataTable("cancelled");
                    }
                    else {
                        loadDataTable("all");
                    }
                }
            }
        }
    }
});

function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": '/admin/order/getall?status=' + status,
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "id", "width": "5%" },
            { "data": "name", "width": "15%" },
            { "data": "phoneNumber", "width": "14%" },
            {
                "data": "applicationUser",
                "width": "20%",
                "render": function (data, type, row) {
                    return data ? data.email : 'No email';
                }
            },
            {
                "data": "shippingDate",
                "width": "18%",
                "render": function (data) {
                    if (!data || data.startsWith("0001-01-01")) return 'N/A';
                    return moment(data, "YYYY-MM-DD HH:mm:ss").format("YYYY-MM-DD HH:mm:ss");
                }
            },
            { "data": "orderStatus", "width": "9%" },
            { "data": "orderTotal", "width": "9%" },
            {
                "data": "id",
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                     <a href="/admin/order/details?orderId=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i></a>
                    </div>`;
                },
                "width": "10%"
            }
        ]
    });
}