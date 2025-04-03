let locationRequested = false;
let locationAccessDenied = false;
let userLatitude;
let userLongitude;


function allowLocation()
{

    //const selectedLocation = getCookie("userSelectedLocation");


    //if (selectedLocation) {
    //    alert(selectedLocation);
    //    activateCountryByCountryDropdown(selectedLocation);
    //}
    //else {
    //    alert(selectedLocation);



    const selectedLocation = getCookie("userSelectedLocation");

    if (!selectedLocation) {
        const savedLocation = getCookie("userLocation");

        if (savedLocation) {
            const [latitude, longitude] = savedLocation.split(",");
            userLatitude = parseFloat(latitude);
            userLongitude = parseFloat(longitude);
            getCountryByMaps();
        } else {
            // Initial call to get location
            getCurrentLocation();
            setInterval(function () {
                if (!locationRequested && !locationAccessDenied) {
                    getCurrentLocation();

                }
            }, 3000);
        }
    }
    else {
        activateCountryByCountryDropdown(selectedLocation);
    }
        
   // }
    
}


function getCurrentLocation()
{
     
    if (!locationRequested)
    {
        locationRequested = true;
        if ("geolocation" in navigator)
        {
            navigator.geolocation.getCurrentPosition(showPosition, showError, {
                enableHighAccuracy: true,
                timeout: 5000,
                maximumAge: 0
            });
        } else
        {
            console.log("Browser doesn't support geolocation!");
            locationRequested = false;
        }
    }
}

function showPosition(position)
{
    userLatitude = position.coords.latitude;
    userLongitude = position.coords.longitude;
    setCookie("userLocation", `${userLatitude},${userLongitude}`, 7); // Expires in 7 days

    getCountryByMaps();
}

function showError(error)
{
    switch (error.code)
    {
        case error.PERMISSION_DENIED:
            locationAccessDenied = true;
            console.error("User denied the request for Geolocation.");
            getCountryByMaps();
            break;
        case error.POSITION_UNAVAILABLE:
            //alert("Location information is unavailable.");
            break;
        case error.TIMEOUT:
            //alert("The request to get user location timed out.");
            break;
        case error.UNKNOWN_ERROR:
            //alert("An unknown error occurred.");
            break;
    }
    locationRequested = false;
}

function getCountryByMaps()
{
     

    let country = 'australia';
    let city = 'sydney';
    // alert("hello geo location");
    if (userLatitude !== undefined && userLongitude !== undefined)
    {
        let latitude = userLatitude;  // Example latitude
        let longitude = userLongitude;// Example longitude

        const url = `https://maps.googleapis.com/maps/api/geocode/json?latlng=${latitude},${longitude}&key=${mapsApiKey}`;
        fetch(url)
            .then(response => response.json())
            .then(data =>
            {
                if (data.status === 'OK')
                {
                    const addressComponents = data.results[0].address_components;


                    addressComponents.forEach(component =>
                    {
                        if (component.types.includes('administrative_area_level_3'))
                        {
                            city = component.long_name;
                        }
                        if (component.types.includes('country'))
                        {
                            country = component.long_name;
                        }
                    });


                    if (city != '' && country != '')
                    {
                        deactivateCountries();
                        let selectedCountry = $('#countries a').filter('[data-country="' + country.toLowerCase() + '"]');
                        $(selectedCountry[0]).addClass('active');
                        SetSelectedActiveCountryInDropdown();
                        let selectedCountrySlickIndex = $(selectedCountry[0]).closest('.slick-slide').data('slick-index');
                        $('#countries').slick('slickGoTo', selectedCountrySlickIndex);
                        if ($('section.featuredEscorts').length > 0)
                        {
                            getFilteredEscort(1, false);
                            getPopularEscort(country.toLowerCase());
                            getVipEscort(country.toLowerCase());
                        }
                    }

                } else
                {
                    deactivateCountries();
                    $('#countries a').filter('[data-country="australia"]').addClass('active');
                    SetSelectedActiveCountryInDropdown();
                    if ($('section.featuredEscorts').length > 0)
                    {
                        getFilteredEscort(1, false);
                        getPopularEscort('australia');
                        getVipEscort('australia');
                    }
                    console.error('Geocoding failed: ' + data.status);
                }
            })
            .catch(error =>
            {
                console.error('Error occurred: ', error);
            });
    }
    else
    {
        let selectedCountry = $('#countries a').filter('[data-country="' + country.toLowerCase() + '"]');
        $(selectedCountry[0]).addClass('active');

        SetSelectedActiveCountryInDropdown();

        let selectedCountrySlickIndex = $(selectedCountry[0]).closest('.slick-slide').data('slick-index');

        $('#countries').slick('slickGoTo', selectedCountrySlickIndex);
        getFilteredEscort(1, false);
        getPopularEscort(country.toLowerCase());
        getVipEscort(country.toLowerCase());
    }

}

function deactivateCountries()
{
    let activeCountries = $('#countries a.active')
    $.each(activeCountries, function (index, item)
    {
        $(item).removeClass('active');
    });
}
