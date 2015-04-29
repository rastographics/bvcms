var gulp = require('gulp'),
    less = require('gulp-less'),
    minify = require('gulp-minify-css'),
    sourcemaps = require('gulp-sourcemaps'),
    uglify = require('gulp-uglify'),
    concat = require('gulp-concat');

var cssOutput = 'content/touchpoint/css',
    select2Output = 'content/touchpoint/lib/select2/css',
    jsOutput = 'content/touchpoint/js';

gulp.task('default', ['less', 'compress-js'], function() {
    // place code here
});

gulp.task('less', function() {
    var handleLess = function (lessFile, output) {
        gulp.src('content/touchpoint/src/less/' + lessFile)
            .pipe(less())
            .pipe(minify({restructuring: false}))
            .pipe(gulp.dest(output));
    };

    ['app.less',
    'account.less',
    'error.less',
    'print.less',
    'editor.less'].map(function(lessFile) {
        handleLess(lessFile, cssOutput);
    });

    handleLess('select2/select2-bootstrap.less', select2Output);
});

gulp.task('compress-js', function() {
    var handleJs = function(filesToInclude, outputName) {
        gulp.src('content/touchpoint/js/' + filesToInclude)
            .pipe(concat(outputName))
            .pipe(uglify())
            .pipe(gulp.dest(jsOutput));
    };

    /*
    ['admin',
    'dialog',
    'email',
    'finance',
    'meeting',
    'org',
    'people',
    'search',
    'support'].map(function(folder) {
        handleJs(folder + '/*.js', folder + '.min.js');
    });

    handleJs('*.js', 'app.min.js');
    */
});

gulp.task('less-sourcemaps', function() {
    gulp.src('content/touchpoint/src/less/app.less')
        .pipe(sourcemaps.init())
        .pipe(less())
        .pipe(minify({restructuring: false}))
        .pipe(sourcemaps.write())
        .pipe(gulp.dest(cssOutput));
});
