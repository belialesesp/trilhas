angular
    .module('trilhasapp')
    .controller('EventoFormularioController', EventoFormularioController);

EventoFormularioController.$inject = ['$state', '$stateParams', '$http', '$q', '$scope', 'ServerErrorsService', 'spinnerService'];

function EventoFormularioController($state, $stateParams, $http, $q, $scope, ServerErrorsService, spinnerService) {
    var vm = this;

    vm.inseriroNovoAposSalvar;
    vm.horarioEdicao = null;

    vm.flagEad = false;
    vm.isVagas = true;

    vm.totalDeHoras = 0;

    vm.evento = {
        agenda: {},
        entidade: {},
        agendas: [],
        recursos: [],
        cotas: [],
        horarios: [],
        limitarVagas: true,
        vagasPorEntidade: 3
    };

    vm.eventoCota = {};

    vm.agenda = {};

    vm.horario = {
        modulo: {},
        docente: {},
        sala: {}
    };

    vm.recursos = [];
    vm.docentes = [];
    vm.locais = [];
    vm.salas = [];
    vm.coordenadores = [];

    vm.query = {
        'cursoId': $stateParams.cursoId,
        'tipoSolucao': $stateParams.tipoSolucao,
        'entidadeId': $stateParams.entidadeId,
        'uf': $stateParams.uf,
        'municipio': $stateParams.municipio,
        'docenteId': $stateParams.docenteId,
        'dataInicio': $stateParams.dataInicio,
        'dataFim': $stateParams.dataFim,
        'excluidos': $stateParams.excluidos,
        'page': $stateParams.page
    };

    vm.init = function () {

        var promises = [
            vm.carregarFuncoesDocente(),
            vm.buscarCertificados(),
            vm.buscarDeclaracaoCursista(),
            vm.buscarDeclaracaoDocente()
        ];

        spinnerService.show('loader');
        $q.all(promises).then(function () {
            if ($stateParams.id) {
                vm.carregarEvento($stateParams.id);
            }
        }).finally(onComplete);
    };

    var onComplete = function () {
        spinnerService.close('loader');
    };

    vm.carregarEvento = function (id) {

        return $http.get('/eventos/recuperar/' + id).then(function (response) {
            vm.evento = response.data;
            vm.evento.agenda.dataInscricaoInicio = new Date(vm.evento.agenda.dataInscricaoInicio);
            vm.evento.agenda.dataInscricaoFim = new Date(vm.evento.agenda.dataInscricaoFim);
            vm.evento.agenda.horaInscricaoInicio = new Date(vm.evento.agenda.dataInscricaoInicio);
            vm.evento.agenda.horaInscricaoFim = new Date(vm.evento.agenda.dataInscricaoFim);

            vm.totalDeHoras = vm.evento.curso.cargaHorariaTotal;
            vm.flagEad = vm.evento.flagEad;

            if (vm.evento.curso.permiteCertificado && !vm.evento.certificadoId) {
                vm.evento.certificadoId = vm.certificados.find(x => x.padrao).id;
            }
            if (!vm.evento.declaracaoCursistaId) {
                vm.evento.declaracaoCursistaId = vm.declaracaoCursista.find(x => x.padrao).id;
            }
            if (!vm.evento.declaracaoDocenteId) {
                vm.evento.declaracaoDocenteId = vm.declaracaoDocente.find(x => x.padrao).id;
            }

            validaVagas();

            $http.get('/locais/recuperarSalas/' + vm.evento.local.id).then(function (responseSalas) {
                vm.salas = responseSalas.data;
            });
        });
    };

    vm.salvarVoltar = function () {
        vm.inseriroNovoAposSalvar = false;
    };

    vm.salvarNovo = function () {
        vm.inseriroNovoAposSalvar = true;
    };

    vm.salvar = function (form) {

        var success = function (response) {
            toastr["success"]("Registro salvo com sucesso.", "Sucesso");
            form.$setPristine();

            if (vm.inseriroNovoAposSalvar) {
                $state.go('eventos-cadastro', {}, { reload: true });

            } else {
                history.back();
            }
        };

        var error = function (response) {
            ServerErrorsService.handleServerErrors(response, form);
            toastr["error"](response.data.message, "Erro");
            console.log(response.data.internalMessage);
        };

        spinnerService.show('loader');

        return salvarEvento(form, success, error);
    };

    var salvarEvento = function (form, success, error) {

        form['dataInscricaoInicio'].$setValidity('retroativa', true);
        form['horaInscricaoInicio'].$setValidity('retroativa', true);

        if (form.$invalid || vm.evento.horarios.length === 0) {

            if (form.$invalid) {
                if (vm.evento.limitarVagas && !vm.evento.vagasPorEntidade) {
                    vm.tab = 4;

                } else {
                    vm.tab = 1;
                }
            }
            else if (vm.evento.horarios.length === 0) {
                vm.tab = 2;
            }
            else {
                vm.tab = 1;
            }

            spinnerService.close('loader');
            toastr["error"]('Preencha corretamente os campos assinalados no formulário.', 'Preenchimento inválido');

            return;
        }

        return $http.post('/eventos/salvar', vm.evento).then(success, error).finally(onComplete);
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
                console.log(response.data.internalMessage);
            };

            return $http.post('/eventos/finalizarEvento?eventoId=' + vm.evento.id).then(success, error).finally(onComplete);
        }
    };

    vm.desomologarEvento = function () {

        if (confirm('Tem certeza que deseja Desomologar o Evento?')) {

            var success = function (response) {
                toastr["success"]("Evento Desomologado.");
                $state.go('eventos-encerramento', { 'id': response.data });
            };

            var error = function (response) {
                ServerErrorsService.handleServerErrors(response, form);
                toastr["error"](response.data.message, "Erro");
                console.log(response.data.internalMessage);
            };

            return $http.post('/eventos/desomologarEvento?eventoId=' + vm.evento.id).then(success, error).finally(onComplete);
        }
    };

    vm.resultados = function () {
        $state.go('eventos-encerramento', { 'id': vm.evento.id });
    };

    vm.excluir = function () {

        var success = function (response) {
            toastr["success"]("Registro excluído com sucesso.");
            $state.go('pessoas');
        };

        var error = function (response) {
            toastr["error"](response.data, "Erro");
            console.log('Erro ao excluir pessoa id = ' + vm.pessoa.id);
        };

        spinnerService.show('loader');
        return $http.delete('/pessoas/excluir/' + vm.pessoa.id).then(success, error).finally(onComplete);
    };

    vm.carregarTipoContato = function () {
        return $http.get('/locais/recuperarTipoContatoAll').then(function (response) {
            vm.tiposContato = response.data;
        });
    };

    vm.carregarFuncoesDocente = function () {
        return $http.get('/eventos/recuperarFuncoesDocente').then(function (response) {
            vm.funcoesDocente = response.data;
        });
    };

    $scope.selecionarCoordenador = function (coordenador) {
        if (coordenador) {
            vm.evento.coordenador = {
                id: coordenador.id,
                nome: coordenador.nome
            };
        } else {
            toastr["error"]('Erro inesperado.');
        }
    };

    //DOCENTE
    $scope.selecionarDocente = function (docente) {
        if (docente) {
            vm.horario.docente = {
                'id': docente.id,
                'nome': docente.nome
            };
        } else {
            toastr["error"]('Erro inesperado.');
        }
    };

    //ENTIDADE
    $scope.selecionarEntidade = function (entidade) {
        if (entidade) {
            vm.evento.entidade = {
                id: entidade.id,
                nome: entidade.nome
            };
        } else {
            toastr["error"]('Erro inesperado.');
        }
    };

    //LOCAL
    $scope.selecionarLocal = function (local) {
        if (local) {
            vm.evento.local = {
                id: local.id,
                nome: local.nome,
                capacidadeTotal: local.capacidadeTotal
            };
            vm.salas = local.salas;
        } else {
            toastr["error"]('Erro inesperado.');
        }
    };

    //RECURSO
    $scope.selecionarRecurso = function (recurso) {
        if (recurso) {
            vm.eventoRecurso = {
                recursoId: recurso.id,
                nome: recurso.nome,
                custo: recurso.custo
            };
        } else {
            toastr["error"]('Erro inesperado.');
        }
    };

    vm.adicionarRecurso = function (event, form) {
        vm.submitedRecurso = true;

        if (event) {
            event.preventDefault();
        }

        if (vm.eventoRecurso && vm.eventoRecurso.recursoId > 0 && vm.eventoRecurso.quantidade > 0) {
            vm.evento.recursos.push(vm.eventoRecurso);
            vm.eventoRecurso = {};

            vm.submitedRecurso = false;
            form.$setDirty();

        } else {
            toastr["error"]('Ocorreu um erro ao adicionar Recurso ao Evento.');
        }

        $("#recurso").focus();
    };

    vm.excluirRecurso = function (form, idx) {
        if (idx >= 0 || idx < vm.evento.recursos.length) {
            vm.evento.recursos.splice(idx, 1);
            form.$setDirty();
        }
    };

    //SOLUCAO
    $scope.selecionarCurso = function (curso) {
        if (curso) {
            vm.flagEad = curso.modalidadeDeCurso.descricao.toUpperCase() === 'EAD' ? true : false;
            vm.evento.curso = curso;
            vm.evento.curso.modalidadeDeCurso = curso.modalidadeDeCurso.id;
            vm.evento.certificadoId = (vm.evento.curso.permiteCertificado && vm.certificados) ? vm.certificados.find(x => x.padrao).id : null;
            vm.evento.declaracaoCursistaId = vm.declaracaoCursista ? vm.declaracaoCursista.find(x => x.padrao).id : null;
            vm.evento.declaracaoDocenteId = vm.declaracaoDocente ? vm.declaracaoDocente.find(x => x.padrao).id : null;
        } else {
            toastr["error"]('Erro inesperado.');
        }
    };

    //COTA
    $scope.selecionarEntidadeCota = function (entidade) {
        if (entidade) {
            vm.eventoCota.entidadeId = entidade.id;
            vm.eventoCota.entidadeNome = entidade.nome;
        } else {
            toastr["error"]('Erro inesperado.');
        }
    };

    //Excluir evento
    $scope.excluirEvento = function (motivoExclusao) {
        var success = function (response) {
            toastr["success"]("Registro excluído com sucesso.");
            history.back();
            spinnerService.show('loader');
        };

        var error = function (response) {
            toastr["error"](response.data, "Erro");
            console.log('Erro ao excluir evento id = ' + vm.evento.id);
        };

        spinnerService.show('loader');

        return $http.delete('/eventos/excluir?id=' + vm.evento.id + '&motivoExclusao=' + motivoExclusao).then(success, error).finally(onComplete);
    };

    vm.adicionarCota = function (event, form) {
        vm.submitedCota = true;

        if (event) {
            event.preventDefault();
        }

        if (vm.eventoCota && vm.eventoCota.entidadeId > 0 && vm.eventoCota.quantidade > 0) {
            if (validaVagas(vm.eventoCota.quantidade)) {
                vm.evento.cotas.push(vm.eventoCota);
                vm.eventoCota = {};
                vm.submitedCota = false;
                form.$setDirty();
            }
        } else {
            toastr["error"]('Ocorreu um erro ao adicionar Cota ao Evento.');
        }

        $("#entidade").focus();
    };

    vm.excluirCota = function (form, idx) {
        if (idx >= 0 || idx < vm.evento.cotas.length) {
            vm.evento.cotas.splice(idx, 1);
            validaVagas();

            form.$setDirty();
        }
    };

    //HORARIO
    vm.adicionarHorario = function (event, form) {
        if (event) {
            event.preventDefault();
        }

        vm.submitedHorario = true;

        if (vm.horario.moduloId > 0
            && vm.horario.docente.id > 0
            && vm.horario.funcaoDocenteId > 0
            && (!(vm.horario.salaId <= 0 && vm.horario.dataInicio >= new Date()) || vm.flagEad)
            && vm.horario.horaInicio > 0
            && vm.horario.horaFim > 0
            && vm.horario.dataInicio > 0
            && vm.horario.dataFim > 0
            && vm.validarDataModulo(form)
        ) {
            inserirHorarios(vm.horario);

            form.$setDirty();

            vm.cancelarEdicaoHorario();
            vm.docente = {};

            vm.submitedHorario = false;

        } else {
            toastr["error"]('Ocorreu um erro ao adicionar Horário ao Evento.');
        }

        $("#modulo").focus();
    };

    var inserirHorarios = function (horario) {

        if (horario.dataInicio <= horario.dataFim) {

            var dataAux = horario.dataInicio;

            while (dataAux <= horario.dataFim) {

                if (horario.dataInicio < horario.dataFim && (dataAux.getDay() === 0 || dataAux.getDay() === 6)) {
                    dataAux.setDate(dataAux.getDate() + 1);
                    continue;
                }

                var novoHorario = {
                    'id': 0,
                    'dataInicio': new Date(dataAux),
                    'dataFim': new Date(dataAux),
                    'horaInicio': new Date(horario.horaInicio),
                    'horaFim': new Date(horario.horaFim),
                    'docenteId': horario.docente.id,
                    'funcaoDocenteId': horario.funcaoDocenteId,
                    'funcaoDocenteNome': vm.funcoesDocente.find(function (funcao) { return funcao.id === horario.funcaoDocenteId; }).nome,
                    'docenteNome': horario.docente.nome,
                    'moduloId': horario.moduloId,
                    'moduloNome': vm.evento.curso.modulos.find(function (modulo) { return modulo.id === horario.moduloId; }).nome,
                    'salaId': vm.horario.salaId,
                    'sala': vm.salas.find(function (sala) { return sala.id === horario.salaId; })
                };

                if (vm.horarioEdicao !== null) {
                    vm.totalDeHoras -= timeDiff(vm.evento.horarios[vm.horarioEdicao].horaFim, vm.evento.horarios[vm.horarioEdicao].horaInicio);
                    novoHorario.id = vm.evento.horarios[vm.horarioEdicao].id;
                    vm.evento.horarios[vm.horarioEdicao] = novoHorario;
                    vm.horarioEdicao = null;
                }
                else {
                    vm.evento.horarios.push(novoHorario);
                }

                vm.totalDeHoras += timeDiff(novoHorario.horaFim, novoHorario.horaInicio);

                dataAux.setDate(dataAux.getDate() + 1);
            }
        }
    };

    vm.excluirHorario = function (event, form, idx) {
        if (event) {
            event.preventDefault();
        }

        if (idx >= 0 || idx < vm.evento.horarios.length) {
            var horario = vm.evento.horarios.splice(idx, 1);

            vm.totalDeHoras -= timeDiff(horario[0].horaFim, horario[0].horaInicio);

            vm.calcularHorasRestantes();

            form.$setDirty();

            if (vm.horarioEdicao !== null) {
                vm.horarioEdicao = null;
                vm.horario = {};
            }
        }
    };

    vm.calcularHorasRestantes = function () {
        vm.horasRestantes = 0;

        if (!vm.evento.curso.modulos || !vm.horario.moduloId) {
            return;
        }

        var horarios = vm.evento.horarios.filter(function (horario) {
            return horario.moduloId === vm.horario.moduloId;
        });

        var somatorio = horarios.reduce(function (acc, horario) {
            return acc + timeDiff(horario.horaFim, horario.horaInicio);
        }, 0);

        var modulo = vm.evento.curso.modulos.find(function (modulo) {
            return modulo.id === vm.horario.moduloId;
        });

        vm.horasRestantes = modulo.cargaHoraria - somatorio;
    };

    vm.cancelarEdicaoHorario = function () {
        vm.horarioEdicao = null;
        vm.horario = {};
    };

    vm.selecionarHorario = function (idx) {
        if (vm.horarioEdicao !== null) {
            if (vm.horarioEdicao === idx) {
                vm.cancelarEdicaoHorario();
                return;
            }

            vm.cancelarEdicaoHorario();
        }

        vm.horarioEdicao = idx;

        angular.extend(vm.horario, vm.evento.horarios[idx]);

        vm.horario.dataInicio = new Date(vm.horario.dataInicio);
        vm.horario.dataFim = new Date(vm.horario.dataFim);
        vm.horario.horaInicio = new Date(vm.horario.horaInicio);
        vm.horario.horaFim = new Date(vm.horario.horaFim);
        vm.horario.docente = {
            'id': vm.horario.docenteId,
            'nome': vm.horario.docenteNome
        };

        vm.calcularHorasRestantes();
    };

    vm.validarDataInicioInscricao = function (form) {
        vm.validarDataInscricao(form);

        var hoje = new Date();
        hoje = new Date(hoje.getFullYear(), hoje.getMonth(), hoje.getDate());

        if (vm.evento.agenda.dataInscricaoInicio < hoje) {
            form['dataInscricaoInicio'].$setValidity('retroativa', false);
            form['horaInscricaoInicio'].$setValidity('retroativa', false);
        } else {
            form['dataInscricaoInicio'].$setValidity('retroativa', true);
            form['horaInscricaoInicio'].$setValidity('retroativa', true);
        }
    };

    vm.validarDataInscricao = function (form) {
        if (vm.evento.agenda.dataInscricaoInicio > vm.evento.agenda.dataInscricaoFim) {
            form['dataInscricaoInicio'].$setValidity('invalid', false);
            form['horaInscricaoInicio'].$setValidity('invalid', false);
            form['dataInscricaoFim'].$setValidity('invalid', false);
            form['horaInscricaoFim'].$setValidity('invalid', false);
        } else {
            form['dataInscricaoInicio'].$setValidity('invalid', true);
            form['horaInscricaoInicio'].$setValidity('invalid', true);
            form['dataInscricaoFim'].$setValidity('invalid', true);
            form['horaInscricaoFim'].$setValidity('invalid', true);
        }
    };

    vm.validarDataModulo = function (form) {
        var ret;

        var dataInscricaoFim = new Date(vm.evento.agenda.dataInscricaoFim.getFullYear(),
            vm.evento.agenda.dataInscricaoFim.getMonth(),
            vm.evento.agenda.dataInscricaoFim.getDate(),
            vm.evento.agenda.horaInscricaoFim.getHours(),
            vm.evento.agenda.horaInscricaoFim.getMinutes(), 0);

        if (dataInscricaoFim >= vm.horario.dataInicio) {
            form['dataInicioModulo'].$setValidity('invalid-inscricao', false);
            form['horarioInicioModulo'].$setValidity('invalid-inscricao', false);
            ret = false;
        }
        else {
            form['dataInicioModulo'].$setValidity('invalid-inscricao', true);
            form['horarioInicioModulo'].$setValidity('invalid-inscricao', true);
            ret = true;
        }

        if (vm.horario.dataInicio > vm.horario.dataFim) {
            form['dataInicioModulo'].$setValidity('invalid', false);
            form['dataFimModulo'].$setValidity('invalid', false);
            form['horarioInicioModulo'].$setValidity('invalid', false);
            form['horarioFimModulo'].$setValidity('invalid', false);
            ret = ret && false;
        } else {
            form['dataInicioModulo'].$setValidity('invalid', true);
            form['dataFimModulo'].$setValidity('invalid', true);
            form['horarioInicioModulo'].$setValidity('invalid', true);
            form['horarioFimModulo'].$setValidity('invalid', true);
            //ret = ret && true;
        }

        return ret;
    };

    vm.podeCancelar = function () {
        return vm.evento.id && vm.evento.situacao !== 'Encerrado' && vm.evento.situacao !== 'Finalizado';
    };

    vm.validaVagas = function (input) {
        //var v = input ? parseInt(input.$$element.val()) : (form.quantidadeCota.value || 0);
        validaVagas(0);
    };

    var validaVagas = function (vagas = 0) {
        vagas = parseInt(vagas);
        vm.quantidadeVaga = 0;

        vm.evento.cotas.forEach(function (item) {
            vm.quantidadeVaga += item.quantidade;
        });

        vm.quantidadeVaga += vagas;

        vm.isVagas = !(vm.quantidadeVaga > (vm.evento.agenda.numeroVagas || 0));

        return vm.isVagas;
    };

    vm.alternaAba = function (aba) {
        if (!(vm.flagEad && aba === 3)) {
            vm.tab = aba;
        }
    };

    var timeDiff = function (d1, d2) {
        var date1, date2;

        date1 = new Date(d1);
        date2 = new Date(d2);

        date1.setMinutes(0);
        date2.setMinutes(0);

        return parseInt(Math.abs(date1 - date2) / (1000 * 60 * 60));
    };

    vm.buscarCertificados = function () {
        return $http.get('/certificados/buscar?tipoCertificado=0').then(function (response) {
            vm.certificados = response.data;
        });
    };

    vm.buscarDeclaracaoCursista = function () {
        return $http.get('/certificados/buscar?tipoCertificado=1').then(function (response) {
            vm.declaracaoCursista = response.data;
        });
    };

    vm.buscarDeclaracaoDocente = function () {
        return $http.get('/certificados/buscar?tipoCertificado=2').then(function (response) {
            vm.declaracaoDocente = response.data;
        });
    };

    vm.init();
}
