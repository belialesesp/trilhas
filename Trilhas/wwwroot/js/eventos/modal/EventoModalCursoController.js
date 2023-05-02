angular
	.module('trilhasapp')
	.controller('EventoModalCursoController', EventoModalCursoController);

EventoModalCursoController.$inject = ['$scope', '$http', 'PaginationService', 'spinnerService', 'ModalidadeFactory'];

function EventoModalCursoController($scope, $http, paginationService, spinnerService, modalidadeFactory) {
	var vm = this;

	vm.pager = {};
	vm.pageSize = 5;

	vm.listPageSize = [
		{ qtd: 5, desc: '5' },
		{ qtd: 10, desc: '10' },
		{ qtd: 20, desc: '20' },
		{ qtd: 50, desc: '50' }
	];

	var init = function () {

		$('#modalBuscarSolucao').on('show.bs.modal', function (event) {

			vm.cursos = undefined;

			vm.queryCurso = {
				'titulo': '',
				'eixoId': null,
				'estacaoId': null,
				'tipoSolucao': 'curso',
				'modalidadeCurso': null
			};

			carregarDropDownModalidades();
			carregarDropDownEixos();
			$scope.$apply();
		});
	};

	vm.filtrar = function (page) {

		spinnerService.show('loader');

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
			toastr["error"]("Ocorreu um erro ao consultar os registros.");
		};

		return $http.get("/solucoes/quantidade", { params: vm.queryCurso }).then(success, error);
	};

	var buscar = function () {

		var success = function (response) {
			vm.cursos = response.data;
			spinnerService.close('loader');
		};

		var error = function (response) {
			toastr["error"]("Ocorreu um erro ao consultar os registros.");
			spinnerService.close('loader');
			console.log(response.data);
		};

		vm.queryCurso.naoIniciados = true;
		vm.queryCurso.andamentos = true;
		vm.queryCurso.concluidos = true;
		vm.queryCurso.inscricao = true;
		vm.queryCurso.finalizados = true;
		vm.queryCurso.cancelados = false;
		vm.queryCurso.start = vm.pager.startIndex;
		vm.queryCurso.count = vm.pager.pageSize;

		return $http.get("/solucoes/buscar", { params: vm.queryCurso }).then(success, error).finally(onComplete);
	};

	vm.selecionarCurso = function (curso) {
		$scope.selecionarCurso(curso);
	};

	var carregarDropDownModalidades = function () {
		modalidadeFactory.getModalidade().then(function (response) {
			vm.modalidades = response;
		});
	};

	var carregarDropDownEixos = function () {
		var success = function (response) {
			vm.eixos = response.data;
		};

		var error = function (response) {
			toastr["error"]("Ocorreu um erro ao consultar os registros dos eixos.");
			console.log(response.data);
		};

		return $http.get("/eixos/dropdown").then(success, error);
	};

	vm.carregarDropDownEstacoes = function () {
		var success = function (response) {
			vm.estacoes = response.data;
		};

		var error = function (response) {
			toastr["error"]("Ocorreu um erro ao consultar os registros das estações.");
			console.log(response.data);
		};

		return $http.get("/estacoes/dropdown", { params: vm.queryCurso.eixoId }).then(success, error);
	};

	init();
}
