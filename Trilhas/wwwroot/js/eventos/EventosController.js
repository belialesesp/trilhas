
angular
    .module('trilhasapp')
    .controller('EventosController', EventosController);

EventosController.$inject = ['$scope', '$stateParams', '$state', '$http', '$q', 'PaginationService', 'spinnerService', 'ModalidadeFactory'];

function EventosController($scope, $stateParams, $state, $http, $q, paginationService, spinnerService, modalidadeFactory) {
    var vm = this;

    vm.dataAtual = Date.now();

    vm.pager = {};
    vm.pageSize = 20;
    vm.listPageSize = [
        { qtd: 10, desc: '10' },
        { qtd: 20, desc: '20' },
        { qtd: 30, desc: '30' },
        { qtd: 50, desc: '50' },
        { qtd: 100, desc: '100' }
    ];

    vm.eventos = null;
    vm.registroParaExcluir = null;

    $scope.ufs = [];
    $scope.municipios = [];
    $scope.entidades = [];

    vm.query = {
        'cursoId': null,
        'modalidade': null,
        'entidadeDemandanteId': null,
        'uf': null,
        'municipioId': null,
        'docenteId': null,
        'cursistaId': null,
        'dataInicio': null,
        'dataFim': null,
        'cancelados': false,
        'naoIniciados': true,
        'andamentos': true,
        'concluidos': true,
        'inscricao': true,
        'finalizados': true
    };

    vm.init = function () {
        var promises = [
            vm.carregarUfs(),
            vm.carregarModalidades(),
            vm.carregarEntidades()
        ];

        if ($stateParams.cursoId
            || $stateParams.modalidade
            || $stateParams.entidadeDemandanteId
            || $stateParams.uf
            || $stateParams.municipioId
            || $stateParams.docenteId
            || $stateParams.cursistaId
            || $stateParams.dataInicio
            || $stateParams.dataFim
            || $stateParams.cancelados
            || $stateParams.naoIniciados
            || $stateParams.andamentos
            || $stateParams.concluidos
            || $stateParams.page
            || $stateParams.pageSize
            || $stateParams.inscricao
            || $stateParams.finalizados) {

            spinnerService.show('loader');

            vm.query = {
                'cursoId': $stateParams.cursoId,
                'modalidade': $stateParams.modalidade ? parseInt($stateParams.modalidade) : undefined,
                'entidadeDemandanteId': $stateParams.entidadeDemandanteId ? parseInt($stateParams.entidadeDemandanteId) : undefined,
                'uf': $stateParams.uf,
                'municipioId': $stateParams.municipioId ? parseInt($stateParams.municipioId) : undefined,
                'docenteId': $stateParams.docenteId,
                'cursistaId': $stateParams.cursistaId,
                'dataInicio': $stateParams.dataInicio ? new Date($stateParams.dataInicio) : undefined,
                'dataFim': $stateParams.dataFim ? new Date($stateParams.dataFim) : undefined,
                'naoIniciados': !$stateParams.naoIniciados || $stateParams.naoIniciados === "true",
                'andamentos': !$stateParams.andamentos || $stateParams.andamentos === "true",
                'concluidos': !$stateParams.concluidos || $stateParams.concluidos === "true",
                'inscricao': !$stateParams.inscricao || $stateParams.inscricao === "true",
                'finalizados': !$stateParams.finalizados || $stateParams.finalizados === "true",
                'cancelados': $stateParams.cancelados === "true"
            };

            if (vm.query.cursoId) {
                promises.push(carregarSolucao());
            }
            if (vm.query.entidadeDemandanteId) {
                promises.push(carregarEntidade());
            }
            if (vm.query.docenteId) {
                promises.push(carregarDocente());
            }
            if (vm.query.cursistaId) {
                promises.push(carregarCursista());
            }
            if (vm.query.uf) {
                promises.push(vm.carregarMunicipios());
            }

            vm.pageSize = !$stateParams.pageSize ? vm.pageSize : parseInt($stateParams.pageSize);

            promises.push(consultar($stateParams.page));
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

        $state.go('eventos',
            {
                'cursoId': vm.query.cursoId,
                'modalidade': vm.query.modalidade,
                'entidadeDemandanteId': vm.query.entidadeDemandanteId,
                'uf': vm.query.uf,
                'municipioId': vm.query.municipioId,
                'docenteId': vm.query.docenteId,
                'cursistaId': vm.query.cursistaId,
                'dataInicio': vm.query.dataInicio,
                'dataFim': vm.query.dataFim,
                'cancelados': vm.query.cancelados,
                'naoIniciados': vm.query.naoIniciados,
                'andamentos': vm.query.andamentos,
                'concluidos': vm.query.concluidos,
                'page': page,
                'pageSize': vm.pageSize,
                'inscricao': vm.query.inscricao,
                'finalizados': vm.query.finalizados
            },
            { reload: true });
    };

    vm.filtrarRelatorio = function (page) {

        if (vm.query.dataInicio > vm.query.dataFim) {

            toastr["warning"]("Data Início não pode ser maior que Data Fim.");

            return false;
        }

        if (!page && vm.pager.currentPage) {
            page = vm.pager.currentPage;
        }
        if (page < 1 || (page > vm.pager.totalPages && vm.pager.totalPages > 0)) {
            page = 1;
        }

        $state.go('eventosRelatoriocapacitadosPorPeriodo',
            {
                'cursoId': vm.query.cursoId,
                'modalidade': vm.query.modalidade,
                'entidadeDemandanteId': vm.query.entidadeDemandanteId,
                'uf': vm.query.uf,
                'municipioId': vm.query.municipioId,
                'docenteId': vm.query.docenteId,
                'cursistaId': vm.query.cursistaId,
                'dataInicio': vm.query.dataInicio,
                'dataFim': vm.query.dataFim,
                'cancelados': vm.query.cancelados,
                'naoIniciados': vm.query.naoIniciados,
                'andamentos': vm.query.andamentos,
                'concluidos': vm.query.concluidos,
                'page': page,
                'pageSize': vm.pageSize,
                'inscricao': vm.query.inscricao,
                'finalizados': vm.query.finalizados
            },
            { reload: true });
    };

    vm.filtrarRelatorioCapacitadosPorSolucoesEducacionais = function (page) {

        if (vm.query.dataInicio > vm.query.dataFim) {

            toastr["warning"]("Data Início não pode ser maior que Data Fim.");

            return false;
        }

        if (!page && vm.pager.currentPage) {
            page = vm.pager.currentPage;
        }
        if (page < 1 || (page > vm.pager.totalPages && vm.pager.totalPages > 0)) {
            page = 1;
        }

        $state.go('eventosRelatoriocapacitadosPorSolucoesEducacionais',
            {
                'cursoId': vm.query.cursoId,
                'modalidade': vm.query.modalidade,
                'entidadeDemandanteId': vm.query.entidadeDemandanteId,
                'uf': vm.query.uf,
                'municipioId': vm.query.municipioId,
                'docenteId': vm.query.docenteId,
                'cursistaId': vm.query.cursistaId,
                'dataInicio': vm.query.dataInicio,
                'dataFim': vm.query.dataFim,
                'cancelados': vm.query.cancelados,
                'naoIniciados': vm.query.naoIniciados,
                'andamentos': vm.query.andamentos,
                'concluidos': vm.query.concluidos,
                'page': page,
                'pageSize': vm.pageSize,
                'inscricao': vm.query.inscricao,
                'finalizados': vm.query.finalizados
            },
            { reload: true });
    };
    vm.filtrarRelatorioCapacitadosPorCurso = function (page) {

        if (vm.query.dataInicio > vm.query.dataFim) {

            toastr["warning"]("Data Início não pode ser maior que Data Fim.");

            return false;
        }

        if (!vm.query.cursoId) {
            toastr["warning"]("É obrigatório informar o curso.");

            return false;
        }

        if (!page && vm.pager.currentPage) {
            page = vm.pager.currentPage;
        }
        if (page < 1 || (page > vm.pager.totalPages && vm.pager.totalPages > 0)) {
            page = 1;
        }
        vm.query.naoIniciados = false;
        vm.query.andamentos = false;
        vm.query.concluidos = false;
        vm.query.inscricao = false;

        $state.go('eventosRelatoriocapacitadosPorCurso',
            {
                'cursoId': vm.query.cursoId,
                'modalidade': vm.query.modalidade,
                'entidadeDemandanteId': vm.query.entidadeDemandanteId,
                'uf': vm.query.uf,
                'municipioId': vm.query.municipioId,
                'docenteId': vm.query.docenteId,
                'cursistaId': vm.query.cursistaId,
                'dataInicio': vm.query.dataInicio,
                'dataFim': vm.query.dataFim,
                'cancelados': vm.query.cancelados,
                'naoIniciados': vm.query.naoIniciados,
                'andamentos': vm.query.andamentos,
                'concluidos': vm.query.concluidos,
                'page': page,
                'pageSize': vm.pageSize,
                'inscricao': vm.query.inscricao,
                'finalizados': vm.query.finalizados
            },
            { reload: true });
    };


    vm.filtrarRelatorioCapacitadosPorEntidade = function (page) {

        if (vm.query.dataInicio > vm.query.dataFim) {

            toastr["warning"]("Data Início não pode ser maior que Data Fim.");

            return false;
        }


        if (!vm.query.entidadeDemandanteId) {
            toastr["warning"]("É obrigatório informar a entidade.");

            return false;
        }

        if (!page && vm.pager.currentPage) {
            page = vm.pager.currentPage;
        }
        if (page < 1 || (page > vm.pager.totalPages && vm.pager.totalPages > 0)) {
            page = 1;
        }

        $state.go('eventosRelatoriocapacitadosPorEntidade',
            {
                'cursoId': vm.query.cursoId,
                'modalidade': vm.query.modalidade,
                'entidadeDemandanteId': vm.query.entidadeDemandanteId,
                'uf': vm.query.uf,
                'municipioId': vm.query.municipioId,
                'docenteId': vm.query.docenteId,
                'cursistaId': vm.query.cursistaId,
                'dataInicio': vm.query.dataInicio,
                'dataFim': vm.query.dataFim,
                'cancelados': vm.query.cancelados,
                'naoIniciados': vm.query.naoIniciados,
                'andamentos': vm.query.andamentos,
                'concluidos': vm.query.concluidos,
                'page': page,
                'pageSize': vm.pageSize,
                'inscricao': vm.query.inscricao,
                'finalizados': vm.query.finalizados
            },
            { reload: true });
    };



    vm.filtrarRelatorioCursistas = function (page) {

        if (vm.query.dataInicio > vm.query.dataFim) {

            toastr["warning"]("Data Início não pode ser maior que Data Fim.");

            return false;
        }


        if (!vm.query.entidadeDemandanteId) {
            toastr["warning"]("É obrigatório informar a entidade.");

            return false;
        }

        if (!page && vm.pager.currentPage) {
            page = vm.pager.currentPage;
        }
        if (page < 1 || (page > vm.pager.totalPages && vm.pager.totalPages > 0)) {
            page = 1;
        }



        $state.go('eventosRelatorioDeCursista',
            {
                'cursoId': vm.query.cursoId,
                'modalidade': vm.query.modalidade,
                'entidadeDemandanteId': vm.query.entidadeDemandanteId,
                'uf': vm.query.uf,
                'municipioId': vm.query.municipioId,
                'docenteId': vm.query.docenteId,
                'cursistaId': vm.query.cursistaId,
                'dataInicio': vm.query.dataInicio,
                'dataFim': vm.query.dataFim,
                'cancelados': vm.query.cancelados,
                'naoIniciados': vm.query.naoIniciados,
                'andamentos': vm.query.andamentos,
                'concluidos': vm.query.concluidos,
                'page': page,
                'pageSize': vm.pageSize,
                'inscricao': vm.query.inscricao,
                'finalizados': vm.query.finalizados
            },
            { reload: true });
    };

    vm.filtrarRelatorioModalide = function (page) {

        if (vm.query.dataInicio > vm.query.dataFim) {

            toastr["warning"]("Data Início não pode ser maior que Data Fim.");

            return false;
        }

        if (!vm.query.entidadeDemandanteId) {
            toastr["warning"]("É obrigatório informar a entidade.");

            return false;
        }

        if (!page && vm.pager.currentPage) {
            page = vm.pager.currentPage;
        }
        if (page < 1 || (page > vm.pager.totalPages && vm.pager.totalPages > 0)) {
            page = 1;
        }

        if (!vm.query.cursoId) {
            toastr["warning"]("É obrigatório informar o curso.");

            return false;
        }

        if (!page && vm.pager.currentPage) {
            page = vm.pager.currentPage;
        }
        if (page < 1 || (page > vm.pager.totalPages && vm.pager.totalPages > 0)) {
            page = 1;
        }


        $state.go('eventosRelatoriocapacitadosPorModalidade',
            {
                'cursoId': vm.query.cursoId,
                'modalidade': vm.query.modalidade,
                'entidadeDemandanteId': vm.query.entidadeDemandanteId,
                'uf': vm.query.uf,
                'municipioId': vm.query.municipioId,
                'docenteId': vm.query.docenteId,
                'cursistaId': vm.query.cursistaId,
                'dataInicio': vm.query.dataInicio,
                'dataFim': vm.query.dataFim,
                'cancelados': vm.query.cancelados,
                'naoIniciados': vm.query.naoIniciados,
                'andamentos': vm.query.andamentos,
                'concluidos': vm.query.concluidos,
                'page': page,
                'pageSize': vm.pageSize,
                'inscricao': vm.query.inscricao,
                'finalizados': vm.query.finalizados
            },
            { reload: true });
    };

    vm.imprimir = function () {
        window.print();
    }


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

        return $http.get("/eventos/quantidade", { params: vm.query }).then(success, error);
    };

    vm.consultarEExportarExcel = function () {

        if (vm.query.dataInicio > vm.query.dataFim) {

            toastr["warning"]("Data Início não pode ser maior que Data Fim.");

            return false;
        }

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
        };

        var errorBaixarArquivo = function (response) {
            toastr["error"]("Ocorreu um erro ao consultar os registros.");
        };

        spinnerService.show('loader');
        return $http.get("/eventos/exportarRelatorioCapacitadosPorPeriodoExcel", { params: vm.query }).then(successBaixarArquivo, errorBaixarArquivo).finally(onComplete);

    }

    function s2ab(s) {
        var buf = new ArrayBuffer(s.length);
        var view = new Uint8Array(buf);
        for (var i = 0; i != s.length; ++i) view[i] = s.charCodeAt(i) & 0xFF;
        return buf;
    }

    var buscar = function () {

        var success = function (response) {
			vm.eventos = response.data;
        };

        var error = function (response) {
            toastr["error"]("Ocorreu um erro ao consultar os registros.");
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
            vm.registroParaExcluir = null;
        };

        spinnerService.show('loader');

        return $http.delete('/eventos/excluir/' + vm.registroParaExcluir).then(success, error).finally(onComplete);
    };

    vm.finalizarEvento = function () {

        if (vm.evento.situacao.toUpperCase() !== 'ENCERRADO') {
            toastr["error"]('O Evento ainda não está Encerrado.', 'Erro');
        }

        if (confirm('Tem certeza que deseja Finalizar o Evento?')) {

            var success = function (response) {
                toastr["success"]("Evento Finalizado.");
                $state.go('eventos-encerramento', { 'id': response.data });
            };

            var error = function (response) {
                ServerErrorsService.handleServerErrors(response, form);
                toastr["error"](response.data.message, "Erro");
            };

            return $http.post('/eventos/finalizarEvento?eventoId=' + vm.evento.id).then(success, error).finally(onComplete);
        }
    };

    //MUNICIPIO / UF
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

    vm.carregarModalidades = function () {
        modalidadeFactory.getModalidade().then(function (response) {
            vm.modalidades = response;
        });
    };

    //SOLUCAO
    $scope.selecionarCurso = function (curso) {
        if (curso) {
            vm.eventoTitulo = curso.titulo;
            vm.query.cursoId = curso.id;
        } else {
            toastr["error"]('Selecionar Solucao.');
        }
    };

    var carregarSolucao = function () {
        return $http.get('/solucoes/RecuperarBasico/' + vm.query.cursoId).then(function (response) {
            vm.eventoTitulo = response.data.titulo;
        });
    };

    //ENTIDADE
    $scope.selecionarEntidade = function (entidade) {
        if (entidade) {
            vm.entidadeNome = entidade.nome;
            vm.query.entidadeDemandanteId = entidade.id;
        } else {
            toastr["error"]('Selecionar Entidade.');
        }
    };



    var carregarEntidade = function () {
        return $http.get('/entidades/RecuperarBasico/' + vm.query.entidadeDemandanteId).then(function (response) {
            vm.entidadeNome = response.data.sigla + ' - ' + response.data.nome;
        });
    };

    vm.carregarEntidades = function () {
        return $http.get('/entidades/buscar').then(function (response) {
            vm.entidades = response.data;
        });
    };

    //DOCENTE
    $scope.selecionarDocente = function (docente) {
        if (docente) {
            vm.docenteNome = docente.nome;
            vm.query.docenteId = docente.id;
        } else {
            toastr["error"]('Selecionar Docente.');
        }
    };

    var carregarDocente = function (id) {
        return $http.get('/docentes/RecuperarBasico/' + vm.query.docenteId).then(function (response) {
            vm.docenteNome = response.data.nome;
        });
    };

    //CURSISTA
    $scope.selecionarCursista = function (cursista) {
        if (cursista) {
            vm.cursistaNome = cursista.nome;
            vm.query.cursistaId = cursista.id;
        } else {
            toastr["error"]('Selecionar Cursista.');
        }
    };

    var carregarCursista = function (id) {
        return $http.get('/pessoas/RecuperarBasico/' + vm.query.cursistaId).then(function (response) {
            vm.cursistaNome = response.data.nome;
        });
    };

    var onComplete = function () {
        spinnerService.close('loader');
    };


    vm.init();
}