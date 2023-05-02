angular
    .module('trilhasapp')
    .controller('EventoModalEntidadeCotaController', EventoModalEntidadeCotaController);

EventoModalEntidadeCotaController.$inject = ['$scope', '$http', 'PaginationService', 'spinnerService'];

function EventoModalEntidadeCotaController($scope, $http, paginationService, spinnerService) {
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

        $('#modalBuscarEntidadeCota').on('show.bs.modal', function (event) {

            vm.entidades = undefined;

            vm.queryEntidade = {
                'nome': null,
                'tipoEntidadeId': null,
                'uf': null,
                'municipioId': null
            };

            carregarTiposEntidade();
            carregarUfs();
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

        return $http.get("/entidades/quantidade", { params: vm.queryEntidade }).then(success, error);
    };

    var buscar = function () {

        var success = function (response) {
            vm.entidades = response.data;
            spinnerService.close('loader');
        };

        var error = function (response) {
            toastr["error"]("Ocorreu um erro ao consultar os registros.");
            spinnerService.close('loader');
            console.log(response.data);
        };

        vm.queryEntidade.start = vm.pager.startIndex;
        vm.queryEntidade.count = vm.pager.pageSize;

        return $http.get("/entidades/buscar", { params: vm.queryEntidade })
            .then(success, error)
            .finally(onComplete);
    };

    vm.selecionarEntidadeCota = function (entidade) {
        $scope.selecionarEntidadeCota(entidade);
    };

    var carregarTiposEntidade = function () {
        return $http.get('/entidades/recuperarTipos').then(function (response) {
            vm.tiposEntidade = response.data;
        });
    };

    var carregarUfs = function () {
        return $http.get('/municipios/recuperarUfs').then(function (response) {
            vm.ufs = response.data;
        });
    };

    vm.carregarMunicipiosEntidade = function () {
        if (!vm.queryEntidade.uf) {
            vm.queryEntidade.municipioId = null;
            return;
        }

        return $http.get('/municipios/recuperarMunicipios?uf=' + vm.queryEntidade.uf).then(function (response) {
            vm.municipios = response.data;
        });
    };

    init();
}
