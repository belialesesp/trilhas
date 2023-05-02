angular.module('trilhasapp')
	.controller('CursistasController', CursistasController);

CursistasController.$inject = ['$scope', '$stateParams', '$state', '$http', '$q', 'PaginationService', 'spinnerService', 'ModalidadeFactory'];

function CursistasController($scope, $stateParams, $state, $http, $q, paginationService, spinnerService, modalidadeFactory) {
	var vm = this;

    vm.dataAtual = new Date()

	vm.cursistas = null;
    vm.solucoes = [];

    vm.pager = {};
	vm.pageSize = 20;
	vm.listPageSize = [
		{ qtd: 10, desc: '10' },
		{ qtd: 20, desc: '20' },
		{ qtd: 30, desc: '30' },
		{ qtd: 50, desc: '50' },
		{ qtd: 100, desc: '100' }
    ];

    $scope.ufs = [];

	vm.query = {
		'cursista': null,
		'curso': null,
		'modalidade': null,
		'entidade': null,
		'uf': null,
		'municipio': null,
		'dataInicio': null,
        'dataFim': null,
        'desistentes': false
	};

	vm.init = function () {

		var promises = [
			vm.carregarModalidades(),
			vm.carregarUfs()
		];

		if ($stateParams.cursista
			|| $stateParams.curso
			|| $stateParams.modalidade
			|| $stateParams.entidade
			|| $stateParams.uf
			|| $stateParams.municipio
			|| $stateParams.dataInicio
			|| $stateParams.dataFim
			|| $stateParams.page
            || $stateParams.pageSize
            || $stateParams.desistentes
        ) {
			spinnerService.show('loader');

			vm.query = {
				'cursista': $stateParams.cursista,
				'curso': $stateParams.curso,
				'modalidade': $stateParams.modalidade ? parseInt($stateParams.modalidade) : undefined,
				'entidade': $stateParams.entidade ? parseInt($stateParams.entidade) : undefined,
				'uf': $stateParams.uf,
				'municipio': $stateParams.municipio ? parseInt($stateParams.municipio) : undefined,
				'dataInicio': $stateParams.dataInicio ? new Date($stateParams.dataInicio) : undefined,
                'dataFim': $stateParams.dataFim ? new Date($stateParams.dataFim) : undefined,
                'desistentes': $stateParams.desistentes === 'true',
            };

            if (vm.query.cursista) {
                promises.push(carregarCursista());
            }
            if (vm.query.curso) {
                promises.push(carregarSolucao());
            }
            if (vm.query.entidade) {
                promises.push(carregarEntidade());
            }

            vm.pageSize = !$stateParams.pageSize ? vm.pageSize : parseInt($stateParams.pageSize);

			if ($stateParams.uf) {				
				promises.push(vm.carregarMunicipios());
            }

			promises.push(vm.consultar($stateParams.page));
		}

        $q.all(promises).then(function () {
            $scope.ufs = vm.ufs;
        }).finally(onComplete);
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

		$state.go('cursistas',
			{
				'cursista': vm.query.cursista,
				'curso': vm.query.curso,
				'modalidade': vm.query.modalidade,
				'entidade': vm.query.entidade,
				'uf': vm.query.uf,
				'municipio': vm.query.municipio,
				'dataInicio': vm.query.dataInicio,
				'dataFim': vm.query.dataFim,
				'page': page,
                'pageSize': vm.pageSize,
                'desistentes': vm.query.desistentes
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

		return $http.get("/pessoas/quantidadeCursistaGrid", { params: vm.query }).then(success, error);
	};

	var buscar = function () {

		var success = function (response) {
			vm.cursistas = response.data;
		};

		var error = function (response) {
			toastr["error"]("Ocorreu um erro ao consultar os registros.");
			console.log(response.data);
		};

		vm.query.start = vm.pager.startIndex;
		vm.query.count = vm.pager.pageSize;

		return $http.get("/pessoas/buscarCursistas", { params: vm.query }).then(success, error).finally(onComplete);
	};

	vm.carregarModalidades = function () {
        modalidadeFactory.getModalidade().then(function (response) {
            vm.modalidades = response;
        });
	};

	vm.carregarUfs = function () {
		return $http.get('/municipios/recuperarUfs').then(function (response) {
			vm.ufs = response.data;
		});
	};

	vm.carregarMunicipios = function () {
		return $http.get('/municipios/recuperarMunicipios?uf=' + vm.query.uf).then(function (response) {
			vm.municipios = response.data;
		});
	};

	vm.selecionarCursista = function (cursista) {
		if (vm.cursistaSelecionado.id === cursista.id) {
			vm.cursistaSelecionado = {};
		} else {
			vm.cursistaSelecionado = {
				'id': cursista.id
			};
		}
	};

	//ENTIDADE
	$scope.selecionarEntidade = function (entidade) {
		if (entidade) {
			vm.entidadeNome = entidade.nome;
			vm.query.entidade = entidade.id;
		} else {
			toastr["error"]('Selecionar Entidade.');
		}
	};

	//CURSO
	$scope.selecionarCurso = function (curso) {
		if (curso) {
			vm.cursoTitulo = curso.titulo;
			vm.query.curso = curso.id;
		} else {
			toastr["error"]('Selecionar Curso.');
		}
	};

	//PESSOA
	//vm.carregarPessoa = function (cpf) {
	//	return $http.get('/pessoas/recuperarPessoaPorCpf/' + cpf).then(function (response) {
	//		vm.docente.nome = response.data.nome;
	//		vm.docente.pis = response.data.pis;
	//		vm.docente.titulo = response.data.numeroTitulo;
	//		vm.docente.zona = response.data.zonaTitulo;
	//		vm.docente.secao = response.data.secaoTitulo;
	//		vm.docente.Pessoa = response.data.id;
	//	});
	//};

	$scope.selecionarPessoa = function (pessoa) {
		if (pessoa) {
			vm.cursistaNome = pessoa.nome;
			vm.query.cursista = pessoa.id;
		} else {
			toastr["error"]('Selecionar Pessoa.');
		}
    };

    var carregarCursista = function (id) {
        return $http.get('/pessoas/RecuperarBasico/' + vm.query.cursista).then(function (response) {
            vm.cursistaNome = response.data.nome;
        });
    };

    var carregarEntidade = function () {
        return $http.get('/entidades/RecuperarBasico/' + vm.query.entidade).then(function (response) {
            vm.entidadeNome = response.data.sigla + ' - ' +response.data.nome;
        });
    };

    var carregarSolucao = function () {
        return $http.get('/solucoes/RecuperarBasico/' + vm.query.curso).then(function (response) {
            vm.cursoTitulo = response.data.titulo;
        });
    };

	vm.init();
}