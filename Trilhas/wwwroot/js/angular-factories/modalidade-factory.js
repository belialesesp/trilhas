angular
    .module('trilhasapp')
    .factory('ModalidadeFactory', ModalidadeFactory);

ModalidadeFactory.$inject = ['$http', '$q'];

function ModalidadeFactory($http, $q) {
    var vm = this;
    vm.modalidades = null;
    
    var getModalidade = function () {
        if (vm.modalidades !== null) {
            if (typeof vm.modalidades === 'function') {
                return vm.modalidades;
            }
            return $q.resolve(vm.modalidades);
        }

        vm.modalidades = $http.get('/enum/recuperarEnumModalidade').then(function (response) {
            return response.data;
        });

        return vm.modalidades;
    };

    return {
        getModalidade: getModalidade
    };
}