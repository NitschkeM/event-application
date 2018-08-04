'use strict';
angular.module("authModule")
.factory('authInterceptorService', ['$q', '$location', 'localStorageService', '$injector', function ($q, $location, localStorageService, $injector) {

    var authInterceptorServiceFactory = {};

    var _request = function (config) {

        config.headers = config.headers || {};

        var authData = localStorageService.get('authorizationData');
        if (authData) {
            config.headers.Authorization = 'Bearer ' + authData.token;
        }

        return config;
    }

    // Redirect on 401 Unauthorized
    var _responseError = function (rejection) {

        if (rejection.status === 401) {
            // $state usage: Consider notes @bottom.
            var stateService = $injector.get('$state');
            stateService.go("login");
            //$location.path('/login');
        }
        return $q.reject(rejection);
    }

    authInterceptorServiceFactory.request = _request;
    authInterceptorServiceFactory.responseError = _responseError;

    return authInterceptorServiceFactory;
}]);

// Getting $state to work w/o circular dependency: 
// http://stackoverflow.com/questions/25495942/circular-dependency-found-http-templatefactory-view-state
// IMPORTANT

// I would recommend using the state change events (see https://github.com/angular-ui/ui-router/wiki#state-change-events) 
// to watch for errors using $stateChangeError and inspecting the error returned from the 401.