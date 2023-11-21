angular
    .module('trilhasapp')
    .controller('EventoFinalizarController', EventoFinalizarController);

EventoFinalizarController.$inject = ['$stateParams', '$q', '$http', 'spinnerService', '$scope'];

function EventoFinalizarController($stateParams, $q, $http, spinnerService, $scope) {
    var vm = this;

    vm.eventoListaInscrito = {};
	vm.dataAtual = new Date();
    vm.query = {};

    vm.init = function () {
        var promises = [];

        if ($stateParams.id) {
            vm.carregarEvento($stateParams.id);
        }

        $q.all(promises).finally(onComplete);
    };

    var onComplete = function () {
        spinnerService.close('loader');
    };

    vm.carregarEvento = function (id) {
        return $http.get('/eventos/encerramento/' + id).then(function (response) {
            vm.evento = response.data;
        });
    };

    vm.visualizarPenalidade = function (penalidade) {
        $scope.penalidade = penalidade;
    };

    vm.imprimir = function () {
        window.print();
    }



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
        return $http.get("/eventos/exportarRelatorioCapacitadosPorCursoExcel?id=" + $stateParams.id, { params: vm.query }).then(successBaixarArquivo, errorBaixarArquivo).finally(onComplete);

    }

    function s2ab(s) {
        var buf = new ArrayBuffer(s.length);
        var view = new Uint8Array(buf);
        for (var i = 0; i != s.length; ++i) view[i] = s.charCodeAt(i) & 0xFF;
        return buf;
    }

    vm.init();
}
