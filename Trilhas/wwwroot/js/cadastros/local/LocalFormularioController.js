angular
    .module('trilhasapp')
    .controller('LocalFormularioController', LocalFormularioController);

LocalFormularioController.$inject = ['$state', '$stateParams', '$q', '$http', '$scope', 'ServerErrorsService', 'spinnerService'];

function LocalFormularioController($state, $stateParams, $q, $http, $scope, ServerErrorsService, spinnerService) {
    var vm = this;

    vm.inseriroNovoAposSalvar;

    vm.local = {
        salas: [],
        contatos: [],
        recursos: []
    };

    vm.sala = {
        sigla: '',
        numero: '',
        capacidade: null
    };
    vm.recurso = {
        nome: '',
        quantidade: null
    };
    vm.contato = {
        numero: ''
    };

    vm.ufs = [];
    vm.municipios = [];
    vm.tiposContato = [];

    vm.submitedSala = false;
    vm.submitedRecurso = false;
    vm.submitedContato = false;

    vm.init = function () {

        var promises = [
            vm.carregarUfs(),
            vm.carregarTiposContato()
        ];

        if ($stateParams.id) {
            spinnerService.show('loader');
            promises.push(vm.carregarLocal($stateParams.id));
        }

        $q.all(promises).finally(onComplete);
    };

    var onComplete = function () {
        spinnerService.close('loader');
    };

    vm.carregarLocal = function (id) {
        return $http.get('/locais/recuperar/' + id).then(function (response) {
            vm.local = response.data;
            vm.local.municipioId = parseInt(vm.local.municipioId);
            vm.carregarMunicipios(vm.local.uf);
        });
    };

    vm.salvarVoltar = function () {
        vm.inseriroNovoAposSalvar = false;
    };

    vm.salvarNovo = function () {
        vm.inseriroNovoAposSalvar = true;
    };

    vm.salvar = function (form) {

        if (form.$invalid || vm.local.salas.length === 0 || vm.local.contatos.length === 0) {

            toastr["error"]('Preencha corretamente os campos assinalados no formulário.', 'Preenchimento inválido');

            if (form.nome.$invalid || vm.local.salas.length === 0 || vm.local.contatos.length === 0) {
                vm.tab = 1;
            } else {
                vm.tab = 2;
            }

            return;
        }

        var success = function (response) {
            toastr["success"]("Registro salvo com sucesso.", "Sucesso");

            form.$setPristine();

            if (vm.inseriroNovoAposSalvar) {
                $state.go('local-cadastro', {}, { reload: true });

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

        return $http.post('/locais/salvar', vm.local).then(success, error).finally(onComplete);
    };

    vm.excluir = function () {

        var success = function (response) {
            toastr["success"]("Registro excluído com sucesso.");
            $state.go('locais');
        };

        var error = function (response) {
            toastr["error"](response.data, "Erro");
            console.log('Erro ao excluir local id = ' + vm.local.id);
        };

        spinnerService.show('loader');

        return $http.delete('/locais/excluir/' + vm.local.id).then(success, error).finally(onComplete);
    };

    vm.carregarTiposContato = function () {
        return $http.get('/locais/recuperarTiposDeContato').then(function (response) {
            vm.tiposContato = response.data;
        });
    };

    vm.carregarUfs = function () {
        return $http.get('/municipios/recuperarUfs').then(function (response) {
            vm.ufs = response.data;
        });
    };

    vm.carregarMunicipios = function (uf) {
        return $http.get('/municipios/recuperarMunicipios?uf=' + uf).then(function (response) {
            vm.municipios = response.data;
        });
    };

    vm.filtrarRecurso = function () {

        var success = function (response) {
            vm.recursos = response.data;
        };

        var error = function (response) {
            toastr["error"]("Ocorreu um erro ao consultar os registros.");
            console.log(response.data);
        };

        return $http.get("/recursos/buscar", { params: vm.queryRecurso })
            .then(success, error);
    };


    vm.adicionarSala = function (event) {

        vm.submitedSala = true;

        if (event) {
            event.preventDefault();
        }

        if (vm.sala.numero.length > 0
            && vm.sala.capacidade > 0
            && vm.sala.sigla.length > 0) {

            vm.local.salas.push(vm.sala);

            vm.sala = {};

            vm.submitedSala = false;

        } else {
            toastr["error"]('Preencha corretamente os campos assinalados para adicionar uma Sala.');
        }

        $("#sigla").focus();
    };

    vm.excluirSala = function (idx) {
        if (idx >= 0 || idx < vm.local.salas.length) {
            if (vm.local.salas[idx].alocada) {
                toastr["warning"]('Esta sala está alocada em algum Evento ativo e não pode ser excluída no momento.', 'Sala alocada');
            } else {
                vm.local.salas.splice(idx, 1);
            }
        }
    };

    //RECURSO
    vm.adicionarRecurso = function (event) {

        vm.submitedRecurso = true;

        if (event) {
            event.preventDefault();
        }

        if (!vm.local.recursos) {
            vm.local.recursos = [];
        }

        if ((vm.recurso.recursoId > 0) && (vm.recurso.quantidade > 0)) {

            vm.local.recursos.push(vm.recurso);
            vm.recurso = {};
            vm.submitedRecurso = false;

        } else {
            toastr["error"]('Preencha corretamente os campos assinalados para adicionar um Recurso.');
        }

        $("#recurso").focus();
    };

    $scope.selecionarRecurso = function (recurso) {
        if (recurso) {
            vm.recurso = {
                recursoId: recurso.id,
                nome: recurso.nome,
                descricao: recurso.descricao
            };
        } else {
            toastr["error"]('Erro inesperado.');
        }
    };

    vm.excluirRecurso = function (idx) {
        if (idx >= 0 || idx < vm.local.recursos.length) {
            vm.local.recursos.splice(idx, 1);
        }
    };

    vm.adicionarContato = function (event) {

        vm.submitedContato = true;

        if (event) {
            event.preventDefault();
        }

        if (!vm.local.contatos) {
            vm.local.contatos = [];
        }

        if ((vm.contato.tipo && vm.contato.tipo.id > 0) && (vm.contato.numero.length > 0)) {

            vm.contato.tipoContatoId = vm.contato.tipo.id;
            vm.local.contatos.push(vm.contato);

            vm.contato = {};

            vm.submitedContato = false;

        } else {
            toastr["error"]('Preencha corretamente os campos assinalados para adicionar um Contato.');
        }

        $("#tipoContato").focus();
    };

    vm.excluirContato = function (idx) {
        if (idx >= 0 || idx < vm.local.contatos.length) {
            vm.local.contatos.splice(idx, 1);
        }
    };

    vm.queryRecurso = {};

    vm.init();
}