var gulp = require('gulp'),
  less = require('gulp-less'),
  minify = require('gulp-minify-css'),
  sourcemaps = require('gulp-sourcemaps');

var cssOutput = 'content/touchpoint/css';

gulp.task('default', function() {
  // place code here
});

gulp.task('less', function() {
    gulp.src('src/less/app.less')
        .pipe(less())
        .pipe(minify())
        .pipe(gulp.dest(cssOutput));
    gulp.src('src/less/account.less')
        .pipe(less())
        .pipe(minify())
        .pipe(gulp.dest(cssOutput));
    gulp.src('src/less/error.less')
        .pipe(less())
        .pipe(minify())
        .pipe(gulp.dest(cssOutput));
    gulp.src('src/less/print.less')
        .pipe(less())
        .pipe(minify())
        .pipe(gulp.dest(cssOutput));
});

gulp.task('less-sourcemaps', function() {
    gulp.src('src/less/app.less')
        .pipe(sourcemaps.init())
        .pipe(less())
        .pipe(minify())
        .pipe(sourcemaps.write())
        .pipe(gulp.dest(cssOutput));
});
