

    let tourLocation = {
        postCode: '',
        subUrb: '',
        city: '',
        state: '',
        country: '',
        countryCode: '',
        latitude: '',
        longitude: '',
        location: ''
    }

    let baseLocation = {
        postCode: '',
        subUrb: '',
        city: '',
        state: '',
        country: '',
        countryCode: '',
        latitude: '',
        longitude: '',
        location: ''
    }

    let componentForm = {
        street_number: 'short_name',
        route: 'long_name',
        locality: 'long_name',
        administrative_area_level_1: 'short_name',
        country: 'long_name',
        postal_code: 'short_name'
    };

    const autoCompletes = [];

    let options = {
        componentRestrictions: { country: "au" }
    };
  

    const setAutoCompleteOnInput = (elementId) => 
    {
        const autoComplete = new google.maps.places.Autocomplete(document.getElementById(elementId));

        autoCompletes[elementId] = autoComplete;

        autoComplete.addListener('place_changed', function (event)
        {
            const place = autoComplete.getPlace();

            if (!place.geometry)
            {
                console.log('No details available for input: ' + place.name);
                return;
            }

            const addressComponents = place.address_components;

            const locationDetails = getLocationDetails(addressComponents);

            // Determine which input element triggered the event
            const inputElementId = Object.keys(autoCompletes).find(key => autoCompletes[key] === autoComplete);

            if (inputElementId == "setTourLocation")
            {
                tourLocation = {
                    postCode: locationDetails.postCode,
                    subUrb: locationDetails.subUrb,
                    state: locationDetails.state,
                    city: locationDetails.city,
                    country: locationDetails.country,
                    countryCode: locationDetails.countryCode,
                    latitude: place.geometry.location.lat(),
                    longitude: place.geometry.location.lng(),
                    location: place.formatted_address
                }
            }
            else if (inputElementId == "setBaseLocation")
            {
                baseLocation = {
                    postCode: locationDetails.postCode,
                    subUrb: locationDetails.subUrb,
                    state: locationDetails.state,
                    city: locationDetails.city,
                    country: locationDetails.country,
                    countryCode: locationDetails.countryCode,
                    latitude: place.geometry.location.lat(),
                    longitude: place.geometry.location.lng(),
                    location: place.formatted_address
                }
            }
        });
    }
    
    const getLocationDetails = (components) =>
    {
        let state = '';
        let city = '';
        let postCode = '';
        let subUrb = '';
        let country = '';
        let countryCode = '';

        components.forEach(component =>
        {
            const types = component.types;

            if (types.includes('postal_code'))
            {
                postCode = component.long_name;
            } else if (types.includes('locality') || types.includes('administrative_area_level_2'))
            {
                subUrb = component.long_name;
                city = component.long_name;
            } else if (types.includes('administrative_area_level_1'))
            {
                state = component.long_name;
            } else if (types.includes('country'))
            {
                country = component.long_name;
                countryCode = component.short_name;
            }
        });

        return { state, city, postCode, subUrb, country, countryCode };
    };

    