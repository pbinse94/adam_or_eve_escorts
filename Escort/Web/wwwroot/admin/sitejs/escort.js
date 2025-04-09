$(function () {
    $.when(Escorts.ResetStateSaveDataTable()).then(function () {
        Escorts.List();
    });
});

//let MemberEmail = [];
/*let isValidate = true;*/
let Escorttable = $('#data-table_Escort');

let Escorts = {

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
                        data: null, name: propName, width: "5%", render: function (data, type, row, meta) {
                            return meta.row + meta.settings._iDisplayStart + 1;
                        }
                    };
                    break;
                case "name":
                    column = {
                        data: null, name: propName, width: "15%", class: 'fw-semibold', mRender: function (data, row) {
                            // return `${data.name}`;
                            //return `<a href="${frontSiteUrl}profile/fullProfile?id=${data.id}" target="_blank">${data.name}</a>`;
                            return `<a href="profile?id=${data.id}">${data.name}</a>`;
                        }
                    };
                    break;
                case "displayname":
                    column = {
                        data: null, name: propName, width: "15%", class: 'fw-semibold', mRender: function (data, row) {
                            // return `${data.name}`;
                            return `<a href="profile?id=${data.id}" >${data.displayName}</a>`;
                        }
                    };
                    break;
                case "age":
                    column = {
                        data: null, name: propName, width: "7%", class: 'fw-semibold', mRender: function (data, row) {
                            return data.age ? data.age + '`s' : '';
                        }
                    };
                    break;
                case "isapprove":
                    column = {
                        data: null, name: propName, mRender: function (data, row)
                        {
                            if (data.packageName && data.packageName.toLowerCase() == "basic package")
                            {
                                if (data.isApprove)
                                {
                                    return `<div class="switchBtn">
                                            <input type="checkbox" hidden="hidden" id="approve_${data.id}" checked  onclick="approveProfile(` + data.id + `, false, this)">
                                            <label class="switch" for="approve_${data.id}"></label>
                                        </div>`;
                                }
                                else
                                {
                                    return `<div class="switchBtn">
                                          <input type="checkbox"  hidden="hidden" id="approve_${data.id}" onclick="approveProfile(` + data.id + `, true, this)">
                                          <label class="switch"  for="approve_${data.id}"></label>
                                       </div>`;
                                }
                            }
                            else
                            {
                                return '';
                            }
                            
                        }
                    };
                    break;
                case "isactive":
                    column = {
                        data: null, name: propName, width: "10%", mRender: function (data, row) {

                            if (data.isActive) {
                                return `<div class="switchBtn">
                                            <input type="checkbox" hidden="hidden" id="checkbox_${data.id}" checked  onclick="ChangeEscortStatus(` + data.id + `, false, ` + propName.toLowerCase().includes("isdeleted") + `, true, this)">
                                            <label class="switch" for="checkbox_${data.id}"></label>
                                        </div>`;
                            }
                            else {
                                return `<div class="switchBtn">
                                          <input type="checkbox"  hidden="hidden" id="checkbox_${data.id}" onclick="ChangeEscortStatus(` + data.id + `, true, ` + propName.toLowerCase().includes("isdeleted") + `, true, this)">
                                          <label class="switch"  for="checkbox_${data.id}"></label>
                                       </div>`;
                            }
                        }
                    };
                    break;
                case "subscription":
                    column = {
                        data: null, name: propName, width: "10%", mRender: function (data, row)
                        {
                            if (data.subscription == null)
                            {
                                return '-';
                            }
                            else if (data.subscription)
                            {
                                return '<span class="greenActive">Active</span>';
                            }
                            else
                            {
                                return '<span class="pinkInactive">Inactive</span>';
                            }                            
                        }
                    };
                    break;
                case "packagename":
                    column = {
                        data: null, name: propName, width: "10%", mRender: function (data, row)
                        {
                            return data.packageName ? data.packageName : '-';
                        }
                    };
                    break;

                case "totalViews":
                    column = {
                       
                        data: null, name: propName, class: 'text-center', mRender: function (data, row) {
                            
                            return data.totalViews;
                        }
                    };
                    break;
                
                default:
                    column = { data: propName.charAt(0).toLowerCase() + propName.slice(1), name: propName, width: '15%' };
                    break;
            }

            columns.push(column);
        });

        let orderColumns = [0, 2,3];

        data = {
            Country: $("#Country").val(),
            Gender: $("#ddlGender").val()
        };
        tbl.BindTable(Escorttable, `/Escort/EscortList`, data, columns, orderColumns);
    },
    filterRecord: function () { 
        Escorts.List();
        dataTable.search($('#seachinput').val().trim()).draw();
    } 
}


function approveProfile(userId, isApprove, element)
{
    let swalText = isApprove ? "Do you want to approve this profile?" : "Do you want to denied this profile?";
    swal({
        title: 'Are you sure?',
        text: swalText,
        type: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#EC881D',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes'
    }).then(async (result) =>
    {
        if (result)
        {
            $('.Loading').show();
            $.ajax({
                type: 'POST',
                url: "/Escort/ApproveAccount",
                data: { userId: userId, isApprove: isApprove },
                success: function (response)
                {
                    if (response.data)
                    {
                        toastr.success(response.message);
                        setTimeout(function ()
                        {
                            Escorts.List();
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
            Escorts.List();
        }
    }).catch(error =>
    {
        Escorts.List();
    });

}

function ChangeEscortStatus(userId, activeStatus, deleteStatus, isActivateStatusChange, _this) {

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
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes'
    }).then( (result) => {
        if (result) {
            $.ajax({
                url: '/Escort/ChangeEscortStatus',
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
                        Escorts.List();
                    }, 1000);
                },
                error: function (response) {
                    toastr.error(response.responseJSON?.message);
                }
            });

        }
        else if (!result) {
            Escorts.List();
        }
    });
}

//swal({
//    title: 'Are you sure?',
//    text: textMessage,
//    type: 'warning',
//    showCancelButton: true,
//    confirmButtonColor: '#3085d6',
//    cancelButtonColor: '#d33',
//    confirmButtonText: 'Yes'
//}).then(function (result) {
//    if (result) {
//        $.ajax({
//            url: '/Escort/ChangeEscortStatus',
//            data:
//            {
//                UserId: userId,
//                ActiveStatus: activeStatus,
//                DeleteStatus: deleteStatus,
//                IsActiveStatusChange: isActivateStatusChange
//            },
//            type: "Post",
//            success: function (response) {
//                toastr.success(response.message);
//                setTimeout(function () {
//                    Escorts.List();
//                }, 1000);
//            },
//            error: function (response) {
//                toastr.error(response.responseJSON?.message);
//            }
//        });

//        LoadTable();
//    }
//    else {
//        Escorts.List();
//        //// Add your cancel button event handling here
//        //if (isChecked) {
//        //    $(_this).addClass("activeSlide");
//        //}
//        //else {
//        //    $(_this).removeClass("activeSlide");
//        //}
//    }

//});