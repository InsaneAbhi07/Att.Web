
$(function () {

	$("#mobile_btn").on("click", function (e) {
		e.preventDefault();
		e.stopPropagation();
		$("body").toggleClass("sidebar-open");

		if ($("body").hasClass("sidebar-open")) {
			$("body").css("overflow", "hidden");
		} else {
			$("body").css("overflow", "");
		}
	});

	$("#toggle_btn").on("click", function (e) {
		e.preventDefault();
		e.stopPropagation();
		$("body").toggleClass("mini-sidebar");

		if ($("body").hasClass("mini-sidebar")) {
			$(".menu-item.submenu").removeClass("active");
		}

		if (typeof (Storage) !== "undefined") {
			localStorage.setItem("sidebarMini", $("body").hasClass("mini-sidebar"));
		}
	});

	$("#sidebarOverlay").on("click", function () {
		$("body").removeClass("sidebar-open");
		$("body").css("overflow", "");
	});

	$(".menu-item.submenu > .menu-link").on("click", function (e) {
		e.preventDefault();
		e.stopPropagation();

		let parent = $(this).closest(".menu-item");
		let isActive = parent.hasClass("active");

		$(".menu-item.submenu").not(parent).removeClass("active");

		parent.toggleClass("active");

		if (!isActive && $(window).width() <= 768) {
			setTimeout(function () {
				let scrollTop = parent.offset().top - 100;
				$(".sidebar-scroll").animate({ scrollTop: scrollTop }, 300);
			}, 100);
		}
	});

	$("#fullscreenBtn").on("click", function () {
		if (!document.fullscreenElement) {
			document.documentElement.requestFullscreen();
			$(this).find("i").removeClass("ti-arrows-maximize").addClass("ti-arrows-minimize");
		} else {
			document.exitFullscreen();
			$(this).find("i").removeClass("ti-arrows-minimize").addClass("ti-arrows-maximize");
		}
	});

	$(document).on("click", function (e) {
		if ($(window).width() <= 768) {
			if (!$(e.target).closest(".sidebar, #mobile_btn").length) {
				if ($("body").hasClass("sidebar-open")) {
					$("body").removeClass("sidebar-open");
					$("body").css("overflow", "");
				}
			}
		}
	});

	$(".submenu-list a").on("click", function () {
		if ($(window).width() <= 768) {
			setTimeout(function () {
				$("body").removeClass("sidebar-open");
				$("body").css("overflow", "");
			}, 300);
		}
	});

	$(".sidebar").on("wheel", function (e) {
		e.stopPropagation();
	});

	function setActiveLink() {
		let currentPath = window.location.pathname;

		$(".menu-link, .submenu-list a").removeClass("active");

		$(".menu-link, .submenu-list a").each(function () {
			let href = $(this).attr("href");
			if (href && href !== "#" && href !== "javascript:void(0);" && currentPath.includes(href)) {
				$(this).addClass("active");

				let parentSubmenu = $(this).closest(".menu-item.submenu");
				if (parentSubmenu.length) {
					parentSubmenu.addClass("active");
				}
			}
		});
	}

	setActiveLink();

	if (typeof (Storage) !== "undefined") {
		let sidebarMini = localStorage.getItem("sidebarMini");
		if (sidebarMini === "true" && $(window).width() > 768) {
			$("body").addClass("mini-sidebar");
		}
	}

	let resizeTimer;
	$(window).on("resize", function () {
		clearTimeout(resizeTimer);
		resizeTimer = setTimeout(function () {
			if ($(window).width() > 768) {
				$("body").removeClass("sidebar-open");
				$("body").css("overflow", "");
			}

			if ($(window).width() <= 768) {
				$("body").removeClass("mini-sidebar");
			}
		}, 250);
	});

	if (typeof $.fn.slimScroll !== 'undefined') {
		$(".sidebar-scroll").slimScroll({
			height: "auto",
			position: "right",
			size: "6px",
			color: "#fed7aa",
			wheelStep: 5,
			touchScrollStep: 50
		});
	}

	$(document).on("keydown", function (e) {
		if (e.key === "Escape" && $("body").hasClass("sidebar-open")) {
			$("body").removeClass("sidebar-open");
			$("body").css("overflow", "");
		}

		if ((e.ctrlKey || e.metaKey) && e.key === "b" && $(window).width() > 768) {
			e.preventDefault();
			$("#toggle_btn").trigger("click");
		}
	});

	let touchStartX = 0;
	let touchEndX = 0;

	$(".sidebar").on("touchstart", function (e) {
		touchStartX = e.originalEvent.touches[0].clientX;
	});

	$(".sidebar").on("touchend", function (e) {
		touchEndX = e.originalEvent.changedTouches[0].clientX;
		handleSwipe();
	});

	function handleSwipe() {
		if (touchStartX - touchEndX > 50 && $(window).width() <= 768) {
			$("body").removeClass("sidebar-open");
			$("body").css("overflow", "");
		}
	}

	$(".menu-item.submenu > .menu-link").on("click", function (e) {
		e.preventDefault();
		return false;
	});

	$(".menu-list a:not(.menu-item.submenu > .menu-link)").on("click", function () {
		$(this).addClass("loading");
	});

	
	console.log("%c? Sidebar Enhanced Script Loaded Successfully", "color: #f97316; font-weight: bold; font-size: 14px;");
	console.log("%cKeyboard Shortcuts:", "color: #6b7280; font-weight: bold;");
	console.log("%c  • ESC - Close mobile sidebar", "color: #9ca3af;");
	console.log("%c  • Ctrl/Cmd + B - Toggle mini sidebar (desktop)", "color: #9ca3af;");

});

 
function openSubmenu(menuItemId) {
	$(menuItemId).addClass("active");
}

function closeAllSubmenus() {
	$(".menu-item.submenu").removeClass("active");
}

function toggleSidebar() {
	if ($(window).width() <= 768) {
		$("#mobile_btn").trigger("click");
	} else {
		$("#toggle_btn").trigger("click");
	}
}