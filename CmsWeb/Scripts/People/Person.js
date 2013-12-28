$(function () {
    var addrtabs = $("#address-tab").tabs();
    $("#enrollment-tab").tabs();
    $("#member-tab").tabs();
    $("#growth-tab").tabs();
    $("#system-tab").tabs();
    $("#main-tab").tabs();
    $("#main-tab").show();
    $(".submitbutton,.bt").button();
    var index = $('#address-tab a[href="#' + $('#addrtab').val() + '"]').parent().index();
    addrtabs.tabs('option', "active", index);
    $('#dialogbox').dialog({
        title: 'Search Dialog',
        bgiframe: true,
        autoOpen: false,
        width: 700,
        height: 630,
        modal: true,
        overlay: {
            opacity: 0.5,
            background: "black"
        }, close: function () {
            $('iframe', this).attr("src", "");
        }
    });
    $("#addoptoutemail").live('click', function(ev) {
        ev.preventDefault();
        $.post($(this).attr("href"), { email: $("#optoutemail").val() }, function() {
            $.getTable($("#optouts-tab form"));
        });
        return false;
    });
    $("#clipaddr").live('click', function () {
        var inElement = $('#addrhidden')[0];
        if (inElement.createTextRange) {
            var range = inElement.createTextRange();
            if (range)
                range.execCommand('Copy');
        }
        return false;
    });
    $('#deleteperson').click(function () {
        var href = $(this).attr("href");
        if (confirm('Are you sure you want to delete?')) {
            $.post(href, null, function (ret) {
                if (ret != "ok") {
                    $.block("delete Failed: " + ret);
                    $('.blockOverlay').attr('title', 'Click to unblock').click($.unblock);
                }
                else {
                    $.block("person deleted");
                    $('.blockOverlay').attr('title', 'Click to unblock').click(function () {
                        $.unblock();
                        window.location = "/";
                    });
                }
            });
        }
        return false;
    });
    $('body').on("click", 'a.deloptout', function (ev) {
        ev.preventDefault();
        var href = $(this).attr("href");
        if (confirm('Are you sure you want to delete?')) {
            $.post(href, {}, function (ret) {
                if (ret != "ok")
                    $.growlUI("failed", ret);
                else {
                    $.updateTable($('#user-tab form'));
                    $.growlUI("Success", "OptOut deleted");
                }
            });
        }
    });
    $('#moveperson').click(function (ev) {
        ev.preventDefault();
        var d = $('#dialogbox');
        $('iframe', d).attr("src", this.href);
        d.dialog("option", "title", "Merge To Person");
        d.dialog("open");
        return false;
    });

    $.editable.addInputType('datepicker', {
        element: function (settings, original) {
            var input = $('<input>');
            if (settings.width != 'none') { input.width(settings.width); }
            if (settings.height != 'none') { input.height(settings.height); }
            input.attr('autocomplete', 'off');
            $(this).append(input);
            return (input);
        },
        plugin: function (settings, original) {
            var form = this;
            settings.onblur = 'ignore';
            $(this).find('input').datepicker().bind('click', function () {
                $(this).datepicker('show');
                return false;
            }).bind('dateSelected', function (e, selectedDate, $td) {
                $(form).submit();
            });
        }
    });
    $.editable.addInputType("multiselect", {
        element: function (settings, original) {
            var select = $('<select multiple="multiple" />');

            if (settings.width != 'none') { select.width(settings.width); }
            if (settings.size) { select.attr('size', settings.size); }

            $(this).append(select);
            return (select);
        },
        content: function (json, settings, original) {
            for (var key in json) {
                var option = $('<option />').val(key).text(key);
                if (json[key] == true)
                    option.attr("selected", true);
                $('select', this).append(option);
            }
            $("select", this).multiselect({
                close: function (event, ui) {
                    var values = $("select").val();
                },
                position: {
                    my: 'left bottom',
                    at: 'left top'
                }
            });
        }
    });
    $.extraEditable = function (table) {
        $('.editarea', table).editable('/Person/EditExtra/', {
            type: 'textarea',
            submit: 'OK',
            rows: 10,
            width: 600,
            indicator: '<img src="/Content/images/loading.gif">',
            tooltip: 'Click to edit...'
        });
        $(".clickEdit", table).editable("/Person/EditExtra/", {
            indicator: "<img src='/images/loading.gif'>",
            tooltip: "Click to edit...",
            style: 'display: inline',
            width: '300px',
            height: 25,
            submit: 'OK'
        });
        $(".clickDatepicker").editable('/Person/EditExtra/', {
            type: 'datepicker',
            tooltip: 'Click to edit...',
            style: 'display: inline',
            width: '300px',
            submit: 'OK'
        });
        $(".clickSelect", table).editable("/Person/EditExtra/", {
            indicator: '<img src="/Content/images/loading.gif">',
            loadurl: "/Person/ExtraValues/",
            loadtype: "POST",
            type: "select",
            submit: "OK",
            style: 'display: inline'
        });
        $(".clickCheckbox", table).editable('/Person/EditExtra', {
            type: 'checkbox',
            onblur: 'ignore',
            submit: 'OK'
        });
        $('.clickMultiselect', table).editable('/Person/EditExtra', {
            indicator: '<img src="/Content/images/loading.gif">',
            loadurl: "/Person/ExtraValues2/",
            loadtype: "POST",
            type: "multiselect",
            submit: "OK",
            onblur: 'ignore',
            style: 'display: inline'
        });
    };
    $.getTable = function (f) {
        var q = f.serialize();
        $.post(f.attr('action'), q, function (ret) {
            $(f).html(ret).ready(function () {
                $('.bt').button();
                $(".datepicker").datepicker();
                $.extraEditable('#extravalues');
                $('.tooltip', f).tooltip({ showURL: false, showBody: '|' });
            });
        });
        return false;
    };
    $('#memberDialog').dialog({
        title: 'Member Dialog',
        bgiframe: true,
        autoOpen: false,
        width: 600,
        height: 550,
        modal: true,
        overlay: {
            opacity: 0.5,
            background: "black"
        }, close: function () {
            $('iframe', this).attr("src", "");
        }
    });
    $('form a.membertype').live("click", function (e) {
        e.preventDefault();
        var d = $('#memberDialog');
        $('iframe', d).attr("src", this.href);
        d.dialog("open");
    });
    $('#previous-tab form a.membertype').live("click", function (e) {
        e.preventDefault();
        var d = $('#memberDialog');
        $('iframe', d).attr("src", this.href);
        d.dialog("open");
    });

    $(".CreateAndGo").click(function () {
        if (confirm($(this).attr("confirm")))
            $.post($(this).attr("href"), null, function (ret) {
                window.location = ret;
            });
        return false;
    });

    $("#enrollment-link").click(function () {
        $.showTable($('#current-tab form'));
    });
    $("#previous-link").click(function () {
        $.showTable($('#previous-tab form'));
    });
    $("#pending-link").click(function () {
        $.showTable($('#pending-tab form'));
    });
    $("#attendance-link").click(function () {
        $.showTable($('#attendance-tab form'));
    });
    $("#contacts-link").click(function () {
        $("#contacts-tab form").each(function () {
            $.showTable($(this));
        });
    });
    $("#member-link").click(function () {
        var f = $("#memberdisplay");
        if ($("table", f).size() == 0) {
            $.post(f.attr('action'), null, function (ret) {
                $(f).html(ret).ready(function () {
                    $.UpdateForSection(f);
                    ShowMemberExtras();
                });
            });
            $.showTable($("#extras-tab form"));
            $.extraEditable('#extravalues');
        }
    });
    $("#system-link").click(function () {
        $.showTable($("#user-tab form"));
    });
    $("#changes-link").click(function () {
        $.showTable($("#changes-tab form"));
    });
    $("#volunteer-link").click(function () {
        $.showTable($("#volunteer-tab form"));
    });
    $("#duplicates-link").click(function () {
        $.showTable($("#duplicates-tab form"));
    });
    $("#optouts-link").click(function () {
        $.showTable($("#optouts-tab form"));
    });
    $("#recreg-link").click(function (ev) {
        ev.preventDefault();
        var f = $('#recreg-tab form');
        if ($('table', f).size() > 0)
            return false;
        var q = f.serialize();
        $.post(f.attr('action'), q, function (ret) {
            $(f).html(ret);
            $(".bt", f).button();
        });
        return false;
    });

    $("a.displayedit").live('click', function (ev) {
        ev.preventDefault();
        var f = $(this).closest('form');
        $.post($(this).attr('href'), null, function (ret) {
            $(f).html(ret).ready(function () {
                $.UpdateForSection(f);
            });
        });
        return false;
    });

    $.UpdateForSection = function (f) {
        $('#Employer', f).autocomplete({ minLength: 3, source: "/Person/Employers" });
        $('#School', f).autocomplete({ minLength: 3, source: "/Person/Schools" });
        $('#Occupation', f).autocomplete({ minLength: 3, source: "/Person/Occupations" });
        $('#NewChurch', f).autocomplete({ minLength: 3, source: "/Person/Churches" });
        $('#PrevChurch', f).autocomplete({ minLength: 3, source: "/Person/Churches" });

        if ($("#newlook").val() == "true") {
            $("form select").chosen();
            $("input.date").datepicker();
        }
        $(".datepicker").datepicker();
        $(".submitbutton,.bt", f).button();
        $("a.trigger-dropdown", f).dropdown2();
        return false;
    };
    $("form").on("click", "#verifyaddress", function () {
        var ff = $(this).closest('form');
        var q = ff.serialize();
        $.post($(this).attr('href'), q, function (ret) {
            if (confirm(ret.address + "\nUse this Address?")) {
                $('#Address1', ff).val(ret.Line1);
                $('#Address2', ff).val(ret.Line2);
                $('#City', ff).val(ret.City);
                $('#State', ff).val(ret.State);
                $('#Zip', ff).val(ret.Zip);
            }
        });
        return false;
    });
    $("form.DisplayEdit a.submitbutton").live('click', function (ev) {
        ev.preventDefault();
        var f = $(this).closest('form');
        if (!$(f).valid())
            return false;
        var q = f.serialize();
        $.post($(this).attr('href'), q, function (ret) {
            $(f).html(ret).ready(function () {
                var bc = $('#businesscard');
                $.post($(bc).attr("href"), null, function (ret) {
                    $(bc).html(ret);
                });
                $(".submitbutton,.bt").button();
                $("a.trigger-dropdown", f).dropdown2();
            });
        });
        return false;
    });
    $("form").on('click', '#future', function (ev) {
        ev.preventDefault();
        var f = $(this).closest('form');
        var q = f.serialize();
        $.post($(f).attr("action"), q, function (ret) {
            $(f).html(ret);
        });
    });
    $("form.DisplayEdit").submit(function () {
        if (!$("#submitit").val())
            return false;
        return true;
    });
    $.validator.addMethod("date2", function (value, element, params) {
        var v = $.DateValid(value);
        return this.optional(element) || v;
    }, $.format("Please enter valid date"));

    $.validator.setDefaults({
        highlight: function (input) {
            $(input).addClass("ui-state-highlight");
        },
        unhighlight: function (input) {
            $(input).removeClass("ui-state-highlight");
        },
        rules: {
            "NickName": { maxlength: 15 },
            "Title": { maxlength: 10 },
            "First": { maxlength: 25 },
            "Middle": { maxlength: 15 },
            "Last": { maxlength: 100, required: true },
            "Suffix": { maxlength: 10 },
            "AltName": { maxlength: 100 },
            "Maiden": { maxlength: 20 },
            "HomePhone": { maxlength: 20 },
            "CellPhone": { maxlength: 20 },
            "WorkPhone": { maxlength: 20 },
            "EmailAddress": { maxlength: 150 },
            "School": { maxlength: 60 },
            "Employer": { maxlength: 60 },
            "Occupation": { maxlength: 60 },
            "WeddingDate": { date2: true },
            "DeceasedDate": { date2: true },
            "Grade": { number: true },
            "Address1": { maxlength: 40 },
            "Address2": { maxlength: 40 },
            "City": { maxlength: 30 },
            "Zip": { maxlength: 15 },
            "FromDt": { date2: true },
            "ToDt": { date2: true },
            "DecisionDate": { date2: true },
            "JoinDate": { date2: true },
            "BaptismDate": { date2: true },
            "BaptismSchedDate": { date2: true },
            "DropDate": { date2: true },
            "NewMemberClassDate": { date2: true }
        }
    });
    $('#addrf').validate();
    $('#addrp').validate();
    $('#basic').validate();
    $("body").on("change", '.atck', function (ev) {
        var ck = $(this);
        $.post("/Meeting/MarkAttendance/", {
            MeetingId: $(this).attr("mid"),
            PeopleId: $(this).attr("pid"),
            Present: ck.is(':checked')
        }, function (ret) {
            if (ret.error) {
                ck.attr("checked", !ck.is(':checked'));
                alert(ret.error);
            }
            else {
                var f = ck.closest('form');
                var q = f.serialize();
                $.post($(f).attr("action"), q, function (ret) {
                    $(f).html(ret);
                });
            }
        });
    });
    $("#newvalueform").dialog({
        autoOpen: false,
        buttons: {
            "Ok": function () {
                var v = $("input[name='typeval']:checked").val();
                var fn = $("#fieldname").val();
                var va = $("#fieldvalue").val();
                if (fn)
                    $.post("/Person/NewExtraValue/" + $("#PeopleId").val(), { field: fn, type: v, value: va }, function (ret) {
                        if (ret.startsWith("error"))
                            alert(ret);
                        else {
                            $.getTable($("#extras-tab form"));
                            $.extraEditable('#extravalues');
                        }
                        $("#fieldname").val("");
                        $("#fieldvalue").val("");
                    });
                $(this).dialog("close");
            }
        }
    });
    $("body").on("click", '#newextravalue', function (ev) {
        ev.preventDefault();
        var d = $('#newvalueform');
        d.dialog("open");
    });
    $("body").on("click", 'a.deleteextra', function (ev) {
        ev.preventDefault();
        if (confirm("are you sure?"))
            $.post("/Person/DeleteExtra/" + $("#PeopleId").val(), { field: $(this).attr("field") }, function (ret) {
                if (ret.startsWith("error"))
                    alert(ret);
                else {
                    $.getTable($("#extras-tab form"));
                    $.extraEditable('#extravalues');
                }
            });
        return false;
    });
    $("form").on('click', 'a.reverse', function (ev) {
        ev.preventDefault();
        var f = $(this).closest('form');
        $.post("/Person/Reverse", {
            id: $("#PeopleId").val(),
            field: $(this).attr("field"),
            value: $(this).attr("value"),
            pf: $(this).attr("pf")
        }, function (ret) {
            $(f).html(ret);
        });
    });
    $.editable.addInputType("checkbox", {
        element: function (settings, original) {
            var input = $('<input type="checkbox">');
            $(this).append(input);
            $(input).click(function () {
                var value = $(input).attr("checked") ? 'True' : 'False';
                $(input).val(value);
            });
            return (input);
        },
        content: function (string, settings, original) {
            var checked = string == "True" ? true : false;
            var input = $(':input:first', this);
            $(input).attr("checked", checked);
            var value = $(input).attr("checked") ? 'True' : 'False';
            $(input).val(value);
        }
    });
    $('#vtab>ul>li').click(function () {
        $('#vtab>ul>li').removeClass('selected');
        $(this).addClass('selected');
        var index = $('#vtab>ul>li').index($(this));
        $('#vtab>div').hide().eq(index).show();
    });

    //$.editable.addInputType("multiselect", {
    //    element: function(settings, original) {
    //        var textarea = $('<select />');
    //        if (settings.rows) {
    //            textarea.attr('rows', settings.rows);
    //        } else {
    //            textarea.height(settings.height);
    //        }
    //        if (settings.cols) {
    //            textarea.attr('cols', settings.cols);
    //        } else {
    //            textarea.width(settings.width);
    //        }
    //        $(this).append(textarea);
    //        return (textarea);
    //    },
    //    plugin: function(settings, original) {
    //        $('textarea', this).multiselect();
    //    },
    //    submit: function(settings, original) {
    //        var value = $('#hour_').val() + ':' + $('#min_').val();
    //        $('input', this).val(value);
    //    }
    //});

});
function RebindMemberGrids(from) {
    $.updateTable($('#current-tab form'));
    $.updateTable($('#pending-tab form'));
    $("#memberDialog").dialog('close');
}
function RebindUserInfoGrid(from) {
    $.updateTable($('#user-tab form'));
    $("#memberDialog").dialog('close');
}
function AddSelected(ret) {
    window.location = "/Merge?PeopleId1=" + $("#PeopleId").val() + "&PeopleId2=" + ret.pid;
}