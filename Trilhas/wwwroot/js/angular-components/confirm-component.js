angular.module('trilhasapp')
    .component('confirmComponent', {

        transclude: false,
        templateUrl: '/js/angular-components/confirm-component.html',
        bindings: {
            confirmationCallback: '<'
        },
        controller: function ConfirmComponent($scope) {

            var confirmou = false;

            this.dialogConfirm = function () {
                confirmou = true;
                $('#confirm-dialog').modal('hide');
            };

            $('#confirm-dialog').on('hidden.bs.modal', function (e) {
                if (confirmou) {
                    $scope.$ctrl.confirmationCallback();
                }
            });
        }
    });