$(function ()
{
    

    $('#loginFrm').on('keypress', function (e)
    {
        if (e.which == 13)
        {
            login($('#submitBtn'));
        }
    });

    $('#forgotPasswordFrm').on('keypress', function (e)
    {
        if (e.which == 13)
        {
            sendForgotPasswordEmail($('#submitBtn'));
        }
    });

    //document.getElementById('forgotPasswordFrm').addEventListener('submit', function (event)
    //{
    //    event.preventDefault(); // Prevent the default form submission
    //    // Your AJAX code to handle the input value and make the request
    //});
    
});

function setTimezoneCookie()
{
    let timezone_cookie = "timezoneoffset";
    // if the timezone cookie not exists create one.
    if (!$.cookie(timezone_cookie)) {
        // check if the browser supports cookie
        let test_cookie = 'test cookie';
        $.cookie(test_cookie, true);

        // browser supports cookie
        if ($.cookie(test_cookie)) {

            // delete the test cookie
            $.cookie(test_cookie, null);

            // create a new cookie
            $.cookie(timezone_cookie, -(new Date().getTimezoneOffset()));
        }
    }
    else {
        // if the current timezone and the one stored in cookie are different
        // then store the new timezone in the cookie and refresh the page.
        let storedOffset = parseInt($.cookie(timezone_cookie));
        let currentOffset = -(new Date().getTimezoneOffset());

        // user may have changed the timezone
        if (storedOffset !== currentOffset) {
            $.cookie(timezone_cookie, -(new Date().getTimezoneOffset()));
        }
    }
}

function sendForgotPasswordEmail(_this)
{
    $('#forgotErrorSpan').hide();
    EnableDisableButton($(_this), true);
    $("#toast-container").remove();
    if ($("#forgotPasswordFrm").valid()) {
        $.ajax({
            type: 'Post',
            url: "/Account/ForgetPassword",
            data: $("#forgotPasswordFrm").serialize(),
            dataType:'json',
            success: function (data)
            {
                toastr.success(data.message);
                $('#Email').val('');
            },
            error: function (data)
            {
                let error = '';
                if (data.responseJSON)
                {
                    error = data.responseJSON?.message;
                }
                else {
                    error = somethingWrongMsg;
                }
                toastr.error(error);
                EnableDisableButton($(_this), false);
            },
            complete: function ()
            {
                EnableDisableButton($(_this), false);
            }
        });
    }
    else {
        EnableDisableButton($(_this), false);
        return false;
    }
}

function resetPassword(_this) {
    EnableDisableButton($(_this), true);
    $("#toast-container").remove();
    let password = $("#Password").val().trim();
    if ($("#ResetPasswordFrm").valid() && password != "") {
        $.ajax({
            type: 'Post',
            url: "/Account/ResetPassword",
            data: $("#ResetPasswordFrm").serialize(),
            success: function (response) {
                toastr.success(response.message);
                setTimeout(function () {
                    window.location.href = "/Account/Login";
                }, setTimeoutIntervalEnum.onRedirection);
            },
            error: function (response) {
                toastr.error(response.responseJSON?.message);
                EnableDisableButton($(_this), false);
            },
            complete: function ()
            {
                EnableDisableButton($(_this), false);
            }
        });
    }
    else {
        EnableDisableButton($(_this), false);
        return false;
    }
}


function sendVerifyEmailLink(_this)
{
    EnableDisableButton($(_this), true);
    $("#toast-container").remove();

    if ($("#VerifyEmailFrm").valid()) {
        $.ajax({
            type: 'Post',
            url: "/Account/VerifyEmail",
            data: $("#VerifyEmailFrm").serialize(),
            success: function (data)
            {
                toastr.success(data.message);
                $('#loginmodel').modal('hide');
            },
            error: function (data) {
                if (data.responseJSON)
                {
                    toastr.error(data.responseJSON?.message);
                }
                else {
                    toastr.error(somethingWrongMsg);
                }
                EnableDisableButton($(_this), false);
            },
            complete: function ()
            {
                EnableDisableButton($(_this), false);
            }
        });
    }
    else {
        EnableDisableButton($(_this), false);
        return false;
    }
}

function login(_this)
{
    $("#toast-container").remove();
    $('#errorSpan').hide();

    EnableDisableButton($(_this), true);

    if ($("#loginFrm").valid())
    {
        $('.Loading').show();
        $.ajax({
            type: 'Post',
            url: "/Account/Login",
            data: $("#loginFrm").serialize(),
            success: function (response)
            {
                $('#login').modal('hide');
                if (response.data.userTypeId == 1) {
                    location.href = 'Admin/Dashboard/index';
                } else if (response.data.userTypeId == 4) {
                    window.location.href = '/establishment/index';
                }
                else {
                    if (response.apiName) {
                        window.location.href = response.apiName;
                    } else {
                        location.href = '/home/index';
                    }                
                }
            },
            complete: function ()
            {
                EnableDisableButton($(_this), false);
                $('.Loading').hide();
            },
            error: function (response)
            {
                if (response.responseJSON?.data.showEmailVerificationPopUp)
                {
                    $('.modal#login').modal('hide');
                    ShowVerifyEmailPopup();
                }
                $('#errorSpan').text(response.responseJSON?.message);
                $('#errorSpan').show();
                EnableDisableButton($(_this), false);
                $('.Loading').hide();
            }
        });
    }
    else
    {
        EnableDisableButton($(_this), false);
        return false;
    }
}

function ShowVerifyEmailPopup()
{
    $.ajax({
        type: "Get",
        url: '/Account/VerifyEmail',
        success: function (data)
        {
            $.when($('#loginmodelbody').html(data)).then(function ()
            {
                document.getElementById('VerifyEmailFrm').addEventListener('submit', function (event)
                {
                    event.preventDefault(); // Prevent the default form submission
                    // Your AJAX code to handle the input value and make the request
                });

                document.getElementById('VerifyEmailFrm').addEventListener('keypress', function (e)
                {
                    if (e.which == 13)
                    {
                        sendVerifyEmailLink($('#verificatonBtn'));
                    }
                });
            });
        }, complete: function (data)
        {
            $('#loginmodel').modal('show');
            $('.backdrop-modal').show();

        },
        error: function (data)
        {
        }
    });
}
function ClosePopup()
{
    $('#loginmodel').hide();
    $('.backdrop-modal').hide();
}

