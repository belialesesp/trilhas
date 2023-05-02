angular.module('trilhasapp')
    .controller('EstacoesController', EstacoesController);

EstacoesController.$inject = ['$stateParams', '$state', '$http', '$q', 'PaginationService', 'spinnerService'];

function EstacoesController($stateParams, $state, $http, $q, paginationService, spinnerService) {
    var vm = this;

    vm.pager = {};
    vm.pageSize = 20;
    vm.listPageSize = [
        { qtd: 10, desc: '10' },
        { qtd: 20, desc: '20' },
        { qtd: 30, desc: '30' },
        { qtd: 50, desc: '50' }
    ];

    vm.estacoes = null;
    vm.eixos = null;

    vm.registroParaExcluir = null;

    vm.query = {
        'eixoId': null,
        'nome': null,
        'excluidos': false
    };

    vm.init = function () {

        spinnerService.show('loader');

        var promises = [vm.carregarDropDownEixos()];

        if ($stateParams.eixoId || $stateParams.nome || $stateParams.excluidos || $stateParams.page || $stateParams.pageSize) {

            vm.query = {
                'eixoId': $stateParams.eixoId ? parseInt($stateParams.eixoId) : null,
                'nome': $stateParams.nome,
                'excluidos': $stateParams.excluidos === "true"
            };

            vm.pageSize = !$stateParams.pageSize ? vm.pageSize : parseInt($stateParams.pageSize);
            promises.push(vm.consultar($stateParams.page));
        }

        $q.all(promises).finally(onComplete);
    };

    var onComplete = function () {
        spinnerService.close('loader');
    };

    vm.filtrar = function (page) {

        if (!page && vm.pager.currentPage) {
            page = vm.pager.currentPage;
        }
        if (page < 1 || (page > vm.pager.totalPages && vm.pager.totalPages > 0)) {
            page = 1;
        }

        $state.go('estacoes',
            {
                'nome': vm.query.nome,
                'eixoId': vm.query.eixoId,
                'excluidos': vm.query.excluidos,
                'page': page,
                'pageSize': vm.pageSize
            },
            { reload: true });
    };

    vm.consultar = function (page) {

        page = parseInt(page);

        var success = function (response) {
            var count = response.data;
            vm.pager = paginationService.GetPager(count, page, vm.pageSize);

            buscar();
        };

        var error = function (response) {
            toastr["error"]("Ocorreu um erro ao consultar os registros.");
        };

        return $http.get("/estacoes/quantidade", { params: vm.query }).then(success, error);
    };

    var buscar = function () {

        var success = function (response) {
            vm.estacoes = response.data;
        };

        var error = function (response) {
            toastr["error"]("Ocorreu um erro ao consultar os registros.");
            console.log(response.data);
        };

        vm.query.start = vm.pager.startIndex;
        vm.query.count = vm.pager.pageSize;

        return $http.get("/estacoes/buscar", { params: vm.query }).then(success, error).finally(onComplete);
    };

    vm.marcarExclusao = function (id) {
        vm.registroParaExcluir = id;
    };

    vm.excluir = function (id) {

        var success = function (response) {
            toastr["success"]("Registro excluído com sucesso.");
            vm.registroParaExcluir = null;

            spinnerService.show('loader');
            vm.consultar();
        };

        var error = function (response) {
            toastr["error"](response.data, "Erro");
            console.log('Erro ao excluir estação id = ' + vm.registroParaExcluir);
            vm.registroParaExcluir = null;
        };

        spinnerService.show('loader');
        return $http.delete('/estacoes/excluir/' + vm.registroParaExcluir).then(success, error).finally(onComplete);
    };

    vm.carregarDropDownEixos = function () {

        var success = function (response) {
            vm.eixos = response.data;
            };

        var error = function (response) {
            toastr["error"]("Ocorreu um erro ao consultar os registros.");
            console.log(response.data);
        };

        return $http.get("/eixos/dropdown").then(success, error);
    };

    vm.init();
}
