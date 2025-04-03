$(function ()
{
   
    common.getCountries(); 
    $(".js-example-placeholder-multiple").select2({
        placeholder: "Please select"
    });

    $('#SexualPreferences').select2(
        {
            placeholder: "Service I preferred"
        });

    $('#EscortCategories').select2(
        {
            placeholder: "Choose your sex partner"
        });

    let $rateSelect = $('#rateSelect');
    let step = 300;
    let start = 1;
    let end = 10000;

    for (let i = start; i <= end; i += step)
    {
        var rangeEnd = i + step - 1;

        // Skip if the range represents a single value
        if (i == rangeEnd)
        {
            continue;
        }

        // Ensure we don't have ranges that include single values at the end
        if (rangeEnd > end)
        {
            rangeEnd = end;
        }

        var text = `$${i} - $${rangeEnd}`;
        $rateSelect.append($("<option>", {
            value: `${i}-${rangeEnd}`,
            text: text
        }));
    }

    let $ageSelect = $('#ageSelect');
    let ageStep = 3;
    let ageStart = 18;
    let ageEnd = 50;

    for (let age = ageStart; age <= ageEnd; age += ageStep)
    {
        var ageRangeEnd = age + ageStep - 1;

        // Ensure we don't have ranges that include single values at the end
        if (ageRangeEnd > ageEnd)
        {
            ageRangeEnd = ageEnd;
        }

        var ageText = `${age} - ${ageRangeEnd}`;
        $ageSelect.append($('<option>', {
            value: ageText,
            text: ageText
        }));
    }

    $('#loadMoreBtn').on('click', function ()
    {
        pageNo += 1;
        getFilteredEscort(pageNo, true)
    })

    $('#searchText').on('keypress', function (e)
    {
        if (e.which == 13)
        {
            pageNo = 1;
            getFilteredEscort(pageNo, false)
        }
    });
});


function getEscortImages()
{
    $($('.escortsBlog')).each(function (index, element)
    {
        const escortImageElement = $(element).find('.escortImg');

        if ($(escortImageElement).data('src') != "")
        {
            common.getFile(`user/thumbnail_profile/${$(escortImageElement).data('src')}`, $(escortImageElement))

        }
    });
}

function getVipEscortImages()
{
    $($('.vipEscorts .escortsBlog')).each(function (index, element)
    {
        const escortImageElement = $(element).find('.escortImg');

        if ($(escortImageElement).data('src') != "")
        {
            common.getFile(`user/thumbnail_profile/${$(escortImageElement).data('src')}`, $(escortImageElement))

        }
    });
}

function SetSelectedActiveCountryInDropdown() { 
    //alert("hello 1");
    const activeElement = document.querySelector('.nav-link.active'); 
   // alert(activeElement);
    // Get the value of the 'data-country' attribute
    const activeCountryName = activeElement ? activeElement.getAttribute('data-country') : null;
   // alert(activeCountryName);
    // Find the value to set based on the active country name
    let selectedValue = null;
    $('#ddlCountry option').each(function () {
        if ($(this).val().length > 0) { 
        const [abbr, country] = $(this).val().split('|');
        
            if (country.toLowerCase() === activeCountryName.toLowerCase()) {
            selectedValue = $(this).val();  // Store the matching value
            return false;  // Break loop
            }
        }
    });
   // alert(selectedValue);
    // Set the selected value if found
    if (selectedValue) {
      //  $('#ddlCountry').val(selectedValue).trigger('change');
        $('#ddlCountry').val(selectedValue); 
        $('#ddlCountry').select2('destroy').select2({
            templateResult: formatCountry,
            templateSelection: formatCountry,
            placeholder: "Select Country"
        }); 
    } else {
        console.warn(`Country "${activeCountryName}" not found in dropdown options.`);
    }
}
let pageNo = 1;

//$(window).scroll(function ()
//{
//    if ($(window).scrollTop() + $(window).height() > $(document).height() - 100)
//    {
//        pageNo += 1;

//        getFilteredEscort(pageNo, true);
//    }

//});

function activateCountry(element)
{
    // alert(element);
    deactivateCountries();
    $(element).addClass('active');
    getFilteredEscort(1, false);
    getPopularEscort($(element).data('country'));
    getVipEscort($(element).data('country'));

    SetSelectedActiveCountryInDropdown();
}


function activateCountryByCountryDropdown(countryName) {

     
    const element = $(`a[data-country="${countryName.toLowerCase()}"]`);
    deactivateCountries();
    $(element).addClass('active');
    
    let selectedCountry = $('#countries a').filter('[data-country="' + countryName.toLowerCase() + '"]');
    $(selectedCountry[0]).addClass('active');
    let selectedCountrySlickIndex = $(selectedCountry[0]).closest('.slick-slide').data('slick-index');
    $('#countries').slick('slickGoTo', selectedCountrySlickIndex);

    getFilteredEscortByCountry(1, false, countryName); 

    getPopularEscort(countryName); 
    getVipEscort(countryName);
}


 

document.addEventListener('DOMContentLoaded', function ()
{
    let inputElement = document.getElementById('searchText');
    let clearIcon = document.querySelector('.clear_icons');    

    inputElement.addEventListener('input', function ()
    {
        if (inputElement.value.length > 0)
        {
            clearIcon.style.display = 'inline';
            $('#searchText').removeClass('search-input-home');
        } else
        {
            clearIcon.style.display = 'none';
            $('#searchText').addClass('search-input-home');
        }
    });

    clearIcon.addEventListener('click', function ()
    {
        $('#searchText').addClass('search-input-home');
        inputElement.value = '';
        clearIcon.style.display = 'none';
        $("#searchText").val('');
        getFilteredEscort(1,false);
    });
});

function getFilteredEscort(pageNo, isAppend)
{
    let ageGroup = $("#ageSelect").val();
    let sexualPreferences = $("#SexualPreferences").val();
    let escortCategories = $("#EscortCategories").val();
    let gender = $("#gender").val();
    let rates = $("#rateSelect").val();
    let search = $("#searchText").val();
    let selectedCountry = $('#countries a.active').data('country') || 'australia';

    
    let escortType = $("#escortType").val();
    let callType = $('#CallType').val();
    let profileType = $('#VerifiedProfile').val();

    $('.Loading').show();
    
    $.ajax({
        url: '/Home/FeaturedEscorts',
        type: 'POST',
        data: { ageGroup: ageGroup, sexualPreferences: sexualPreferences, escortCategories: escortCategories, gender: gender, rates: rates, search: search, location: selectedCountry, escortType: escortType, pageIndex: pageNo, callType: callType, profileType: profileType },
        success: function (result)
        {
            if (isAppend)
            {
                $.when($("#featuredEscorts").append(result)).done(function ()
                {
                    showHideLoadMoreBtn(result, pageNo);
                    getEscortImages();
                });
            }
            else
            {
                $.when($("#featuredEscorts").html(result)).done(function ()
                {
                    showHideLoadMoreBtn(result, pageNo);
                    getEscortImages();
                });
            }
                       

            $('.Loading').hide();
        },
        error: function (xhr, status, error)
        {
            // Handle error response if needed
            $('.Loading').hide();
        }
    });
    
}


function getFilteredEscortByCountry(pageNo, isAppend, countryName) {

    //alert(countryName);
    let ageGroup = $("#ageSelect").val();
    let sexualPreferences = $("#SexualPreferences").val();
    let escortCategories = $("#EscortCategories").val();
    let gender = $("#gender").val();
    let rates = $("#rateSelect").val();
    let search = $("#searchText").val();
    let selectedCountry = countryName || 'australia';
    let escortType = $("#escortType").val();

    $('.Loading').show();

    $.ajax({
        url: '/Home/FeaturedEscorts',
        type: 'POST',
        data: { ageGroup: ageGroup, sexualPreferences: sexualPreferences, escortCategories: escortCategories, gender: gender, rates: rates, search: search, location: selectedCountry, escortType: escortType, pageIndex: pageNo },
        success: function (result) {
            if (isAppend) {
                $.when($("#featuredEscorts").append(result)).done(function () {
                    showHideLoadMoreBtn(result, pageNo);
                    getEscortImages();
                });
            }
            else {
                $.when($("#featuredEscorts").html(result)).done(function () {
                    showHideLoadMoreBtn(result, pageNo);
                    getEscortImages();
                });
            }


            $('.Loading').hide();
        },
        error: function (xhr, status, error) {
            // Handle error response if needed
            $('.Loading').hide();
        }
    });

}

function getVipEscort(country)
{
    $.ajax({
        url: '/Home/GetVipEscorts',
        type: 'POST',
        data: { country: country },
        success: function (result)
        {
            $.when($(".vipEscorts").html(result)).done(function ()
            {
                getVipEscortImages();
            });


            $('.Loading').hide();
        },
        error: function (xhr, status, error)
        {
            // Handle error response if needed
            $('.Loading').hide();
        }
    });

}

function showHideLoadMoreBtn(content, pageNo)
{
    if (content.trim() != "")
    {
        if (pageNo == 1)
        {
            if ($('#featuredEscorts .escortsBlog').length == 12)
            {
                $('#loadMoreBtn').show();
            }
            else
            {
                $('#loadMoreBtn').hide();
            }
        }
        else
        {
            $('#loadMoreBtn').show();
        }
    }
    else
    {
        if (pageNo == 1)
        {
            const noResultFound = `<div class="availableUnder">
                                            No record found
                                        </div>`;
            $("#featuredEscorts").html(noResultFound);
        }

        $('#loadMoreBtn').hide();
    }
}

function getPopularEscort(countryName)
{
    $.ajax({
        url: '/Home/GetHomePopularEscorts',
        type: 'POST',
        data: { pageIndex: 1, countryName: countryName },
        success: function (result)
        {
            $.when($(".popularEscorts").html(result)).done(function ()
            {
                $('section.popularEscorts .carousel').slick({
                    dots: false, // Show dots for navigation
                    infinite: true, // Infinite loop
                    speed: 500, // Transition speed
                    centerMode: false,
                    slidesToShow: 4, // Number of slides to show
                    slidesToScroll: 1, // Number of slides to scroll
                    autoplay: true, // Enable auto play
                    autoplaySpeed: 3000, // Auto play speed (3 seconds)
                    prevArrow: '<button type="button" class="slick-prev">‹</button>', // Custom previous button
                    nextArrow: '<button type="button" class="slick-next">›</button>', // Custom next button
                    responsive: [
                        {
                            breakpoint: 1024,
                            settings: {
                                slidesToShow: 4,
                            }
                        },
                        {
                            breakpoint: 991,
                            settings: {
                                slidesToShow: 3,
                            }
                        },
                        {
                            breakpoint: 767,
                            settings: {
                                slidesToShow: 2,
                            }
                        },
                        {
                            breakpoint: 575,
                            settings: {
                                slidesToShow: 1,
                            }
                        }
                    ]
                });
                getPopularEscortImages();
            });
        },
        error: function (xhr, status, error)
        {
            // Handle error response if needed
        }
    });

}

function getPopularEscortImages()
{
    $($('.popularEscorts .escortsBlog')).each(function (index, element)
    {
        const escortImageElement = $(element).find('.escortImg');

        if ($(escortImageElement).data('src') != "")
        {
            common.getFile(`user/thumbnail_profile/${$(escortImageElement).data('src')}`, $(escortImageElement))

        }
    });
}