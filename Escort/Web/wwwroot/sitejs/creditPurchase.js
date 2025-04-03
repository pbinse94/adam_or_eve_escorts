
function CalculateTotalPrice() {
    toastr.remove();

    let creditQuantity = $("#creditQuantity").val();
    let currency_Symbol = '$';
    
    if (isNaN(creditQuantity) || creditQuantity <= 0) {
        $("#error-msg").show();
        //toastr.error("Credit value should not be blank or zero");
        return false;
    }
    else {
        $("#error-msg").hide();
        EnableDisableButton($('#checkPriceButton'), true);
        $.ajax({
            type: 'GET',
            url: "/Credit/CalculateCreditPrice?creditQuantity=" + creditQuantity,
            success: function (response) {
                if (response.data > 0) {
                    $("#totalAmount").val(currency_Symbol + Number(response.data).toFixed(2).toLocaleString('en'));
                }
                else {
                    $("#totalAmount").val(currency_Symbol + Number(0.00).toLocaleString('en'));
                }

                EnableDisableButton($('#checkPriceButton'), false);
            },
            error: function (data) {
                if (data.responseJSON) {
                    toastr.error(data.responseJSON?.message);
                }
                else {
                    toastr.error(somethingWrongMsg);
                }
                EnableDisableButton($('#checkPriceButton'), false);
            }
        });
    }
}


function purchaseCredit()
{
    toastr.remove();
    let creditQuantity = $("#creditQuantity").val();
    if (isNaN(creditQuantity) || creditQuantity <= 0) {
        //toastr.error("Credit value should not be blank or zero");
        $("#error-msg").show();
        return false;
    }
    else {
        $("#error-msg").hide();
    }
        
    swal({
        title: 'Are you sure?',
        text: "Do you want to purchase "+ creditQuantity + " credit",
        type: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#EC881D',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes'
    }).then(function (result) {
        if (result) {
            EnableDisableButton($('#purchseCreditButton'), true);
            $('.Loading').show();
            $.ajax({
                url: 'Credit/CreateOrder',
                type: 'POST',
                data: { creditQuantity: creditQuantity },
                success: function (data)
                {
                    $('.Loading').hide();
                    if (data.redirect) {
                        window.location.href = data.redirect;
                    } else {
                        toastr.error('Failed to buy credit.');
                    }
                    EnableDisableButton($('#purchseCreditButton'), false);
                },
                error: function (xhr, status, error)
                {
                    $('.Loading').hide();
                    EnableDisableButton($('#purchseCreditButton'), false);
                    toastr.error('Failed to buy credit.');
                }
            });
        }
    });
}