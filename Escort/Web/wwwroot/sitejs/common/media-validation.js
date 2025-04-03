function checkFileSizes(input)
{

    toastr.remove();

    let ImagevalidExt = [".jpg", ".png", ".jpeg", ".heic"];
    let ImagevalidExtmaxSize = 10 * 1024; // In kilobytes // 5120

    if (!input.files || !input.files[0])
    {

        return false;
    }

    let filePath = input.value;
    let getFileExt = filePath.substring(filePath.lastIndexOf('.') + 1).toLowerCase();

    let pos = ImagevalidExt.indexOf("." + getFileExt);
    if (pos < 0)
    {
        $(input).val('');
        toastr.error("Please upload an image in either .jpg, .png, .jpeg or .heic format.");
        return false;
    }

    let file = input.files[0];
    let sizeinmb = file.size / 1024; // In kilobytes
    if (sizeinmb > ImagevalidExtmaxSize)
    {
        $(input).val("");
        toastr.error("Uploaded image should be smaller than 10MB in size");
        return false;
    }
    return true;
}

