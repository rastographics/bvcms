/// <binding ProjectOpened='default' />
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
            //DebugOnlineRegFilesStart
            'Content/touchpoint/lib/jquery-blockUI/js/jquery.blockUI.js',
            'Content/touchpoint/lib/jquery.sortElements.js',
            'Content/touchpoint/lib/jquery.showpassword.js',
            'Content/touchpoint/lib/jquery-validate-globalize/js/jquery.validate.globalize.js',
            'Content/touchpoint/js/extensions.js',
            'Content/touchpoint/lib/idleTimer/js/idle-timer.js'
            //DebugOnlineRegFilesEnd
        ],
        outputName: 'onlineregister.min.js',
        outputDir: 'Content/touchpoint/js'
    }
];

var jsFiles = [
    {
        files: [
            //DebugFilesStart
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
            //DebugFilesEnd
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

gulp.task('less', function(complete) {
    var handleLess = function (lessFile, output) {
        gulp.src(lessFile)
            .pipe(less())
            .pipe(minify({restructuring: false}))
            .pipe(gulp.dest(output));
    };

    lessFiles.map(function(file) {
        handleLess(file.lessFile, file.output);
    });
    complete();
});

gulp.task('compress-js', function(complete) {
    var handleJs = function(files, outputName, destinationFolder) {
        gulp.src(files)
            .pipe(concat(outputName))
            .pipe(uglify())
            .on('error', function (err) { gutil.log(gutil.colors.red('[Error]'), err.toString()); })
            .pipe(gulp.dest(destinationFolder));
    };

    legacyJsFiles.map(function(legacyJs) {
        handleJs(legacyJs.files, legacyJs.outputName, legacyJs.outputDir);
    });

    jsFiles.map(function(legacyJs) {
        handleJs(legacyJs.files, legacyJs.outputName, legacyJs.outputDir);
    });
    complete();
});

gulp.task('less-sourcemaps', function(complete) {
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
    complete();
});

gulp.task('js-sourcemaps', function(complete) {
    var handleJs = function(files, outputName, destinationFolder) {
        gulp.src(files)
            .pipe(sourcemaps.init())
            .pipe(concat(outputName))
            .pipe(uglify())
            .pipe(sourcemaps.write())
            .pipe(gulp.dest(destinationFolder));
    };

    legacyJsFiles.map(function(legacyJs) {
        handleJs(legacyJs.files, legacyJs.outputName, legacyJs.outputDir);
    });

    jsFiles.map(function(legacyJs) {
        handleJs(legacyJs.files, legacyJs.outputName, legacyJs.outputDir);
    });
    complete();
});

gulp.task('default', gulp.series(['less', 'compress-js'], function(complete) {
    complete();
}));
