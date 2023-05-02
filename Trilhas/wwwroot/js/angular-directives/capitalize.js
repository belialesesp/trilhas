angular.module('trilhasapp')
    .directive('capitalize', function () {
        return {
            require: 'ngModel',
            link: function (scope, element, attrs, modelCtrl) {

                var capitalize = function (inputValue) {

                    if (inputValue === undefined) inputValue = '';

                    var capitalized = inputValue.toUpperCase();

                    if (capitalized !== inputValue) {

                        if (element[0].type !== "email") {
                            var selection = element[0].selectionStart;
                        }

                        // see where the cursor is before the update so that we can set it back
                        modelCtrl.$setViewValue(capitalized);
                        modelCtrl.$render();

                        // set back the cursor after rendering
                        if (element[0].type !== "email") {
                            element[0].selectionStart = selection;
                            element[0].selectionEnd = selection;
                        }
                    }

                    return capitalized;
                };

                modelCtrl.$parsers.push(capitalize);
                capitalize(scope[attrs.ngModel]); // capitalize initial value
            }
        };
    });