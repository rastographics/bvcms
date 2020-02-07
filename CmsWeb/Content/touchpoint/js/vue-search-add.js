Vue.component("search-add", {
    props: ["target", "context"],
    template:
        "<input v-on:keyup='searchInput($event)' v-model='searchVal' type='text' class='form-control search-box-input mousetrap' placeholder='People or organizations...'>",
    data() {
        return {
            selectedIndex: null,
            searchVal: ""
        }
    },
    created() {
        var self = this;
        this.selectedIndex = -1;

        $(this.target).on("shown.bs.modal",
            function() {
                $(".search-box-input").focus();
                self.selectedIndex = -1;

                Mousetrap.bind("down",
                    function() {
                        self.keyboardSelectSearchResult("down");
                        return false;
                    });

                Mousetrap.bind("tab",
                    function() {
                        self.keyboardSelectSearchResult("down");
                        return false;
                    });

                Mousetrap.bind("up",
                    function() {
                        self.keyboardSelectSearchResult("up");
                        return false;
                    });

                Mousetrap.bind("shift+tab",
                    function() {
                        self.keyboardSelectSearchResult("up");
                        return false;
                    });

                Mousetrap.bind("enter",
                    function() {
                        self.chooseSearchResult();
                        return false;
                    });

                // call prefetch
                self.prefetch();
            });

        Mousetrap.bind("/",
            function() {
                $("#search-box").modal();
                return false;
            });
    },
    methods: {
        updateResults(results) {
            this.$emit("display-results", results);
        },
        prefetch() {
            $.ajax({
                type: "GET",
                url: "/FastSearchPrefetch",
                success: function (results) {
                    updateResults($.parseJSON(results));
                }
            });
        },
        keyboardSelectSearchResult(direction) {
            var searchResultCount = $(".search-results:visible .dropdown-search-result").length;
            var $lastSelected = $(".dropdown-search-result:nth(" + this.selectedIndex + ")");
            $lastSelected.removeClass("dropdown-search-result-selected");

            var scrollUp = direction === "up";

            if (scrollUp) {
                this.selectedIndex--;
                if (this.selectedIndex < 0) {
                    this.selectedIndex = searchResultCount - 1;
                }
            } else {
                this.selectedIndex++;
                if (this.selectedIndex >= searchResultCount) {
                    this.selectedIndex = 0;
                }
            }

            var $current = $(".dropdown-search-result:nth(" + this.selectedIndex + ")");
            $current.addClass("dropdown-search-result-selected");

            var $navBar = $(".search-results:visible .nav");

            if (this.selectedIndex === 0) {
                $navBar.scrollTop(0);
            } else if (this.selectedIndex === searchResultCount - 1) {
                $navBar.scrollTop(9999);
            } else {
                if (direction === "up") {
                    if ($current.position().top < ($navBar.scrollTop())) {
                        $navBar.scrollTop($navBar.scrollTop() - $current.height() * 2);
                    }
                } else {
                    if (($current.position().top + $current.height()) >
                        ($navBar.height() - $navBar.scrollTop())) {
                        $navBar.scrollTop($navBar.scrollTop() + $current.height() * 2);
                    }
                }
            }
        },
        chooseSearchResult() {
            // if a user hasn't selected anything at this point, select the first item in the list
            if (this.selectedIndex === -1) {
                this.selectedIndex = 0;
            }

            var href = $(".dropdown-search-result:nth(" + this.selectedIndex + ") a").attr("href");
            window.location = href;
        },
        searchInput: _.debounce(function (e) {
            if (e.keyCode !== 40 && e.keyCode !== 38) {
                if (this.searchVal.length > 0) {
                    var self = this;
                    $.ajax({
                        type: "POST",
                        url: "/FastSearch",
                        data: JSON.stringify({
                            "query": self.searchVal,
                            "context": self.context
                        }),
                        dataType: "json",
                        contentType: "application/json",
                        success: function (results) {
                            updateResults(results);
                        }
                    });
                } else {
                    this.prefetch();
                }
            }
        }, 400)
    }
});

new Vue({
    el: "#search-add-component"
})
