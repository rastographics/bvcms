function updateResults(results, context) {
    var list = "<ul class='nav nav-pills nav-stacked'>";
    var appendedPersonHeader = false;
    var appendedOrganizationsHeader = false;
    var appendedSavedSearchesHeader = false;
    var searchBuilderCount = 0;
    _.each(results,
        function (result) {
            if (result.url.indexOf("/Person") > -1 && !appendedPersonHeader) {
                list += "<li class='dropdown-header'>People</li>";
                appendedPersonHeader = true;
            }
            if (result.url.indexOf("/Org/") > -1 && !appendedOrganizationsHeader) {
                list += "<li class='dropdown-header'>Organizations</li>";
                appendedOrganizationsHeader = true;
            }
            if (result.line1 === "Find Person") {
                list += "<li class='dropdown-header'>Options</li>";
            }
            if (result.line1 === "New Search") {
                list += "<li class='dropdown-header'>Search Builder</li>";
            }
            if (result.url.indexOf("/Query/") > -1 && !appendedSavedSearchesHeader) {
                searchBuilderCount++;
                if (searchBuilderCount > 1) {
                    list += "<li class='dropdown-header'>Recent Saved Searches</li>";
                    appendedSavedSearchesHeader = true;
                }
            }
            list += "<li class='dropdown-search-result'><a class='search-person' href='" + result.url + "'>" + result.line1;
            if (result.line2) {
                list += "<br/>" + result.line2;
            }
            list += "</a></li>";
        });
    list += "</ul>";
    $(".search-results").html(list).slideDown();
}
