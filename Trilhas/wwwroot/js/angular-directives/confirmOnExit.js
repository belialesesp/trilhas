angular.module("trilhasapp")
    .directive('dirtyTracking', function ($rootScope) {

        return {
            restrict: 'A',
            link: function ($scope, $elem, $attrs) {

                var x = $scope;

                window.onbeforeunload = function () {
                    if ($scope.form.$dirty) {
                        return "Você possui alterações não salvas. Realmente deseja sair dessa pagina?";
                    }
                };

                //$rootScope.$on('$locationChangeStart', function (event, next, current) {
                //    if (x.form && x.form.$dirty) {
                //        if (!confirm("Você possui alterações não salvas. Realmente deseja sair dessa pagina?")) {
                //            event.preventDefault();
                //            return;
                //        }
                //    }
                //});

                function beforeLeave(event, toState, toParams, fromState, fromParams) {
                    if (!event.defaultPrevented && (x.form && x.form.$dirty) && !confirm("Você possui alterações não salvas. Realmente deseja sair dessa pagina?")) {
                        event.preventDefault();
                        return;
                    }
                }
                $rootScope.$on('$stateChangeStart', beforeLeave);
            }
        };
    });