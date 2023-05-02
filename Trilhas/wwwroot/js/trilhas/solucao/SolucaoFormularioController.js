angular
    .module('trilhasapp')
    .controller('SolucaoFormularioController', SolucaoFormularioController);

SolucaoFormularioController.$inject = ['$state', '$stateParams', '$http', '$q', 'ServerErrorsService', 'spinnerService', 'ModalidadeFactory'];

function SolucaoFormularioController($state, $stateParams, $http, $q, ServerErrorsService, spinnerService, modalidadeFactory) {
    var vm = this;

    vm.hoje = new Date();
    vm.inseriroNovoAposSalvar;
    vm.unidadeDemandanteSelecionada = {};

    vm.solucao = {};
    vm.modulo = {
        nome: '',
        descricao: '',
        cargaHoraria: null
    };

    vm.tiposSolucao = [
        'Curso',
        'Livro / Artigo',
        'Vídeo'
    ];

    vm.areas = {
        'basico': true,
        'conteudo': true,
        'modulos': true
    };

    vm.modulos = [];
    vm.tiposCurso = [];
    vm.modalidades = [];
    vm.niveisCurso = [];
    vm.tipos = [];

    vm.entidades = [];

    vm.init = function () {

        spinnerService.show('loader');

        vm.solucao.modulos = [];

        var promises = [
            carregarDropDownEstacoes(),
            vm.carregarTiposDeCurso(),
            vm.carregarNiveisDeCurso(),
            vm.carregarModalidades()
        ];

        if ($stateParams.id) {
            promises.push(vm.carregarSolucaoEducacional($stateParams.id));
        }

        $q.all(promises).finally(onComplete);
    };

    var onComplete = function () {
        spinnerService.close('loader');
    };

    vm.carregarSolucaoEducacional = function (id) {
        return $http.get('/solucoes/recuperar/' + id).then(function (response) {
            vm.solucao = response.data;

            switch (vm.solucao.tipoDeSolucao) {
                case 'curso':
                    vm.solucao.tipoSolucao = vm.tiposSolucao[0];
                    break;

                case 'livro':
                    vm.solucao.tipoSolucao = vm.tiposSolucao[1];
                    vm.solucao.dataPublicacao = new Date(Date.parse(vm.solucao.dataPublicacao));
                    vm.solucao.dataPublicacao.setDate(vm.solucao.dataPublicacao.getDate() + 1);
                    break;

                case 'video':
                    vm.solucao.tipoSolucao = vm.tiposSolucao[2];
                    vm.solucao.dataProducao = new Date(Date.parse(vm.solucao.dataProducao));
                    vm.solucao.dataProducao.setDate(vm.solucao.dataProducao.getDate() + 1);
                    break;
            }
        });
    };

    vm.salvarPermanecer = function () {
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
                $state.go('solucoes-cadastro', {}, { reload: true });

            } else {
                history.back();
            }
        };

        var error = function (response) {
            ServerErrorsService.handleServerErrors(response, form);
            toastr["error"](response.data.message, "Erro");
            console.log(response.data.internalMessage);
        };

        if (vm.solucao.tipoSolucao === vm.tiposSolucao[0]) {
            return salvarCurso(form, success, error);

        } else if (vm.solucao.tipoSolucao === vm.tiposSolucao[1]) {
            return salvarLivro(form, success, error);

        } else if (vm.solucao.tipoSolucao === vm.tiposSolucao[2]) {
            return salvarVideo(form, success, error);
        }
    };

    var salvarCurso = function (form, success, error) {

        var valido = validarModulosHabilidades();

        if (form.$invalid || !valido) {
            $('#form-curso .collapse').show();
            toastr["error"]('Preencha corretamente os campos assinalados no formulário.', 'Preenchimento inválido');
            return;
        }

        spinnerService.show('loader');

        return $http.post('/solucoes/salvarCurso', vm.solucao).then(success, error).finally(onComplete);
    };

    var salvarLivro = function (form, success, error) {

        if (form.$invalid) {
            toastr["error"]('Preencha corretamente os campos assinalados no formulário.', 'Preenchimento inválido');
            return;
        }

        spinnerService.show('loader');

        return $http.post('/solucoes/salvarLivro', vm.solucao).then(success, error).finally(onComplete);
    };

    var salvarVideo = function (form, success, error) {

        if (form.$invalid) {
            toastr["error"]('Preencha corretamente os campos assinalados no formulário.', 'Preenchimento inválido');
            return;
        }

        spinnerService.show('loader');

        return $http.post('/solucoes/salvarVideo', vm.solucao).then(success, error).finally(onComplete);
    };

    var validarModulosHabilidades = function () {
        if (!vm.solucao.habilidades || vm.solucao.habilidades.length === 0) {
            return false;
        }

        if (!vm.solucao.modulos || vm.solucao.modulos.length === 0) {
            return false;
        }

        return true;
    };

    vm.excluir = function () {

        var success = function (response) {
            toastr["success"]("Registro excluído com sucesso.");
            $state.go('solucoes');
        };

        var error = function (response) {
            toastr["error"](response.data, "Erro");
            console.log('Erro ao excluir Solução Educacional id = ' + vm.solucao.id);
        };

        spinnerService.show('loader');
        return $http.delete('/solucoes/excluir/' + vm.solucao.id).then(success, error).finally(onComplete);
    };

    var carregarDropDownEstacoes = function () {

        var success = function (response) {
            vm.estacoes = response.data;
        };

        var error = function (response) {
            toastr["error"]("Ocorreu um erro ao consultar os registros.");
            console.log(response.data);
        };

        return $http.get("/estacoes/dropdown").then(success, error);
    };

    vm.habilidade = "";
    vm.adicionarHabilidade = function (event) {
        if (event) {
            event.preventDefault();
        }

        if (!vm.solucao.habilidades) {
            vm.solucao.habilidades = [];
        }

        if (vm.solucao.habilidades.length >= 5) {
            toastr["error"]("Só é possível adicionar 5 habilidades por curso.");
            return;
        }

        //var habilidade = $("#habilidade").val().trim();

        if (vm.habilidade.trim().length > 0) {
            vm.solucao.habilidades.push({ "descricao": vm.habilidade.trim() });
            vm.habilidade = "";
        } else {
            toastr["error"]('Descreva a habilidade.');
        }

        $("#habilidade").focus();
    };

    vm.excluirHabilidade = function (idx) {
        if (idx >= 0 || idx < vm.solucao.habilidades.length) {
            vm.solucao.habilidades.splice(idx, 1);
        }
    };

    vm.adicionarModulo = function (event) {
        if (event) {
            event.preventDefault();
        }

        if (vm.modulo.nome.length > 0 && vm.modulo.cargaHoraria > 0) {
            vm.solucao.modulos.push({
                'nome': vm.modulo.nome,
                'descricao': vm.modulo.descricao,
                'cargaHoraria': vm.modulo.cargaHoraria
            });

            atualizarCargaHorariaTotal();

            vm.modulo.nome = '';
            vm.modulo.descricao = '';
            vm.modulo.cargaHoraria = 0;

        } else {
            toastr["error"]('Adicioneo os módulos do curso.');
        }

        $("#nomeModulo").focus();
    };

    vm.excluirModulo = function (idx) {
        if (idx >= 0 || idx < vm.solucao.modulos.length) {
            vm.solucao.modulos.splice(idx, 1);
            atualizarCargaHorariaTotal();
        }
    };

    var atualizarCargaHorariaTotal = function () {

        if (vm.solucao.modulos) {

            var total = 0;

            for (var x = 0; x < vm.solucao.modulos.length; x++) {
                total += vm.solucao.modulos[x].cargaHoraria;
            }

            vm.solucao.cargaHorariaTotal = total;
        }
    };

    vm.carregarTiposDeCurso = function () {
        var success = function (response) {
            vm.tiposCurso = response.data;
        };

        var error = function (response) {
            toastr["error"]("Ocorreu um erro ao consultar os registros dos tipos de curso.");
            console.log(response.data);
        };

        return $http.get('/solucoes/buscarTiposDeCurso').then(success, error);
    };

    vm.carregarModalidades = function () {
        modalidadeFactory.getModalidade().then(function (response) {
            vm.modalidades = response;
        });
    };

    vm.carregarNiveisDeCurso = function () {
        var success = function (response) {
            vm.niveisCurso = response.data;
        };

        var error = function (response) {
            toastr["error"]("Ocorreu um erro ao consultar os registros dos tipos de curso.");
            console.log(response.data);
        };

        return $http.get('/solucoes/buscarNiveisDeCurso').then(success, error);
    };

    vm.init();
}