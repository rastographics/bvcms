(function () {
    var supportsDescriptors = Object.defineProperty && (function () {
        try {
            Object.defineProperty({}, 'x', {});
            return true;
        } catch (e) { /* this is ES3 */
            return false;
        }
    }());

    // Define configurable, writable and non-enumerable props
    // if they don't exist.
    var defineProperty;
    if (supportsDescriptors) {
        defineProperty = function (object, name, method) {
            Object.defineProperty(object, name, {
                configurable: true,
                enumerable: false,
                writable: true,
                value: method
            });
        };
    } else {
        defineProperty = function (object, name, method) {
            object[name] = method;
        };
    }

    if (!String.prototype.startsWith) {
        defineProperty(String.prototype, 'startsWith', function (searchString, position) {
            position = position || 0;
            return this.lastIndexOf(searchString, position) === position;
        });
    }
})();
