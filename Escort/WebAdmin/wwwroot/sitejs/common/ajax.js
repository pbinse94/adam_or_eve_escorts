var ajax = {
    PostForm: function (url, data, callback) {
        $.ajax({
            type: 'POST',
            url: url,
            data: data,
            processData: false,
            contentType: false,
            cache: false,
            success: function (result) {
                return callback(result);
            },
            error: function (result) {
                $("#dvLoading").hide();
                return callback(result);
            }
        });
    },
    doGetAjax: function (url, callback) {
        $.ajax({
            type: 'Get',
            url: url,                    
            success: function (result) {
                return callback(result);
            },
            error: function (result) {
                $("#dvLoading").hide();
                toastr.remove();
                return callback(result);
            }
        });
    },

    Post: function (url, data, callback, errorCallback) {
        $.ajax({
            type: 'POST',
            url: url,
            data: data,      
            async: false,
            cache: false,           
            success: function (result) {
                return callback(result);
            },
            error: function (error) {
                $("#dvLoading").hide();
                toastr.remove();
                EnableDisableButton(this, false);
                toastr.error(error.responseJSON?.message)
                return errorCallback(error);
            }
        });
    }    
}
