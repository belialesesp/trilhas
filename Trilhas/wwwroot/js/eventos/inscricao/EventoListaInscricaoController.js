angular
    .module('trilhasapp')
    .controller('EventoListaInscricaoController', EventoListaInscricaoController);

EventoListaInscricaoController.$inject = ['$scope', '$stateParams', '$q', '$http', 'ServerErrorsService', 'spinnerService'];

function EventoListaInscricaoController($scope, $stateParams, $q, $http, ServerErrorsService, spinnerService) {
    var vm = this;

    vm.inseriroNovoAposSalvar;
    vm.inscricaoNaoIniciada = false;
    vm.inscricaoAndamento = false;
    vm.inscricaoFinalizada = false;
    vm.eventoListaInscrito = {};
    vm.listaInscritosId = $stateParams.id;
	vm.dataAtual = new Date();
    vm.query = {};

    vm.init = function () {
        var promises = [];

        vm.carregarEvento($stateParams.id);

        $q.all(promises).finally(onComplete);
    };

    var onComplete = function () {
        spinnerService.close('loader');
    };

    vm.carregarEvento = function (id) {
        return $http.get('/eventos/recuperarListaInscritos/' + id).then(function (response) {
            vm.eventoListaInscrito = response.data;
            vm.eventoId = response.data.evento.id;

            vm.validaInscricao(vm.eventoListaInscrito.evento);
        });
    };

    vm.validaInscricao = function (evento) {
        var dataAtual = new Date();
        var dataInicio = new Date(evento.dataInicioInscricao);
        var dataFim = new Date(evento.dataFimInscricao);

        if ((dataInicio < dataAtual) && (dataFim > dataAtual)) {
            vm.inscricaoAndamento = true;

        } else if (dataInicio > dataAtual) {
            vm.inscricaoNaoIniciada = true;

        } else if (dataAtual > dataFim) {
            vm.inscricaoFinalizada = true;
        }
    };

    vm.marcarCancelamento = function (id) {
        vm.registroParaCancelar = id;
    };

    vm.cancelarInscricao = function () {

        var success = function (response) {
            vm.eventoId = $stateParams.id;
            vm.carregarEvento(vm.eventoId);
            toastr["success"]("Inscrição Cancelada com Sucesso.", "Sucesso");
        };

        var error = function (response) {
            ServerErrorsService.handleServerErrors(response, form);
            toastr["error"](response.data.message, "Erro");
            console.log(response.data.internalMessage);
        };

        spinnerService.show('loader');

        return $http.post("/eventos/cancelarInscricao", { 'eventoId': vm.eventoListaInscrito.evento.id, 'pessoaId': vm.registroParaCancelar })
            .then(success, error).finally(onComplete);
    };

    vm.init();
}
