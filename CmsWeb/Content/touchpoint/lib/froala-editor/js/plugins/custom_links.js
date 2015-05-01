(function ($) {
    $.Editable.DEFAULTS = $.extend($.Editable.DEFAULTS, {});
    $.Editable.prototype.initCustomLinks = function () {
        var _self = this;
        $('body').on('click', '#ok-custom-link', function (e) {
            _self.insertHTML('foo bar', true);
            $('button')
        });
    }
    $.Editable.initializers.push($.Editable.prototype.initCustomLinks);
    $.Editable.commands = $.extend($.Editable.commands, {
        customLinks: {
            title: 'Custom Links',
            icon: 'fa fa-cog',
            refresh: function () {
                // The button state might have been changed.
            },
            refreshOnShow: function () {
                // Triggered when the dropdown is shown.
            },
            callback: function () {
                // Do something when the button is hit.
            },
            undo: true // Enable only if it might affect the UNDO stack
        }

    });

    // Customize how your dropdown works.
    $.Editable.prototype.command_dispatcher = $.extend($.Editable.prototype.command_dispatcher, {
        customLinks: function (command) {
            // The HTML that appears in the dropdown.
            var dropdown = '<ul class="fr-dropdown-menu"><li><div class="froala-popup froala-link-popup" style=""><h4><span data-text="true">Insert Custom Link</span></h4><div class="f-popup-line"><div><strong>Confirmation</strong></div><select id="confirmation"><option value="">&lt;not set&gt;</option><option value="ltr">Yes, send confirmation</option><option value="rtl">No, do not send confirmation</option></select></div><div class="f-popup-line"><input type="text" placeholder="http://www.example.com" class="f-lu" id="url"></div><div class="f-popup-line"><input type="text" placeholder="OrgId / Meeting Id" class="f-lu" id="orgIdMeetingId"></div><div class="f-popup-line"><input type="text" placeholder="Message" class="f-lu" id="messageText"></div><div class="f-popup-line"><input type="text" placeholder="Small Group" class="f-lu" id="smallGroup"></div><div class="f-popup-line"><input type="checkbox" id="f-checkbox-1"> <label data-text="true" for="f-checkbox-1">Open in new tab</label><button data-text="true" type="button" class="fr-p-bttn f-ok f-submit" id="ok-custom-link">OK</button></div></div></li></ul>';
            var btn = this.buildDropdownButton(command, dropdown);
            return btn;
        }
    });


})(jQuery);