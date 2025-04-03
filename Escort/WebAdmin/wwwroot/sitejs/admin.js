/*const { data } = require("jquery");*/

$(function () {
    $.when(users.ResetStateSaveDataTable()).then(function () {
        users.List();
    });
});

let MemberEmail = [];
let isValidate = true;
let table = $('#data-table_users');

let users = {

    ResetStateSaveDataTable: function () {
        if (common.isReferrer()) {
            table.DataTable().state.clear();
        }
    },

    List: function () {
        let columns = [];
        $(table).find('thead tr th').each(function (i, v) {

            let propName = $(v).data('name');
            let column;

            switch (propName.toLowerCase()) {
                case "id":
                    column = {
                        data: null, name: propName, width: "5%", render: function (data, type, row, meta) {
                            return meta.row + meta.settings._iDisplayStart + 1;
                        }
                    };
                    break;
                case "name":
                    column = {
                        data: null, name: propName, width: "15%", class: 'fw-semibold', mRender: function (data, row) {
                            return `<a href="/User/ProfileDetail?id=${data.id}"  >${data.name}</a>`; //  return `${data.name}`;
                        }
                    };
                    break;
                case "isactive":
                    column = {
                        data: null, name: propName, width: "10%", mRender: function (data, row) {

                            if (data.isActive) {
                                return `<div class="switchBtn">
                                            <input type="checkbox" hidden="hidden" id="checkbox_${data.id}" checked  onclick="ChangeUserStatus(` + data.id + `, false, ` + propName.toLowerCase().includes("isdeleted") + `, true, this)">
                                            <label class="switch" for="checkbox_${data.id}" ></label>
                                            <span class="slider activeSlide"></span>
                                        </div>`;
                            }
                            else {
                                return `<div class="switchBtn">
                                          <input type="checkbox"  hidden="hidden" id="checkbox_${data.id}" onclick="ChangeUserStatus(` + data.id + `, true, ` + propName.toLowerCase().includes("isdeleted") + `, true, this)">
                                          <label class="switch" for="checkbox_${data.id}" ></label>
                                          <span class="slider"></span>
                                       </div>`;
                            }
                        }
                    };
                    break;

                default:
                    column = { data: propName.charAt(0).toLowerCase() + propName.slice(1), name: propName };
                    break;
            }

            columns.push(column);
        });

        let orderColumns = [0, 2, 3];

   
        data = {
            Country: $("#Country").val(),
            Gender:  $("#ddlGender").val() 
        };

        tbl.BindTable(table, `/User/Index`, data, columns, orderColumns);
    },
    UpdateUser: function (_this) {
        if (!$("#frmUpdateUser").valid()) {
            return;
        }

        EnableDisableButton($(_this), true);

        ajax.Post(`/User/Detail`, $('#frmUpdateUser').serialize(),
            function (response) {
                EnableDisableButton(this, false);
                toastr.success(response.message);
                setTimeout(function () {
                    location.href = '/User/Index'
                }, setTimeoutIntervalEnum.onRedirection);
            },
            function (error) {

            }
        );

    },
    filterRecord: function () {
        users.List();
        dataTable.search($('#seachinput').val().trim()).draw();
    },

}





function ChangeUserStatus(userId, activeStatus, deleteStatus, isActivateStatusChange, _this) {
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
            textMessage = $("#activateUserMsg").val()
        }
        else {
            textMessage = $("#deactivateUserMsg").val()
        }
    }
    else if (deleteStatus) {
        textMessage = $("#deleteUserMsg").val();
    }
    else {
        textMessage = $("#recoverUserMsg").val()
    }

    swal({
        title: 'Are you sure?',
        text: textMessage,
        type: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes'
    }).then(function (result) {
        if (result) {
            $.ajax({
                url: '/User/ChangeUserStatus',
                data:
                {
                    UserId: userId,
                    ActiveStatus: activeStatus,
                    DeleteStatus: deleteStatus,
                    IsActiveStatusChange: isActivateStatusChange
                },
                type: "Post",
                success: function (response) {
                    if (!response.message) {
                        let errorStatus = findError(response);
                        if (errorStatus && errorStatus == 401) {
                            showDeactivePopup();
                            return false;
                            //location.href = '/Account/Login';
                        }
                    }

                    toastr.success(response.message);
                    setTimeout(function () {
                        users.List();
                    }, 1000);
                },
                error: function (response) {
                    toastr.error(response.responseJSON?.message);
                }
            });


        }
        else if (!result) {
            users.List();
        }
    });
}
