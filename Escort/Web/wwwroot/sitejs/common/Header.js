if (localStorage.getItem("token") != null) {

    var Token = localStorage.getItem("token");

    const options = {
        headers: {
            'Authorization': 'Bearer ' + Token
        }
    }
}
else {
    location.href=""
}

