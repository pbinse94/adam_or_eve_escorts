$(function () {
    $.when(clientSpendingReport.ResetStateSaveDataTable()).then(function () {
        clientSpendingReport.List();
    });
});

let userstable = $('#data-table_userTransaction');

let clientSpendingReport = {
    ResetStateSaveDataTable: function () {
        if (common.isReferrer()) {
            userstable.DataTable().state.clear();
        }
    },
    List: function () {
        let columns = [];
        $(userstable).find('thead tr th').each(function (i, v) {

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
                
                case "pointpurchased":
                    column = {
                        width: '25%', class: 'dt-right',
                        data: null, name: propName, render: function (data, type, row, meta)
                        {
                            return data.pointsPurchased;
                        }
                    };
                    break;
                case "pointspurchased":
                    column = {
                        width: '25%', class: 'dt-right rpad-26',
                        data: null, name: propName, render: function (data, type, row, meta)
                        {
                            return data.pointsPurchased;
                        }
                    };
                    break;
                case "pointsbalance":
                    column = {
                        width: '25%', class: 'dt-right rpad-26',
                        data: null, name: propName, render: function (data, type, row, meta)
                        {
                            return data.pointsBalance;
                        }
                    };
                    break;
                case "pointsspent":
                    column = {
                        width: '25%', class: 'dt-right rpad-26',
                        data: null, name: propName, render: function (data, type, row, meta)
                        {
                            return data.pointsSpent;
                        }
                    };
                    break;
                case "amount":
                    column = {
                        width: '25%', class: 'dt-right rpad-26',
                        data: null, name: propName, render: function (data, type, row, meta)
                        {
                            return "$" + data.amount.toFixed(2).toLocaleString('en');
                        }
                    };
                    break;
                
                case "date":
                    column = {
                        data: null, // Assuming 'AddedOnUTC' is the key in your data source that holds the date string
                        name: propName,
                        width: '15%',
                        maxWidth: '15%',
                        render: function (data, type, row, meta) {
                            if (type === 'display' || type === 'filter') {
                                const date = new Date(data.date);
                                const day = ("0" + date.getDate()).slice(-2);
                                const month = ("0" + (date.getMonth() + 1)).slice(-2);
                                const year = date.getFullYear();
                                return day + '/' + month + '/' + year;
                            }
                            return data; // Return the original data for other types
                        }
                    };
                    break;
                default:
                    column = { data: propName.charAt(0).toLowerCase() + propName.slice(1), name: propName };

                    break;
            }

            columns.push(column);
        });

        let orderColumns;

        let filterBy = $('#filterBy').val();
        let currentId = $('#currentId').val();

        if (filterBy == 1) {
            orderColumns = [0,2];
        } else if (filterBy == 2) {
            orderColumns = [0];
        } else {
            orderColumns = [0];
        }

        let request = {
            UserId: currentId,
            FromDate: $("#txtFromDate").val(),
            ToDate: $("#txtToDate").val(),
            FilterBy: filterBy
        };

        tbl.BindTable(userstable, `/UserTokenTransaction/GetListByName`, request, columns, orderColumns);

        setTimeout(function () {
            clientSpendingReport.SumOfPoints(filterBy)
        }, 1000);
    },

    SumOfPoints: function (filterBy) {

        let table = userstable.DataTable();

        let totalPurchased = 0;
        let totalSpent = 0;
        let totalBalance = 0;
        let totalAmount = 0;
        
        table.rows().every(function () {
            if (filterBy == 1) {
                totalPurchased = parseInt(totalPurchased) + parseInt(this.data().pointsPurchased);
                totalAmount = totalAmount + this.data().amount;
            }
            else if (filterBy == 2) {
                totalSpent = parseInt(totalSpent) + parseInt(this.data().pointsSpent);
            } else {
                totalPurchased = parseInt(totalPurchased) + parseInt(this.data().pointsPurchased);
                totalSpent = parseInt(totalSpent) + parseInt(this.data().pointsSpent);
                totalBalance = parseInt(totalBalance) + parseInt(this.data().pointsBalance);
            }
        });

        if (filterBy == 1) {
            $('#total_purchase').text(totalPurchased);
            
            $('#total_amount').text("$" + Number(totalAmount).toFixed(2).toLocaleString('en'));
        }
        else if (filterBy == 2) {
            $('#total_spent_2').text(totalSpent);
        } else {
            $('#total_purchase').text(totalPurchased);
            $('#total_spent').text(totalSpent);
            $('#total_balance').text(totalPurchased - totalSpent);
        }
    }

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

    clientSpendingReport.List();
}

function Reset()
{
    $("#txtFromDate").val('');
    $("#txtToDate").val('');

    clientSpendingReport.List();
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