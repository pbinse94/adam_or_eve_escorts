$(function () {
    $.when(payment.ResetStateSaveDataTable()).then(function () {
        payment.List();
    });
});

let paymenttable = $('#data-table_paymentReport');

let payment = {
    ResetStateSaveDataTable: function () {
        if (common.isReferrer()) {
            paymenttable.DataTable().state.clear();
        }
    },
    List: function () {
        let columns = [];
        $(paymenttable).find('thead tr th').each(function (i, v) {

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
                case "escortname":
                    column = {
                        data: null, name: propName, class: 'fw-semibold', mRender: function (data, type, full) {
                            return `<a href="/UserTokenTransaction/EscortPaymentReport?escortId=` + data.escortId + "&isPaid=" + false + "&adminPercentage=" + 25 + "&escortName=" + data.escortName + `"  style="text-decoration: underline;">${data.escortName}</a>`;
                        }
                    };
                    break;
                case "ispaymentdone":
                    column = {
                        data: null, name: propName, render: function (data, type, row, meta) {
                            // Enable or disable checkbox based on isPaymentDone value
                            let isDisabled = data.isPaymentDone ? 'disabled' : '';

                            return `
                        <input type="hidden" class="user-id-hidden" value="${data.escortId}" />
                         <div class="form-check mt-2">
                        <input type="checkbox" class="payment-checkbox form-check-input" data-id="${data.id}" ${isDisabled} ${data.isPaymentDone ? 'checked' : ''} />

                        </div>

                    `;
                        }
                    };
                    break;
                case "payment":
                    column = {
                        width: '15%',class:'dt-right',
                        data: null, name: propName, render: function (data, type, row, meta) {
                            return "$" + data.payment.toFixed(2).toLocaleString('en');
                        }
                    };
                    break;
                case "bankccountholdername":
                    column = {
                        width: '15%',
                        data: null, name: propName, render: function (data, type, row, meta) {
                            return data;
                        }
                    };
                    break;
                case "bsbnumber":
                    column = {
                        data: null, name: propName, render: function (data, type, row, meta) {
                            return data.bsbNumber;
                        }
                    };
                    break;
                default:
                    column = { data: propName.charAt(0).toLowerCase() + propName.slice(1), name: propName, width: '15%' };
                    break;
            }
            columns.push(column);
        });

        let orderColumns;
        if ($("#paidFilter").val() == 1) {
            orderColumns = [0, 5, 6];
        }
        else {
            orderColumns = [0, 1, 6, 7];
        }

        let request = {
            FromDate: $("#txtFromDate").val(),
            ToDate: $("#txtToDate").val(),
            PaidFilter: $("#paidFilter").val()
        };

        tbl.BindTable(paymenttable, `/UserTokenTransaction/PaymentReport`, request, columns, orderColumns);

        $('#selectAll').on('change', function () {
            let isChecked = $(this).is(':checked');
            paymenttable.find('.payment-checkbox').each(function () {
                if (!$(this).prop('disabled')) {
                    $(this).prop('checked', isChecked);
                }
            });
            updateSelectAllState();
        });

        setTimeout(function () {
            updateSelectAllState();
            payment.SumOfPoints()
        }, 1000);

    },
    SumOfPoints: function ()
    {

        let table = paymenttable.DataTable();

        let totalAmount = 0;

        table.rows().every(function ()
        {
            totalAmount = totalAmount + this.data().payment;
        });

        $('#total_amount').text("$" + Number(totalAmount).toFixed(2).toLocaleString('en'));
    }
}

function updateSelectAllState() {
    let allCheckboxes = paymenttable.find('.payment-checkbox');
    let checkedCheckboxes = allCheckboxes.filter(':checked');

    // Check if all non-disabled checkboxes are checked
    let allChecked = allCheckboxes.length > 0 && allCheckboxes.length === checkedCheckboxes.length;

    // Enable "Select All" if there are any checkboxes
    $('#selectAll').prop('disabled', allCheckboxes.length === 0)
        .prop('checked', allChecked);

    //console.log('All Checkboxes:', allCheckboxes.length);
    //console.log('Checked Checkboxes:', checkedCheckboxes.length);
    //console.log('Select All Checked:', allChecked);
}


function LoadTable() {
    toastr.remove();

    if ($("#txtFromDate").val() == '' && $("#txtToDate").val() == '') {
        toastr.error('"Please provide either a From Date or a To Date."');
        return false;
    }
    const fromDate = new Date($("#txtFromDate").val());
    const toDate = new Date($("#txtToDate").val());

    if (fromDate > toDate) {
        toastr.error('From date can not be greater than to date.');
        return false;
    }

    payment.List();
}

function Reset() {
    $("#txtFromDate").val('');
    $("#txtToDate").val('');

    payment.List();
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
        var dt = new Date(selected);
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
    maxDate: -1, // Prevent future dates
    onSelect: function (selected) {
        var dt = new Date(selected);
        // Set minDate for "From Date" as the selected "To Date"
        $("#txtFromDate").datepicker("option", "maxDate", dt);
    }
});






// Event listener for checkboxes
//$(document).on('change', '.payment-checkbox', function () {
//    let userId = $(this).closest('tr').find('.user-id-hidden').val(); // Get the userId from the hidden field in the same row
//    console.log(userId);

//    let hiddenField = $('#hiddenFieldId'); // Assuming this is the ID of the hidden field where you store the IDs

//    let currentValue = hiddenField.val();
//    let idArray = currentValue ? currentValue.split(',') : [];

//    if ($(this).is(':checked')) {
//        if (!idArray.includes(userId)) {
//            idArray.push(userId); // Add the userId if checked
//        }
//    } else {
//        idArray = idArray.filter(val => val !== userId); // Remove the userId if unchecked
//    }

//    hiddenField.val(idArray.join(',')); // Update the hidden field with the modified user IDs
//});



$(document).ready(function () {

    updateSelectAllState();

    // Handle the "Select All" checkbox
    $('#selectAll').on('change', function () {
        let isChecked = $(this).is(':checked');
        let hiddenField = $('#hiddenFieldId'); // Hidden field to store user IDs

        let idArray = []; // Array to keep track of selected user IDs

        paymenttable.find('.payment-checkbox').each(function () {
            if (!$(this).prop('disabled')) {
                $(this).prop('checked', isChecked); // Check or uncheck the checkbox

                let userId = $(this).closest('tr').find('.user-id-hidden').val(); // Get the userId from the same row
                if (isChecked) {
                    if (!idArray.includes(userId)) {
                        idArray.push(userId); // Add the userId if checked
                    }
                } else {
                    idArray = idArray.filter(val => val !== userId); // Remove the userId if unchecked
                }
            }
        });

        hiddenField.val(idArray.join(',')); // Update the hidden field with the modified user IDs
    });

    // Handle individual checkbox changes
    $(document).on('change', '.payment-checkbox', function () {
        let userId = $(this).closest('tr').find('.user-id-hidden').val(); // Get the userId from the same row
        let hiddenField = $('#hiddenFieldId'); // Hidden field to store user IDs

        let currentValue = hiddenField.val();
        let idArray = currentValue ? currentValue.split(',') : [];

        if ($(this).is(':checked')) {
            if (!idArray.includes(userId)) {
                idArray.push(userId); // Add the userId if checked
            }
        } else {
            idArray = idArray.filter(val => val !== userId); // Remove the userId if unchecked
        }

        hiddenField.val(idArray.join(',')); // Update the hidden field with the modified user IDs

        updateSelectAllState();
    });
});



function MarkPaymentDone() {
    toastr.remove();
    if ($("#hiddenFieldId").val() == '') {
        toastr.error('Please select users for payment done');
        return false;
    }

    swal({
        title: 'Are you sure?',
        text: "Do you want to mark payment done",
        type: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes'
    }).then(function (result) {
        if (result) {
            EnableDisableButton($('#markPayment'), true);
            let userids = $("#hiddenFieldId").val();
            $.ajax({
                url: '/UserTokenTransaction/MarkPaymentDone',
                type: 'POST',
                data: { userIds: userids },
                success: function (data) {

                    if (data.data > 0) {
                        toastr.success(data.message);
                    }
                    else {
                        toastr.error(data.message);
                    }
                    setTimeout(function () {
                        payment.List();
                    }, 1000);
                    EnableDisableButton($('#markPayment'), false);
                },
                error: function (xhr, status, error) {
                    EnableDisableButton($('#markPayment'), false);
                    toastr.error(data.responseJSON?.message);
                }
            });
        }
    });
}