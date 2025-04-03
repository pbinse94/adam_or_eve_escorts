let pageNo = 1;
$(function ()
{
    getPopularEscort(pageNo, false);

    $('#loadMoreBtn').on('click', function ()
    {
        pageNo += 1;
        getPopularEscort(pageNo, true)
    })
});

function getPopularEscort(pageNo, isAppend)
{   
    let search = $("#searchText").val();
    $('.Loading').show();

    $.ajax({
        url: '/Home/GetPopularEscorts',
        type: 'POST',
        data: { pageIndex: pageNo, searchText: search },
        success: function (result)
        {
            if (isAppend)
            {
                $.when($("#popularEscorts").append(result)).done(function ()
                {
                    showHideLoadMoreBtn(result, pageNo);
                    getEscortImages();
                });
            }
            else
            {
                $.when($("#popularEscorts").html(result)).done(function ()
                {
                    showHideLoadMoreBtn(result, pageNo);
                    getEscortImages();
                });
            }


            $('.Loading').hide();
        },
        error: function (xhr, status, error)
        {
            // Handle error response if needed
            $('.Loading').hide();
        }
    });

}

function getEscortImages()
{
    $($('.popularEscorts .escortsBlog')).each(function (index, element)
    {
        const escortImageElement = $(element).find('.escortImg');

        if ($(escortImageElement).data('src') != "")
        {
            common.getFile(`user/thumbnail_profile/${$(escortImageElement).data('src')}`, $(escortImageElement))

        }
    });
}

function showHideLoadMoreBtn(content, pageNo)
{
    if (content.trim() != "")
    {
        if (pageNo == 1)
        {
            if ($('#popularEscorts .escortsBlog').length == 12)
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
            const noResultFound = `<div class="availableUnder">
                                            No record found
                                        </div>`;
            $("#popularEscorts").html(noResultFound);
        }

        $('#loadMoreBtn').hide();
    }
}