angular.module('trilhasapp')
    .directive('cpfValido', function () {
        return {
            restrict: 'A',
            require: 'ngModel',
            link: function (scope, elem, attrs, ctrl) {

                scope.$watch(attrs.ngModel, function () {

                    if (elem[0].value.length === 0)
                        ctrl.$setValidity('cpfValido', true);

                    else if (elem[0].value.length < 11) {
                        ctrl.$setValidity('cpfValido', false);
                    }
                    else if (ctrl.$viewValue.length === 14) {
                        var cpf = elem[0].value.replace('.', '').replace('.', '').replace('-', '').substring(0, 11);
                        var isValido = true;
                        var Soma;
                        var Resto;
                        Soma = 0;

                        for (i = 1; i <= 9; i++) {
                            Soma = Soma + parseInt(cpf.substring(i - 1, i)) * (11 - i);
                        }

                        Resto = (Soma * 10) % 11;

                        if ((Resto === 10) || (Resto === 11)) {
                            Resto = 0;
                        }

                        if (Resto !== parseInt(cpf.substring(9, 10))) {
                            isValido = false;
                        }

                        Soma = 0;

                        for (i = 1; i <= 10; i++) {
                            Soma = Soma + parseInt(cpf.substring(i - 1, i)) * (12 - i);
                        }

                        Resto = (Soma * 10) % 11;

                        if ((Resto === 10) || (Resto === 11)) {
                            Resto = 0;
                        }

                        if (Resto !== parseInt(cpf.substring(10, 11))) {
                            isValido = false;
                        }

                        var c = cpf.charAt(0)
                        var cont = 1;
                        for (var i = 1; i < 14; i++) {
                            if (cpf[i] === c)
                                cont++;
                        }

                        if (!isValido || (cont === 11)) {
                            ctrl.$setValidity('cpfValido', false);
                        } else {
                            ctrl.$setValidity('cpfValido', true);
                        }
                    }
                    else
                        ctrl.$setValidity('cpfValido', true);
                });
            }
        };
    });