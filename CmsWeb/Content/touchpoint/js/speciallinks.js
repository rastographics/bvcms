var specialLinks = {
    el: $('#special-links-modal'),
    typeSelect: $('#special_links_type'),
    formInput: $('#special_links_form input'),
    orgRow: $('#org_id.row'),
    orgInput: $('#org_id input'),
    meetingRow: $('#meeting_id.row'),
    meetingInput: $('#meeting_id input'),
    messageInput: $('#message input'),
    confirmationSelect: $('#confirmation select'),
    smallGroupRow: $('#small_group.row'),
    smallGroupInput: $('#small_group input'),
    secondaryRows: $('#small_group.row, #message.row, #confirmation.row'),
    resultRow: $('#result.row'),
    resultInput: $('#result.row input'),

    init: function () {
        // toggle visibility
        $("#editor-modal,#unlayer-editor-modal,#main").on("click", ".create-special-link", function () {
            specialLinks.reset();
            specialLinks.el.modal('show');
        });
        $('.close-special-links-modal').click(function () {
            specialLinks.el.modal('hide');
        });
        $('.done-special-links-modal').click(function () {
            var v = specialLinks.resultInput.val().replace(/ /g, '+');
            specialLinks.resultInput.val(v);
            specialLinks.resultInput.select();
            document.execCommand('copy');
            specialLinks.el.modal('hide');
        });

        // select contents of result
        specialLinks.resultInput.click(function () {
            $(this).select();
        });

        // update result row and toggle inputs
        specialLinks.typeSelect.change(specialLinks.refreshForm);
        specialLinks.confirmationSelect.change(specialLinks.refreshForm);
        specialLinks.formInput.on('input', specialLinks.refreshForm);

        // init
        specialLinks.refreshForm();
    },

    reset: function () {
        specialLinks.typeSelect.val('0');
        specialLinks.refreshForm();
    },

    refreshForm: function () {
        var linkType = specialLinks.typeSelect.val();
        var linkText = 'https://' + linkType;
        var orgId = specialLinks.orgInput.val();

        switch (linkType) {
            case 'registerlink':
            case 'registerlink2':
            case 'sendlink':
            case 'sendlink2':
            case 'supportlink':
                specialLinks.orgRow.show();
                specialLinks.meetingRow.hide();
                specialLinks.secondaryRows.hide();
                if (orgId.length) {
                    linkText += '/?org=' + orgId;
                } else {
                    linkText = '';
                }
                break;

            case 'rsvplink':
            case 'regretslink':
                specialLinks.orgRow.hide();
                specialLinks.meetingRow.show();
                specialLinks.secondaryRows.show();
                var message = specialLinks.messageInput.val();
                var meetingId = specialLinks.meetingInput.val();
                var confirmation = specialLinks.confirmationSelect.val();
                var smallGroup = specialLinks.smallGroupInput.val();
                if (meetingId.length) {
                    linkText += '/?meeting=' + meetingId + '&confirm=' + confirmation;
                    if (smallGroup.length) {
                        linkText += '&group=' + smallGroup;
                    }
                    if (message.length) {
                        linkText += '&msg=' + message;
                    }
                } else {
                    linkText = '';
                }
                break;

            case 'votelink':
                specialLinks.orgRow.show();
                specialLinks.meetingRow.hide();
                specialLinks.secondaryRows.show();
                message = specialLinks.messageInput.val();
                smallGroup = specialLinks.smallGroupInput.val();
                confirmation = specialLinks.confirmationSelect.val();
                if (orgId.length) {
                    linkText += '/?org=' + orgId + '&confirm=' + confirmation;
                    if (message.length) {
                        linkText += '&msg=' + message;
                    }
                    if (smallGroup.length) {
                        linkText += '&group=' + smallGroup;
                    }
                } else {
                    linkText = '';
                }
                break;

            default:
                linkText = '';
                specialLinks.secondaryRows.hide();
                specialLinks.orgRow.hide();
                specialLinks.meetingRow.hide();
                specialLinks.formInput.val('');
                break;
        }
        specialLinks.resultInput.val(linkText);
        if (linkText.length) {
            specialLinks.resultRow.show();
        } else {
            specialLinks.resultRow.hide();
        }
    }
};

specialLinks.init();
