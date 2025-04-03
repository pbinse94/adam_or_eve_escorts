


function openGalleryByIndex(index, element) {
    const galleryElement = document.getElementById("lightgallery");
    const galleryItems = Array.from($('#lightgallery .carousel-item'));

    // Ensure the index is within bounds
    if (index < 0 || index >= galleryItems.length) {
        console.error("Invalid index");
        return;
    }

    let imageArray = $('#lightgallery .lightgalleryimg').map(function () {
        return {
            src: $(this).attr('src')
        };
    }).get();


    // Open LightGallery starting from the specified index
    const dynamicGallery = lightGallery(galleryElement, {
        dynamic: true,
        dynamicEl: imageArray,
        index: index, // Start from the specified image
        controls: true,
        pager: false,
        download: false,
        mobileSettings: {
            controls: true,
            showCloseIcon: true,
            download: false,
            rotate: false,
        },
    });

    // Optional: Destroy gallery instance when closed to prevent duplication
    dynamicGallery.openGallery(index);
    galleryElement.addEventListener("onCloseAfter", () => {
        dynamicGallery.destroy();
    });
}
