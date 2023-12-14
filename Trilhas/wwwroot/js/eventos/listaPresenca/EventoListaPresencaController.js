angular
	.module('trilhasapp')
	.controller('EventoListaPresencaController', EventoListaPresencaController);

EventoListaPresencaController.$inject = ['$scope', '$http', 'ServerErrorsService', 'spinnerService'];

function EventoListaPresencaController($scope, $http, ServerErrorsService, spinnerService) {
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

	vm.consultarEExportarExcel_Individual = function () {
		
		var successBaixarArquivo = function (response) {

			var bin = atob(response.data.fileString);
			var ab = s2ab(bin); // from example above
			var blob = new Blob([ab], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;' });

			var link = document.createElement('a');
			link.href = window.URL.createObjectURL(blob);
			link.download = response.data.fileName;

			document.body.appendChild(link);

			link.click();

			document.body.removeChild(link);

			toastr["success"]("Planilha Criada com Sucesso.");
			spinnerService.close('loader');
		};

		var errorBaixarArquivo = function (response) {
			toastr["error"]("Ocorreu um erro ao consultar os registros.");
		};

		spinnerService.show('loader');
		return $http.get("/listaInscricaoManual/ExportarRelatorioListaIndividual?horarioId=" + vm.horarioId + "&eventoId=" + vm.listaPresenca.eventoId).then(successBaixarArquivo, errorBaixarArquivo).finally(onComplete);

	}

	vm.consultarEExportarExcel_Geral = function () {

		var successBaixarArquivo = function (response) {

			var bin = atob(response.data.fileString);
			var ab = s2ab(bin); // from example above
			var blob = new Blob([ab], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;' });

			var link = document.createElement('a');
			link.href = window.URL.createObjectURL(blob);
			link.download = response.data.fileName;

			document.body.appendChild(link);

			link.click();

			document.body.removeChild(link);

			toastr["success"]("Planilha Criada com Sucesso.");
			spinnerService.close('loader');
		};

		var errorBaixarArquivo = function (response) {
			toastr["error"]("Ocorreu um erro ao consultar os registros.");
		};

		spinnerService.show('loader');
		return $http.get("/listaInscricaoManual/ExportarRelatorioListaCompleta?eventoId=" + vm.listaPresenca.eventoId).then(successBaixarArquivo, errorBaixarArquivo).finally(onComplete);

	}

	function s2ab(s) {
		var buf = new ArrayBuffer(s.length);
		var view = new Uint8Array(buf);
		for (var i = 0; i != s.length; ++i) view[i] = s.charCodeAt(i) & 0xFF;
		return buf;
	}

	//ENTIDADE

};