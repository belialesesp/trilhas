angular
    .module('trilhasapp')
    .controller('PessoaFormularioController', PessoaFormularioController);

PessoaFormularioController.$inject = ['$state', '$stateParams', '$q', '$http', '$scope', 'ServerErrorsService', 'spinnerService'];

function PessoaFormularioController($state, $stateParams, $q, $http, $scope, ServerErrorsService, spinnerService) {
    var vm = this;
    vm.imageToCrop = '';
    vm.croppedImage = '';

    vm.inseriroNovoAposSalvar;

    vm.pessoa = {};
    vm.contato = {};
    vm.entidade = {};
    vm.ufs = [];
    vm.municipios = [];
    vm.tiposEntidade = [];
    vm.sexos = [];
    vm.orgaoExpedidores = [];
    vm.tiposEscolaridade = [];
    vm.tiposContato = [];

    vm.query = {
        'nome': $stateParams.nome,
        'cpf': $stateParams.cpf
    };

    vm.init = function () {

        var promises = [
            vm.carregarUfs(),
            vm.carregarTipoEntidade(),
            vm.carregarTipoContato(),
            vm.carregarTipoEscolaridade(),
            vm.carregarTipoDeficiencia(),
            vm.carregarSexo(),
            vm.carregarOrgaosExpedidor()
        ];

        if ($stateParams.id) {
            spinnerService.show('loader');
            promises.push(vm.carregarPessoa($stateParams.id));
        }

        $q.all(promises).finally(onComplete);
    };

    vm.initExtra = function () {
        angular.element(document.querySelector('#icone')).on('change', onFileSelect);
        angular.element(document.querySelector('#icone')).on('click', onFileSelect);
        angular.element(document.querySelector('#icone')).on('click', function (evt) {
            $scope.$apply(function ($scope) {
                vm.imageToCrop = '';
                vm.croppedImage = '';
            });
        });
    };

    var onComplete = function () {
        spinnerService.close('loader');
    };

    vm.carregarPessoa = function (id) {
        return $http.get('/pessoas/recuperar/' + id).then(function (response) {
            vm.pessoa = response.data;
            vm.carregarMunicipios(vm.pessoa.uf);
            vm.pessoa.cidade = parseInt(vm.pessoa.cidade);
            vm.pessoa.dataNascimento = new Date(vm.pessoa.dataNascimento);
            //vm.carregarEntidade(vm.pessoa.entidade);

            if (vm.pessoa.storageError) {
                toastr["warning"](vm.pessoa.storageError + '<br/><a href="/admin/storagestatus" target="_blank">Mais detalhes...</a>');
            }
        });
    };

    vm.atualizarPessoa = async function (cpf) {
        try
        {
            return $http.get(`/pessoas/atualizar/${cpf}`).then(function (response) {
                debugger;

                var _dadosPessoais = response.data;

                vm.pessoa.nome = _dadosPessoais.nome;
                vm.pessoa.logradouro = _dadosPessoais.logradouro;
                vm.pessoa.bairro  = _dadosPessoais.bairro;
                vm.pessoa.cep = _dadosPessoais.cep;
                vm.pessoa.complemento = _dadosPessoais.complemento;
                vm.pessoa.numero = _dadosPessoais.numero;
                vm.pessoa.uf = _dadosPessoais.uf; 
                vm.carregarMunicipios(vm.pessoa.uf);

                vm.pessoa.email = _dadosPessoais.email;
                
                vm.pessoa.numeroFuncional = _dadosPessoais.numeroFuncional;

                
                vm.pessoa.dataNascimento = new Date(_dadosPessoais.dataNascimento);
                
                vm.pessoa.flagDeficiente = _dadosPessoais.flagDeficiente;
                vm.contato.numero = _dadosPessoais.numeroContato;

               


               


            });
            
            if (vm.pessoa.storageError) {
                toastr["warning"](`${vm.pessoa.storageError}<br/><a href="/admin/storagestatus" target="_blank">Mais detalhes...</a>`);
            }
        } catch (error) {
            console.error('Erro ao atualizar pessoa:', error);
        }
    };


    vm.salvarVoltar = function () {
        vm.inseriroNovoAposSalvar = false;
    };

    vm.salvarNovo = function () {
        vm.inseriroNovoAposSalvar = true;
    };

    vm.salvar = function (formPessoa) {
        var campoAba1 = ['cpf', 'nome', 'sexo', 'dataNascimento', 'numeroIdentidade', 'orgaoExpedidorIdentidade', 'ufIdentidade', 'escolaridade', 'entidade'];
        var campoAba2 = ['logradouro', 'numeroEndereco', 'bairro', 'cep', 'uf', 'cidade'];
        var campoAba3 = ['email'];

        if (formPessoa.$invalid || !vm.pessoa.contatos || vm.pessoa.contatos.length === 0) {

            var erros = formPessoa.$error.required ? formPessoa.$error.required.map(function (e) { return e.$name; }) : [];
            erros = erros.concat(formPessoa.$error.cpfValido ? formPessoa.$error.cpfValido.map(function (e) { return e.$name; }) : []);

            if (erros.filter(function (el) { return campoAba1.indexOf(el) !== -1; }).length > 0) {
                vm.tab = 1;
            }
            else if (erros.filter(function (el) { return campoAba2.indexOf(el) !== -1; }).length > 0) {
                vm.tab = 2;
            }
            else if (erros.filter(function (el) { return campoAba3.indexOf(el) !== -1; }).length > 0 || vm.pessoa.contatos.length === 0) {
                vm.tab = 3;
            }

            toastr["error"]('Preencha corretamente os campos assinalados no formulário.', 'Preenchimento inválido');
            return;
        }

        var success = function (response) {
            toastr["success"]("Registro salvo com sucesso.", "Sucesso");
            formPessoa.$setPristine();

            if (vm.inseriroNovoAposSalvar) {
                $state.go('pessoas-cadastro', {}, { reload: true });

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
        console.log('Pessoa: ', vm.pessoa);

        return $http.post('/pessoas/salvar', vm.pessoa).then(success, error).finally(onComplete);
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


    vm.adicionarContato = function (event) {

        if (event) {
            event.preventDefault();
        }

        vm.numeroContatoRequired = false;
        vm.tipoContatoRequired = false;
        vm.submitedFormContato = true;

        if (!vm.pessoa.contatos) {
            vm.pessoa.contatos = [];
        }

        if ((vm.contato.tipo && vm.contato.tipo.id > 0) && vm.contato.numero) {

            vm.contato.tipoContatoId = vm.contato.tipo.id;
            vm.pessoa.contatos.push(vm.contato);
            vm.contato = {};

            vm.submitedFormContato = false;

        } else {
            if (!vm.contato.tipo || vm.contato.tipo.id <= 0) {
                vm.tipoContatoRequired = true;
            }
            if (!vm.contato.numero) {
                vm.numeroContatoRequired = true;
            }

            toastr["error"]('Preencha corretamente os campos assinalados para adicionar um Contato.');
        }

        $("#tipoContato").focus();
    };

    vm.excluirContato = function (idx) {
        if (idx >= 0 || idx < vm.pessoa.contatos.length) {
            vm.pessoa.contatos.splice(idx, 1);
        }
    };

    vm.carregarTipoEntidade = function () {
        return $http.get('/entidades/recuperarTipos').then(function (response) {
            vm.tiposEntidade = response.data;
        });
    };

    vm.carregarTipoContato = function () {
        return $http.get('/pessoas/RecuperarTipoContatoAll').then(function (response) {
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

    vm.carregarTipoEscolaridade = function () {
        return $http.get('/pessoas/recuperarTiposEscolaridade').then(function (response) {
            vm.tiposEscolaridade = response.data;
        });
    };

    vm.carregarTipoDeficiencia = function () {
        return $http.get('/pessoas/recuperarTiposDeficiencia').then(function (response) {
            vm.tiposDeficiencia = response.data;
        });
    };

    vm.carregarOrgaosExpedidor = function () {
        return $http.get('/pessoas/recuperarTiposOrgaoExpedidor').then(function (response) {
            vm.orgaoExpedidores = response.data;
        });
    };

    vm.carregarSexo = function () {
        return $http.get('/pessoas/recuperarSexos').then(function (response) {
            vm.sexos = response.data;
        });
    };

    var onFileSelect = function (evt) {
        var file = evt.currentTarget.files[0];

        if (!file) {

            vm.pessoa.imagem = null;

            setTimeout(function () {
                $('#modal-crop2').modal('hide');
            }, 100);

            return;
        }

        var reader = new FileReader();

        reader.onload = function (evt) {
            $scope.$apply(function ($scope) {
                vm.imageToCrop = evt.target.result;
            });
        };

        reader.readAsDataURL(file);
    };

    vm.concluirCrop = function () {
        vm.pessoa.imagem = vm.croppedImage.split(';base64,')[1];
        //vm.eixo.imagem = $scope.croppedImage;
        $('#modal-crop2').modal('hide');
    };

    vm.limparImagem = function () {
        vm.pessoa.imagem = undefined;
    };

    vm.validarDataNascimento = function (form) {
        if (vm.pessoa.dataNascimento >= new Date()) {
            form.dataNascimento.$setValidity('invalid', false);
        } else {
            form.dataNascimento.$setValidity('invalid', true);
        }
    };

    $scope.selecionarEntidade = function (entidade) {
        if (entidade) {
            vm.pessoa.entidadeId = entidade.id;
            vm.pessoa.entidadeNome = entidade.nome;
        } else {
            toastr["error"]('Selecionar Entidade.');
        }
    };

    vm.carregarMunicipiosEntidade = function () {
        return $http.get('/municipios/recuperarMunicipios?uf=' + vm.queryEntidade.uf).then(function (response) {
            vm.municipios = response.data;
        });
    };

    vm.init();
}
