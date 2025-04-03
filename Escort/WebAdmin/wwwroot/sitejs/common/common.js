$('input[type="text"], textarea').on('blur', function () {
    // Trim the value of the input and update it
    $(this).val($(this).val().trim());
});


$('.modal').on('hidden.bs.modal', function () {
    // Ensure modal backdrop is removed after hiding with fade animation
    $('.modal-backdrop').hide();

});

$('.modal').on('shown.bs.modal', function () {
    // Ensure modal backdrop is removed after hiding with fade animation
    $('.modal-backdrop').show();

});


function EnableDisableButton(element, action) {
    if (action) {
        $('.Loading').show();
        $(element).attr("disabled", "disabled");
    }
    else {
        $('.Loading').hide();
        $(element).removeAttr("disabled");
    }
}

$(document).on('keydown', '.numbersOnly', function (event) {
    if (event.shiftKey == true) {
        event.preventDefault();
    }

    if ((event.keyCode >= 48 && event.keyCode <= 57) ||
        (event.keyCode >= 96 && event.keyCode <= 105) ||
        event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 37 ||
        event.keyCode == 39 || event.keyCode == 46) {
    } else {
        event.preventDefault();
    }

    if ($(this).val().indexOf('.') !== -1 && event.keyCode == 190)
        event.preventDefault();
});

$('.numbersOnly').on("drag drop", function (e) {
    e.preventDefault();
});

$(document).on('keydown', '.decimalOnly', function (event) {
    if (event.shiftKey == true) {
        event.preventDefault();
    }

    if ((event.keyCode >= 48 && event.keyCode <= 57) ||
        (event.keyCode >= 96 && event.keyCode <= 105) ||
        event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 37 ||
        event.keyCode == 39 || event.keyCode == 46 || event.keyCode == 190 || event.keyCode == 110) {

        if ($(this).val().indexOf('.') !== -1 && (event.keyCode == 190 || event.keyCode == 110)) {
            event.preventDefault();
        }
        if ($(this).val().indexOf('.') !== -1) {
            var indexOfDecimal = $(this).val().indexOf('.');

            var decimalPartLength = ($(this).val().split('.')[1]).length;
            if (decimalPartLength >= 2 && this.selectionStart > indexOfDecimal && event.keyCode != 8 && event.keyCode != 46 && event.keyCode != 37 && event.keyCode != 39) {
                event.preventDefault();
            }
        }
    } else {
        event.preventDefault();
    }
}).on("drag drop", function (e) {
    e.preventDefault();
});

const common = {

    isReferrer: function () {
        let initreferrer = document.referrer;

        if (initreferrer == undefined || initreferrer == '' || initreferrer == window.location.origin + '/') {
            return true;
        }
        return initreferrer.toLowerCase() == window.location.href.toLowerCase();
    },
    validateDate: function () {
        let dtValue = $('.datepicker').val();
        let dtRegex = new RegExp("^([0]?[1-9]|[1-2]\\d|3[0-1])-(JAN|FEB|MAR|APR|MAY|JUN|JULY|AUG|SEP|OCT|NOV|DEC)-[1-2]\\d{3}$", 'i');
        return dtRegex.test(dtValue);
    },

    eighteenPlusConsentShowPopupToAccessSite: function (isShow) {
        const isShowText = isShow ? 'show' : 'hide';
        $('.modal#siteaccess').modal(isShowText);
    },

    eighteenPlusConsentShowPopup: function (isShow) {
        const isShowText = isShow ? 'show' : 'hide';
        $('.modal#plan').modal(isShowText);
    },

    getFile: function (pathToImage, element, container)
    {
        $.ajax({
            url: '/Home/GetDocumentUrl',
            type: 'GET',
            data: { pathToImage: pathToImage, mediaType: container },
            success: function (imageUrl)
            {
                if (container != "videos")
                {
                    if (imageUrl)
                    {
                        $(element).attr('src', imageUrl);
                    }
                    else if ($(element).data('dimension') == "vertical")
                    {
                        $(element).attr('src', '/assets/images/est-vt-default-profile.png');
                    }
                    else
                    {
                        $(element).attr('src', '/assets/images/est-ht-default-profile.png');
                    }
                }
                else
                {
                    $(element).attr('src', imageUrl);
                }

            },
            error: function (xhr, status, error)
            {

            }
        });
    },
    getCountries: function () {
        $.ajax({
            url: '/Home/GetHeaderCountries',
            type: 'GET',
            data: {},
            success: function (response) {
                $.when($('#countries').html(response)).then(function () {
                    setTimeout(() => {
                        $('#countries').show();
                    })
                    $('.multiple-items').slick({
                        infinite: true,
                        slidesToShow: 8,
                        slidesToScroll: 8,
                        variableWidth: true,
                    });
                    $('#countries a').filter('[data-country="Australia"]').addClass('active');
                });
            },
            error: function (xhr, status, error) {

            }
        });
    }


}



$(document).ajaxError(function (event, jqxhr, settings, exception) {
    if (jqxhr.status == 401) {
        toastr.error("Session Timeout");
        setTimeout(function () {
            location.href = "/Account/Login";
        }, 2000)
    }
});

$(function () {
    $('.modal#siteaccess button.agreeBtn').on('click', function () {
        // Set the cookie if checkbox is checked
        setCookie("ageVerified", true, 365); // Expires in 1 year
        $('.modal#siteaccess').modal('hide');
    });

    $('.modal#siteaccess button.declineBtn').on('click', function () {
        location.href = "https://google.com";
    });
});

function findError(htmlString) {
    var tempDiv = $('<div>').html(htmlString);
    var errorStatus = tempDiv.find('#errorStatus').val();
    return errorStatus;
}

function showDeactivePopup() {
    swal({
        title: 'Invalid Request',
        text: 'Your account has been deactivated. Please contact admin.',
        type: 'error',
        showCancelButton: false,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Okay',
        allowOutsideClick: false,
        allowEscapeKey: false
    }).then(function (result) {
        if (result) {
            location.href='/Account/Login'
        }
    })
}