var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/user/getall' },
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "email", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
            { "data": "company.name", "width": "15%" },
            { "data": "role", "width": "15%" },
            {
                data: { id: "id", lockoutEnd: "lockoutEnd" },
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();

                    var buttonClass = lockout > today ? "btn-danger" : "btn-success";
                    var buttonText = lockout > today ? "Lock" : "UnLock";
                    var buttonIcon = lockout > today ? "bi-lock-fill" : "bi-unlock-fill";

                    return `
                        <div class="text-center" style="display: flex; justify-content: space-between; width: 100%;">
                            <a onclick=LockUnlock('${data.id}') class="btn ${buttonClass} text-white" style="cursor:pointer; width:100px; margin-right: 10px;">
                                <i class="bi ${buttonIcon}"></i> ${buttonText}
                            </a>
                            <a href="/admin/user/RoleManagment?userId=${data.id}" class="btn btn-danger text-white" style="cursor:pointer; width:150px;">
                                <i class="bi bi-pencil-square"></i> Permission
                            </a>
                        </div>
                    `;
                },
                "width": "25%"
            }
        ]
    });
}

function LockUnlock(id) {
    $.ajax({
        type: "POST",
        url: '/Admin/User/LockUnlock',
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                dataTable.ajax.reload();
            }
        }
    });
}