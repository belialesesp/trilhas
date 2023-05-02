angular
    .module('trilhasapp')
    .controller('DocenteModalCursoController', DocenteModalCursoController);

DocenteModalCursoController.$inject = ['$scope', '$http', 'PaginationService', 'spinnerService', 'ModalidadeFactory'];

function DocenteModalCursoController($scope, $http, paginationService, spinnerService, modalidadeFactory) {
    var vm = this;

    vm.pager = {};
    vm.pageSize = 5;

    vm.listPageSize = [
        { qtd: 5, desc: '5' },
        { qtd: 10, desc: '10' },
        { qtd: 20, desc: '20' },
        { qtd: 50, desc: '50' }
    ];

    vm.queryCurso = {
        'titulo': '',
        'eixoId': null,
        'estacaoId': null,
        'tipoSolucao': 'curso',
        'modalidadeCurso': null
    };

    var init = function () {
        carregarDropDownModalidades();
        carregarDropDownEixos();
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

        vm.queryCurso.start = vm.pager.startIndex;
        vm.queryCurso.count = vm.pager.pageSize;

        return $http.get("/solucoes/buscar", { params: vm.queryCurso }).then(success, error).finally(onComplete);
    };

    var onComplete = function () {
        spinnerService.close('loader');
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
