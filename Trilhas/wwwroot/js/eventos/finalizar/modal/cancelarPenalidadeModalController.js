angular
    .module('trilhasapp')
    .controller('CancelarPenalidadeModalController', CancelarPenalidadeModalController);

CancelarPenalidadeModalController.$inject = ['$scope', '$http', 'ServerErrorsService'];

function CancelarPenalidadeModalController($scope, $http, ServerErrorsService) {
    var vm = this;

    vm.penalidade = {
        cancelada: false
    };

    var init = function () {
        $('#modalCancelarPenalidade').on('show.bs.modal', function (event) {
            vm.penalidade = $scope.penalidade;
            $scope.$apply();
        });
    };

    vm.salvar = function () {
        var success = function () {
            toastr["success"]("Penalidade cancelada com sucesso.", "Sucesso");
        };

        var error = function (response) {
            ServerErrorsService.handleServerErrors(response, form);
            toastr["error"](response.data.message, "Erro");
            console.log(response.data.internalMessage);
        };

        return $http.post('/eventos/salvarPenalidade', vm.penalidade).then(success, error);
    };

    init();
}