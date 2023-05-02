angular
    .module('trilhasapp')
    .controller('MenuController', MenuController);

MenuController.$inject = ['$state','$rootScope'];

function MenuController($state, $rootScope) {
    var vm = this;

    vm.mudaRota = function (rota) {
        $state.go(rota);
    };

    $rootScope.$on('$stateChangeSuccess', function (event, toState, toParams, fromState, fromParams) {
        vm.menuAtivo = $state.current.name.split('-')[0];
    });
}