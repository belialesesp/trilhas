angular
    .module('trilhasapp')
    .controller('EventoModalMotivoExclusaoController', EventoModalMotivoExclusaoController);

EventoModalMotivoExclusaoController.$inject = ['$scope'];

function EventoModalMotivoExclusaoController($scope) {
    var vm = this;

    vm.excluir = function () {
        $scope.excluirEvento(vm.motivoExclusao);
    };
}