angular
	.module('trilhasapp')
	.controller('RecursoFormularioController', RecursoFormularioController);

RecursoFormularioController.$inject = ['$state', '$stateParams', '$q', '$http', 'ServerErrorsService', 'spinnerService'];

function RecursoFormularioController($state, $stateParams, $q, $http, ServerErrorsService, spinnerService) {
	var vm = this;

	vm.inseriroNovoAposSalvar;

	vm.recurso = {};
    
	vm.init = function () {

		var promises = [];

		if ($stateParams.id) {
			spinnerService.show('loader');
			promises.push(vm.carregarRecurso($stateParams.id));
		}

		$q.all(promises).finally(onComplete);
	};

	var onComplete = function () {
		spinnerService.close('loader');
	};

	vm.carregarRecurso = function (id) {
		return $http.get('/recursos/recuperar/' + id).then(function (response) {
			vm.recurso = response.data;
		});
	};

	vm.salvarVoltar = function () {
		vm.inseriroNovoAposSalvar = false;
	};

	vm.salvarNovo = function () {
		vm.inseriroNovoAposSalvar = true;
	};

	vm.salvar = function (formRecurso) {

		if (formRecurso.$invalid) {
			toastr["error"]('Preencha corretamente os campos assinalados no formulário.', 'Preenchimento inválido');
			return;
		}

		var success = function (response) {
			toastr["success"]("Registro salvo com sucesso.", "Sucesso");
			formRecurso.$setPristine();
			if (vm.inseriroNovoAposSalvar) {
				$state.go('recursos-cadastro', {}, { reload: true });

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
		return $http.post('/recursos/salvar', vm.recurso).then(success, error).finally(onComplete);
	};

	vm.excluir = function () {

		var success = function (response) {
			toastr["success"]("Registro excluído com sucesso.");
			$state.go('recursos');
		};

		var error = function (response) {
            toastr["error"](response.data, "Erro");
			console.log('Erro ao excluir recurso id = ' + vm.recurso.id);
		};

		spinnerService.show('loader');
		return $http.delete('/recursos/excluir/' + vm.recurso.id).then(success, error).finally(onComplete);
	};

	vm.init();
}