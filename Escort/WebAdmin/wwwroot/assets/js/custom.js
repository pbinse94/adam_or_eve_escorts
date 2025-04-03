$(document).ready(function(){
    // function setheight() {
    //   var windowheight = window.innerHeight;
    //   var bodyheight = windowheight;
    //   var headerheight = $('.header').outerHeight();
    //   var footerheight = $('.footer').outerHeight();
    //   var mainbodyheight = bodyheight - footerheight - headerheight;
    //   $('.main-body').css({
    //     'min-height': mainbodyheight + 'px'
    //   });
    //   $('.content').css({
    //     'min-height': mainbodyheight + 'px'
    //   });
    // }

  //  setheight();
    // $(window).resize(function() {
    //   setheight();
    // });

    jQuery('.menu-btn').click(function(){
      if (jQuery('body').hasClass("menu-clicked")){
          jQuery('body').removeClass("menu-clicked");
        }
        else{
          jQuery('body').addClass("menu-clicked");
        }
      }
    );
});

$( function() {
  $( "#datepicker" ).datepicker();
} );
// forms
(function () {
  'use strict'
  var forms = document.querySelectorAll('.needs-validation')
  Array.prototype.slice.call(forms)
    .forEach(function (form) {
      form.addEventListener('submit', function (event) {
        if (!form.checkValidity()) {
          event.preventDefault()
          event.stopPropagation()
        }

        form.classList.add('was-validated')
      }, false)
    })
})()


// Tooltip
var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
  return new bootstrap.Tooltip(tooltipTriggerEl)
});

// popover
var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'))
var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
  return new bootstrap.Popover(popoverTriggerEl)
})