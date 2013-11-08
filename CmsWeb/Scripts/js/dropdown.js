$(function() {
    $.fn.dropdown2 = function() {
        return this.each(function() {
            $(this).click(function(ev) {
                ev.preventDefault();
                var trigger = $(this),
                    dd = $($(this).next('.dropdown2-menu')),
                    isOpen = trigger.hasClass('dropdown-open');
                $("div.dropdown2-menu").hide();
                $(".trigger-dropdown").removeClass("dropdown-open");
                if (isOpen)
                    return false;
                dd.show();
                dd.css({
                    left: dd.hasClass('anchor-right') ?
                        trigger.position().left - (dd.outerWidth() - trigger.outerWidth())
                        : trigger.position().left,
                    top: trigger.position().top + 22
                });
                trigger.addClass('dropdown-open');
                return false;
            });
        });
    };
   $.fn.dropdownFocus = function() {
        return this.each(function() {
            $(this).focus(function(ev) {
                ev.preventDefault();
                var trigger = $(this),
                    dd = $($(this).next('.dropdown2-menu')),
                    isOpen = trigger.hasClass('dropdown-open');
                $("div.dropdown2-menu").hide();
                $(".trigger-dropdown").removeClass("dropdown-open");
                if (isOpen)
                    return false;
                dd.show();
                dd.css({
                    left: dd.hasClass('anchor-right') ?
                        trigger.position().left - (dd.outerWidth() - trigger.outerWidth())
                        : trigger.position().left 
                });
                trigger.addClass('dropdown-open');
                return false;
            });
            $(this).click(function(ev) {
                var trigger = $(this), isOpen = trigger.hasClass('dropdown-open');
                if (isOpen)
                    return false;
                return true;
            });
        });
    };
	$(function () {
		$('HTML').on('click.dropdown', hideDropdowns);
		$(window).on('resize.dropdown', hideDropdowns);
	    $(document).keydown(function(e) {
	        if (e.keyCode == 17 && $("a.trigger-dropdown").hasClass("dropdown-open")) {
	            $("li.hideAlt").toggle();
	        }
	    });
        $("li.hideAlt").hide();
	});
});
    function hideDropdowns(event) {
		var targetGroup = event ? $(event.target).parents().andSelf() : null;
		if (targetGroup && targetGroup.is('.dropdown2-menu') && !targetGroup.is('A'))
		    return;
		$('div.dropdown2-menu').hide();
		$('.trigger-dropdown').removeClass('dropdown-open');
        $("li.hideAlt").hide();
	};