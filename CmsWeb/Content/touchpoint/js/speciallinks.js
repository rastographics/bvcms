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
    givingPagesRow: $('#givingpages.row'),
    givingPagesSelect: $('#givingPagesSelect'),

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
        specialLinks.givingPagesSelect.change(specialLinks.refreshForm);
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
        var givingPagesList = {};

        switch (linkType) {
            case 'registerlink':
            case 'registerlink2':
            case 'sendlink':
            case 'sendlink2':
            case 'supportlink':
                specialLinks.orgRow.show();
                specialLinks.givingPagesRow.hide();
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
                specialLinks.givingPagesRow.hide();
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
                specialLinks.givingPagesRow.hide();
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

            case 'givinglink':
                specialLinks.orgRow.hide();
                specialLinks.givingPagesRow.show();
                specialLinks.meetingRow.hide();
                specialLinks.secondaryRows.hide();
                var length = givingPagesSelect.options.length;
                if (length === 0) {
                    linkText = '';
                    GetGivingPages();
                } else {
                    let givingPageSelected = givingPagesSelect.options[givingPagesSelect.selectedIndex].value;
                    if (givingPageSelected > 0) {
                        //let index = parseInt(givingPageSelected) - 1;
                        //console.log(givingPagesList);
                        //console.log(givingPagesList[index]);
                        linkText += '/?page=';// + givingPagesList[parseInt(index)].PageUrl;
                    } else {
                        linkText = '';
                    }
                }
                break;

            default:
                linkText = '';
                specialLinks.secondaryRows.hide();
                specialLinks.orgRow.hide();
                specialLinks.givingPagesRow.hide();
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

function GetGivingPages() {
    $.ajax({
        type: "Get",
        dataType: "json",
        url: "/Giving/GetSimpleGivingPages",
        success: function (data) {
            console.log(data);
            //givingPagesList = data;
            let givingPagesSelect = $("#givingPagesSelect");
            let optionZero = $("<option/>", {
                value: 0,
                text: 'Select Giving Page',
                select: true
            });
            givingPagesSelect.append(optionZero);
            $.each(data, function (key, value) {
                let option = $("<option/>", {
                    value: key + 1,
                    text: value.PageName
                });
                givingPagesSelect.append(option);
            });
        }
    });
}
