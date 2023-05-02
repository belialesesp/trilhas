angular
    .module('trilhasapp')
    .controller('EventoInscricaoController', EventoInscricaoController);

EventoInscricaoController.$inject = ['$stateParams', '$scope', '$q', '$http', 'PaginationService', 'ServerErrorsService', 'spinnerService'];

function EventoInscricaoController($stateParams, $scope, $q, $http, paginationService, ServerErrorsService, spinnerService) {
    var vm = this;

    vm.pager = {};
    vm.pageSize = 20;

    vm.listPageSize = [
        { qtd: 10, desc: '10' },
        { qtd: 20, desc: '20' },
        { qtd: 30, desc: '30' },
        { qtd: 50, desc: '50' }
    ];

    vm.eventoListaInscrito = {};

    vm.query = {};
    vm.cursistas = [];

    vm.init = function () {

        spinnerService.show('loader');

        var promises = [
            vm.carregarEvento($stateParams.id)
        ];

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

        var success = function (response) {
            var count = response.data;
            vm.pager = paginationService.GetPager(count, page, vm.pageSize);

            buscar();
        };

        var error = function (response) {
            toastr["error"]("Ocorreu um erro ao consultar os registros de cursistas.");
        };

        spinnerService.show('loader');

        return $http.get("/eventos/BuscarCursistasQuantidade", { params: vm.query }).then(success, error);
    };

    var buscar = function () {
        var success = function (response) {
            vm.cursistas = response.data;
        };

        var error = function (response) {
            toastr["error"]("Ocorreu um erro ao consultar os registros de cursistas.");
            console.log(response.data);
        };

        spinnerService.show('loader');

        return $http.get("/eventos/buscarCursistas", { params: vm.query }).then(success, error).finally(onComplete);
    };

    vm.carregarEvento = function (id) {
        return $http.get('/eventos/recuperarListaInscritos/' + id).then(function (response) {
            vm.eventoListaInscrito = response.data;
            vm.query.eventoId = response.data.evento.id;
        });
    };

    //ENTIDADE
    $scope.selecionarEntidade = function (entidade) {

        if (entidade) {
            vm.entidade = {
                'id': entidade.id,
                'nome': entidade.nome
            };
            vm.query.entidadeId = entidade.id;

        } else {
            toastr["error"]('Selecionar Entidade.');
        }
    };

    vm.marcarParaInscrever = function (id) {
        vm.registroParaInscrever = id;
    };

    vm.inscreverCursista = function () {

        var success = function (response) {
            vm.filtrar();
            toastr["success"]("Cursista " + response.data + " Inscrito(a) com Sucesso.", "Sucesso");
        };

        var error = function (response) {
            ServerErrorsService.handleServerErrors(response);
            toastr["error"](response.data.message, "Erro");
            console.log(response.data.internalMessage);
        };

        spinnerService.show('loader');

        return $http.post("/eventos/inscreverCursista", { 'pessoaId': vm.registroParaInscrever, 'eventoId': vm.eventoListaInscrito.evento.id })
            .then(success, error).finally(onComplete);
    };

    vm.init();
}
