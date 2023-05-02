angular
	.module('trilhasapp')
    .controller('EntidadeFormularioController', EntidadeFormularioController);

EntidadeFormularioController.$inject = ['$state', '$stateParams', '$scope', '$q', '$http', 'ServerErrorsService', 'spinnerService'];

function EntidadeFormularioController($state, $stateParams, $scope, $q, $http, ServerErrorsService, spinnerService) {
	var vm = this;

	vm.inseriroNovoAposSalvar;

    vm.entidade = {};
    vm.gestores = [];

	vm.tipos = [];
	vm.ufs = [];
	vm.municipios = [];

    vm.init = function () {

        var promises = [
            vm.carregarUfs(),
            vm.carregarTiposEntidade()
        ];

		if ($stateParams.id) {
            promises.push(vm.carregarEntidade($stateParams.id).then(function () {
                vm.carregarMunicipios(vm.entidade.uf);
            }));
        }

        spinnerService.show('loader');
        $q.all(promises).finally(onComplete);
    };

    vm.setForm = function (form) {
        vm.form = form;
    };

	var onComplete = function () {
		spinnerService.close('loader');
    };

	vm.carregarEntidade = function (id) {
        return $http.get('/entidades/recuperar/' + id).then(function (response) {
            vm.entidade = response.data;
            vm.gestores = vm.entidade.gestores;
            vm.entidade.gestores = [];
		});
    };

    vm.carregarUfs = function () {
        return $http.get('/municipios/recuperarUfs').then(function (response) {
            vm.ufs = response.data;
        });
    };

	vm.carregarMunicipios = function (uf) {
		return $http.get('/municipios/recuperarMunicipios?uf=' + uf).then(function (response) {
			vm.municipios = response.data;
		});
    };

	vm.carregarTiposEntidade = function () {
		return $http.get('/entidades/recuperarTipos').then(function (response) {
			vm.tipos = response.data;
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
				$state.go('entidades-cadastro', {}, { reload: true });

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

        vm.entidade.gestores = vm.gestores.map(function (g) { return g.id; });
        
		return $http.post('/entidades/salvar', vm.entidade).then(success, error).finally(onComplete);
	};

	vm.excluir = function () {

		var success = function (response) {
			toastr["success"]("Registro excluído com sucesso.");
			$state.go('entidades');
		};

		var error = function (response) {
			toastr["error"](response.data);
			console.log('Erro ao excluir entidade id = ' + vm.entidade.id);
		};

        spinnerService.show('loader');

		return $http.delete('/entidades/excluir/' + vm.entidade.id).then(success, error).finally(onComplete);
    };

    /*** MODAL GESTORES ***/

    vm.excluirGestor = function (id) {
        vm.gestores = vm.gestores.filter(function (gestor) {
            return gestor.id !== id;
        });
        vm.form.form.$setDirty();
    };

    $scope.selecionarGestor = function (gestor) {
        if (!vm.gestores) {
            vm.gestores = [];
        }

        var existe = vm.gestores.find(function (v) { return v.id === gestor.id; });

        if (!existe) {
            vm.gestores.push(gestor);
            vm.form.form.$setDirty();
        }
    };

	vm.init();
}