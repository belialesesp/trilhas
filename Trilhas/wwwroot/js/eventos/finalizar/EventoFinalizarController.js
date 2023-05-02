angular
    .module('trilhasapp')
    .controller('EventoFinalizarController', EventoFinalizarController);

EventoFinalizarController.$inject = ['$stateParams', '$q', '$http', 'spinnerService', '$scope'];

function EventoFinalizarController($stateParams, $q, $http, spinnerService, $scope) {
    var vm = this;

    vm.eventoListaInscrito = {};
	vm.dataAtual = new Date();
    vm.query = {};

    vm.init = function () {
        var promises = [];

        if ($stateParams.id) {
            vm.carregarEvento($stateParams.id);
        }

        $q.all(promises).finally(onComplete);
    };

    var onComplete = function () {
        spinnerService.close('loader');
    };

    vm.carregarEvento = function (id) {
        return $http.get('/eventos/encerramento/' + id).then(function (response) {
            vm.evento = response.data;
        });
    };

    vm.visualizarPenalidade = function (penalidade) {
        $scope.penalidade = penalidade;
    };

    //vm.alterarFrequencia = function (inscritoId, frequencia) {
    //    spinnerService.show('loader');

    //    return $http.post('/eventos/alterarFrequencia?inscritoId=' + inscritoId + '&frequencia=' + frequencia).then(function () {
    //        vm.carregarEvento($stateParams.id).finally(onComplete);
    //    });
    //};

    vm.init();
}
