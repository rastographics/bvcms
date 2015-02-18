var gulp = require('gulp'),
  less = require('gulp-less'),
  minify = require('gulp-minify-css'),
  sourcemaps = require('gulp-sourcemaps');

var cssOutput = 'content/touchpoint/css';
var select2Output = 'content/touchpoint/lib/select2/css';

gulp.task('default', function() {
  // place code here
});

gulp.task('less', function() {
    gulp.src('content/touchpoint/src/less/app.less')
        .pipe(less())
        .pipe(minify())
        .pipe(gulp.dest(cssOutput));
    gulp.src('content/touchpoint/src/less/account.less')
        .pipe(less())
        .pipe(minify())
        .pipe(gulp.dest(cssOutput));
    gulp.src('content/touchpoint/src/less/error.less')
        .pipe(less())
        .pipe(minify())
        .pipe(gulp.dest(cssOutput));
    gulp.src('content/touchpoint/src/less/print.less')
        .pipe(less())
        .pipe(minify())
        .pipe(gulp.dest(cssOutput));
    gulp.src('content/touchpoint/src/less/select2/select2-bootstrap.less')
        .pipe(less())
        .pipe(minify())
        .pipe(gulp.dest(select2Output));
});

gulp.task('less-sourcemaps', function() {
    gulp.src('content/touchpoint/src/less/app.less')
        .pipe(sourcemaps.init())
        .pipe(less())
        .pipe(minify())
        .pipe(sourcemaps.write())
        .pipe(gulp.dest(cssOutput));
});
