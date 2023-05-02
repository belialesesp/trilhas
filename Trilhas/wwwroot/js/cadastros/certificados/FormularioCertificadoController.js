angular.module('trilhasapp')
    .controller('FormularioCertificadoController', FormularioCertificadoController);

FormularioCertificadoController.$inject = ['$state', '$stateParams', '$http', '$q', 'ServerErrorsService', 'spinnerService'];

function FormularioCertificadoController($state, $stateParams, $http, $q, ServerErrorsService, spinnerService) {
    var vm = this;

    vm.editorOptions = {
        language: 'pt-br',
        allowedContent: true,
        entities: false
    };

    vm.certificado = {};

    vm.init = function () {
        var promises = [];

        if ($stateParams.id) {
            spinnerService.show('loader');

            promises.push(vm.carregarCertificado($stateParams.id).then(function () {
                vm.impressao = vm.certificado.dados;
            }));
        } else {
            vm.certificado.tipoCertificado = vm.tipoCertificado();
        }

        $q.all(promises).finally(onComplete);
    };

    vm.tipoCertificado = function () {
        if ($state.current.name === 'certificados-cadastro' || $state.current.name === 'certificados-edicao') {
            return 0;
        } else if ($state.current.name === 'certificados-declaracaoCursista-cadastro' || $state.current.name === 'certificados-declaracaoCursista-edicao') {
            return 1;
        } else if ($state.current.name === 'certificados-declaracaoDocente-cadastro' || $state.current.name === 'certificados-declaracaoDocente-edicao') {
            return 2;
        }
    };

    var onComplete = function () {
        spinnerService.close('loader');
    };

    vm.salvar = function (form) {
        if (form.$invalid) {
            toastr["error"]('Preencha corretamente os campos assinalados no formulário.', 'Preenchimento inválido');
            return;
        }

        var success = function () {
            toastr["success"]("Registro salvo com sucesso.", "Sucesso");
            form.$setPristine();
            history.back();
        };

        var error = function (response) {
            ServerErrorsService.handleServerErrors(response, form);
            toastr["error"](response.data.message, "Erro");
            console.log(response.data.internalMessage);
        };

        spinnerService.show('loader');
        return $http.post('/certificados/salvar', vm.certificado).then(success, error).finally(onComplete);
    };

    vm.carregarCertificado = function (id) {
        return $http.get('/certificados/recuperar?id=' + id + '&tipoCertificado=' + vm.tipoCertificado()).then(function (response) {
            vm.certificado = response.data;
        }).catch(function () {
            history.back();
            toastr["error"]("Ocorreu ao recuperar certificado.");
        });
    };

    vm.onComplete = function () {
        vm.impressao = vm.certificado.dados;
    };

    vm.init();
}