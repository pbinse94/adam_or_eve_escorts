
let pageNo = 1;

$(function ()
{
    getFavoriteEscort(pageNo);

    $('#loadMoreBtn').on('click', function ()
    {
        pageNo += 1;
        getFavoriteEscort(pageNo, true)
    })
});


function getEscortImages() {
    $($('.escortsBlog')).each(function (index, element) {
        const escortImageElement = $(element).find('.escortImg');

        if ($(escortImageElement).data('src') != "") {
            common.getFile(`user/thumbnail_profile/${$(escortImageElement).data('src')}`, $(escortImageElement))

        }
    });
}




//$(window).scroll(function () {
//    if ($(window).scrollTop() + $(window).height() > $(document).height() - 100) {
//        pageNo += 1;

//        getFavoriteEscort(pageNo, true);
//    }

//});

function getFavoriteEscort(pageNo, isAppend) { 
    let search = $("#searchText").val();
    $('.Loading').show();

    $.ajax({
        url: '/Home/FavoriteEscorts',
        type: 'GET',
        data: { search: search,   pageIndex: pageNo },
        success: function (result) {
            if (isAppend) {
                $.when($("#favoriteEscorts").append(result)).done(function ()
                {
                    showHideLoadMoreBtn(result, pageNo);
                    getEscortImages();
                });
            }
            else {
                $.when($("#favoriteEscorts").html(result)).done(function ()
                {
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

function showHideLoadMoreBtn(content, pageNo)
{
    if (content.trim() != "")
    {
        if (pageNo == 1)
        {
            if ($('#favoriteEscorts .escortsBlog').length == 12)
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
            const noResultFound = `<div class="col-lg-12">
                                        <div class="availableUnder">
                            No record found
                            </div>
                        </div>`;
            $("#favoriteEscorts").html(noResultFound);
        }

        $('#loadMoreBtn').hide();
    }
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
        } else
        {
            clearIcon.style.display = 'none';
        }
    });

    clearIcon.addEventListener('click', function ()
    {
        inputElement.value = '';
        clearIcon.style.display = 'none';
        $("#searchText").val('');
        getFavoriteEscort(1, false);
    });
});