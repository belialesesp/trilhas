angular
    .module('trilhasapp')
    .controller('LocalModalRecursoController', LocalModalRecursoController);

LocalModalRecursoController.$inject = ['$scope', '$http', 'PaginationService', 'spinnerService'];

function LocalModalRecursoController($scope, $http, paginationService, spinnerService) {
    var vm = this;

    vm.pager = {};
    vm.pageSize = 5;

    vm.listPageSize = [
        { qtd: 5, desc: '5' },
        { qtd: 10, desc: '10' },
        { qtd: 20, desc: '20' },
        { qtd: 50, desc: '50' }
    ];

    var init = function () {

        $('#modalBuscarRecurso').on('show.bs.modal', function (event) {

            vm.recursos = undefined;

            vm.queryRecurso = {
                'nome': '',
                'descricao': ''
            };

            $scope.$apply();
        });
    };

    vm.filtrar = function (page) {

        spinnerService.show('loader');

        if (!page && vm.pager.currentPage) {
            page = vm.pager.currentPage;
        }
        if (page < 1 || (page > vm.pager.totalPages && vm.pager.totalPages > 0)) {
            page = 1;
        }

        var success = function (response) {
            var count = response.data;
            vm.pager = paginationService.GetPager(count, page, vm.pageSize);

            buscar();
        };

        var error = function (response) {
            toastr["error"]("Ocorreu um erro ao consultar os registros.");
        };

        return $http.get("/recursos/quantidade", { params: vm.queryRecurso }).then(success, error);
    };

    var buscar = function () {

        var success = function (response) {
            vm.recursos = response.data;
            spinnerService.close('loader');
        };

        var error = function (response) {
            toastr["error"]("Ocorreu um erro ao consultar os registros.");
            spinnerService.close('loader');
            console.log(response.data);
        };

        vm.queryRecurso.start = vm.pager.startIndex;
        vm.queryRecurso.count = vm.pager.pageSize;

        return $http.get("/recursos/buscar", { params: vm.queryRecurso })
            .then(success, error)
            .finally(onComplete);
    };

    vm.selecionarRecurso = function (recurso) {
        $scope.selecionarRecurso(recurso);
    };

    init();
}
