$(function () {
    $.when(Establishments.ResetStateSaveDataTable()).then(function ()
    {
        Establishments.List();
    });
});


let Establishmenttable = $('#data-table_Establishments');

let Establishments = {

    ResetStateSaveDataTable: function () {
        if (common.isReferrer()) {
            Establishmenttable.DataTable().state.clear();
        }
    },

    List: function () {
        let columns = [];
        $(Establishmenttable).find('thead tr th').each(function (i, v) {

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
                        data: null, name: propName, class: 'fw-semibold',width:"15%", mRender: function (data, row) {
                            return `<a href="/Establishment/ProfileDetail?id=${data.id}"  >${data.name}</a>`; // return `${data.name}`;
                        }
                    };
                    break; 

                case "displayname":
                    column = {
                        data: null, name: propName, width: "15%", class: 'fw-semibold', mRender: function (data, row) {
                            // return `${data.name}`;
                            return `<a href="/Establishment/ProfileDetail?id=${data.id}"  >${data.displayName}</a>`;
                        }
                    };
                    break;
                case "isactive":
                    column = {
                        data: null, name: propName, width: "10%", mRender: function (data, row) {
                            
                            if (data.isActive) {
                                return `<div class="switchBtn">
                                            <input type="checkbox" hidden="hidden" id="checkbox_${data.id}" checked  onclick="ChangeEstablishmentStatus(` + data.id + `, false, true, this)">
                                            <label class="switch" for="checkbox_${data.id}"></label>
                                        </div>`;
                            }
                            else {
                                return `<div class="switchBtn">
                                          <input type="checkbox"  hidden="hidden" id="checkbox_${data.id}" onclick="ChangeEstablishmentStatus(` + data.id + `, true, true, this)">
                                          <label class="switch"  for="checkbox_${data.id}"></label>
                                       </div>`;
                            }
                        }
                    };
                    break;
                case "subscription":
                    column = {
                        data: null, name: propName, width:"10%", mRender: function (data, row) {
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

        let orderColumns = [0, 2, 3];

        data = {
            Country: $("#Country").val()  
        };
        tbl.BindTable(Establishmenttable, `/Establishment/EstablishmentList`, data, columns, orderColumns);
    },
    filterRecord: function () {
        Establishments.List();
        dataTable.search($('#seachinput').val().trim()).draw();
    }
}


function ChangeEstablishmentStatus(userId, activeStatus, isActivateStatusChange, _this) {

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
            textMessage = $("#activateEstablishmentMsg").val()
        }
        else {
            textMessage = $("#deactivateEstablishmentMsg").val()
        }
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
                url: '/Establishment/ChangeEstablishmentStatus',
                data:
                {
                    UserId: userId,
                    ActiveStatus: activeStatus
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
                        Establishments.List();
                    }, 1000);
                },
                error: function (response) {
                    toastr.error(response.responseJSON?.message);
                }
            });

        }
        else if (!result) {
            Establishments.List();
        }
    });
}