angular.module('trilhasapp')
	.component('confirmCancelarInscricaoComponent', {

		transclude: false,
		templateUrl: '/js/angular-components/confirm-cancelar-inscricao-component.html',
		bindings: {
			confirmationCallback: '<'
		},
		controller: function ConfirmCancelarInscricaoComponent($scope) {

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
