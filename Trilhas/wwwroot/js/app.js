'use strict';

var app = angular.module('trilhasapp', [
    'ui.router',
    'ui.router.state.events',
    'ngRoute',
    'ngSanitize',
    'ngImgCrop',
    'ngMessages',
    'sarsha.spinner',
    'ui.mask',
    'ui.utils.masks',
	'ngAnimate',
	'ngCkeditor'
])

.config(['$qProvider', function ($qProvider) {
    $qProvider.errorOnUnhandledRejections(false);
}]);

/** Rotas Movidas para /Views/_ViewRotas.cshtml  */

// Toastr
toastr.options = {
    "closeButton": true,
    "debug": false,
    "newestOnTop": true,
    "progressBar": true,
    "positionClass": "toast-bottom-right",
    "preventDuplicates": true,
    "onclick": null,
    "showDuration": "300",
    "hideDuration": "1000",
    "timeOut": "5000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
};

   