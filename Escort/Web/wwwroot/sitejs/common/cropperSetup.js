function setupCropper(inputId, labelId)
{
    // cropper js code

    var inputUploadImage = document.getElementById(inputId);
    inputUploadImage.onclick = function ()
    {
        this.value = null;
    };

    var Cropper = window.Cropper;
    var URL = window.URL || window.webkitURL;
    var container = document.querySelector('.img-container');
    var image = container.getElementsByTagName('img').item(0);
    var download = document.getElementById('download');
    var actions = document.getElementById('actions');
    var dataX = document.getElementById('dataX');
    var dataY = document.getElementById('dataY');
    var dataHeight = document.getElementById('dataHeight');
    var dataWidth = document.getElementById('dataWidth');
    var dataRotate = document.getElementById('dataRotate');
    var dataScaleX = document.getElementById('dataScaleX');
    var dataScaleY = document.getElementById('dataScaleY');

    var options = {
        aspectRatio: 330 / 450,
        cropBoxResizable: true,
        preview: '.img-preview',
        dragMode: 'move',
        built: function ()
        {
            $("#image").cropper('setCropBoxData', { width: 640 });
        },
        cropmove: function ()
        {
            $("#image").cropper('setCropBoxData', { width: 640 });
        },
        ready: function (e)
        {

        },
        cropstart: function (e)
        {

        },
        cropmove: function (e)
        {

        },
        cropend: function (e)
        {

        },
        crop: function (e)
        {
            var data = e.detail;
            dataX.value = Math.round(data.x);
            dataY.value = Math.round(data.y);
            dataHeight.value = Math.round(data.height);
            dataWidth.value = Math.round(data.width);
            dataRotate.value = typeof data.rotate !== 'undefined' ? data.rotate : '';
            dataScaleX.value = typeof data.scaleX !== 'undefined' ? data.scaleX : '';
            dataScaleY.value = typeof data.scaleY !== 'undefined' ? data.scaleY : '';
        },
        zoom: function (e)
        {

        }
    };
    var cropper = new Cropper(image, options);

    var originalImageURL = image.src;
    var uploadedImageType = 'image/jpeg';
    var uploadedImageName = 'cropped.jpg';
    var uploadedImageURL;

    // Tooltip
    $('[data-toggle="tooltip"]').tooltip();


    // Methods
    actions.querySelector('.docs-buttons').onclick = function (event)
    {
        var e = event || window.event;
        var target = e.target || e.srcElement;
        var cropped;
        var result;
        var input;
        var data;

        if (!cropper)
        {
            return;
        }

        while (target !== this)
        {
            if (target.getAttribute('data-method'))
            {
                break;
            }

            target = target.parentNode;
        }

        if (target === this || target.disabled || target.className.indexOf('disabled') > -1)
        {
            return;
        }

        data = {
            method: target.getAttribute('data-method'),
            target: target.getAttribute('data-target'),
            option: target.getAttribute('data-option') || undefined,
            secondOption: target.getAttribute('data-second-option') || undefined
        };

        cropped = cropper.cropped;

        if (data.method)
        {
            if (typeof data.target !== 'undefined')
            {
                input = document.querySelector(data.target);

                if (!target.hasAttribute('data-option') && data.target && input)
                {
                    try
                    {
                        data.option = JSON.parse(input.value);
                    } catch (e)
                    {
                    }
                }
            }

            switch (data.method)
            {
                case 'rotate':
                    if (cropped && options.viewMode > 0)
                    {
                        cropper.clear();
                    }

                    break;

                case 'getCroppedCanvas':
                    try
                    {
                        data.option = JSON.parse(data.option);
                    } catch (e)
                    {
                    }

                    if (uploadedImageType === 'image/jpeg')
                    {
                        if (!data.option)
                        {
                            data.option = {};
                        }

                        data.option.fillColor = '#fff';
                    }

                    break;
            }

            result = cropper[data.method](data.option, data.secondOption);

            switch (data.method)
            {
                case 'rotate':
                    if (cropped && options.viewMode > 0)
                    {
                        cropper.crop();
                    }

                    break;

                case 'scaleX':
                case 'scaleY':
                    target.setAttribute('data-option', -data.option);
                    break;

                case 'getCroppedCanvas':

                    if (result)
                    {

                        if ($('#Image').length > 0)
                        {
                            $('#Image').val(result.toDataURL(uploadedImageType));

                        }
                        else if ($(labelId).length > 0)
                        {
                            $(labelId).val(result.toDataURL(uploadedImageType));
                        }
                        $(labelId).show();
                        $(labelId).attr("src", result.toDataURL(uploadedImageType));
                        $('#CroppedProfileFile').val(result.toDataURL(uploadedImageType));
                    
                        $('#myModal').modal('hide');
                   
                        document.getElementById("PixlateSpan").removeAttribute("hidden");
                        openModalVarify();
                    }
                    break;

                case 'destroy':
                    cropper = null;

                    if (uploadedImageURL)
                    {
                        URL.revokeObjectURL(uploadedImageURL);
                        uploadedImageURL = '';
                        image.src = originalImageURL;
                    }

                    break;
            }

            if (typeof result === 'object' && result !== cropper && input)
            {
                try
                {
                    input.value = JSON.stringify(result);
                } catch (e)
                {
                    //console.log(e.message);
                }
            }
        }
    };



    

    //Text watermark 

  //function drawWatermark() {
  //      const imgElement = document.getElementById('ProfileImage');
  //      const canvas = document.createElement('canvas');
  //      const ctx = canvas.getContext('2d');

  //      // Set canvas dimensions to match the image
  //      canvas.width = imgElement.naturalWidth;
  //      canvas.height = imgElement.naturalHeight;

  //      // Draw the image on the canvas
  //      ctx.drawImage(imgElement, 0, 0, canvas.width, canvas.height);

  //    const watermarkText = "ADAM OR EVE";
  //    const fontSize = 50;
  //    const fontFamily = "cursive";
  //    const textColor = "#ff71f1"; // Use the specified color
  //    const margin = 10;

  //    // Set font and text properties
  //    ctx.font = `${fontSize}px ${fontFamily}`;
  //    ctx.fillStyle = textColor;
  //    ctx.textAlign = "center"; // Center the text horizontally
  //    ctx.textBaseline = "middle"; // Center the text vertically
  //    ctx.globalAlpha = 0.1;

  //    // Calculate text position
  //    const x = canvas.width / 2; // Center horizontally
  //    const y = canvas.height / 2; // Center vertically

  //    // Calculate the angle for diagonal text (45 degrees)
  //    const angle = Math.PI / 4; // 45 degrees in radians

  //    // Save the current context state
  //    ctx.save();

  //    // Translate the context to the center of the canvas
  //    ctx.translate(x, y);

  //    // Rotate the context
  //    ctx.rotate(angle);

  //    // Draw the text at the center of the canvas
  //    ctx.fillText(watermarkText, 0, 0);

  //    // Restore the context to its original state
  //    ctx.restore();

  //      // Convert the canvas to a Base64-encoded image
  //       imgElement.src = canvas.toDataURL('image/jpeg');
  //    $('#CroppedProfileFile').val(canvas.toDataURL('image/jpeg'));
  //      // Log the Base64 image string to debug
       

  //      // You can now use the Base64 image string in your application as needed
  //      // For example, you might want to send this data to your server or use it elsewhere
  //  }
    document.body.onkeydown = function (event)
    {
        var e = event || window.event;

        if (e.target !== this || !cropper || this.scrollTop > 300)
        {
            return;
        }

        switch (e.keyCode)
        {
            case 37:
                e.preventDefault();
                cropper.move(-1, 0);
                break;

            case 38:
                e.preventDefault();
                cropper.move(0, -1);
                break;

            case 39:
                e.preventDefault();
                cropper.move(1, 0);
                break;

            case 40:
                e.preventDefault();
                cropper.move(0, 1);
                break;
        }
    };

    // Import image
    var inputImage = document.getElementById('inputImage');


    if (URL)
    {

        inputUploadImage.onchange = function ()
        {
            if (!checkFileSizes(this)) { return false; }

            var files = this.files;
            var file;
            let maxsize = 20.97152; //: 5 MB : //(5 * 1024 * 1024) / 500 / 500;


            let filePath = this.value;
            let getFileExt = filePath.substring(filePath.lastIndexOf('.') + 1).toLowerCase();

            $('#myModal').modal('show');
            if (files && files.length)
            {
                file = files[0];

                if (/^image\/\w+/.test(file.type) || getFileExt === "heic")
                {
                    uploadedImageType = file.type;
                    uploadedImageName = file.name;

                    if (uploadedImageURL)
                    {
                        URL.revokeObjectURL(uploadedImageURL);
                    }

                    if (getFileExt === "heic")
                    {
                        heic2any({
                            blob: file,
                            toType: "image/jpeg",
                        }).then(function (conversionResult)
                        {
                            image.src = URL.createObjectURL(conversionResult);
                            if (cropper)
                            {
                                cropper.destroy();
                            }
                            cropper = new Cropper(image, options);

                        }).catch(function (err)
                        {
                            console.error("Error converting HEIC file:", err);
                            window.alert('Error processing HEIC file.');
                            $('#myModal').modal('hide');
                        });

                    }
                    else
                    {
                        image.src = uploadedImageURL = URL.createObjectURL(file);
                        if (cropper)
                        {
                            cropper.destroy();
                        }
                        cropper = new Cropper(image, options);
                    }
                    inputImage.value = null;

                } else
                {
                    window.alert('Please choose an image file.');
                    $('#myModal').modal('hide');
                }
                // $('#myModal').modal('show');
            }

        };

    } else
    {
        inputImage.disabled = true;
        inputImage.parentNode.className += ' disabled';
    }
}



function applyWatermark(elementid) {
    return new Promise((resolve, reject) => {
        var imgElement = document.getElementById(elementid);
        if (elementid == "CroppedProfileFile" && imgElement.src == '') {
            imgElement = document.getElementById("ProfileImage");
            if ($('#' + elementid).val().length == 0) {
                resolve(true);
            } 
        }

        // Ensure image is loaded before applying watermark
        if (imgElement.complete && imgElement.naturalWidth !== 0) {
            drawWatermark(imgElement).then(() => resolve(true)).catch((error) => reject(error));
        } else {
            // Add onload event if image is not fully loaded
            imgElement.onload = () => {
                drawWatermark(imgElement).then(() => resolve(true)).catch((error) => reject(error));
            };
            imgElement.onerror = () => {
                console.error('Failed to load image');
                reject(new Error('Failed to load image'));
            };
        }
    });
}

// Image watermark
function drawWatermark(imgElement) {
    return new Promise((resolve, reject) => {
        const canvas = document.createElement('canvas');
        const ctx = canvas.getContext('2d');

        // Set canvas dimensions to match the image
        canvas.width = imgElement.naturalWidth;
        canvas.height = imgElement.naturalHeight;

        // Draw the image on the canvas
        ctx.drawImage(imgElement, 0, 0, canvas.width, canvas.height);

        // Load the watermark image
        const watermarkImg = new Image();
        watermarkImg.src = '../assets/images/watermark.png';  // Replace with the path to your watermark image

        watermarkImg.onload = function () {
            const watermarkWidth = canvas.width * 0.8; // Set watermark width to 80% of the canvas width
            const watermarkHeight = watermarkImg.height * (watermarkWidth / watermarkImg.width); // Maintain aspect ratio
            const x = (canvas.width - watermarkWidth) / 2; // Center horizontally
            const y = (canvas.height - watermarkHeight) / 2; // Center vertically

            // Set global alpha for transparency
            // ctx.globalAlpha = 0.5; // Adjust transparency as needed

            // Draw the watermark image
            ctx.drawImage(watermarkImg, x, y, watermarkWidth, watermarkHeight);

            // Convert the canvas to a Base64-encoded image
            imgElement.src = canvas.toDataURL('image/jpeg');
            if (imgElement.id == "ProfileImage") {
                $('#CroppedProfileFile').val(canvas.toDataURL('image/jpeg'));
            }
            resolve(true);
        };

        watermarkImg.onerror = () => {
            console.error('Failed to load watermark image');
            reject(new Error('Failed to load watermark image'));
        };
    });
}
