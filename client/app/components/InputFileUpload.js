'use strict';

angular.module('myApp.directives.inputFileUploadHandler', []).directive('inputFileUploadHandler', [function() {
        return{
            scope:{
                inputFileUploadHandler:"&"
            },
            link:function($scope, $element, $attrs){
                $element.on("change",function(event){
                    $scope.inputFileUploadHandler({$event: event})
                });
                $scope.$on("$destroy",function(){
                    $element.off();
                });
            }
        }
    }]);
