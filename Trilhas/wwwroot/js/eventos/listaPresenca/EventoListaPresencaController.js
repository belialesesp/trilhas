angular
	.module('trilhasapp')
	.controller('EventoListaPresencaController', EventoListaPresencaController);

EventoListaPresencaController.$inject = ['$scope', '$http', 'ServerErrorsService'];

function EventoListaPresencaController($scope, $http, ServerErrorsService) {
	var vm = this;

	vm.desabilitarListaPresenca = true;

	vm.query = {
		id: 0,
		codigoBarras: ''
	};

	$scope.selecionarEvento = function (evento) {
		if (evento) {
			vm.query.codigoBarras = '';
			vm.query.id = evento.id;
			vm.buscarDadosParaRegistrarPresenca();
			//vm.evento = evento;
		} else {
			toastr["error"]('Selecionar Solucao.');
		}
	};

	vm.buscarDadosParaRegistrarPresenca = function () {
		var success = function (response) {
			vm.listaPresenca = response.data;
			vm.listaInscritoAux = response.data.inscritos;
			vm.desabilitarListaPresenca = true;

			var horario = vm.listaPresenca.eventoHorarios.find(x => x.selecionar);

			if (horario !== undefined) {
				vm.horarioId = horario.eventoHorarioId;
				vm.desabilitarListaPresenca = false;
			}
		};
		var error = function (response) {
			ServerErrorsService.handleServerErrors(response, form);
			toastr["error"](response.data.message, "Erro");
			console.log(response.data.internalMessage);
		};

		return $http.get('/listaPresenca/buscarDadosParaRegistrarPresenca', { params: vm.query }).then(success, error);
	};


	vm.filtrarRelatorio = function (page) {
		debugger;

		if (!vm.cursistaNome) {
			toastr["warning"]("É obrigatório informar o Cursista.");

			return false;
		}

		if (!page && vm.pager.currentPage) {
			page = vm.pager.currentPage;
		}
		if (page < 1 || (page > vm.pager.totalPages && vm.pager.totalPages > 0)) {
			page = 1;
		}

		$state.go('relatorioHistoricoDeCursista',
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




	vm.atualizarListaInscritos = function (horario) {
		if (horario.selecionar) {
			for (var i in vm.listaPresenca.eventoHorarios) {
				if (vm.listaPresenca.eventoHorarios[i].eventoHorarioId !== horario.eventoHorarioId) {
					vm.listaPresenca.eventoHorarios[i].selecionar = false;
				}
			}

			vm.query.eventoHorarioId = horario.eventoHorarioId;

			return $http.get('/listaPresenca/atualizarListaInscritorPorHorario', { params: vm.query }).then(function (response) {
				vm.listaPresenca.inscritos = response.data;
				vm.desabilitarListaPresenca = false;
				vm.horarioId = horario.eventoHorarioId;
			});
		} else {
			for (var j in vm.listaPresenca.eventoHorarios) {
				vm.listaPresenca.eventoHorarios[j].selecionar = false;
			}

			vm.desabilitarListaPresenca = true;
			vm.listaPresenca.inscritos = vm.listaInscritoAux;
		}
	};

	vm.salvarPresenca = function (inscrito) {
		return $http.post('/listaPresenca/salvarListaPresenca', inscrito).catch(function () {
			toastr["error"]("Ocorreu um erro ao salvar o registro.");
		});
	};



	vm.relatorioHorario = function () {
		var success = function (response) { };

		var error = function (response) { };

		return $http.get('/listaInscricaoManual/index?idHorario=' + vm.idHorario + '&eventoId=' + vm.evento.id).then(success, error);
	};
};