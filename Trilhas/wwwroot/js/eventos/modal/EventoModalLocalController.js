angular
    .module('trilhasapp')
    .controller('EventoModalLocalController', EventoModalLocalController);

EventoModalLocalController.$inject = ['$scope', '$http', 'PaginationService', 'spinnerService'];

function EventoModalLocalController($scope, $http, paginationService, spinnerService) {
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

        $('#modalBuscarLocal').on('show.bs.modal', function (event) {

            vm.locais = undefined;

            vm.queryLocal = {
                'nome': '',
                'endereco': '',
                'capacidade': ''
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

        return $http.get("/locais/quantidade", { params: vm.queryLocal }).then(success, error);
    };

    var buscar = function () {

        var success = function (response) {
            vm.locais = response.data;
            spinnerService.close('loader');
        };

        var error = function (response) {
            toastr["error"]("Ocorreu um erro ao consultar os registros.");
            spinnerService.close('loader');
            console.log(response.data);
        };

        vm.queryLocal.start = vm.pager.startIndex;
        vm.queryLocal.count = vm.pager.pageSize;

        return $http.get("/locais/buscar", { params: vm.queryLocal })
            .then(success, error)
            .finally(onComplete);
    };

    vm.selecionarLocal = function (local) {
        $scope.selecionarLocal(local);
    };

    init();
}
