let defaultDate = new Date();
let fromDate = new Date();
let toDate = new Date();
fromDate.setDate(defaultDate.getDate() - 7); // Set to 7 days before
toDate.setDate(defaultDate.getDate()); // Set to Today date
$(function ()
{
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

    

    $("#txtFromDate").datepicker("setDate", fromDate);

    // Datepicker for "To Date"
    $("#txtToDate").datepicker({
        weekStart: 1,
        autoclose: true,
        todayHighlight: false,
        dateFormat: "dd-M-yy",
        changeMonth: true,
        changeYear: true,
        //maxDate: -1, // Prevent future dates
        minDate: fromDate,
        onSelect: function (selected)
        {
            const dt = new Date(selected);
            // Set minDate for "From Date" as the selected "To Date"
           // $("#txtFromDate").datepicker("option", "maxDate", dt);
        }
    });

    $("#txtToDate").datepicker("setDate", toDate);
    
    setTimeout(() =>
    {
        getEscortGiftSummary();

    })
})


function Reset()
{
    $("#txtFromDate").datepicker("setDate", fromDate);
    $("#txtToDate").datepicker("setDate", toDate);

    getEscortGiftSummary();
}

function getEscortGiftSummary()
{
    const fromDate = new Date($("#txtFromDate").val());
    const toDate = new Date($("#txtToDate").val());

    if (fromDate > toDate)
    {
        toastr.error('From date can not be greater than to date.');
        return false;
    }

    let request = {
        fromDate: $("#txtFromDate").val(),
        toDate: $("#txtToDate").val()
    };

    $.ajax({
        type: "POST",
        url: "/escort/GetEscortLiveCamGiftSummery",
        data: request,
        dataType: 'html',
        success: function (htmlData)
        {
            $.when($('#giftSummary').html(htmlData)).then(() =>
            {
                if ($('#giftSummary .giftRow').length > 0)
                {
                    $('#data-table_estGiftReport').dataTable();
                    setTimeout(() =>
                    {
                        sumOfTable();
                    })
                }
                
            })

        }
    });
}

function sumOfTable()
{
    let giftTable = $('#data-table_estGiftReport');

    let table = giftTable.DataTable();

    let totalAmount = 0;
    let totalClients = 0;
    let totalReceivedCredits = 0;
    let totalReceivedGift = 0;    
    
    table.rows().every(function ()
    {
        totalClients = totalClients + parseInt(this.data()[2]);
        totalReceivedCredits = totalReceivedCredits + parseInt(this.data()[3]);
        totalReceivedGift = totalReceivedGift + parseInt(this.data()[4]);
        totalAmount = totalAmount + parseFloat(this.data()[5].replace('$', ''));
    });

    $('#total_clients').text(totalClients);
    $('#total_receivedcredits').text(totalReceivedCredits);
    $('#total_giftcredits').text(totalReceivedGift);
    $('#total_amount').text("$" + Number(totalAmount).toFixed(2).toLocaleString('en'));
}

