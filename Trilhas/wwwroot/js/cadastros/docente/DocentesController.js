angular.module('trilhasapp')
    .controller('DocentesController', DocentesController);

DocentesController.$inject = ['$stateParams', '$state', '$http', '$q', 'PaginationService', 'spinnerService', 'ModalidadeFactory', '$scope'];

function DocentesController($stateParams, $state, $http, $q, paginationService, spinnerService, modalidadeFactory, $scope) {
    var vm = this;

    vm.pager = {};
    vm.docentes = null;
    vm.solucoes = [];
	vm.pageSize = 20;
	vm.dataAtual = {};
    vm.listPageSize = [
        { qtd: 10, desc: '10' },
        { qtd: 20, desc: '20' },
        { qtd: 30, desc: '30' },
        { qtd: 50, desc: '50' },
        { qtd: 100, desc: '100' }
    ];

    vm.registroParaExcluir = null;

    vm.query = {
        'nomeDocente': null,
        'cursoId': null,
        'modalidadeCurso': null,
        'dataInicio': null,
        'dataFim': null,
        'excluidos': false,
        'modalidade': null
    };

    vm.init = function () {

        var promises = [vm.carregarModalidades()];

        if ($stateParams.nomeDocente
            || $stateParams.cursoId
            || $stateParams.modalidadeCurso
            || $stateParams.dataInicio
            || $stateParams.dataFim
            || $stateParams.excluidos
            || $stateParams.page
            || $stateParams.pageSize) {

            spinnerService.show('loader');

            vm.query = {
                'nomeDocente': $stateParams.nomeDocente,
                'cursoId': $stateParams.cursoId,
                'modalidadeCurso': $stateParams.modalidadeCurso ? parseInt($stateParams.modalidadeCurso) : undefined,
                'dataInicio': $stateParams.dataInicio ? new Date($stateParams.dataInicio) : undefined,
                'dataFim': $stateParams.dataFim ? new Date($stateParams.dataFim) : undefined,
                'excluidos': $stateParams.excluidos === "true"
            };

            if (vm.query.cursoId) {
                promises.push(carregarSolucao());
            }

            vm.pageSize = !$stateParams.pageSize ? vm.pageSize : parseInt($stateParams.pageSize);

            promises.push(vm.consultar($stateParams.page));
        }
		vm.dataAtual = new Date();
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

        $state.go('docentes',
            {
                'nomeDocente': vm.query.nomeDocente,
                'cursoId': vm.query.cursoId,
                'modalidadeCurso': vm.query.modalidadeCurso,
                'dataInicio': vm.query.dataInicio,
                'dataFim': vm.query.dataFim,
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

        return $http.get("/docentes/quantidadeGrid", { params: vm.query })
            .then(success, error);
    };

    vm.carregarModalidades = function () {
        modalidadeFactory.getModalidade().then(function (response) {
            vm.modalidades = response;
        });
    };

    var buscar = function () {

        var success = function (response) {
            vm.docentes = response.data;
        };

        var error = function (response) {
            toastr["error"]("Ocorreu um erro ao consultar os registros.");
            console.log(response.data);
        };

        vm.query.start = vm.pager.startIndex;
        vm.query.count = vm.pager.pageSize;

        return $http.get("/docentes/buscarDadosGrid", { params: vm.query })
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
            console.log('Erro ao excluir docente id = ' + vm.registroParaExcluir);
            vm.registroParaExcluir = null;
        };

        spinnerService.show('loader');
        return $http.delete('/docentes/excluir/' + vm.registroParaExcluir)
            .then(success, error)
            .finally(onComplete);
    };

    vm.selecionarDocente = function (docente) {
        if (vm.docenteSelecionado.id === docente.id) {
            vm.docenteSelecionado = {};
        } else {
            vm.docenteSelecionado = {
                'id': docente.id
            };
        }
    };

    vm.carregarSolucoes = function (tipo) {

        var success = function (response) {
            vm.solucoes = response.data;
        };

        var error = function (response) {
            toastr["error"]("Ocorreu um erro ao consultar os registros.");
            console.log(response.data);
        };

        vm.query.start = vm.pager.startIndex;
        vm.query.count = vm.pager.pageSize;

        return $http.get("/solucoes/buscar", tipo).then(success, error).finally(onComplete);
    };

    $scope.selecionarCurso = function (curso) {
        if (curso) {
            vm.eventoTitulo = curso.titulo;
            vm.query.cursoId = curso.id;
        } else {
            toastr["error"]('Selecionar Solucao.');
        }
    };

    var carregarSolucao = function () {
        var success = function (response) {
            vm.eventoTitulo = response.data.titulo;
        };

        var error = function (response) {
            toastr["error"]("Ocorreu um erro ao consultar os registros da solução");
        };

        return $http.get('/solucoes/RecuperarBasico/' + vm.query.cursoId).then(success, error);
    };

    vm.init();
}