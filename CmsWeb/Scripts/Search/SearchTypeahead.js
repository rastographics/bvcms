$(function () {
    $('#SearchText').typeahead({
        name: 'search',
        valueKey: "line1",
        limit: 25,
        remote: {
            url: '/Home/Names22?q=%QUERY',
            filter: function (parsedResponse) {
                return parsedResponse;
            }
        },
        local: [
            { order: "001", id: -1, line1: "Find Person" },
            { order: "002", id: -2, line1: "Advanced Search Builder" },
            { order: "003", id: -3, line1: "Saved Searches" },
            { order: "004", id: -4, line1: "New Advanced Search" },
            { order: "005", id: -5, line1: "Organization Search" }
        ],
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
        if (datum.id === -1)
            window.location = "/PeopleSearch?name=" + this.query;
        else if (datum.id === -2)
            window.location = "/Query/";
        else if (datum.id === -3)
            window.location = "/SavedQuery2";
        else if (datum.id === -4)
            window.location = "/Query/NewQuery";
        else if (datum.id === -5)
            window.location = "/OrgSearch";
        else
            window.location = (datum.isOrg ? "/Organization/Index/" : "/Person2/") + datum.id;
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