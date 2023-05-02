angular.module('trilhasapp')
    .service('ServerErrorsService', function () {

        this.handleServerErrors = function (errorResponse, formController) {

            switch (errorResponse.status) {
                case 400:
                case 500:
                    if (errorResponse.data.errors && formController) {
                        //formController.$setValidity('required', false);
                        formController.$setSubmitted();
                        formController.serverErrors = errorResponse.data.errors;
                    }

                    //for (var x = 0; x < errorResponse.data.errors.length; x++) {
                    //    var field = errorResponse.data.errors[x].key.toLowerCase();
                    //    if (angular.isObject(formController[field]) && formController[field].hasOwnProperty('$modelValue')) {
                    //        formController[field].$setValidity('required', false);
                    //        formController[field].$setTouched();
                    //        formController[field].serverValidationMessage = errorResponse.data.errors[x].message;
                    //    } else {
                    //        console.error(`Unknown server error ${field}.`);
                    //    }
                    //}
                    break;
                default:
                    console.error('Unknown server error response.');
                    return;
            }
        };
    });