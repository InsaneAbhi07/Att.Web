$(function () {

    // mobile hamburger
    $("#mobile_btn").click(function (e) {
        e.preventDefault();
        $("body").toggleClass("sidebar-open");
    });

    // overlay close
    $("#sidebarOverlay").click(function () {
        $("body").removeClass("sidebar-open");
    });

    // desktop mini
    $("#toggle_btn").click(function (e) {
        e.preventDefault();
        $("body").toggleClass("mini-sidebar");
    });

    // submenu toggle
    $("#sidebar-menu > ul > li.submenu > a").click(function (e) {
        e.preventDefault();
        let parent = $(this).parent();
        parent.toggleClass("active");
        parent.children("ul").slideToggle(200);
    });

});
