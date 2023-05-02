angular
    .module('trilhasapp')
	.controller('ProgramacaoEventoModalController', ProgramacaoEventoModalController);

ProgramacaoEventoModalController.$inject = ['$scope', '$q', '$http', 'PaginationService', 'ModalidadeFactory'];

function ProgramacaoEventoModalController($scope, $q, $http, paginationService, modalidadeFactory) {
    var vm = this;

    vm.pager = {};
    vm.pageSize = 5;

    vm.listPageSize = [
        { qtd: 5, desc: '5' },
        { qtd: 10, desc: '10' },
        { qtd: 20, desc: '20' },
        { qtd: 50, desc: '50' }
    ];

    vm.query = {
        'curso': null,
        'modalidade': null,
        'entidadeDemandante': null,
        'municipioId': null,
        'docente': null,
        'dataInicio': null,
		'dataFim': null,
		'cancelados': false,
		'naoIniciados': true,
		'andamentos': true,
		'concluidos': true,
		'inscricao': true,
		'finalizados': true
    };

    function init() {
        var promises = [
            vm.carregarModalidades(),
            vm.carregarUfs()
        ];

        $q.all(promises).finally(onComplete);
    }

    var onComplete = function () {
        spinnerService.close('loader');
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

    vm.filtrar = function (page) {

        page = parseInt(page);

        var success = function (response) {
            var count = response.data;
            vm.pager = paginationService.GetPager(count, page, vm.pageSize);

            buscar();
        };

        var error = function (response) {
            toastr["error"]("Ocorreu um erro ao consultar os registros.");
        };

        return $http.get("/eventos/quantidade", { params: vm.query }).then(success, error);
    };

    var buscar = function () {

		var success = function (response) {
			vm.eventos = response.data.listaEventos;
			//vm.eventos = response.data;
        };

        var error = function (response) {
            toastr["error"]("Ocorreu um erro ao consultar os registros.");
            console.log(response.data);
        };

        if (vm.query.dataInicio) {
            vm.query.dataInicio = new Date(vm.query.dataInicio);
        }
        if (vm.query.dataFim) {
            vm.query.dataFim = new Date(vm.query.dataFim);
        }

        vm.query.start = vm.pager.startIndex;
        vm.query.count = vm.pager.pageSize;

        return $http.get("/eventos/buscar", { params: vm.query }).then(success, error).finally(onComplete);
    };

    vm.selecionarEvento = function (evento) {
        $scope.selecionarEvento(evento);
    };

    init();
}