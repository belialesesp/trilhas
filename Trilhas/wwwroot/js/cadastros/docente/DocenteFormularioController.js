angular
    .module('trilhasapp')
    .controller('DocenteFormularioController', DocenteFormularioController);

DocenteFormularioController.$inject = ['$state', '$stateParams', '$q', '$http', '$scope', 'ServerErrorsService', 'spinnerService'];

function DocenteFormularioController($state, $stateParams, $q, $http, $scope, ServerErrorsService, spinnerService) {
    var vm = this;

    vm.inseriroNovoAposSalvar;

    vm.pessoas = [];
    vm.docente = {
        dadosBancarios: [],
        habilitacao: [],
        formacao: []
    };

    vm.solucoes = [];
    vm.solucao = {};

    vm.habilitacao = {};

    vm.habilitacoes = [];

    vm.formacoes = [];
    vm.formacao = {
        dataInicio: null,
        dataFim: null
    };

    vm.dadosBancarios = [];
    vm.dadosBancario = {};

    vm.init = function () {
        var promises = [];

        if ($stateParams.id) {
            spinnerService.show('loader');
            promises.push(vm.carregarDocente($stateParams.id));
        }

        vm.isDataFormacao = true;
        $q.all(promises).finally(onComplete);

    };

    var onComplete = function () {
        spinnerService.close('loader');
    };

    vm.carregarDocente = function (id) {
        return $http.get('/docentes/recuperar/' + id).then(function (response) {
            vm.docente = response.data;
        });
    };

    vm.salvarVoltar = function () {
        vm.inseriroNovoAposSalvar = false;
    };

    vm.salvarNovo = function () {
        vm.inseriroNovoAposSalvar = true;
    };

    vm.salvar = function (form) {

        if (form.$invalid || vm.docente.dadosBancarios.length === 0 || vm.docente.formacao.length === 0) {

            if (form.$invalid || vm.docente.dadosBancarios.length === 0) {
                vm.tab = 1;
            }
            else if (vm.docente.formacao.length === 0) {
                vm.tab = 3;
            }

            toastr["error"]('Preencha corretamente os campos assinalados no formulário.', 'Preenchimento inválido');
            return;
        }

        var success = function (response) {
            toastr["success"]("Registro salvo com sucesso.", "Sucesso");
            form.$setPristine();

            if (vm.inseriroNovoAposSalvar) {
                $state.go('docentes-cadastro', {}, { reload: true });

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

        return $http.post('/docentes/salvar', vm.docente).then(success, error).finally(onComplete);
    };

    vm.excluir = function () {
        var success = function (response) {
            toastr["success"]("Registro excluído com sucesso.");
            $state.go('docentes');
        };

        var error = function (response) {
            toastr["error"](response.data, "Erro");
            console.log('Erro ao excluir docente id = ' + vm.docente.id);
        };

        spinnerService.show('loader');

        return $http.delete('/docentes/excluir/' + vm.docente.id).then(success, error).finally(onComplete);
    };

    vm.carregarPessoa = function (event) {

        if (!vm.docente.cpf) {
            vm.docente = {};
            return;
        }

        var success = function (response) {
            vm.docente.nome = response.data.nome;
            vm.docente.pis = response.data.pis;
            vm.docente.titulo = response.data.numeroTitulo;
            vm.docente.pessoaId = response.data.id;
        };

        var error = function (response) {
            toastr["error"](response.data, "Erro");
            console.log('Erro ao consultar pessoa CPF = ' + cpf);
        };

        return $http.get('/pessoas/recuperarPessoaPorCpf?cpf=' + vm.docente.cpf).then(success, error);
    };

    $scope.selecionarPessoa = function (pessoa) {
        if (pessoa) {
            vm.docente.nome = pessoa.nome;
            vm.docente.pis = pessoa.pis;
            vm.docente.titulo = pessoa.numeroTitulo;
            vm.docente.pessoaId = pessoa.id;
            vm.docente.cpf = pessoa.cpf;
        } else {
            toastr["error"]('Erro ao selecionar Pessoa.');
        }
    };

    $scope.selecionarCurso = function (curso) {
        if (curso) {
            vm.habilitacao.curso = {
                'id': curso.id,
                'titulo': curso.titulo,
                'modalidade': curso.modalidadeDeCurso,
                'cargaHoraria': curso.cargaHorariaTotal
            };
        } else {
            toastr["error"]('Erro ao selecionar Curso.');
        }
    };

    //CONTA
    vm.adicionarDadosBancarios= function (event) {
        vm.bancoRequired = false;
        vm.numeroContaRequired = false;
        vm.agenciaRequired = false;
        vm.submitedDadosBancario = true;

        if (event) {
            event.preventDefault();
        }

        if (vm.dadosBancario.banco && vm.dadosBancario.contaCorrente && vm.dadosBancario.agencia) {
            vm.docente.dadosBancarios.push(vm.dadosBancario);
            vm.dadosBancario = {};
            vm.submitedDadosBancario = false;

        } else {
            if (!vm.dadosBancario.banco) vm.bancoRequired = true;
            if (!vm.dadosBancario.contaCorrente) vm.numeroContaRequired = true;
            if (!vm.dadosBancario.agencia) vm.agenciaRequired = true;
        }

        $("#banco").focus();
    };

    vm.excluirDadosBancario = function (idx) {
        if (idx >= 0 || idx < vm.docente.dadosBancarios.length) {
            vm.docente.dadosBancarios.splice(idx, 1);
        }
    };


    //HABILITACAO
    vm.adicionarHabilitacao = function (event) {
        vm.cursoHabilitacaoRequired = false;
        vm.submitedHabilitacao = true;

        if (event) {
            event.preventDefault();
        }

        if (vm.habilitacao.curso) {

            var found = vm.docente.habilitacao.find(function (habilitacao) { return habilitacao.curso.id == vm.habilitacao.curso.id });

            if (found) {
                toastr["warning"]('O Curso selecionado já está na lista.');
                return;
            }

            vm.docente.habilitacao.push(vm.habilitacao);
            vm.habilitacao = {};
            vm.submitedHabilitacao = false;

        } else {
            if (!vm.habilitacao.curso) {
                vm.cursoHabilitacaoRequired = true;
            }
        }

        $("#solucao").focus();
    };

    vm.excluirHabilitacao = function (idx) {
        if (idx >= 0 || idx < vm.docente.habilitacao.length) {
            vm.docente.habilitacao.splice(idx, 1);
        }
    };

    //FORMACAO
    vm.adicionarFormacao = function (event) {
        vm.cursoFormacaoRequired = false;
        vm.titulacaoRequired = false;
        vm.instituicaoRequired = false;
        vm.cargaHorariaRequired = false;
        vm.dataInicioRequired = false;
        vm.dataFimRequired = false;

        vm.validaDataFormacao();

        vm.submitedFormacao = true;

        if (event) {
            event.preventDefault();
        }

        if (vm.formacao.curso !== undefined
            && vm.formacao.titulacao !== undefined
            && vm.formacao.instituicao !== undefined
            && vm.formacao.cargaHoraria !== undefined
            && vm.formacao.dataInicio !== undefined
            && vm.formacao.dataFim !== undefined
            && vm.isDataFormacao
        ) {
            vm.formacao.dataInicio = new Date(vm.formacao.dataInicio);
            vm.formacao.dataFim = new Date(vm.formacao.dataFim);
            vm.docente.formacao.push(vm.formacao);

            vm.formacao = {};
            vm.isDataFormacao = true;
            vm.submitedFormacao = false;

        } else {
            if (!vm.formacao.curso) vm.cursoFormacaoRequired = true;
            if (!vm.formacao.titulacao) vm.titulacaoRequired = true;
            if (!vm.formacao.instituicao) vm.instituicaoRequired = true;
            if (!vm.formacao.cargaHoraria) vm.cargaHorariaRequired = true;
            if (!vm.formacao.dataInicio) vm.dataInicioRequired = true;
            if (!vm.formacao.dataFim || vm.isDataFormacao) vm.dataFimRequired = true;
            toastr["error"]('Erro ao adicionar a Formação do Docente.');
        }

        $("#cursoFormacao").focus();
    };

    vm.excluirFormacao = function (idx) {
        if (idx >= 0 || idx < vm.docente.formacao.length) {
            vm.docente.formacao.splice(idx, 1);
        }
    };

    vm.validaDataFormacao = function () {
        if (vm.formacao.dataInicio > vm.formacao.dataFim) {
            vm.isDataFormacao = false;
        } else {
            vm.isDataFormacao = true;
        }
    };

    vm.init();
}
