$(function () {
    $.when(esc.ResetStateSaveDataTable()).then(function () {
        esc.List();
    });
});

let Escorttable = $('#data-table_EscortList');

let esc = {
    ResetStateSaveDataTable: function () {
        if (common.isReferrer()) {
            Escorttable.DataTable().state.clear();
        }
    },
    List: function () {
        let columns = [];
        $(Escorttable).find('thead tr th').each(function (i, v) {

            let propName = $(v).data('name');
            let column;

            switch (propName.toLowerCase()) {
                case "id":
                    column = {
                        data: null, name: propName, render: function (data, type, row, meta) {
                            return meta.row + meta.settings._iDisplayStart + 1;
                        }
                    };
                    break;
                case "name":
                    column = {
                        data: null, name: propName, class: 'fw-semibold', mRender: function (data, row) {
                            
                            return `<a href="/escort/profile?id=${data.id}" target="_blank">${data.name}</a>`
                        }
                    };
                    break;
                case "isactive":
                    column = {
                        data: null, name: propName, mRender: function (data, row) {
                            return data.isActive ? '<span class="greenActive">Active</span>' : '<span class="pinkInactive">Inactive</span>'; 
                        }
                    };
                    break;
                case "subscription":
                    column = {
                        data: null, name: propName, mRender: function (data, row) {
                            return data.subscription ? '<span class="greenActive">Active</span>' : '<span class="pinkInactive">Inactive</span>';
                        }
                    };
                    break;
                
                default:
                    column = { data: propName.charAt(0).toLowerCase() + propName.slice(1), name: propName, width: '15%' };
                    break;
            }

            columns.push(column);
        });

        let orderColumns = [0, 6];

        data = {
            EstablishmentId: establishmentId,
            Status: $("#ddlStatus").val()
          
        };
        
        tbl.BindTable(Escorttable, `/Establishment/EscortList`, data, columns, orderColumns);
    },
    filterRecord: function () {
        esc.List();
        dataTable.search($('#seachinput').val().trim()).draw();
    }
}


function ChangeEscortListStatus(userId, activeStatus, deleteStatus, isActivateStatusChange, _this) {
    toastr.remove();
    let isChecked = _this.hasAttribute("checked")
    if (isChecked) {
        $(_this).prop("checked", true)
    } else {
        $(_this).prop("checked", false)
    }
    let textMessage = "";

    if (isActivateStatusChange) {
        if (activeStatus) {
            textMessage = $("#activateEscortMsg").val()
        }
        else {
            textMessage = $("#deactivateEscortMsg").val()
        }
    }
    else if (deleteStatus) {
        textMessage = $("#deleteEscortMsg").val();
    }
    else {
        textMessage = $("#recoverEscortMsg").val()
    }

    swal({
        title: 'Are you sure?',
        text: textMessage,
        type: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#EC881D',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes'
    }).then(function (result) {
        if (result) {
            $.ajax({
                url: '/establishment/ChangeEscortListStatus',
                data:
                {
                    UserId: userId,
                    ActiveStatus: activeStatus,
                    DeleteStatus: deleteStatus,
                    IsActiveStatusChange: isActivateStatusChange
                },
                type: "Post",
                success: function (response) {
                    toastr.success(response.message);
                    setTimeout(function () {
                        esc.List();
                    }, 1000);
                },
                error: function (response) {
                    toastr.error(response.responseJSON?.message);
                }
            });


        }
        else if (!result) {
            esc.List();
        }
    });
}