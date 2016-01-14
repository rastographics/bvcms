var gulp = require('gulp'),
    less = require('gulp-less'),
    minify = require('gulp-minify-css'),
    sourcemaps = require('gulp-sourcemaps'),
    uglify = require('gulp-uglify'),
    concat = require('gulp-concat');

var cssOutput = 'content/touchpoint/css',
    select2Output = 'content/touchpoint/lib/select2/css',
    jsOutput = 'content/touchpoint/js';

var legacyJsFiles = [
    {
        files: [
            'Scripts/js/extensions.js',
            'Scripts/jQuery/jquery-migrate-1.1.1.js',
            'Scripts/Bootstrap/bootstrap-modalmanager.js',
            'Scripts/Bootstrap/bootstrap-modal.js',
            'Scripts/Bootstrap/bootstrap-editable.js',
            'Scripts/Bootstrap/bootbox.js',
            'Scripts/Bootstrap/bootstrap-datetimepicker.js',
            'Scripts/jQuery/jquery.cookie.js',
            'Scripts/jQuery/jquery.blockUI.js',
            'Scripts/jQuery/jquery.mousewheel.js',
            'Scripts/jquery.validate.js',
            'Scripts/additional-methods.js',
            'Scripts/jquery.validate.globalize.js',
            'Scripts/jQuery/jquery.sortElements.js',
            'Scripts/jQuery/jquery.textarea.js',
            'Scripts/jQuery/jquery.tooltip.js',
            'Scripts/jquery/jquery.hiddenposition.1.1.js',
            'Scripts/Bootstrap/select2.js',
            'Scripts/js/Pager3.js',
            'Scripts/js/header.js',
            'Scripts/js/ExportToolBar2.js',
            'Scripts/js/headermenu2.js',
            'Scripts/Bootstrap/typeahead.js',
            'Scripts/Search/SearchTypeahead.js',
            'Scripts/js/form-ajax.js',
            'Scripts/js/ExtraValue.js',
            'Scripts/Search/SearchAdd.js'
        ],
        outputName: 'bundle.main2.js',
        outputDir: 'Scripts'
    },
    {
        files: [
            'Scripts/js/extensions.js',
            'Scripts/jQuery/jquery-migrate-1.1.1.js',
            'Scripts/Bootstrap/bootstrap-modalmanager.js',
            'Scripts/Bootstrap/bootstrap-modal.js',
            'Scripts/Bootstrap/bootstrap-editable.js',
            'Scripts/Bootstrap/bootbox.js',
            'Scripts/Bootstrap/bootstrap-datetimepicker.js',
            'Scripts/jQuery/jquery.cookie.js',
            'Scripts/jQuery/jquery.blockUI.js',
            'Scripts/jQuery/jquery.mousewheel.js',
            'Scripts/jquery.validate.js',
            'Scripts/additional-methods.js',
            'Scripts/jquery.validate.globalize.js',
            'Scripts/jQuery/jquery.sortElements.js',
            'Scripts/jQuery/jquery.textarea.js',
            'Scripts/jQuery/jquery.tooltip.js',
            'Scripts/jQuery/jquery.jscrollpane.js',
            'Scripts/jQuery/jquery.jeditable.js',
            'Scripts/jQuery/jquery.multiSelect.js',
            'Scripts/jquery/jquery.hiddenposition.1.1.js',
            'Scripts/Bootstrap/select2.js',
            'Scripts/chosen/chosen.jquery.js',
            'Scripts/js/Pager.js',
            'Scripts/js/header.js',
            'Scripts/js/dropdown.js',
            'Scripts/js/headermenu1c.js',
            'Scripts/Bootstrap/typeahead.js',
            'Scripts/Search/SearchTypeahead.js',
            'Scripts/js/form-ajax.js',
            'Scripts/js/ExtraValue.js',
            'Scripts/js/ExportToolBar2.js',
            'Scripts/js/headermenu2.js',
            'Scripts/Search/SearchAdd.js'
        ],
        outputName: 'bundle.main2c.js',
        outputDir: 'Scripts'
    },
    {
        files: [
            'Scripts/js/extensions.js',
            'Scripts/jQuery/jquery-migrate-1.1.1.js',
            'Scripts/jquery.validate.js',
            'Scripts/globalize.js',
            'Scripts/jquery.validate.globalize.js',
            'Content/touchpoint/lib/idleTimer/js/idle-timer.js',
            'Scripts/jQuery/jquery.blockUI.js',
            'Scripts/jQuery/jquery.sortElements.js',
            'Scripts/jQuery/jquery.showpassword.js'
        ],
        outputName: 'bundle.onlineregister.js',
        outputDir: 'Scripts'
    },
    {
        files: [
            'Scripts/Org/OrganizationOld.js',
            'Scripts/Dialog/SearchUsers.js',
            'Scripts/Org/RegSetting2.js',
            'Scripts/Org/OrgMemberDialog2.js'
        ],
        outputName: 'bundle.organization.js',
        outputDir: 'Scripts'
    },
    {
        files: [
            'Scripts/Org/Organization.js',
            'Scripts/Dialog/SearchUsers.js',
            'Scripts/Org/RegSetting.js',
            'Scripts/Org/OrgMemberDialog2.js'
        ],
        outputName: 'bundle.org.js',
        outputDir: 'Scripts'
    },
    {
        files: [
            'Scripts/Bootstrap/bootstrap-multiselect.js',
            'Scripts/jQuery/jquery.scrollintoview.js',
            'Scripts/Search/Query.js',
            'Scripts/jQuery/jquery.smoothscroll.js',
            'Scripts/Bootstrap/bootstrap-tour.js'
        ],
        outputName: 'bundle.query2.js',
        outputDir: 'Scripts'
    },
    {
        files: [
            'Scripts/jQuery/jquery.smoothscroll.js',
            'Scripts/Bootstrap/bootstrap-tour.js',
            'Scripts/People/person1.js',
            'Scripts/Org/OrgMemberDialog2.js'
        ],
        outputName: 'bundle.person.js',
        outputDir: 'Scripts'
    },
    {
        files: [
            'Scripts/jQuery/jquery.form.js',
            'Scripts/jQuery/jquery.form2.js',
            'Scripts/js/Task.js'
        ],
        outputName: 'bundle.taskpage.js',
        outputDir: 'Scripts'
    }
];

var jsFiles = [
    {
        files: [
            'Content/touchpoint/lib/jquery-validate-globalize/js/jquery.validate.globalize.js',
            'Content/touchpoint/lib/bootstrap-datetimepicker/js/bootstrap-datetimepicker.min.js',
            'Content/touchpoint/lib/bootstrap-sweetalert/js/sweetalert.min.js',
            'Content/touchpoint/lib/bootstrap-bootbox/js/bootbox.min.js',
            'Content/touchpoint/lib/mousetrap/js/mousetrap.min.js',
            'Content/touchpoint/lib/jquery-blockUI/js/jquery.blockUI.js',
            'Content/touchpoint/js/main.js',
            'Content/touchpoint/js/export-toolbar.js',
            'Content/touchpoint/js/extra-value.js',
            'Content/touchpoint/js/form-ajax.js',
            'Content/touchpoint/js/search/search-add.js'
        ],
        outputName: 'app.min.js',
        outputDir: 'Content/touchpoint/js'
    }
];

var lessFiles = [
    {
        lessFile: 'content/touchpoint/src/less/app.less',
        output: cssOutput
    },
    {
        lessFile: 'content/touchpoint/src/less/account.less',
        output: cssOutput
    },
    {
        lessFile: 'content/touchpoint/src/less/error.less',
        output: cssOutput
    },
    {
        lessFile: 'content/touchpoint/src/less/print.less',
        output: cssOutput
    },
    {
        lessFile: 'content/touchpoint/src/less/editor.less',
        output: cssOutput
    },
    {
        lessFile: 'content/touchpoint/src/less/select2/select2-bootstrap.less',
        output: select2Output
    }
];

gulp.task('default', ['less', 'compress-js'], function() {
    // place code here
});

gulp.task('less', function() {
    var handleLess = function (lessFile, output) {
        gulp.src(lessFile)
            .pipe(less())
            .pipe(minify({restructuring: false}))
            .pipe(gulp.dest(output));
    };

    lessFiles.map(function(file) {
        handleLess(file.lessFile, file.output);
    });
});

gulp.task('compress-js', function() {
    var handleJs = function(files, outputName, destinationFolder) {
        gulp.src(files)
            .pipe(concat(outputName))
            .pipe(uglify({preserveComments: 'some'}))
            .pipe(gulp.dest(destinationFolder));
    };

    legacyJsFiles.map(function(legacyJs) {
        handleJs(legacyJs.files, legacyJs.outputName, legacyJs.outputDir);
    });

    jsFiles.map(function(legacyJs) {
        handleJs(legacyJs.files, legacyJs.outputName, legacyJs.outputDir);
    });
});

gulp.task('less-sourcemaps', function() {
    var handleLess = function (lessFile, output) {
        gulp.src(lessFile)
            .pipe(sourcemaps.init())
            .pipe(less())
            .pipe(minify({restructuring: false}))
            .pipe(sourcemaps.write())
            .pipe(gulp.dest(output));
    };

    lessFiles.map(function(file) {
        handleLess(file.lessFile, file.output);
    });
});

gulp.task('js-sourcemaps', function() {
    var handleJs = function(files, outputName, destinationFolder) {
        gulp.src(files)
            .pipe(sourcemaps.init())
            .pipe(concat(outputName))
            .pipe(uglify({ preserveComments: 'some' }))
            .pipe(sourcemaps.write())
            .pipe(gulp.dest(destinationFolder));
    };

    legacyJsFiles.map(function(legacyJs) {
        handleJs(legacyJs.files, legacyJs.outputName, legacyJs.outputDir);
    });

    jsFiles.map(function(legacyJs) {
        handleJs(legacyJs.files, legacyJs.outputName, legacyJs.outputDir);
    });
});
