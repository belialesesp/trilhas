angular.module('trilhasapp')
    .component('notificationComponent', {

        transclude: false,
        templateUrl: '/js/angular-components/notification-alert-component.html',
        bindings: {
            message: '@'
        }
    });