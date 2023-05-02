angular.module('trilhasapp')
    .controller('SolucoesController', SolucoesController);

SolucoesController.$inject = ['$stateParams', '$state', '$http', '$q', 'PaginationService', 'spinnerService', 'ModalidadeFactory'];

function SolucoesController($stateParams, $state, $http, $q, paginationService, spinnerService, modalidadeFactory) {
    var vm = this;

    vm.pager = {};
    vm.pageSize = 20;
    vm.listPageSize = [
        { qtd: 10, desc: '10' },
        { qtd: 20, desc: '20' },
        { qtd: 30, desc: '30' },
        { qtd: 50, desc: '50' }
    ];

    vm.eixos = null;
    vm.estacoes = null;
    vm.solucoes = null;

    vm.tiposSolucao = [
        { 'value': 'curso', 'text': 'Curso' },
        { 'value': 'livro', 'text': 'Livro/Artigo' },
        { 'value': 'video', 'text': 'Vídeo' }
    ];

    vm.registroParaExcluir = null;

    vm.query = {
        'eixoId': null,
        'estacaoId': null,
        'titulo': null,
        'excluidos': false
    };

    vm.init = function () {

        spinnerService.show('loader');

        var promises = [
            vm.carregarDropDownEixos(),
            vm.carregarDropDownEstacoes(),
            carregarDropDownModalidadesDeCurso(),
            carregarDropDownNiveisDeCurso()
        ];

        if ($stateParams.eixoId || $stateParams.estacaoId || $stateParams.titulo || $stateParams.excluidos || $stateParams.page
            || $stateParams.tipoSolucao || $stateParams.sigla || $stateParams.modalidadeCurso || $stateParams.nivelCurso
            || $stateParams.responsavel || $stateParams.autor || $stateParams.editora || $stateParams.pageSize) {

            vm.query = {
                'eixoId': $stateParams.eixoId ? parseInt($stateParams.eixoId) : null,
                'estacaoId': $stateParams.estacaoId ? parseInt($stateParams.estacaoId) : null,
                'titulo': $stateParams.titulo,
                'excluidos': $stateParams.excluidos === "true",
                'tipoSolucao': $stateParams.tipoSolucao,
                'sigla': $stateParams.sigla,
                'modalidadeCurso': $stateParams.modalidadeCurso ? parseInt($stateParams.modalidadeCurso) : null,
                'nivelCurso': $stateParams.nivelCurso ? parseInt($stateParams.nivelCurso) : null,
                'autor': $stateParams.autor,
                'editora': $stateParams.editora,
                'responsavel': $stateParams.responsavel
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

        var params = {
            'eixoId': vm.query.eixoId,
            'estacaoId': vm.query.estacaoId,
            'titulo': vm.query.titulo,
            'excluidos': vm.query.excluidos,
            'tipoSolucao': vm.query.tipoSolucao,
            'sigla': null,
            'modalidadeCurso': null,
            'nivelCurso': null,
            'autor': null,
            'editora': null,
            'responsavel': null,
            'page': page,
            'pageSize': vm.pageSize
        };

        if (vm.query.tipoSolucao === vm.tiposSolucao[0].value) {
            params.sigla = vm.query.sigla;
            params.modalidadeCurso = vm.query.modalidadeCurso;
            params.nivelCurso = vm.query.nivelCurso;

        } else if (vm.query.tipoSolucao === vm.tiposSolucao[1].value) {
            params.autor = vm.query.autor;
            params.editora = vm.query.editora;

        } else if (vm.query.tipoSolucao === vm.tiposSolucao[2].value) {
            params.responsavel = vm.query.responsavel;
        }

        $state.go('solucoes', params, { reload: true });
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

        return $http.get("/solucoes/quantidade", { params: vm.query }).then(success, error);
    };

    var buscar = function () {

        var success = function (response) {
            vm.solucoes = response.data;
        };

        var error = function (response) {
            toastr["error"]("Ocorreu um erro ao consultar os registros.");
            console.log(response.data);
        };

        vm.query.start = vm.pager.startIndex;
        vm.query.count = vm.pager.pageSize;

        return $http.get("/solucoes/buscar", { params: vm.query }).then(success, error).finally(onComplete);
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
            console.log('Erro ao excluir Solução Educacional id = ' + vm.registroParaExcluir);
            vm.registroParaExcluir = null;
        };

        spinnerService.show('loader');
        return $http.delete('/solucoes/excluir/' + vm.registroParaExcluir).then(success, error).finally(onComplete);
    };

    vm.carregarDropDownEixos = function () {

        var success = function (response) {
            vm.eixos = response.data;
        };

        var error = function (response) {
            toastr["error"]("Ocorreu um erro ao consultar Eixos.");
            console.log(response.data);
        };

        return $http.get("/eixos/dropdown").then(success, error);
    };

    vm.carregarDropDownEstacoes = function () {

        spinnerService.show('loader');

        var success = function (response) {
            vm.estacoes = response.data;
        };

        var error = function (response) {
            toastr["error"]("Ocorreu um erro ao consultar Estações.");
            console.log(response.data);
        };

        return $http.get("/estacoes/dropdown?eixoId=" + vm.query.eixoId).then(success, error).finally(onComplete);
    };

    var carregarDropDownModalidadesDeCurso = function () {
        modalidadeFactory.getModalidade().then(function (response) {
            vm.modalidadesCurso = response;
        });
    };

    var carregarDropDownNiveisDeCurso = function () {
        var success = function (response) {
            vm.niveisCurso = response.data;
        };

        var error = function (response) {
            toastr["error"]("Ocorreu um erro ao consultar Níveis de curso.");
            console.log(response.data);
        };

        return $http.get('/solucoes/buscarNiveisDeCurso').then(success, error);
    };

    vm.limparFiltrosAvancados = function () {
        if (vm.query) {
            vm.query.sigla = null;
            vm.query.modalidadeCurso = null;
            vm.query.nivelCurso = null;
            vm.query.autor = null;
            vm.query.editora = null;
            vm.query.responsavel = null;
        }
    };

    vm.init();
}