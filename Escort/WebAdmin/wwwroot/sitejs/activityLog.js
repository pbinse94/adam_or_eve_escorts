$(function ()
{
    $.when(activityLogHistory.ResetStateSaveDataTable()).then(function ()
    {
        activityLogHistory.List();
    });
});

let MemberEmail = [];
let isValidate = true;
let table = $('#data-table_activityLogHistory');

let activityLogHistory = {

    ResetStateSaveDataTable: function ()
    {
        if (common.isReferrer())
        {
            table.DataTable().state.clear();
        }
    },

    List: function ()
    {
        let columns = [];
        $(table).find('thead tr th').each(function (i, v)
        {

            let propName = $(v).data('name');
            let column;

            switch (propName.toLowerCase())
            {
                case "id":
                    column = {
                        data: null, name: propName, width: "5%", render: function (data, type, row, meta)
                        {
                            return meta.row + meta.settings._iDisplayStart + 1;
                        }
                    };
                    break;

                default:
                    column = { data: propName.charAt(0).toLowerCase() + propName.slice(1), name: propName };
                    break;
            }

            columns.push(column);
        });

        let orderColumns = [0,3];
        let request = {
            AdminUserId: $("#AdminUserId").val(),
            FromDate: $("#txtFromDate").val(),
            ToDate: $("#txtToDate").val(),
        }
        tbl.BindTable(table, `/AdminUser/ActivityLogHistory`, request, columns, orderColumns);
    },
    filterRecord: function ()
    {
        activityLogHistory.List();

    },
}


function LoadTable()
{
    toastr.remove();

    if ($("#txtFromDate").val() == '' && $("#txtToDate").val() == '')
    {
        toastr.error('"Please provide either a From Date or a To Date."');
        return false;
    }
    const fromDate = new Date($("#txtFromDate").val());
    const toDate = new Date($("#txtToDate").val());

    if (fromDate > toDate)
    {
        toastr.error('From date can not be greater than to date.');
        return false;
    }

    activityLogHistory.List();
}

function Reset()
{
    $("#txtFromDate").val('');
    $("#txtToDate").val('');
    $("#AdminUserId").val('0');
    activityLogHistory.List();
}


// Datepicker for "From Date"
$("#txtFromDate").datepicker({
    weekStart: 1,
    autoclose: true,
    todayHighlight: false,
    dateFormat: "dd-M-yy",
    changeMonth: true,
    changeYear: true,
    maxDate: -1, // Prevent future dates
    onSelect: function (selected)
    {
        const dt = new Date(selected);
        // Set maxDate for "To Date" as the selected "From Date"
        $("#txtToDate").datepicker("option", "minDate", dt);
    }
});

// Datepicker for "To Date"
$("#txtToDate").datepicker({
    weekStart: 1,
    autoclose: true,
    todayHighlight: false,
    dateFormat: "dd-M-yy",
    changeMonth: true,
    changeYear: true,
    maxDate: 0, // Prevent future dates
    onSelect: function (selected)
    {
        const dt = new Date(selected);
        // Set minDate for "From Date" as the selected "To Date"
        $("#txtFromDate").datepicker("option", "maxDate", dt);
    }
});