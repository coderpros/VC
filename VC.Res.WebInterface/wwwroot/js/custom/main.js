//////////////////////////////////////////                              
//// GRIDSTA - FRONT END DEVELOPMENT STARTER KIT                   
//// Created by Jordan Sayner     
//// http://gridsta.jordansayner.co.uk                                
//////////////////////////////////////////

//////
// Global variable to turn console logs off etc.
// true / false
//////
var inProduction = "true";

(function (module, $, undefined) {

    ///////////////////
    // Detect Device
    ///////////////////
    module.detectDevice = function () {
        if (/android|webos|iphone|ipad|ipod|blackberry|iemobile|opera mini/i.test(navigator.userAgent)) {
            $('body').addClass('is-mobile');
        }
        else {
            $('body').addClass('is-desktop');
        }
    };


    ///////////////////
    // Profile Menu
    ///////////////////
    module.profileMenu = function () {
        $('[data-header-profile]').off('mouseenter.dataHeaderProfile');
        $('[data-header-profile]').on('mouseenter.dataHeaderProfile', function () {
            $('[data-header-profile-links]').addClass('js-active');

            $(".header__nav-dropdown").hide();
        });

        $(document).off('click.dataHeaderProfileLinks');
        $(document).on('click.dataHeaderProfileLinks', function (e) {
            if ($(e.target).closest('[data-header-profile-links]').length === 0) {
                $('[data-header-profile-links]').removeClass('js-active');
            }
        });
    };


    ///////////////////
    // HANDHELD NAVIGATION
    ///////////////////
    module.handheldNavigation = function () {
        $('[data-handheld-nav-toggle]').off('click.dataHandheldNavToggle');
        $("[data-handheld-nav-toggle]").on('click.dataHandheldNavToggle', function (e) {
            $(".handheld-navigation").toggleClass("js-active");
        });

        $('[data-has-dropdown]').off('click.dataHasDropdown');
        $("[data-has-dropdown]").on('click.dataHasDropdown', function (e) {
            $(".handheld-navigation__sub-list").removeClass("js-active");
            $(this).next().toggleClass("js-active");
        });
    };


    ///////////////////
    // NavBarComponent
    ///////////////////
    module.navBarComponent = function () {

        settings = {
            objHorzNav: ".header__nav",
            objHorzNavHasChild: ".header__nav-btn.has-dropdown",
            objHorzNavDropdown: ".header__nav-dropdown",
            objHorzNavDropdownItem: ".header__nav-dropdown-link",
            objHorzNavBreakpoint: 768
        };

        $(settings.objHorzNavHasChild).off('mouseenter.dataHeaderProfileLinks');
        $(settings.objHorzNavHasChild).on('mouseenter.dataHeaderProfileLinks', function (e) {
            if ($(e.target).closest(settings.objHorzNavDropdown).length === 0) {
                if ($(settings.objHorzNavDropdown, this).is(':hidden')) {
                    $(settings.objHorzNavDropdown).removeAttr('style');
                }
                $(settings.objHorzNavDropdown, this).toggle();
                $('[data-header-profile-links]').removeClass('js-active');
                return false
            }
        });

        $(settings.objHorzNavDropdownItem).off('click.navBarDropdownItem');
        $(settings.objHorzNavDropdownItem).on('click.navBarDropdownItem', function (e) {
            $(settings.objHorzNavDropdown).hide();
        });

        $(document).off('click.navBarComponent');
        $(document).on('click.navBarComponent', function (e) {
            if ($(e.target).closest(settings.objHorzNavDropdown).length === 0) {
                $(settings.objHorzNavDropdown).hide();
            }
        });

        $(window).off('resize.navBarComponent');
        $(window).on('resize.navBarComponent', function () {
            if ($(window).width() > settings.objHorzNavBreakpoint && $(settings.objHorzNav).is(':hidden')) {
                $(settings.objHorzNav).removeAttr('style');
            }
        });
    };


    ///////////////////
    // Sidebar
    ///////////////////
    module.sidebar = function () {
        if ($(".sidebar__wrapper").length) {

            var toggled = false;
            $("[data-sidebar-toggle]").off('click.dataSidebarToggle');
            $("[data-sidebar-toggle]").on('click.dataSidebarToggle', function () {

                if ($(this).find("use").attr('xlink:href') === '/imgs/icons/icons__defs.svg#icon__close') {
                    $(this).find("use").attr("xlink:href", "/imgs/icons/icons__defs.svg#icon__menu");
                }
                else {
                    $(this).find("use").attr("xlink:href", "/imgs/icons/icons__defs.svg#icon__close");
                }
                $("[data-sidebar]").toggleClass("js-hidden");

                var text = $(this).find(".sub-header__btn-text").text();
                $(this).find(".sub-header__btn-text").text(text === "Show" ? "Hide" : "Show");
            });

            function sidebarResize() {
                if ($(document).width() < 1024) {
                    var contentHeight = $(".main-content").outerHeight();
                    var sidebarHeight = contentHeight;

                    if ($("[data-sidebar-toggle]").find("use").attr('xlink:href') === '/imgs/icons/icons__defs.svg#icon__close') {
                        $("[data-sidebar-toggle]").find("use").attr("xlink:href", "/imgs/icons/icons__defs.svg#icon__menu");
                    }

                    $(".sidebar__wrapper").addClass("js-hidden");
                    $(".sidebar__wrapper").css({
                        height: sidebarHeight,
                        overflow: "hidden"
                    })

                    $("[data-sidebar]").mCustomScrollbar({
                        axis: "y",
                        scrollbarPosition: "outside"
                    });
                    $("[data-sidebar-toggle]").find(".sub-header__btn-text").text(text === "Show" ? "Hide" : "Show");
                }
                else {
                    var text = $(this).find(".sub-header__btn-text").text();

                    $("[data-sidebar-toggle]").find(".sub-header__btn-text").text(text === "Hide" ? "Show" : "Hide");

                    $("[data-sidebar]").mCustomScrollbar("destroy");
                    $(".sidebar__wrapper").removeClass("js-hidden");
                    $(".sidebar__wrapper").removeAttr("style");

                    if ($("[data-sidebar-toggle]").find("use").attr('xlink:href') === '/imgs/icons/icons__defs.svg#icon__menu') {
                        $("[data-sidebar-toggle]").find("use").attr("xlink:href", "/imgs/icons/icons__defs.svg#icon__close");
                    }
                }
            }

            $(document).off('ready.sidebar');
            $(document).on('ready.sidebar', sidebarResize);

            $(window).off('resize.sidebar');
            $(window).on('resize.sidebar', sidebarResize);
        }
    };


	///////////////////
	// TABLE SCROLL
	///////////////////
    module.tableScroll = function () {

        if ($("[data-table-scroll]").length) {

            $("[data-table-scroll]").off("scroll.tableScroll");
            $("[data-table-scroll]").on("scroll.tableScroll", function () {
                var currentScrollLeftPosition = $(this).scrollLeft();

                if (currentScrollLeftPosition === 0) {
                    $('.table__shadow-right').fadeIn("fast");
                    $('.table__shadow-left').fadeOut("fast");
                }
                else {
                    var maxScrollPosition = $(this)[0].scrollWidth - $(this).parent().outerWidth();

                    if (currentScrollLeftPosition === maxScrollPosition) {
                        $('.table__shadow-right').fadeOut("fast");
                        $('.table__shadow-left').fadeIn("fast");
                    } else {
                        $('.table__shadow-right').fadeIn("fast");
                        $('.table__shadow-left').fadeIn("fast");
                    }
                }

            });

            $(".table__shadow").off("click.tableScroll");
            $(".table__shadow").on("click.tableScroll", function () {
                var scrollContainer = $(".table__scroll"),
                    horizontalScroll;
                if ($(this).hasClass("table__shadow-right")) {
                    horizontalScroll = 200 + scrollContainer.scrollLeft();
                    scrollContainer.animate({
                        scrollLeft: horizontalScroll
                    });
                } else if ($(this).hasClass("table__shadow-left")) {
                    horizontalScroll = 200 - scrollContainer.scrollLeft();
                    scrollContainer.animate({
                        scrollLeft: -horizontalScroll
                    });
                }
            });

            // ONLOAD
            function onload() {
                var scrollTableWidth = $(".table__scroll").outerWidth();
                var tableWidth = $(".table").outerWidth();

                if (tableWidth >= scrollTableWidth) {
                    $('.table__shadow-right').fadeIn("fast");
                    $('.table__shadow-left').fadeOut("fast");
                } else {
                    $('.table__shadow-right').fadeOut("fast");
                    $('.table__shadow-left').fadeOut("fast");
                }
            }

            $(document).off('ready.tableScroll');
            $(document).on('ready.tableScroll', onload);

            $(window).off('resize.tableScroll');
            $(window).on('resize.tableScroll', onload);
        }
    };


    ///////////////////
    // TABLE DORPDOWN
    ///////////////////
    module.tableDropdown = function () {
        $('[data-table-toggle-dropdown]').off('mouseenter.tableDropdown');
        $('[data-table-toggle-dropdown]').on('mouseenter.tableDropdown', function () {
            //$(this).addClass('js-active');
            $('[data-table-dropdown]').removeClass('js-active');
            $(this).next('[data-table-dropdown]').addClass('js-active');
        });

        $(document).off('click.tableDropdown');
        $(document).on('click', function (e) {
            if ($(e.target).closest('[data-table-dropdown]').length === 0) {
                $('[data-table-dropdown]').removeClass('js-active');
            }
        });
    };


    ///////////////////
    // PANEL HEADER TOGGLE
    ///////////////////
    module.panelHeaderToggle = function () {

        $(document).off('click.panelHeaderrToggle', '[data-dropdown-toggle]');
        $(document).on('click.panelHeaderrToggle', '[data-dropdown-toggle]', function (e) {
            $(this).toggleClass("js-active");
            $(this).siblings("[data-dropdown]").toggleClass("js-active");

            return false;
        });

        $(document).off('click.panelHeaderrToggleOff');
        $(document).on('click.panelHeaderrToggleOff', function (e) {
            if ($(e.target).closest("[data-dropdown]").length === 0) {
                $("[data-dropdown-toggle]").removeClass("js-active");
                $("[data-dropdown]").removeClass("js-active");
            }
        });
    };

    module.setup = function() {
        // All Pages
        module.detectDevice();
        module.profileMenu();
        module.navBarComponent();
        module.sidebar();
        module.tableScroll();
        module.handheldNavigation();
        module.tableDropdown();
        module.panelHeaderToggle();

        // activate a timer for when session is about to expire
        sessMan.Refresh();
    };
}(window.module = window.module || {}, jQuery));



//////////////////////////////////////////                              
////                    
//// Testing Purposes only. 
//// Change "inProduction" varbile at top of page when in production
////                              
//////////////////////////////////////////
if(inProduction === "false") {

    $(document).click(function (event) {
        console.log($(event.target).attr('class'));  
    });

    var docWidth = document.documentElement.offsetWidth;

    [].forEach.call(
        document.querySelectorAll('*'),
        function (el) {
            if (el.offsetWidth > docWidth) {
                console.log(el);
                $(el).css('border', '1px solid #ff00ff');
            }
        }
    );
}





function SaveAsFile(filename, bytesBase64) {
    const anchorElement = document.createElement('a');
    anchorElement.download = filename;
    anchorElement.href = "data:application/octet-stream;base64," + bytesBase64;
    document.body.appendChild(anchorElement); // Needed for Firefox
    anchorElement.click();
    document.body.removeChild(anchorElement);
}

function TriggerFileDownload(filename, url) {
    const anchorElement = document.createElement('a');
    anchorElement.href = url;
    anchorElement.download = filename ?? '';
    document.body.appendChild(anchorElement); // Needed for Firefox
    anchorElement.click();
    document.body.removeChild(anchorElement);
    anchorElement.remove();
}

function ScrollToTop() {
    /*document.documentElement.scrollTop = 0;*/
    $('html,body').animate({ scrollTop: 0 }, 'fast');
}