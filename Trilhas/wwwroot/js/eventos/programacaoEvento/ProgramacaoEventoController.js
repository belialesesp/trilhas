angular
	.module('trilhasapp')
	.controller('ProgramacaoEventoController', ProgramacaoEventoController);

ProgramacaoEventoController.$inject = ['$state', '$stateParams', '$http', '$scope', 'ServerErrorsService', 'spinnerService', '$timeout'];

function ProgramacaoEventoController($state, $stateParams, $http, $scope, ServerErrorsService, spinnerService, $timeout) {
	var vm = this;
	vm.data;

	vm.meses = [
		{ 'id': '1', 'value': 'Janeiro' },
		{ 'id': '2', 'value': 'Fevereiro' },
		{ 'id': '3', 'value': 'Março' },
		{ 'id': '4', 'value': 'Abril' },
		{ 'id': '5', 'value': 'Maio' },
		{ 'id': '6', 'value': 'Junho' },
		{ 'id': '7', 'value': 'Julho' },
		{ 'id': '8', 'value': 'Agosto' },
		{ 'id': '9', 'value': 'Setembro' },
		{ 'id': '10','value': 'Outubro' },
		{ 'id': '11','value': 'Novembro' },
		{ 'id': '12', 'value': 'Dezembro' }
	];

	vm.formataData = function () {
		vm.data = new Date();
		//vm.mes = vm.mes;
		vm.ano = vm.data.getFullYear();
	};

	$scope.selecionarEvento = function (evento) {
		if (evento) {
			//vm.query.id = evento.id;
			vm.evento = evento;
		} else {
			toastr["error"]('Selecionar Solucao.');
		}
	};
}
