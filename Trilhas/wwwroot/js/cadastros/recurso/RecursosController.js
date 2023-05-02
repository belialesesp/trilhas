angular.module('trilhasapp')
	.controller('RecursosController', RecursosController);

RecursosController.$inject = ['$stateParams', '$state', '$http', '$q', 'PaginationService', 'spinnerService'];

function RecursosController($stateParams, $state, $http, $q, paginationService, spinnerService) {
	var vm = this;

	vm.pager = {};
    vm.pageSize = 20;
    vm.listPageSize = [
        { qtd: 10, desc: '10' },
        { qtd: 20, desc: '20' },
        { qtd: 30, desc: '30' },
        { qtd: 50, desc: '50' }
    ];

	vm.recursos = null;

	vm.registroParaExcluir = null;

	vm.query = {
		'nome': null,
		'descricao': null,
		'excluidos': false
	};

	vm.init = function () {

		var promises = [];

        if ($stateParams.nome || $stateParams.descricao || $stateParams.excluidos || $stateParams.page || $stateParams.pageSize) {

			spinnerService.show('loader');

			vm.query = {
				'nome': $stateParams.nome,
				'descricao': $stateParams.descricao,
				'excluidos': $stateParams.excluidos === "true"
			};

            vm.pageSize = !$stateParams.pageSize ? vm.pageSize : parseInt($stateParams.pageSize);
			promises.push(vm.consultar($stateParams.page));
		}

		$q.all(promises).finally(onComplete);
	};

	var onComplete = function () {
		spinnerService.close('loader');
	};

	vm.filtrar = function (page) {

		if (!page && vm.pager.currentPage) {
			page = vm.pager.currentPage;
		}
		if (page < 1 || (page > vm.pager.totalPages && vm.pager.totalPages > 0)) {
			page = 1;
		}

		$state.go('recursos',
			{
				'nome': vm.query.nome,
				'descricao': vm.query.descricao,
				'excluidos': vm.query.excluidos,
                'page': page,
                'pageSize': vm.pageSize
			},
			{ reload: true });
	};

    vm.consultar = function (page) {

        page = parseInt(page);

		var success = function (response) {
			var count = response.data;
			vm.pager = paginationService.GetPager(count, page, vm.pageSize);

			buscar();
		};

		var error = function (response) {
			toastr["error"]("Ocorreu um erro ao consultar os registros.");
		};

		return $http.get("/recursos/quantidade", { params: vm.query })
			.then(success, error);
	};

	var buscar = function () {

		var success = function (response) {
			vm.recursos = response.data;
		};

		var error = function (response) {
			toastr["error"]("Ocorreu um erro ao consultar os registros.");
			console.log(response.data);
		};

		vm.query.start = vm.pager.startIndex;
		vm.query.count = vm.pager.pageSize;

		return $http.get("/recursos/buscar", { params: vm.query })
			.then(success, error)
			.finally(onComplete);
	};

	vm.marcarExclusao = function (id) {
		vm.registroParaExcluir = id;
	};

	vm.excluir = function (id) {

		var success = function (response) {
			toastr["success"]("Registro excluído com sucesso.");
			vm.registroParaExcluir = null;

			spinnerService.show('loader');
			vm.consultar();
		};

		var error = function (response) {
            toastr["error"](response.data, "Erro");
			console.log('Erro ao excluir recurso id = ' + vm.registroParaExcluir);
			vm.registroParaExcluir = null;
		};

		spinnerService.show('loader');
		return $http.delete('/recursos/excluir/' + vm.registroParaExcluir)
			.then(success, error)
			.finally(onComplete);
	};

	vm.init();
}