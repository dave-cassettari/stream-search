
$(function () {

    var searchTimeout = null,
        searchViewModel = {
            term: ko.observable('test'),
            results: ko.observableArray(),
        };

    searchViewModel.term.subscribe(function (term) {

        if (searchTimeout) {
            clearTimeout(searchTimeout);
        }

        searchTimeout = setTimeout(function () {

            if (term == null || term == '') {

                searchViewModel.results.removeAll();

                return;
            }

            var data = {
                page: 1,
                term: term,
            };

            $.post('/api/search', data, function (response) {

                console.log(response);
            });
        }, 250);
    });

    ko.applyBindings(searchViewModel);

});