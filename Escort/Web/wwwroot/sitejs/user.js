function saveDetails(id, _this) {
    EnableDisableButton($(_this), true);
    let form = $(id)[0];
    let formData = new FormData(form);
    if ($(id).valid()) {
        $.ajax({
            type: "POST",
            url: '/ContactUs/SaveContactUsDetails',
            data: formData,
            contentType: false,
            cache: false,
            processData: false,
            success: function (response) {
                if (response.data) {
                    toastr.success(response.message);
                    setTimeout(function () {
                        location.href = "/Home/Index/";
                    }, setTimeoutIntervalEnum.contactUsSave);
                }
                EnableDisableButton($(_this), false);
            },
            error: function (response) {
                toastr.error(response.responseJSON?.message);
                EnableDisableButton($(_this), false);
            }
        })
    }
    else {
        EnableDisableButton($(_this), false);
    }
}

let IsImageSelected = false;
function UpdateProfile(_this) {
    EnableDisableButton($(_this), true);
    toastr.remove();
   
    if ($("#frmUpdateprofile").valid()) {
        let formSerializeData = $('#frmUpdateprofile').serialize();
        let formData = new FormData();
        $.each(formSerializeData.split('&'), function (index, field) {
            let fieldParts = field.split('=');
            formData.append(decodeURIComponent(fieldParts[0]), decodeURIComponent(fieldParts[1]));
        });


        $.ajax({
            type: 'POST',
            url: '/User/Profile',
            data: formData,
            cache: false,
            contentType: false,
            processData: false,
            success: function (response) {
                toastr.success(response.message)
                setTimeout(function () {
                    window.location.href = '/User/Profile';
                }, setTimeoutIntervalEnum.onRedirection);
            },
            error: function (data) {

                if (data.responseJSON) {
                    toastr.error(response.responseJSON?.message)
                    setTimeout(function () {
                        window.location.href = '/User/Profile';
                    }, setTimeoutIntervalEnum.onRedirection);
                }
                else {
                    toastr.error(somethingWrongMsg);
                }
                EnableDisableButton($(_this), false);
            }
        });
    }
    else {
        EnableDisableButton($(_this), false);
    }
}

function ChangePasswordUser(_this)
{
    EnableDisableButton($(_this), true);
    $("#ChangePasswordFrm").validate();
    toastr.remove();
    if ($("#ChangePasswordFrm").valid()) {
        $.ajax({
            type: 'Post',
            url: '/Profile/ChangePassword',
            data: $("#ChangePasswordFrm").serialize(),
            success: function (response)
            {
                swal({
                    title: "Thanks!",
                    text: response.message,
                    type: "success"
                }).then(function ()
                {
                    window.location.href = "/Account/Logout";
                });
                
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

function ShowProfileImage(fileInput) {
    let defaultImageUrl = $("#DefaultUserImage").val();
    if (fileInput.files.length > 0) {
        toastr.remove();

        let imageBox = document.getElementById("imgUser");
        imageBox.src = "";

        $("#imgUser").hide();
        if (fileInput.files && fileInput.files[0]) {
            if (imageBox != null) {
                $("#imgUser").show();
                imageBox.src = URL.createObjectURL(fileInput.files[0]);
            }
            else {
                $("#imgUser").hide();
            }
        }
        else {
            $("#imgUser").attr("src", "test.jpg");
        }

        if (fileInput.files.length != 0) {
            IsImageSelected = true;

            $("#images").parent().next(".text-danger").remove();
            $("#images").parent().after('<span class="arrMessage text-danger field-validation-valid" data-valmsg-for="ProfileImage" data-valmsg-replace="true"></span>');
        }
        else {
            IsImageSelected = false;

            $("#images").parent().next(".text-danger").remove();
            $("#images").parent().after('<span class="arrMessage text-danger field-validation-valid" data-valmsg-for="ProfileImage" data-valmsg-replace="true">Profile Image is required.</span>');
        }
        if (fileInput.files) {
            for (i = 0; i < fileInput.files.length; i++) {
                let file = fileInput.files[i];
                if (file) {
                    if (!file.type.match('image/jpeg') && !file.type.match('image/png')) {
                        $(fileInput).val("");
                        toastr.warning("Please upload image .png, .jpg only.");
                        IsImageSelected = false;
                        if ($("#Id").val() > 0)
                            $('#imgUser').attr('src', '');
                        else
                            $('#imgUser').attr('src', defaultImageUrl);
                        return false;
                    }
                }
            }
        }
        if ($("#images").val().length > 0) {
            let sizeInMB = (images.files[0].size / 1024 / 1024);
            if (sizeInMB > 5) {
                $(".clsbtnsmt").removeAttr("disabled")
                $(images).val("");
                toastr.error("Uploaded image should be smaller than 5MB in size");
                IsImageSelected = false;
                let imageBox = document.getElementById("imgUser");
                imageBox.src = $("#DefaultUserImage").val();
                $("#images").val("");
                return false;
            }
        }
    }
    else {
        //on cancel of browsing image window.
        $('#imgUser').attr('src', defaultImageUrl);
        $('#imgUser').css("display", "block");
        IsImageSelected = false;
    }
}