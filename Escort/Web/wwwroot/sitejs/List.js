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
                            return `${data.name}`;
                        }
                    };
                    break;
                case "isactive":
                    column = {
                        data: null, name: propName, mRender: function (data, row) {
                            if (data.isActive) {
                                return `<div class="switchBtn">
                                            <input type="checkbox" hidden="hidden" id="checkbox_${data.id}" checked  onclick="ChangeEscortListStatus(` + data.id + `, false, ` + propName.toLowerCase().includes("isdeleted") + `, true, this)">
                                            <label class="switch" for="checkbox_${data.id}"></label>
                                        </div>`;
                            }
                            else {
                                return `<div class="switchBtn">
                                          <input type="checkbox"  hidden="hidden" id="checkbox_${data.id}" onclick="ChangeEscortListStatus(` + data.id + `, true, ` + propName.toLowerCase().includes("isdeleted") + `, true, this)">
                                          <label class="switch"  for="checkbox_${data.id}"></label>
                                       </div>`;
                            }
                        }
                    };
                    break;
                case "ispaused":
                    column = {
                        data: null, name: propName, mRender: function (data, row)
                        {
                            if (data.isPaused)
                            {
                                return `<div class="switchBtn">
                                            <input type="checkbox" hidden="hidden" id="pause_${data.id}" checked  onclick="pauseProfile(` + data.id + `, false, this)">
                                            <label class="switch" for="pause_${data.id}"></label>
                                        </div>`;
                            }
                            else
                            {
                                return `<div class="switchBtn">
                                          <input type="checkbox"  hidden="hidden" id="pause_${data.id}" onclick="pauseProfile(` + data.id + `, true, this)">
                                          <label class="switch"  for="pause_${data.id}"></label>
                                       </div>`;
                            }
                        }
                    };
                    break;
                case "subscription":
                    column = {
                        data: null, name: propName, mRender: function (data, row) {
                            return data.subscription ? 'Active' : 'Inactive';
                        }
                    };
                    break;
                case "action":
                    column = {
                        data: null, name: null, mRender: function (data, row) {
                            return `<a href="/profile/fullProfile?id=${data.id}" class="me-1" onclick = "" title = "details" > <img src="/assets/images/view-icons.svg"/></a >
                            <a href="/profile/edit?id=${data.id}" class="me-1" onclick = "" title = "details" > <img src="/assets/images/edit-icons.svg"/></a >
                            <a href="javascript:void(0)" title="Delete" style="cursor: pointer;" onclick="ChangeEscortListStatus(` + data.id + `, ` + data.isActive + `, true, false, this)"><img src="/assets/images/delete-icons.svg" /></a>`
                        }
                    };
                   
                    break;
                default:
                    column = { data: propName.charAt(0).toLowerCase() + propName.slice(1), name: propName, width: '15%' };
                    break;
            }

            columns.push(column);
        });

        let orderColumns = [0,7,6];
        tbl.BindTable(Escorttable, `/establishment/List`, {}, columns, orderColumns);
    }
}

function pauseProfile(userId, isPause, element)
{
    let swalText = isPause ? "Do you want to pause your account?" : "Do you want to publish your account?";
    swal({
        title: 'Are you sure?',
        text: swalText,
        type: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#EC881D',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes'
    }).then(async (result)=>
    {
        if (result)
        {
            $('.Loading').show();
            $.ajax({
                type: 'POST',
                url: "/establishment/PauseAccount",
                data: { userId:userId, isPause: isPause },
                success: function (response)
                {
                    if (response.data)
                    {
                        toastr.success(response.message);
                        setTimeout(function ()
                        {
                            esc.List();
                        }, 1000);
                    }
                    else
                    {
                        toastr.error(response.message);
                    }
                    $('.Loading').hide();
                },
                error: function (response)
                {
                    $('.Loading').hide();
                    toastr.error(response.responseJSON?.message);
                }
            });
        }
        else if (!result)
        {
            esc.List();
        }
    }).catch(error =>
    {
        esc.List();
    });

}


function ChangeEscortListStatus(userId, activeStatus, deleteStatus, isActivateStatusChange, _this)
{
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