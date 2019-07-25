
/* standard block / unblock */
$.block = function () {

    var svg = '<svg id="loading" width="154px" height="154px" version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" xml:space="preserve" style="fill-rule:evenodd;clip-rule:evenodd;stroke-linejoin:round;stroke-miterlimit:1.41421;"><g><g id="Layer1" transform="matrix(1,0,0,1,-80,-80)"><g transform="matrix(1,0,0,1,-1.12697,-0.632622)"><g transform="matrix(1,0,0,1,41.6672,-269.696)"><path class="top" d="M132.173,355.19c4.85333,2.85999 5.71333,10.4 1.91666,16.8367c-3.79332,6.44 -10.8033,9.34332 -15.6567,6.48334c-4.85,-2.86005 -5.71,-10.3967 -1.91666,-16.84c3.79333,-6.43665 10.8033,-9.34003 15.6567,-6.47998Z" style="fill:#b8bff8;fill-rule:nonzero;"/></g><g transform="matrix(1,0,0,1,41.6672,-269.696)"><path class="right" d="M188.63,447.523c-3.25999,4.59332 -10.8433,4.81 -16.94,0.483337c-6.09666,-4.32666 -8.39333,-11.5533 -5.13666,-16.15c3.26332,-4.58997 10.8467,-4.81 16.9433,-0.486633c6.09334,4.32666 8.39333,11.5566 5.13333,16.1533Z" style="fill:#b8bff8;fill-rule:nonzero;"/></g><g transform="matrix(1,0,0,1,41.6672,-269.696)"><path class="bottom" d="M94.5767,498.91c-4.47,-3.42664 -4.41333,-11.0167 0.130005,-16.95c4.54333,-5.93665 11.8533,-7.96997 16.3267,-4.54999c4.46667,3.4267 4.41334,11.0134 -0.126663,16.95c-4.54333,5.93335 -11.8533,7.96997 -16.33,4.54999Z" style="fill:#b8bff8;fill-rule:nonzero;"/></g><g transform="matrix(1,0,0,1,41.6672,-269.696)"><path class="left" d="M44.3533,409.163c3.32,-4.54669 10.9067,-4.66669 16.9433,-0.26001c6.04333,4.40332 8.24333,11.6633 4.92667,16.2167c-3.32,4.54333 -10.9067,4.66663 -16.9467,0.266663c-6.03667,-4.40668 -8.24334,-11.6667 -4.92334,-16.2233Z" style="fill:#b8bff8;fill-rule:nonzero;"/></g><g transform="matrix(1,0,0,1,41.6672,-269.696)"><path class="left" d="M79.45,442.743c1.20667,-7.34332 3.95,-12.4 6.17667,-15.47c1.34,-4.97998 4.69666,-12.0734 8.16,-16.2433c4.00667,-4.82001 9.65,-9.62665 15.3,-12.15c-2.17667,-0.980042 -5.5,-2.06335 -10.4333,-2.8067c-20.2933,-3.05664 -34.1333,11.6867 -34.1333,11.6867c0,0 0.0533295,0.0499878 0.149994,0.136658c0.240005,0.156616 0.480003,0.306641 0.716667,0.476624c6.04667,4.41003 8.53667,11.5667 6.42001,17.16c-0.100006,0.276672 -0.209999,0.549988 -0.33667,0.82666c-0.0199966,0.039978 -0.0400009,0.083313 -0.0633316,0.126648c-0.120003,0.246704 -0.246666,0.48999 -0.386665,0.730042c-0.0133362,0.0199585 -0.0200043,0.043335 -0.0333328,0.0632935c-0.136673,0.233337 -0.283333,0.450012 -0.43,0.66333c-0.0333328,0.0466919 -0.0600052,0.103333 -0.0966721,0.150024c-0.00333405,0.00665283 -0.00999451,0.0100098 -0.0133286,0.0166626c-0.810005,1.1333 -1.76,1.99335 -2.78667,2.65002c-0.046669,0.0266113 -0.090004,0.0532837 -0.136665,0.0799561c-0.17334,0.109985 -0.35334,0.213318 -0.53334,0.309998c-3.51333,1.98999 -8.14333,2.19336 -12.5367,0.5c-1.41667,-0.389954 -2.31334,-0.76001 -2.31334,-0.76001c0,0 -0.179996,8.63 9.67334,20.77c7.80667,9.62 21.79,17.86 21.79,17.86c0,0 -6.39999,-13.11 -4.15333,-26.7767Z" style="fill:#b8bff8;fill-rule:nonzero;"/></g><g transform="matrix(1,0,0,1,41.6672,-269.696)"><path class="bottom" d="M129.807,464.53c-6.12,-1.03333 -10.5167,-3.58667 -13.2733,-5.70996c-5.55666,-1.08002 -12.52,-3.6167 -17.55,-7.54333c-5.75,-4.49005 -11.2267,-12.1133 -13.1967,-17.0934c-1.10667,2.21667 -2.22334,5.44 -3.04,10.1334c-3.09,17.76 10.8933,34.3967 10.8933,34.3967c0,0 0.046669,-0.0533447 0.133339,-0.143311c0.166664,-0.240051 0.323334,-0.480042 0.5,-0.710022c4.54666,-5.94 11.76,-8.26666 17.3033,-6.02002c0.273338,0.103333 0.546669,0.220032 0.816666,0.356689c0.043335,0.0200195 0.0866699,0.043335 0.130005,0.0666504c0.243332,0.123352 0.48333,0.253357 0.716667,0.400024c0.0199966,0.0133057 0.0433273,0.0233154 0.0666656,0.039978c0.246666,0.15332 0.476662,0.313354 0.703331,0.476685c0.0299988,0.0266724 0.0666656,0.043335 0.0966644,0.0666504l0.00333405,0.00335693c1.12334,0.839966 1.96667,1.81665 2.6,2.86664c0.0200043,0.0366821 0.043335,0.0766602 0.0666733,0.113342c0.110001,0.186646 0.213333,0.376648 0.309998,0.56665c1.90334,3.56 1.99333,8.18671 0.203331,12.5367c-0.426666,1.40668 -0.813332,2.28998 -0.813332,2.28998c0,0 8.62334,0.380005 20.9867,-9.18665c9.8,-7.58331 18.3367,-21.37 18.3367,-21.37c0,0 -11.3433,5.93335 -25.9933,3.46332Z" style="fill:#b8bff8;fill-rule:nonzero;"/></g><g transform="matrix(1,0,0,1,41.6672,-269.696)"><path class="right" d="M170.62,405.26c-7.93333,-9.52002 -22.02,-17.59 -22.02,-17.59c0,0 8.21333,10.49 4.50333,26.7367c-1.33665,5.84332 -3.3,10.29 -5.07333,13.4366c-0.00666809,0.419983 -0.0333252,0.82666 -0.106659,1.20667c-0.979996,5.18665 -4.74667,12.44 -9.01001,17.6533c-3.72665,4.54669 -9.34999,10.6567 -14.2867,12.2867c2.02,0.830017 5.11666,1.67999 9.87667,2.33331c17.8633,2.44666 33.9833,-12.13 33.9833,-12.13c0,0 -0.0566559,-0.0499878 -0.15332,-0.133301c-0.243332,-0.156677 -0.486679,-0.303345 -0.723343,-0.470032c-6.09332,-4.32666 -8.67999,-11.4366 -6.64999,-17.0533c0.093338,-0.276611 0.199997,-0.549988 0.323334,-0.82666c0.0266571,-0.0599976 0.0566559,-0.126648 0.0866699,-0.190002c0.110001,-0.226624 0.223328,-0.456665 0.349991,-0.679993c0.0166626,-0.0266724 0.0299988,-0.0533447 0.0466766,-0.0866699c0.896652,-1.56665 2.03,-2.70331 3.29332,-3.52667c0.030014,-0.0199585 0.0633392,-0.039978 0.093338,-0.0599976c0.190002,-0.119995 0.383331,-0.236633 0.576675,-0.339966c3.48666,-2.03003 8.10333,-2.28668 12.51,-0.656677c1.42,0.373352 2.31999,0.733337 2.31999,0.733337c0,0 0.0700073,-8.63336 -9.93999,-20.6433Z" style="fill:#b8bff8;fill-rule:nonzero;"/></g><g transform="matrix(1,0,0,1,41.6672,-269.696)"><path class="top" d="M135.543,375.12c0,0 -0.043335,0.0632935 -0.119995,0.166626c-0.136673,0.253357 -0.26001,0.51001 -0.406677,0.76001c-3.79332,6.43335 -10.6567,9.61334 -16.4233,8.07001c-0.279999,-0.0700073 -0.563332,-0.15332 -0.843338,-0.25c-0.0766602,-0.0266724 -0.149994,-0.0533447 -0.223328,-0.083313c-0.230003,-0.0866699 -0.459999,-0.176697 -0.683334,-0.280029c-0.0333328,-0.0133057 -0.0666656,-0.0233154 -0.0999985,-0.039978c-1.64001,-0.763367 -2.87,-1.79999 -3.8,-2.99335c-0.0166702,-0.0200195 -0.0333328,-0.0466309 -0.0533371,-0.0700073c-0.139999,-0.183289 -0.276665,-0.373291 -0.406662,-0.570007c-2.30667,-3.29663 -2.95,-7.86664 -1.70333,-12.39c0.253326,-1.45001 0.533333,-2.38 0.533333,-2.38c0,0 -8.60334,0.66333 -19.7233,11.65c-8.81667,8.71002 -15.7567,20.2567 -15.7567,20.2567c0,0 13.55,-6.22003 26.3467,-3.57336c5.91334,1.22333 9.68667,2.90997 12.05,4.40668c5.49,0.446655 15.2333,4.26666 21.7067,10.01c5.48999,4.87665 9.62,10.7533 11.29,15.6233c1.39667,-3.35333 2.91667,-8.48663 3.27667,-15.4766c0.926666,-18.0067 -14.96,-32.8367 -14.96,-32.8367Z" style="fill:#b8bff8;fill-rule:nonzero;"/></g></g></g></g></svg>';

    // set defaults to block-ui when in extra small mode.
    var xxs = $('.device-2xs').is(':visible');
    if (xxs) {
        $.blockUI.defaults.css = {
            padding: 0,
            margin: 0,
            width: '30%',
            top: '32%',
            left: '29%',
            textAlign: 'center',
            color: '#000',
            border: 'none',
            backgroundColor: 'transparent',
            cursor: 'wait'
        };
    } else {
        $.blockUI.defaults.css = {
            padding: 0,
            margin: 0,
            width: '30%',
            top: '32%',
            left: '35%',
            textAlign: 'center',
            color: '#000',
            border: 'none',
            backgroundColor: 'transparent',
            cursor: 'wait'
        };
    }
    $.blockUI({
        message: svg,
        overlayCSS: {
            backgroundColor: '#fff',
            opacity: .75
        }
    });
    initializeLoading();
};

$.unblock = function () {
    setTimeout(function () {
        $.unblockUI();
    }, 500);

};

function initializeLoading() {
    $("#loading").velocity("fadeIn", { display: "inline", duration: 500 });
    animateColors();
}

function animateColors() {
    $("#loading .top").velocity({ fill: "#1f3869" }, 250, function () {
        $("#loading .right").velocity({ fill: "#1f3869" }, 250, function () {
            $("#loading .bottom").velocity({ fill: "#1f3869" }, 250, function () {
                $("#loading .left").velocity({ fill: "#1f3869" }, 250, function () {
                    animateColors();
                }).velocity({ fill: "#b8bff8" }, 350);
            }).velocity({ fill: "#b8bff8" }, 350);
        }).velocity({ fill: "#b8bff8" }, 350);
    }).velocity({ fill: "#b8bff8" }, 350);
}

$.growl = function (title, text, type) {
    var backColor = '#46494d';

    if (type == 'danger') {
        backColor = '#d9534f';
    }
    else if (type == 'info') {
        backColor = '#548cc5';
    }
    else if (type == 'warning') {
        backColor = '#eaab00';
    }
    else if (type == 'success') {
        backColor = '#5cb85c';
    }

    $.blockUI.defaults.growlCSS = {
        width: '250px',
        top: '63px',
        left: '',
        right: '10px',
        border: 'none',
        padding: '5px',
        opacity: .97,
        cursor: 'default',
        color: '#fff',
        backgroundColor: backColor,
        '-webkit-border-radius': '0',
        '-moz-border-radius': '0',
        'border-radius': '0'
    };

    $.growlUI(title, text);
}

/* helper methods */

function getISODateTime(d) {
    // padding function
    var s = function (p) {
        return ('' + p).length < 2 ? '0' + p : '' + p;
    };

    // default parameter
    if (typeof d === 'undefined') {
        var d = new Date();
    };

    // return ISO datetime
    return d.getFullYear() + '-' +
        s(d.getMonth() + 1) + '-' +
        s(d.getDate()) + ' ' +
        s(d.getHours()) + ':' +
        s(d.getMinutes()) + ':' +
        s(d.getSeconds());
}

String.prototype.format = function () {
    var args = arguments;
    return this.replace(/{(\d+)}/g, function (match, number) {
        return typeof args[number] != 'undefined'
          ? args[number]
          : match
        ;
    });
};

$.navigate = function (url, data) {
    url += (url.match(/\?/) ? "&" : "?") + data;
    window.location = url;
};

$.fn.getCheckboxVal = function () {
    var vals = [];
    var i = 0;
    this.each(function () {
        vals[i++] = $(this).val();
    });
    return vals;
};

String.prototype.startsWith = function (t, i) {
    return (t == this.substring(0, t.length));
};

$.DateValid = function (d, displayError) {
    var extraSmallDevice = $('.device-xs').is(':visible');
    var smallDevice = $('.device-sm').is(':visible');

    var reDate = /^(0?[1-9]|1[012])[\/-](0?[1-9]|[12][0-9]|3[01])[\/-]((19|20)?[0-9]{2})$/i;
    if ($.dateFormat.startsWith('d'))
        reDate = /^(0?[1-9]|[12][0-9]|3[01])[\/-](0?[1-9]|1[012])[\/-]((19|20)?[0-9]{2})$/i;

    if (extraSmallDevice || smallDevice) {
        reDate = /(\d{4})-(\d{2})-(\d{2})/;
    }

    var v = true;
    if (!reDate.test(d)) {
        if (displayError == true)
            swal("Error!", "Enter valid date.", "error");
        v = false;
    }
    return v;
};

$.SortableDate = function (s) {
    var dt;
    if ($.dateFormat.startsWith('d'))
        dt = new Date(s.split('/')[2], s.split('/')[1] - 1, s.split('/')[0]);
    else
        dt = new Date(s.split('/')[2], s.split('/')[0] - 1, s.split('/')[1]);
    var dt2 = dt.getFullYear() + '-' + (dt.getMonth() + 1) + '-' + dt.getDate();
    return dt2;
};

$.InitializeDateElements = function () {
    var extraSmallDevice = $('.device-xs').is(':visible');
    var smallDevice = $('.device-sm').is(':visible');
    if (extraSmallDevice || smallDevice) {
        $(".input-group.date input[type=text]").each(function (index) {
            var isoSelector = '#' + $(this).attr('id') + 'Iso';
            $(this).val($(isoSelector).val());
            if ($(this).data("rule-date")) {
                $(this).data('rule-date', false);
            }
            $(this).attr('type', 'date');
        });

        $(".input-group.datetime input[type=text]").each(function (index) {
            var isoSelector = '#' + $(this).attr('id') + 'Iso';
            $(this).val($(isoSelector).val());
            if ($(this).data("rule-date")) {
                $(this).data('rule-date', false);
            }
            $(this).attr('type', 'datetime-local');
        });
    }
    else {
        if ($.cultureDateFormatAlt != '') {
            $(".input-group.date").datetimepicker({ format: $.cultureDateFormat, extraFormats: [$.cultureDateFormatAlt], widgetPositioning: { horizontal: 'left' } });
        } else {
            $(".input-group.date").datetimepicker({ format: $.cultureDateFormat, widgetPositioning: { horizontal: 'left' } });
        }
        if ($.cultureDateTimeFormatAlt != '') {
            $(".input-group.datetime").datetimepicker({ format: $.cultureDateTimeFormat, extraFormats: [$.cultureDateTimeFormatAlt], widgetPositioning: { horizontal: 'left' } });
        } else {
            $(".input-group.datetime").datetimepicker({ format: $.cultureDateTimeFormat, widgetPositioning: { horizontal: 'left' } });
        }

        $(".input-group.time").datetimepicker({ format: 'h:mm A', widgetPositioning: { horizontal: 'left' } });

        $('.input-group.birthdate span.input-group-addon').click(function () {
            var inputGroup = $(this).parent();
            if ($.cultureDateFormatAlt != '') {
                $(inputGroup).datetimepicker({ format: $.cultureDateFormat, extraFormats: [$.cultureDateFormatAlt], widgetPositioning: { horizontal: 'left' }, keepInvalid: true });
            } else {
                $(inputGroup).datetimepicker({ format: $.cultureDateFormat, widgetPositioning: { horizontal: 'left' }, keepInvalid: true });
            }
            $(inputGroup).data("DateTimePicker").show();
        });

        $('.input-group.birthdate').on("dp.hide", function (e) {
            $(this).data("DateTimePicker").destroy();
        });
    }
}

// hookup initialize events when common empty dialog is shown.
$('#empty-dialog').on('shown.bs.modal', function () {
    $.InitializeDateElements();
});

/* dom load functions */
$(function () {

    // initialize any date fields.
    $.InitializeDateElements();

    // scroll to the top the collapsed menu when selecting dropdown sub menus.
    $('#navbar ul.navbar-nav li.dropdown').on('show.bs.dropdown', function() {
        var xs = $('.device-xs').is(':visible');
        if (xs) {
            var position = $(this).position();
            position.top -= 55;
            $('#navbar').animate({ scrollTop: position.top });
        }
    });


    // clear tags
    $('a.cleartag').click(function (e) {
        e.preventDefault();
        var href = this.href;

        swal({
            title: "Are you sure?",
            text: "This will empty your active tag.",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-warning",
            confirmButtonText: "Yes, empty it!",
            closeOnConfirm: false
        },
        function () {
            $.post(href, {}, function () {
                swal({
                    title: "Done!",
                    type: "success"
                },
                function () {
                    window.location.reload();
                });
            });
        });
    });


});
