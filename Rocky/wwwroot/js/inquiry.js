var dataTable;

$(document).ready(function () {
    loadDataTable('GetInquiryList');
})

function loadDataTable(url) {
    dataTable = $("#tblData").dataTable({
        "ajax": {
            "url": "/inquiry/" + url
        },
        "columns": [
            { "data": "id", "width": "10%" },
            { "data": "fullName", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
            { "data": "email", "width": "15%" },
            {
                // Render Button
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Inquiry/Details/${data}" class="btn btn-success text-white" style="cursor: pointer;">
                                <icon class="fas fa-edit"></icon>
                            </a>
                        </div>
                    `;
                },
                "width": "5%"
            },
        ]
    });
}