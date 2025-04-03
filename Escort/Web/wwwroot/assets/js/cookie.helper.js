////// Cookies

$(function ()
{
    const isAgeVerified = checkCookie("ageVerified");
    if (!isAgeVerified)
    {
        common.eighteenPlusConsentShowPopupToAccessSite(true);
    }
});


// Function to set a cookie
function setCookie(name, value, days)
{
    const expires = new Date(Date.now() + days * 24 * 60 * 60 * 1000);
    document.cookie = `${name}=${value}; expires=${expires.toUTCString()}; path=/`;
}

// Function to check for a cookie
function checkCookie(name)
{
    return document.cookie.split(';').some(cookie =>
    {
        const [cookieName, cookieValue] = cookie.trim().split('=');
        return cookieName === name && cookieValue === 'true';
    });
}

function getCookie(name)
{
    const nameEQ = name + "=";
    const ca = document.cookie.split(';');
    for (let i = 0; i < ca.length; i++)
    {
        let c = ca[i];
        while (c.charAt(0) === ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) === 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

// Function to erase a cookie
function eraseCookie(name)
{
    setCookie(name, "", -1);
}