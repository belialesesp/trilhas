angular.module('trilhasapp')
    .service('PaginationService', function () {

        this.GetPager = function (totalItems, currentPage, pageSize) {
            currentPage = currentPage || 1;
            pageSize = pageSize || 10;

            var totalPages = Math.ceil(totalItems / pageSize);

            currentPage = currentPage <= totalPages ? currentPage : totalPages;

            var startPage, endPage;

            if (totalPages <= 10) {
                startPage = 1;
                endPage = totalPages;

            } else {
                if (currentPage <= 6) {
                    startPage = 1;
                    endPage = 10;
                } else if (currentPage + 4 >= totalPages) {
                    startPage = totalPages - 9;
                    endPage = totalPages;
                } else {
                    startPage = currentPage - 5;
                    endPage = currentPage + 4;
                }
            }

            var startIndex = (currentPage - 1) * pageSize;
            var endIndex = Math.min(startIndex + pageSize - 1, totalItems - 1);

            var pages = [];
            for (var i = startPage; i < endPage + 1; i++) {
                pages.push(i);
            }

            return {
                totalItems: totalItems,
                currentPage: currentPage,
                pageSize: pageSize,
                totalPages: totalPages,
                startPage: startPage,
                endPage: endPage,
                startIndex: startIndex,
                endIndex: endIndex,
                pages: pages
            };
        };
    });