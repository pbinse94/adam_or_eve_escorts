$(function () {
    $("#verifyButton").hide();
    let current_fs, next_fs, previous_fs; //fieldsets
    let opacity;
    let current = 1;
    let steps = $("fieldset").length;

    setProgressBar(current);

    $("button[name='signUpSubmit']").on('click', function () {
        if (!$('#TermsAndCondtion').is(':checked')) {
            common.eighteenPlusConsentShowPopup(true);
            return false;
        }

        if (!$("#frmSignup").valid()) {
            return false;
        }

        current_fs = $(this).parent();
        next_fs = $(this).parent().next();

        //Add Class Active
        $("#progressbar li").eq($("fieldset").index(next_fs)).addClass("active");

        //show the next fieldset
        //next_fs.show();
        ////hide the current fieldset with style
        //current_fs.animate({ opacity: 0 }, {
        //    step: function (now)
        //    {
        //        // for making fielset appear animation
        //        opacity = 1 - now;

        //        current_fs.css({
        //            'display': 'none',
        //            'position': 'relative'
        //        });
        //        next_fs.css({ 'opacity': opacity });
        //    },
        //    duration: 500
        //});
        //getSubscriptionPlans();
        if (!$('#progressbar li:eq(0)').hasClass('completed')) {
            $('#progressbar li:eq(0)').addClass('completed');
        }
    });

    //    $(".previous").on('click', function ()
    //    {
    //        current_fs = $(this).parent();
    //        previous_fs = $(this).parent().prev();

    //        //Remove class active
    //        $("#progressbar li").eq($("fieldset").index(current_fs)).removeClass("active");

    //        //show the previous fieldset
    //        previous_fs.show();

    //        //hide the current fieldset with style
    //        current_fs.animate({ opacity: 0 }, {
    //            step: function (now)
    //            {
    //                // for making fielset appear animation
    //                opacity = 1 - now;

    //                current_fs.css({
    //                    'display': 'none',
    //                    'position': 'relative'
    //                });
    //                previous_fs.css({ 'opacity': opacity });
    //            },
    //            duration: 500
    //        });
    //        setProgressBar(--current);
    //        if ($('#progressbar li:eq(0)').hasClass('completed'))
    //        {
    //            $('#progressbar li:eq(0)').removeClass('completed');
    //        }
    //    });

    function setProgressBar(curStep) {
        let percent = parseFloat(100 / steps) * curStep;
        percent = percent.toFixed();
        $(".progress-bar")
            .css("width", percent + "%")
    }

    $(".submit").on('click', function () {
        return false;
    })

    // Handle checkbox change event
    $("#TermsAndCondtion").on('change', function () {
        if ($(this).is(":checked")) {
            // Set the cookie if checkbox is checked
            setCookie("ageVerified", true, 365); // Expires in 1 year
        } else {
            // Remove the cookie if checkbox is unchecked
            eraseCookie("ageVerified");
        }
    });

    $('.modal#plan button.agreeBtn').on('click', function () {
        $("#TermsAndCondtion").prop("checked", true);
        common.eighteenPlusConsentShowPopup(false);
    });

});
let input = document.querySelector("#CountryCode");
let phonenumber = window.intlTelInput(input, {

    autoPlaceholder: "off",
    initialCountry: "au",
    showFlags: true,
    utilsScript: "/lib/intlTelInput/js/utils.js"
});
function signUp(_this) {
    $("#toast-container").remove();

    let phoneNumber = phonenumber.getSelectedCountryData().dialCode;

    $("#CountryCode").val('+' + phoneNumber);

    EnableDisableButton($(_this), true);

    if ($("#frmSignup").valid()) {
        $('#TermsAndCondtion').val('true');
        $.ajax({
            type: 'Post',
            url: "/Account/Registration",
            data: $("#frmSignup").serialize(),
            success: function (response) {

                toastr.success(response.message);
                setTimeout(function () {
                    if (response.data.userTypeId == 3) {
                        window.location.href = '/subscription/index';
                    }
                    else if (response.data.userTypeId == 4) {
                        window.location.href = '/subscription/index';
                    }
                    else {
                        window.location.href = "/Home/Index";
                    }
                }, 4000);
            },
            complete: function () {
                EnableDisableButton($(_this), false);
            },
            error: function (response) {
                if (response.status == 422) {
                    common.eighteenPlusConsentShowPopup(true);
                }
                else {
                    toastr.error(response.responseJSON?.message);
                }

                EnableDisableButton($(_this), false);
            }
        });
    }
    else {
        EnableDisableButton($(_this), false);
        return false;
    }
}

function sendVerifyCode(_this) {
    $("#toast-container").remove();
    EnableDisableButton($(_this), true);
    $.ajax({
        type: 'Post',
        url: "/Account/SendVerifyEmail",
        data: $("#frmSignup").serialize(),
        success: function (response) {
            toastr.success(response.message);
            $("#VerifyCodeModel").modal('show');
        },
        complete: function () {
            EnableDisableButton($(_this), false);
        },
        error: function (response) {
            toastr.error(response.responseJSON?.message);
            EnableDisableButton($(_this), false);
        }
    });
}

function verifyCode(_this) {
    $("#toast-container").remove();
    if ($("#verifyCode").val() == '') {
        toastr.error("Please enter 6 digit code.");
        return;
    }
    $.ajax({
        type: 'Post',
        url: "/Account/VerifyEmailCode",
        data: {
            Email: $("#frmSignup input[name='Email']").val(),
            Code: $("#verifyCode").val()
        },
        success: function (response) {
            toastr.success(response.message);
            $("#sendOtpButton").hide();
            $("#frmSignup input[name='Email']").attr("readonly", true);
            $("#verifyButton").show();
            $("#VerifyCodeModel").modal('hide');
        },
        complete: function () {
            EnableDisableButton($(_this), false);
        },
        error: function (response) {
            toastr.error(response.responseJSON?.message);
            EnableDisableButton($(_this), false);
            $("#sendOtpButton").show();
            $("#frmSignup input[name='Email']").attr("readonly", false);
            $("#verifyButton").hide();
        }
    });
}
