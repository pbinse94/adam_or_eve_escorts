
$(window).scroll(function () {
    if ($(window).scrollTop() >= 40) {
        $('.top-header').addClass('fixedheader');
    }
    else {
        $('.top-header').removeClass('fixedheader');
    }
});

$(".js-example-placeholder-multiple").select2({
  placeholder: "Please select"
});
 


  $(".js-example-placeholder-multiple").select2({
    placeholder: "Please select"
  });






//sterp js
$(document).ready(function(){

    let current_fs, next_fs, previous_fs; //fieldsets
    let opacity;
    let current = 1;
    let steps = $("fieldset").length;
    
    setProgressBar(current);
    
    $(".next").click(function(){
    
            current_fs = $(this).parent();
            next_fs = $(this).parent().next();
    
            //Add Class Active
            $("#progressbar li").eq($("fieldset").index(next_fs)).addClass("active");
    
            //show the next fieldset
            next_fs.show();
            //hide the current fieldset with style
            current_fs.animate({opacity: 0}, {
            step: function(now) {
            // for making fielset appear animation
            opacity = 1 - now;
    
            current_fs.css({
            'display': 'none',
            'position': 'relative'
            });
            next_fs.css({'opacity': opacity});
            },
            duration: 500
        });
        setProgressBar(++current);
    });
    
    $(".previous").click(function(){
    
        current_fs = $(this).parent();
        previous_fs = $(this).parent().prev();
    
        //Remove class active
        $("#progressbar li").eq($("fieldset").index(current_fs)).removeClass("active");
    
        //show the previous fieldset
        previous_fs.show();
    
        //hide the current fieldset with style
        current_fs.animate({opacity: 0}, {
            step: function(now) {
                // for making fielset appear animation
                opacity = 1 - now;
    
                current_fs.css({
                'display': 'none',
                'position': 'relative'
            });
            previous_fs.css({'opacity': opacity});
            },
            duration: 500
        });
        setProgressBar(--current);
    });
    
    function setProgressBar(curStep){
        let percent = parseFloat(100 / steps) * curStep;
        percent = percent.toFixed();
        $(".progress-bar")
        .css("width",percent+"%")
    }
    
    $(".submit").click(function(){
        return false;
    })
    
    });
//sterp js