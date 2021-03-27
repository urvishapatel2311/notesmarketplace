/* ===================================================
              password toggle visibility
====================================================*/
function changeVisible(el) {
    var x = $($(el).parent()).find("input.form-control");
    if ($(x).attr("type") === "password") {
        $(x).attr("type", "text");
    } else {
        $(x).attr("type", "password");
    }
}


/* ==================================
            Navigation
====================================*/

/* Show & Hide White Navigation */
$(function () {

    // show/hide nav on page load
    showHideNav();

    $(window).scroll(function () {

        showHideNav();

    });

    function showHideNav() {

        if ($(window).scrollTop() > 50) {

            //Show White nav
            $(".navbar-movable").addClass("white-nav-top");

            // Show dark logo
            $(".navbar-movable .navbar-brand img").attr("src", "/Content/images/logo/logo.png");

        } else {

            // Hide White nav
            $(".navbar-movable").removeClass("white-nav-top");

            // Show logo
            $(".navbar-movable .navbar-brand img").attr("src", "/Content/images/logo/top-logo.png");

        }
    }
});

/* ==================================
            Mobile Menu
====================================*/
$(function () {

    // Show Mobile nav
    $("#mobile-nav-open-btn").click(function () {
        $("#mobile-nav").css("height", "100%");
    });

    // Hide Mobile nav
    $("#mobile-nav-close-btn").click(function () {
        $("#mobile-nav").css("height", "0");
    });


});


/* ================================================
                    Add Review
===================================================*/
$(function () {
    $("a[class='Update']").click(function () {
        $("#review_popup").modal("show");
        return false;
    });
});

/* ================================================
                    Pagination
===================================================*/
$(function () {

    $('.next').click(function () {
        $('.pagination').find('.pgno.active').next().addClass('active');

        $('.pagination').find('.pgno.active').prev().removeClass('active');
    });

    $('.prev').click(function () {
        $('.pagination').find('.pgno.active').prev().addClass('active');

        $('.pagination').find('.pgno.active').next().removeClass('active');
    });

    $('.pgno').click(function () {
        $('.pagination').find('.pgno').removeClass('active');
        $('.pagination').find(this).addClass('active');
    });


});

/* ================================================
                    FAQ
===================================================*/

$(document).ready(function () {
    $('.accordion-toggle').click(function () {        
        if ($(this).hasClass("collapsed")) {
            $(this).parent().css('background-color', '#fff');
        } else {
            $(this).parent().css('background-color', '#f5f5f5');
        }
    });
});
$(document).ready(function () {
    // Add minus icon for collapse element which is open by default
    $(".collapse.show").each(function () {
        $(this).prev(".panel-heading").find(".icon").addClass("minus-i").removeClass("add-i");
    });

    // Toggle plus minus icon on show hide of collapse element
    $(".collapse").on('show.bs.collapse', function () {
        
        $(this).prev(".panel-heading").find(".add-i").removeAttr("src", "/Content/images/icon-image/add.png").attr("src", "/Content/images/icon-image/minus.png");
        $(this).prev(".panel-heading").find(".icon").addClass("minus-i").removeClass("add-i");

    });
    $(".collapse").on('hide.bs.collapse', function () {
        
        $(this).prev(".panel-heading").find(".minus-i").removeAttr("src", "/Content/images/icon-image/minus.png").attr("src", "/Content/images/icon-image/add.png");
        $(this).prev(".panel-heading").find(".icon").addClass("add-i").removeClass("minus-i");
    });
});
