angular.module('trilhasapp')
	.component('confirmInscreverCursistaComponent', {

		transclude: false,
		templateUrl: '/js/angular-components/confirm-inscrever-cursista-component.html',
		bindings: {
			confirmationCallback: '<'
		},
		controller: function ConfirmInscreverCursistaComponent($scope) {

			var confirmou = false;

			this.dialogConfirm = function () {
				confirmou = true;
				$('#confirm-dialog').modal('hide');
			};

			$('#confirm-dialog').on('hidden.bs.modal', function (e) {
				if (confirmou) {
					$scope.$ctrl.confirmationCallback();
					confirmou = false;
				}
			});
		}
	});
