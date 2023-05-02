angular
    .module('trilhasapp')
    .controller('EixoFormularioController', EixoFormularioController);

EixoFormularioController.$inject = ['$state', '$stateParams', '$http', '$scope', 'ServerErrorsService', 'spinnerService'];

function EixoFormularioController($state, $stateParams, $http, $scope, ServerErrorsService, spinnerService) {
    var vm = this;

    vm.imageToCrop = '';
    vm.croppedImage = '';

    vm.inseriroNovoAposSalvar;

    vm.eixo = {};

    vm.isDirty = function () { return true; };

    vm.initExtra = function () {
        angular.element(document.querySelector('#icone')).on('change', onFileSelect);
        angular.element(document.querySelector('#icone')).on('click', onFileSelect);
        angular.element(document.querySelector('#icone')).on('click', function (evt) {
            $scope.$apply(function ($scope) {
                vm.imageToCrop = '';
                vm.croppedImage = '';
            });
        });
    };

    vm.init = function () {
        if ($stateParams.id) {
            spinnerService.show('loader');
            vm.carregarEixo($stateParams.id).then(onComplete);
        }
    };

    var onComplete = function () {
        spinnerService.close('loader');
    };

    vm.carregarEixo = function (id) {
        return $http.get('/eixos/recuperar/' + id).then(function (response) {
            vm.eixo = response.data;

            if (vm.eixo.storageError) {
                toastr["warning"](vm.eixo.storageError + '<br/><a href="/admin/storagestatus" target="_blank">Mais detalhes...</a>');
            }
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
                $state.go('eixos-cadastro', {}, { reload: true });

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
        return $http.post('/eixos/salvar', vm.eixo).then(success, error).finally(onComplete);
    };

    vm.excluir = function () {

        var success = function (response) {
            toastr["success"]("Registro excluído com sucesso.");
            $state.go('eixos');
        };

        var error = function (response) {
            toastr["error"](response.data, "Erro");
            console.log('Erro ao excluir eixo id = ' + vm.eixo.id);
        };

        spinnerService.show('loader');
        return $http.delete('/eixos/excluir/' + vm.eixo.id).then(success, error).finally(onComplete);
    };

    var onFileSelect = function (evt) {

        var file = evt.currentTarget.files[0];

        if (!file) {

            vm.pessoa.imagem = null;

            setTimeout(function () {
                $('#modal-crop').modal('hide');
            }, 100);

            return;
        }

        var reader = new FileReader();

        reader.onload = function (evt) {
            $scope.$apply(function ($scope) {
                vm.imageToCrop = evt.target.result;
            });
        };

        reader.readAsDataURL(file);
    };

    vm.concluirCrop = function () {
        vm.eixo.imagem = vm.croppedImage.split(';base64,')[1];
        //vm.eixo.imagem = $scope.croppedImage;
        $('#modal-crop').modal('hide');
    };

    vm.limparImagem = function () {
        vm.eixo.imagem = undefined;
    };

    vm.init();
}