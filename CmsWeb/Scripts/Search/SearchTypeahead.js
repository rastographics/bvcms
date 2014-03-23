$(function () {
    $('#SearchText').typeahead({
        name: 'search',
        valueKey: "url",
        limit: 25,
        beforeSend: function(jqXhr, settings) {
            $.SetLoadingIndicator();
        },
        remote: '/FastSearch?q=%QUERY',
        prefetch: {
            url: '/FastSearchPrefetch',
            ttl: 1
        },
        minLength: 0,
        template: 'dummy string',
        engine: {
            compile: function (t) {
                return {
                    render: function (context) {
                        var r = "<div{2}>{0}{1}</div>".format(
                            context.line1,
                            context.line2
                                ? "<br>" + context.line2
                                : "",
                            context.addmargin
                                ? " style='margin-bottom:.7em'"
                                : "");
                        return r;
                    }
                };
            }
        }
    });
    $('#SearchText').bind('typeahead:selected', function (obj, datum, name) {

        window.location = datum.url;
    });
});
String.prototype.format = function () {
    var args = arguments;
    return this.replace(/{(\d+)}/g, function (match, number) {
        return typeof args[number] != 'undefined'
          ? args[number]
          : match
        ;
    });
};