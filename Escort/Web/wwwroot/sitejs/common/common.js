$('input[type="text"], textarea').on('blur', function () {
    // Trim the value of the input and update it
    $(this).val($(this).val().trim());
});


$('.modal').on('hidden.bs.modal', function ()
{
    // Ensure modal backdrop is removed after hiding with fade animation
    $('.modal-backdrop').hide();
    
});

$('.modal').on('shown.bs.modal', function ()
{
    // Ensure modal backdrop is removed after hiding with fade animation
    $('.modal-backdrop').show();
    
});


function EnableDisableButton(element, action) {    
    if (action)
    {
        $('.Loading').show();
        $(element).attr("disabled", "disabled");
    }
    else
    {
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

    isReferrer: function ()
    {
        let initreferrer = document.referrer;

        if (initreferrer == undefined || initreferrer == '' || initreferrer == window.location.origin + '/')
        {
            return true;
        }
        return initreferrer.toLowerCase() == window.location.href.toLowerCase();
    },
    validateDate: function ()
    {
        let dtValue = $('.datepicker').val();
        let dtRegex = new RegExp("^([0]?[1-9]|[1-2]\\d|3[0-1])-(JAN|FEB|MAR|APR|MAY|JUN|JULY|AUG|SEP|OCT|NOV|DEC)-[1-2]\\d{3}$", 'i');
        return dtRegex.test(dtValue);
    },

    eighteenPlusConsentShowPopupToAccessSite: function (isShow)
    {
        const isShowText = isShow ? 'show' : 'hide';
        $('.modal#siteaccess').modal(isShowText);
    },

    eighteenPlusConsentShowPopup: function (isShow)
    {
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
    getCountries: function ()
    {
        $.ajax({
            url: '/Home/GetHeaderCountries',
            type: 'GET',
            data: { },
            success: function (response)
            {
                $.when($('#countries').html(response)).then(function ()
                {
                    setTimeout(() =>
                    {
                        $('#countries').show();
                    })
                    $('.multiple-items').slick({
                        infinite: true,
                        slidesToShow: 8,
                        slidesToScroll: 8,
                        variableWidth: true,
                        responsive: [
                            {
                                breakpoint: 1200, 
                                settings: {
                                    slidesToShow: 6, 
                                    slidesToScroll: 6,
                                }
                            },
                            {
                                breakpoint: 992, 
                                settings: {
                                    slidesToShow: 4, 
                                    slidesToScroll: 4,
                                }
                            },
                            {
                                breakpoint: 768, 
                                settings: {
                                    slidesToShow: 2, 
                                    slidesToScroll: 2,
                                }
                            },
                            {
                                breakpoint: 576, 
                                settings: {
                                    slidesToShow: 1, 
                                    slidesToScroll: 1,
                                }
                            }
                        ]
                    });  

                    const selectedLocation = getCookie("userSelectedLocation");
                     
                    if (selectedLocation) {
                        $('#countries a').filter(`[data-country="${selectedLocation.toLowerCase()}"]`).addClass('active');
                    }
                    else {
                        $('#countries a').filter('[data-country="australia"]').addClass('active'); 
                    }
                    
                    SetSelectedActiveCountryInDropdown();
                    
                    allowLocation();
                });
                 
                 
            },
            error: function (xhr, status, error)
            {

            }
        });
    },
    getEmojiIcons: function ()
    {
        return [
            { title: 'smiley face', character: '😊' },
            { title: 'face blowing a kiss', character: '😘' },
            { title: 'heart eyes', character: '😍' },
            { title: 'smirking face', character: '😏' },
            { title: 'smiling face with horns', character: '😈' },
            { title: 'winking face with tongue', character: '😜' },
            { title: 'drooling face', character: '🤤' },
            { title: 'hot face', character: '🥵' },
            { title: 'face savoring food', character: '😋' },
            { title: 'face with hand over mouth', character: '🤭' },
            { title: 'OK hand', character: '👌' },
            { title: 'fire', character: '🔥' },
            { title: 'sparkling heart', character: '💖' },
            { title: 'heart with arrow', character: '💘' },
            { title: 'revolving hearts', character: '💞' },
            { title: 'baby bottle', character: '🍼' },
            { title: 'clinking beer mugs', character: '🍻' },
            { title: 'clinking glasses', character: '🥂' },
            { title: 'bottle with popping cork', character: '🍾' },
            { title: 'cupcake', character: '🧁' },
            { title: 'peach', character: '🍑' },
            { title: 'sweat droplets', character: '💦' },
            { title: 'eyes', character: '👀' },
            { title: 'lips', character: '🫦' },
            { title: 'tongue', character: '👅' },            
            { title: 'rocket', character: '🚀' },
            { title: 'wind blowing face', character: '🌬️' },
            { title: 'eggplant', character: '🍆' },
            { title: 'banana', character: '🍌' },
            { title: 'cocktail glass', character: '🍸' }, 
            { title: 'see-no-evil monkey', character: '🙈' },
            { title: 'kiss mark', character: '💋' },
            { title: 'hot pepper', character: '🌶️' },
            { title: 'bikini', character: '👙' },
            { title: 'wine glass', character: '🍷' },
            { title: 'party popper', character: '🎉' },
            { title: 'woman dancing', character: '💃' },
            { title: 'man dancing', character: '🕺' },
            { title: 'candy', character: '🍬' },            
            { title: 'dog', character: '🐶' },
            { title: 'skull', character: '💀' },
            { title: '18+', character: '🔞' },            
            { title: 'cherries', character: '🍒' },
            { title: 'ear of corn', character: '🌽' },
            { title: 'green apple', character: '🍏' },
            { title: 'grapes', character: '🍇' },
            { title: 'rainbow', character: '🌈' },
            { title: 'water wave', character: '🌊' },
            { title: 'high heel shoe', character: '👠' },            
            { title: 'strawberry', character: '🍓' },
            { title: 'ninja cat', character: '🐱‍👤' },
            { title: 'volcano', character: '🌋' },
        ];
    },
    favoriteIcon: function (_this)
    {
        let $this = $(_this);
        let id = $this.data("id");
        let isfav = $this.data("bool");

        $.ajax({
            url: '/profile/markFavorites',
            type: 'POST',
            data: { escortId: id },
            success: function (result)
            {
                // Handle success response if needed
                if (isfav)
                {
                    toastr.remove();
                    toastr.success("Favorites unmarked successfully.");

                    $this.find('img').attr("src", "/assets/images/favorite.svg");
                    $this.data("bool", false);
                } else
                {
                    toastr.remove();
                    toastr.success("Favorites marked successfully.");
                    $this.find('img').attr("src", "/assets/images/favorite_fi.svg");
                    $this.data("bool", true);
                }

            },
            error: function (xhr, status, error)
            {
                // Handle error response if needed
            }
        });
    }

}



$(document).ajaxError(function (event, jqxhr, settings, exception)
{
    if (jqxhr.status == 401) {
        toastr.error("Session Timeout");
        setTimeout(function () {
            location.href = "/Account/Login";
        }, 2000)
    }
});

$(function ()
{
    $('.modal#siteaccess button.agreeBtn').on('click', function ()
    {
        // Set the cookie if checkbox is checked
        setCookie("ageVerified", true, 365); // Expires in 1 year
        $('.modal#siteaccess').modal('hide');
    });

    $('.modal#siteaccess button.declineBtn').on('click', function ()
    {
        location.href = "https://google.com";
    });
});

 