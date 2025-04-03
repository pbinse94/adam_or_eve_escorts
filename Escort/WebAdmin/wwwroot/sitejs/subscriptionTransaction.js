$(function () {
    $.when(subscriptionTransactions.ResetStateSaveDataTable()).then(function () {
        subscriptionTransactions.List();
    });
});

let MemberEmail = [];
let isValidate = true;
let table = $('#data-table_subscriptionTransaction');

let subscriptionTransactions = {

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
                        data: null, name: propName, mRender: function (data, row) {
                            return `${ data.userName }`;
                        }
                    };
                    break;
                case "transactionfee":
                    column = {
                        data: null, name: propName, className: "text-end", mRender: function (data, row) {
                            return `<div class="tableAccount">${data.transactionFee !== null ? parseFloat(data.transactionFee).toFixed(2) : "0.00"}</div>`;
                        }
                    };
                    break;
                case "transactionamount":
                    column = {
                        data: null, name: propName, className: "text-end", mRender: function (data, row) {
                            return `<div class="tableAccount">${data.transactionAmount !== null ? parseFloat(data.transactionAmount).toFixed(2) : "0.00"}</div>`;
                        }
                    };
                    break;
                default:
                    column = { data: propName.charAt(0).toLowerCase() + propName.slice(1), name: propName };
                    break;
            }

            columns.push(column);
        });
        
        let orderColumns = [0,8];
        let request = {
            SearchKeyword: $("#searchinput").val(),
            FromDate: $("#txtFromDate").val(),
            ToDate: $("#txtToDate").val(),
        }
        tbl.BindTable(table, `/SubscriptionTransactions/GetSubscriptionTransactions`, request, columns, orderColumns);
    },
    filterRecord: function () {
        subscriptionTransactions.List();

    },
}


function LoadSubscriptionTransactionsTable() {
    toastr.remove();

    const fromDate = new Date($("#txtFromDate").val());
    const toDate = new Date($("#txtToDate").val());

    if (fromDate > toDate) {
        toastr.error('From date can not be greater than to date.');
        return false;
    }

    subscriptionTransactions.List();
}

function ResetSubscriptionTransactionsTable() {
    $("#txtFromDate").val('');
    $("#txtToDate").val('');
    $("#searchinput").val('');
    subscriptionTransactions.List();
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
    onSelect: function (selected) {
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
    onSelect: function (selected) {
        const dt = new Date(selected);
        // Set minDate for "From Date" as the selected "To Date"
        $("#txtFromDate").datepicker("option", "maxDate", dt);
    }
});

$('#exportTransactionListButton').click(function () {
   
    var SearchKeyword = $('#searchinput').val();
    var FromDate = $("#txtFromDate").val();
    var ToDate = $("#txtToDate").val();


    // Create a hidden form
    var form = $('<form action="/SubscriptionTransactions/ExportSubscriptionTransactionsReport" method="GET"></form>');

    // Add a hidden input field for passing the data

    var searchInput = $('<input type="hidden" name="SearchKeyword" value="' + SearchKeyword + '">');
    var fromDate = $('<input type="hidden" name="FromDate" value="' + FromDate + '">');
    var toDate = $('<input type="hidden" name="ToDate" value="' + ToDate + '">');
    form.append(searchInput);
    form.append(fromDate);
    form.append(toDate);

    // Append the form to the body and submit it
    $('body').append(form);
    form.submit();

    // Remove the form after submission
    form.remove();
});

