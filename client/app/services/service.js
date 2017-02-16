/**
 * Created by nir.moreno on 21/12/2016.
 */
angular.module('myApp.services', [])
    .factory('serviceFactory', ['$q', '$http', function ($q, $http) {
        var serviceFactory = {};

        serviceFactory.getData = function (protectedVraFilePath,recoveryVraFilePath) {
            return $http({
                method: 'GET',
                url: 'http://localhost:57904/api/pma?protectedVraFilePath=C:\\logs\\a.dat&recoveryVraFilePath=C:\\logs\\a.dat'
            }).then(function successCallback(response) {
                // this callback will be called asynchronously
                // when the response is available
                return response.data;
            }, function errorCallback(response) {
                // called asynchronously if an error occurs
                // or server returns response with an error status.
                console.log('error occurs in data request', response);
            });
        };
        return serviceFactory;
    }]);
