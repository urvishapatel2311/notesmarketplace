/*$(function () {
    $("#btn-publish").click(function (event) {
        event.preventDefault();
        $('<div title="Confirm Box"></div>').dialog({
            open: function (event, ui) {
                $(this).html("Yes or No question?");
            },
            close: function () {
                $(this).remove();
            },
            resizable: false,
            height: 140,
            modal: true,
            buttons: {
                'Yes': function () {
                    $(this).dialog('close');
                    $.post('~/Views/user/Login.cshtml');

                },
                'No': function () {
                    $(this).dialog('close');
                    $.post('~/Views/user/AddNote.cshtml');
                }
            }
        });
    });
});



document.getElementById("sell-p").value = "0";
function Sell() {
    document.getElementById("sell-p").removeAttribute("readonly", "readonly");
}
function Free() {
    document.getElementById("sell-p").setAttribute("readonly", "readonly");
    document.getElementById("sell-p").value = "0";
}*/
/* ==========================================================
            sell price  textbox enables/disable
============================================================
$(function () {
    $("input[name='role']").click(function () {
        if ($("#paid").is(":checked")) {
            $("#sell-p").removeAttr("disabled");
            $("#sell-p").focus();
        } else {
            $("sell-p").attr("disabled", "disabled");
        }
    });
});

function EnableDisableTextBox() {
    var chkYes = document.getElementById("paid");
    var txtPassportNumber = document.getElementById("sell-p");
    txtPassportNumber.disabled = chkYes.checked ? false : true;
    if (!txtPassportNumber.disabled) {
        txtPassportNumber.focus();
    }
}*/
/* ==================================
            Compare password
====================================*/

var password = document.getElementById("password")
    , confirm_password = document.getElementById("cnfmpwd");

function validatePassword() {
    if (password.value != confirm_password.value) {
        /*$("#register_status").html("Passwords do not match!");*/
        confirm_password.setCustomValidity("Passwords Don't Match");
    } else {
       /* $("#divCheckPassword").html(" ");*/
        confirm_password.setCustomValidity('');
    }
}
password.onchange = validatePassword;
confirm_password.onkeyup = validatePassword;

/* ===================================================
              password toggle visibility
====================================================*/
function pwdVisible() {
    var x = document.getElementById("password");
    if (x.type === "password") {
        x.type = "text";
    } else {
        x.type = "password";
    }
}

function confirmPwdVisible() {
    var y = document.getElementById("cnfmpwd");
    if (y.type === "password") {
        y.type = "text";
    } else {
        y.type = "password";
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
            $(".navbar-movable .navbar-brand img").attr("src", "~/Content/images/logo/logo.png");

        } else {

            // Hide White nav
            $(".navbar-movable").removeClass("white-nav-top");

            // Show logo
            $(".navbar-movable .navbar-brand img").attr("src", "~/Content/images/logo/top-logo.png");

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
        /In Anchor tag Click first resetting all the background colors & padding to hack the BootStrap look n feel/

        /* $('.accordion-toggle').parent().css('background-color', '#f5f5f5');
         $('.accordion-toggle').parent().parent().removeClass('panel-heading');
         $('.accordion-toggle').parent().css('padding', '10px 15px');
         /If the respective content panel is visible, updating its background color/*/
        if (!$($(this).attr('href')).is(':visible')) {
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
        $(this).prev(".panel-heading").find(".add-i").removeAttr("src", "~/Content/images/icon-image/add.png").attr("src", "~/Content/images/icon-image/minus.png");

    });
    $(".collapse").on('hide.bs.collapse', function () {
        $(this).prev(".panel-heading").find(".minus-i").removeAttr("src", "~/Content/images/icon-image/add.png").attr("src", "~/Content/images/icon-image/minus.png");
    });
});
