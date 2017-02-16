var app = angular.module("myApp.loader", []);
app.directive("loader", function() {
    return {
        restrict: 'E',
        templateUrl : "components/loader.html"
    };
});