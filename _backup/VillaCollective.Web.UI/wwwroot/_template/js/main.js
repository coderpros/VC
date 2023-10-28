// Header-sticky
$(window).on('scroll',function() {
    if ($(this).scrollTop() >0){  
        $('.header-sticky').addClass("is-sticky");
    }
    else{
        $('.header-sticky').removeClass("is-sticky");
    }
});

// Header Active
$(document).ready(function () {
    var path = window.location.href; 
    $('ul a').each(function() {
        if (this.href === path) {
            $(this).addClass('active');
        }
    });
});

//Search Dropdown 
$(document).ready(function(){
    $('#search').click(function() {
        $('.home-filter').slideToggle("slow");
    });
});

//Search Sub Filter Dropdown 
$(document).ready(function(){
    $('#search').click(function() {
        $('.sub-filter').slideToggle("slow");
    });
});


//Search Dropdown 
$(document).ready(function(){
    $('#search').click(function() {
        $('.vila-filter').slideToggle("slow");
    });
});

// Month View
/*$(document).ready(function(){
    $('#want-to-go').click(function() {
      $('.month-view').slideToggle("slow");
    });
});*/

// Month View
$(document).ready(function () {
    $('#want-to-go').click(function () {
        $('.month-view').slideToggle("slow", function () {
            const propval = $('.month-view').css("display");
            if (propval == "block") {
                $('#span-daterange').focus();
            }
            else {
                $(".custom-daterange").css('display', 'none');
            }
        });
    });
});

//Country Code Selection
$("#mobile_code").intlTelInput({
    initialCountry: "in",
    separateDialCode: true,
    utilsScript: "https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/11.0.4/js/utils.js"
});

//Filter Dropdown
$(function ($) {
    $(".filter-dropdown > a").click(function() {
    $(".filter-menu").slideUp(200);
        if (
        $(this)
            .parent()
            .hasClass("active")
        ) {
        $(".filter-dropdown").removeClass("active");
        $(this)
            .parent()
            .removeClass("active");
        } else {
        $(".filter-dropdown").removeClass("active");
        $(this)
            .next(".filter-menu")
            .slideDown(200);
        $(this)
            .parent()
            .addClass("active");
        }
    });
});

// Destinations Slider
$('#destinations').owlCarousel({
    loop:true,
    margin:20,
    autoWidth:true,
    center:true,
    nav: true,
    navText: [
        "<i class='fa-solid fa-angle-left'></i>",
        "<i class='fa-solid fa-angle-right'></i>"
    ],
    dots: false,
    autoplay: true,
    autoplayTimeout: 4000,
    smartSpeed: 1000,
    autoplayHoverPause: true,
    responsiveClass:true,
    responsive:{
        0:{
            items:1
        },
        600:{
            items:4
        },
        1000:{
            items:5
        }
    }
});

// CURATED COLLECTIONS Slider
$('#curated-collections').owlCarousel({
    autoplay:true,
    loop:true,
    margin:30,
    nav:true,
    navText: [
        "<i class='fa-solid fa-angle-left'></i>",
        "<i class='fa-solid fa-angle-right'></i>"
    ],
    dots: false,
    mouseDrag: true,
    autoplayTimeout:4000,
    smartSpeed: 1000,
    responsiveClass: true,
    responsive:{
        0:{
            items:1
        },
        575:{
            items:2
        },
        768:{
            items:2
        },
        992:{
            items:3
        },
        1200:{
            items:4
        }
    }
});

// GREAT FOR FAMILIES Slider
$('#great-for-families-slider').owlCarousel({
    autoplay:true,
    loop:true,
    margin:30,
    nav:true,
    navText: [
        "<i class='fa-solid fa-angle-left'></i>",
        "<i class='fa-solid fa-angle-right'></i>"
    ],
    dots: false,
    mouseDrag: true,
    autoplayTimeout:4000,
    smartSpeed:1000,
    responsiveClass:true,
    responsive:{
        0:{
            items:1
        },
        575:{
            items:2
        },
        768:{
            items:2
        },
        992:{
            items:2
        },
        1000:{
            items:3
        }
    }
});

// Swimming Pool Slider
$('#swimming-pool-slider').owlCarousel({
    autoplay:true,
    loop:true,
    margin:30,
    nav:true,
    navText: [
        "<i class='fa-solid fa-angle-left'></i>",
        "<i class='fa-solid fa-angle-right'></i>"
    ],
    dots: false,
    mouseDrag: true,
    autoplayTimeout:4000,
    smartSpeed:1000,
    responsiveClass:true,
    responsive:{
        0:{
            items:1
        },
        575:{
            items:2
        },
        768:{
            items:2
        },
        992:{
            items:2
        },
        1000:{
            items:3
        }
    }
});

// Water Front Slider
$('#water-front-slider').owlCarousel({
    autoplay:true,
    loop:true,
    margin:30,
    nav:true,
    navText: [
        "<i class='fa-solid fa-angle-left'></i>",
        "<i class='fa-solid fa-angle-right'></i>"
    ],
    dots: false,
    mouseDrag: true,
    autoplayTimeout: 4000,
    smartSpeed: 1000,
    responsiveClass: true,
    responsive:{
        0:{
            items:1
        },
        575:{
            items:2
        },
        768:{
            items:2
        },
        992:{
            items:2
        },
        1000:{
            items:3
        }
    }
});

// Suitable For Events Slider
$('#suitable-for-events-slider').owlCarousel({
    autoplay:true,
    loop:true,
    margin:30,
    nav:true,
    navText: [
        "<i class='fa-solid fa-angle-left'></i>",
        "<i class='fa-solid fa-angle-right'></i>"
    ],
    dots: false,
    mouseDrag: true,
    autoplayTimeout:4000,
    smartSpeed:1000,
    responsiveClass:true,
    responsive:{
        0:{
            items:1
        },
        575:{
            items:2
        },
        768:{
            items:2
        },
        992:{
            items:2
        },
        1000:{
            items:3
        }
    }
});

// Urban Slider
$('#urban-slider').owlCarousel({
    autoplay:true,
    loop:true,
    margin:30,
    nav:true,
    navText: [
        "<i class='fa-solid fa-angle-left'></i>",
        "<i class='fa-solid fa-angle-right'></i>"
    ],
    dots: false,
    mouseDrag: true,
    autoplayTimeout:4000,
    smartSpeed:1000,
    responsiveClass:true,
    responsive:{
        0:{
            items:1
        },
        575:{
            items:2
        },
        768:{
            items:2
        },
        992:{
            items:2
        },
        1000:{
            items:3
        }
    }
});

//Price Select Box
$('.select-dropdown-button').on('click', function(){
    var hasActive = $(this).next('.select-dropdown-list').hasClass('active');
    $('.select-dropdown-list').removeClass('active');
    $(this).next('.select-dropdown-list').toggleClass('active');
    hasActive && $(this).next('.select-dropdown-list').removeClass('active');
});

$('.select-dropdown-list-item').on('click', function(){
    var itemValue = $(this).data('value');
    //$('.select-dropdown-button').text($(this).text()).parent().attr('data-value', itemValue);
    $('.select-dropdown-list.active').prev('.select-dropdown-button').find('span').text($(this).text()).parent().attr('data-value', itemValue);
    $('.select-dropdown-list.active').toggleClass('active');
});

//Price Range
$('#price-filter').each( function() {
    var $filter_selector = $(this);
    var a = $filter_selector.data("min-value");
    var b = $filter_selector.data("max-value");
    var c = $filter_selector.data("price-sign");
    $filter_selector.slider({
        range: true,
        min: $filter_selector.data("min"),
        max: $filter_selector.data("max"),
        values: [ a, b ],
        slide: function( event, ui ) {
            $( "#flt-price" ).html( c + ui.values[ 0 ] + " - " + c + ui.values[ 1 ] );
            $( "#price-first" ).val(ui.values[ 0 ]);
            $( "#price-second" ).val(ui.values[ 1 ]);
        }
    });
    $( "#flt-price" ).html( c + $filter_selector.slider( "values", 0 ) + " - " + c + $filter_selector.slider( "values", 1 ) );
});

//Bedrooms Range
$('#bedrooms-filter').each( function() {
    var $filter_selector = $(this);
    var a = $filter_selector.data("min-value");
    var b = $filter_selector.data("max-value");
    var c = $filter_selector.data("bedrooms-sign");
    $filter_selector.slider({
        range: true,
        min: $filter_selector.data("min"),
        max: $filter_selector.data("max"),
        values: [ a, b ],
        slide: function( event, ui ) {
            $( "#flt-bedrooms" ).html(ui.values[ 0 ] + " - " +ui.values[ 1 ] );
            $( "#bedrooms-first" ).val(ui.values[ 0 ]);
            $( "#bedrooms-second" ).val(ui.values[ 1 ]);
        }
    });
    $( "#flt-bedrooms" ).html($filter_selector.slider( "values", 0 ) + " - " + $filter_selector.slider( "values", 1 ) );
});

//Filter On Off
$(document).on('click', '.filter-title', function() {
    $(this).parent()
        .toggleClass('filter-on')
        .toggleClass('filter-off');
});

//Journal Filter
$(function(){
    $('#journal-filter').mixItUp();
});

//Quantity Plus Minus
function wcqib_refresh_quantity_increments() {
    jQuery("div.quantity:not(.buttons_added), td.quantity:not(.buttons_added)").each(function(a, b) {
        var c = jQuery(b);
        c.addClass("buttons_added"), c.children().first().before('<input type="button" value="-" class="minus" />'), c.children().last().after('<input type="button" value="+" class="plus" />')
    })
}
String.prototype.getDecimals || (String.prototype.getDecimals = function() {
    var a = this,
        b = ("" + a).match(/(?:\.(\d+))?(?:[eE]([+-]?\d+))?$/);
    return b ? Math.max(0, (b[1] ? b[1].length : 0) - (b[2] ? +b[2] : 0)) : 0
}), jQuery(document).ready(function() {
    wcqib_refresh_quantity_increments()
}), jQuery(document).on("updated_wc_div", function() {
    wcqib_refresh_quantity_increments()
}), jQuery(document).on("click", ".plus, .minus", function() {
    var a = jQuery(this).closest(".quantity").find(".qty"),
        b = parseFloat(a.val()),
        c = parseFloat(a.attr("max")),
        d = parseFloat(a.attr("min")),
        e = a.attr("step");
    b && "" !== b && "NaN" !== b || (b = 0), "" !== c && "NaN" !== c || (c = ""), "" !== d && "NaN" !== d || (d = 0), "any" !== e && "" !== e && void 0 !== e && "NaN" !== parseFloat(e) || (e = 1), jQuery(this).is(".plus") ? c && b >= c ? a.val(c) : a.val((b + parseFloat(e)).toFixed(e.getDecimals())) : d && b <= d ? a.val(d) : b > 0 && a.val((b - parseFloat(e)).toFixed(e.getDecimals())), a.trigger("change")
});

// GREAT RENTED TOGETHER Slider
$('#great-rented-together-slider').owlCarousel({
    autoplay:true,
    loop:true,
    margin:30,
    nav:true,
    navText: [
        "<i class='fa-solid fa-angle-left'></i>",
        "<i class='fa-solid fa-angle-right'></i>"
    ],
    dots: false,
    mouseDrag: true,
    autoplayTimeout:4000,
    smartSpeed:1000,
    responsiveClass:true,
    responsive:{
        0:{
            items:1
        },
        575:{
            items:2
        },
        768:{
            items:2
        },
        992:{
            items:2
        },
        1000:{
            items:2
        }
    }
});

// WHAT OUR CUSTOMERS SAY Slider
$('#what-our-customers-say-slider').owlCarousel({
    autoplay:true,
    loop:true,
    margin:10,
    nav:true,
    navText: [
        "<i class='fa-solid fa-angle-left'></i>",
        "<i class='fa-solid fa-angle-right'></i>"
    ],
    dots: false,
    mouseDrag: true,
    autoplayTimeout:4000,
    smartSpeed:1000,
    responsiveClass:true,
    responsive:{
        0:{
            items:1
        }
    }
});

// YOU MIGHT ALSO LIKE Slider
$('#you-might-also-like-slider').owlCarousel({
    autoplay:true,
    loop:true,
    margin:30,
    nav:true,
    navText: [
        "<i class='fa-solid fa-angle-left'></i>",
        "<i class='fa-solid fa-angle-right'></i>"
    ],
    dots: false,
    mouseDrag: true,
    autoplayTimeout:4000,
    smartSpeed:1000,
    responsiveClass:true,
    responsive:{
        0:{
            items:1
        },
        575:{
            items:2
        },
        768:{
            items:2
        },
        992:{
            items:2
        },
        1000:{
            items:3
        }
    }
});

// GALLERY Slider
$('#gallery-slider').owlCarousel({
    autoplay:true,
    loop:true,
    margin:10,
    nav:true,
    navText: [
        "<i class='fa-solid fa-angle-left'></i>",
        "<i class='fa-solid fa-angle-right'></i>"
    ],
    dots: false,
    mouseDrag: true,
    autoplayTimeout:4000,
    smartSpeed:1000,
    responsiveClass:true,
    responsive:{
        0:{
            items:1
        }
    }
});

//Lightgallery
$(document).ready(function(){
    $('.lightgallery').lightGallery({
        share: false,
        size: false,
        autoplayControls: false,
        download: false
    });
});