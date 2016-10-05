
$(function () {

    var searchTimeout = null,
        searchViewModel = {
            term: ko.observable(),
            results: ko.observableArray(),
        };

    searchViewModel.term.subscribe(function (term) {

        if (searchTimeout) {
            clearTimeout(searchTimeout);
        }

        searchTimeout = setTimeout(function () {

            var results = searchViewModel.results;

            if (term == null || term == '') {

                results.removeAll();

                return;
            }

            var data = {
                page: 0,
                term: term,
            };

            $.post('/api/search', data, function (response) {

                results.removeAll();

                for (var i = 0; i < response.length; i++) {
                    results.push(response[i]);
                }
            });
        }, 250);
    });

    ko.applyBindings(searchViewModel);
});