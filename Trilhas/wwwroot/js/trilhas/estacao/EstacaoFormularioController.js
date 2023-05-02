angular
    .module('trilhasapp')
    .controller('EstacaoFormularioController', EstacaoFormularioController);

EstacaoFormularioController.$inject = ['$state', '$stateParams', '$http', '$q', 'ServerErrorsService', 'spinnerService'];

function EstacaoFormularioController($state, $stateParams, $http, $q, ServerErrorsService, spinnerService) {
    var vm = this;

    vm.inseriroNovoAposSalvar;

    vm.estacao = {};

    vm.init = function () {

        spinnerService.show('loader');

        var promises = [carregarDropDownEixos()];

        if ($stateParams.id) {
            promises.push(vm.carregarEstacao($stateParams.id));
        }

        $q.all(promises).finally(onComplete);
    };

    var onComplete = function () {
        spinnerService.close('loader');
    };

    var carregarDropDownEixos = function () {

        var success = function (response) {
            vm.eixos = response.data;
        };

        var error = function (response) {
            toastr["error"]("Ocorreu um erro ao consultar os registros.");
            console.log(response.data);
        };

        return $http.get("/eixos/dropdown").then(success, error);
    };

    vm.carregarEstacao = function (id) {
        return $http.get('/estacoes/recuperar/' + id).then(function (response) {
            vm.estacao = response.data;
        });
    };

    vm.salvarVoltar = function () {
        vm.inseriroNovoAposSalvar = false;
    };

    vm.salvarNovo = function () {
        vm.inseriroNovoAposSalvar = true;
    };

    vm.salvar = function (form) {

        if (form.$invalid) {
            toastr["error"]('Preencha corretamente os campos assinalados no formulário.', 'Preenchimento inválido');
            return;
        }

        var success = function (response) {
            toastr["success"]("Registro salvo com sucesso.", "Sucesso");
			form.$setPristine();
            if (vm.inseriroNovoAposSalvar) {
                $state.go('estacoes-cadastro', {}, { reload: true });

            } else {
                history.back();
            }
        };

        var error = function (response) {
            ServerErrorsService.handleServerErrors(response, form);
            toastr["error"](response.data.message, "Erro");
            console.log(response.data.internalMessage);
        };

        spinnerService.show('loader');
        return $http.post('/estacoes/salvar', vm.estacao).then(success, error).finally(onComplete);
    };

    vm.excluir = function () {

        var success = function (response) {
            toastr["success"]("Registro excluído com sucesso.");
            $state.go('estacoes');
        };

        var error = function (response) {
            toastr["error"](response.data, "Erro");
            console.log('Erro ao excluir estação id = ' + vm.estacao.id);
        };

        spinnerService.show('loader');
        return $http.delete('/estacoes/excluir/' + vm.estacao.id).then(success, error).finally(onComplete);
    };

    vm.init();
}