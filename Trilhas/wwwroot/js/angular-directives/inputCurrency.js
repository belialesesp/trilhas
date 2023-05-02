(function() {
  // Directive
    angular.module('trilhasapp')
        .directive('inputCurrency', ['$locale', '$filter', function ($locale, $filter) {

    // For input validation
    var isValid = function(val) {
      return angular.isNumber(val) && !isNaN(val);
    };

    // Helper for creating RegExp's
    var toRegExp = function(val) {
      var escaped = val.replace(/[-\/\\^$*+?.()|[\]{}]/g, '\\$&');
      return new RegExp(escaped, 'g');
    };

    // Saved to your $scope/model
    var toModel = function(val) {

      // Locale currency support
      var decimal = toRegExp('.');
      var group = toRegExp(',');
      var currency = toRegExp('');

      // Strip currency related characters from string
      val = val.replace(decimal, '').replace(group, '').replace(currency, '').trim();

      return parseInt(val, 10);
    };

    // Displayed in the input to users
    var toView = function(val) {
      return $filter('currency')(val, '', 0);
    };

    // Link to DOM
    var link = function($scope, $element, $attrs, $ngModel) {
      $ngModel.$formatters.push(toView);
      $ngModel.$parsers.push(toModel);
      $ngModel.$validators.currency = isValid;

      $element.on('keyup', function() {
        $ngModel.$viewValue = toView($ngModel.$modelValue);
        $ngModel.$render();
      });
    };

    return {
      restrict: 'A',
      require: 'ngModel',
      link: link
    };
  }]);
})();