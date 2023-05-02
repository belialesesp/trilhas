angular
    .module('trilhasapp')
    .controller('EventoModalCursistaController', EventoModalCursistaController);

EventoModalCursistaController.$inject = ['$scope', '$http', 'PaginationService', 'spinnerService'];

function EventoModalCursistaController($scope, $http, paginationService, spinnerService) {
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

        $('#modalBuscarCursista').on('show.bs.modal', function (event) {

            vm.cursistasPesquisa = undefined;

            vm.queryCursista = {
                'nome': null,
                'email': null,
                'cpf': null,
                'numeroFuncioanl': null,
                'excluidos': false
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

        return $http.get("/pessoas/quantidade", { params: vm.queryCursista }).then(success, error);
    };

    var buscar = function () {

        var success = function (response) {
            vm.cursistasPesquisa = response.data;
            spinnerService.close('loader');
        };

        var error = function (response) {
            toastr["error"]("Ocorreu um erro ao consultar os registros.");
            spinnerService.close('loader');
            console.log(response.data);
        };

        vm.queryCursista.start = vm.pager.startIndex;
        vm.queryCursista.count = vm.pager.pageSize;

        return $http.get("/pessoas/buscar", { params: vm.queryCursista }).then(success, error).finally(onComplete);
    };

    vm.selecionarCursista = function (cursista) {
        $scope.selecionarCursista(cursista);
    };

    init();
}
