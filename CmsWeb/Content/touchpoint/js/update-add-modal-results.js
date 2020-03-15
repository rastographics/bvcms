function updateAddResults(results, context) {
    var list = '<ul class="nav nav-pills nav-stacked">';
    _.each(results,
        function (result) {
            list += '<li class="dropdown-search-result"><a class="search-add-person" href="' + result.url + '">' + result.line1;
            if (result.line2) {
                list += '<br/>' + result.line2;
            }
            if (result.cellphone != null && result.cellphone !== '') list += '<br/>' + result.cellphone;
            if (result.email != null && result.email !== '') list += '<br/>' + result.email;
            list += '</a></li>';
        });
    list += '</ul>';
    $('.search-results').html(list).slideDown();
}
