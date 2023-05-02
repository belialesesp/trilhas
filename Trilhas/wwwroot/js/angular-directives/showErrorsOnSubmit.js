(function () {

    angular.module('trilhasapp').directive('showErrorsOnSubmit', showErrorsOnSubmit);

    /**
    * Configura campos $pristine como $dirty e $touched durante a submissão do formulário
    *
    * @param {Object} $log - angular $log service
    *
    * @returns {void}
    */
    function showErrorsOnSubmit($log) {
        return {
            restrict: 'A',
            link: function ($scope, elem, attrs) {
                var form = $scope[attrs.name];

                if (!elem.is('form')) {
                    return;
                }

                elem.bind('submit', onSubmit);

                /**
                * Intercepta submit do form e configura campos $pristine como $dirty e $touched
                *
                * @param {Object} e - o evento de submit
                *
                * @returns {void}
                */
                function onSubmit(e) {

                    var field = null;
                    for (field in form) {
                        if (form[field] && form[field].hasOwnProperty('$pristine') && form[field].$pristine) {
                            form[field].$setDirty();
                            form[field].$setTouched();

                            $scope.$digest();
                        }
                    }
                    e.stopPropagation();
                    e.preventDefault();
                }
            }
        };
    }
})();