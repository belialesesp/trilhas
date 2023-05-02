angular.module('trilhasapp')
	.controller('EntidadesController', EntidadesController);

EntidadesController.$inject = ['$stateParams', '$state', '$http', '$q', 'PaginationService', 'spinnerService'];

function EntidadesController($stateParams, $state, $http, $q, paginationService, spinnerService) {
	var vm = this;

	vm.pager = {};
    vm.pageSize = 20;
    vm.listPageSize = [
        { qtd: 10, desc: '10' },
        { qtd: 20, desc: '20' },
        { qtd: 30, desc: '30' },
        { qtd: 50, desc: '50' }
    ];

	vm.entidades = null;

	vm.registroParaExcluir = null;

	vm.tipos = [];
	vm.municipios = [];
	vm.ufs = [];

	vm.query = {
		'nome': null,
		'tipoEntidadeId': null,
		'uf': null,
		'municipioId': null,
		'excluidos': false
	};

    vm.init = function () {

        var promises = [
            vm.carregarTiposEntidade(),
            vm.carregarUfs()
        ];

        if ($stateParams.nome || $stateParams.tipoEntidadeId || $stateParams.uf || $stateParams.municipioId
            || $stateParams.excluidos || $stateParams.page || $stateParams.pageSize) {

			spinnerService.show('loader');

			vm.query = {
				'nome': $stateParams.nome,
                'tipoEntidadeId': $stateParams.tipoEntidadeId ? parseInt($stateParams.tipoEntidadeId) : null,
				'uf': $stateParams.uf,
                'municipioId': $stateParams.municipioId ? parseInt($stateParams.municipioId) : null,
				'excluidos': $stateParams.excluidos === "true"
			};

            vm.pageSize = !$stateParams.pageSize ? vm.pageSize : parseInt($stateParams.pageSize);
            promises.push(vm.carregarMunicipios());
            promises.push(consultar($stateParams.page));
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

		$state.go('entidades',
			{
				'nome': vm.query.nome,
				'tipoEntidadeId': vm.query.tipoEntidadeId,
				'uf': vm.query.uf,
				'municipioId': vm.query.municipioId,
				'excluidos': vm.query.excluidos,
                'page': page,
                'pageSize': vm.pageSize
			},
			{ reload: true });
	};

	var consultar = function (page) {

        page = parseInt(page);

		var success = function (response) {
			var count = response.data;
			vm.pager = paginationService.GetPager(count, page, vm.pageSize);

			buscar();
		};

		var error = function (response) {
			toastr["error"]("Ocorreu um erro ao consultar os registros.");
        };

		return $http.get("/entidades/quantidade", { params: vm.query }).then(success, error);
	};

	var buscar = function () {

		var success = function (response) {
			vm.entidades = response.data;
		};

		var error = function (response) {
			toastr["error"]("Ocorreu um erro ao consultar os registros.");
			console.log(response.data);
		};

		vm.query.start = vm.pager.startIndex;
		vm.query.count = vm.pager.pageSize;

		return $http.get("/entidades/buscar", { params: vm.query }).then(success, error).finally(onComplete);
	};

	vm.marcarExclusao = function (id) {
		vm.registroParaExcluir = id;
	};

	vm.excluir = function (id) {

		var success = function (response) {
			toastr["success"]("Registro excluído com sucesso.");
			vm.registroParaExcluir = null;

            spinnerService.show('loader');

			consultar();
		};

		var error = function (response) {
            toastr["error"](response.data, "Erro");
			console.log('Erro ao excluir entidade id = ' + vm.registroParaExcluir);
			vm.registroParaExcluir = null;
		};

        spinnerService.show('loader');

		return $http.delete('/entidades/excluir/' + vm.registroParaExcluir).then(success, error).finally(onComplete);
    };

    vm.carregarUfs = function () {
        return $http.get('/municipios/recuperarUfs').then(function (response) {
            vm.ufs = response.data;
        });
    };

    vm.carregarMunicipios = function () {
        if (!vm.query.uf) {
            vm.query.municipioId = null;
            return;
        }

		return $http.get('/municipios/recuperarMunicipios?uf=' + vm.query.uf).then(function (response) {
            vm.municipios = response.data;
		});
    };

	vm.carregarTiposEntidade = function () {
		return $http.get('/entidades/recuperarTipos').then(function (response) {
			vm.tipos = response.data;
		});
	};

	vm.init();
}