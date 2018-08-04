
// ************             NOTE                  ************
// ************ .success and .error is DEPRECATED ************


// This service is responsible for: 
// *** signing up new users
// *** log-in/log-out registerd users
// *** store the generated token in client local storage

'use strict';
angular.module("authModule")
.factory('authService', ['$http', '$q', 'localStorageService', function ($http, $q, localStorageService) {

    var serviceBase = 'http://localhost:55560';
    var authServiceFactory = {};

    var _authentication = {
        isAuth: false,
        userName: ""
    };

    // Register new user
    var _saveRegistration = function (registration) {

        _logOut();
        // Note: ***** Here he uses .then *****
        return $http.post(serviceBase + '/api/accounts/register', registration).then(function (response) {
            return response;
        });

    };

    // Login
    var _login = function (loginData) {

        // build credentials body
        var data = "grant_type=password&username=" + loginData.userName + "&password=" + loginData.password;

        var deferred = $q.defer();

        // ************ .success and .error is DEPRECATED ************
        // Notice that we have configured the POST request for this endpoint:
        // to use “application/x-www-form-urlencoded” as its Content-Type and sent the data *****as string not JSON object.*****
        // post credentials: (url, body, config)
        $http.post(serviceBase + '/oauth/token', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } }).success(function (response) {

            // Add token and userName to localStorage
            // Notice how username is independent of response - it comes from credentials data
            localStorageService.set('authorizationData', { token: response.access_token, userName: loginData.userName });

            // set user info variables: isAuthenticated and userName 
            _authentication.isAuth = true;
            _authentication.userName = loginData.userName;

            deferred.resolve(response);

        }).error(function (err, status) {
            _logOut();
            deferred.reject(err);
        });

        return deferred.promise;

    };

    // Logout
    var _logOut = function () {

        // Clear local storage
        localStorageService.remove('authorizationData');

        _authentication.isAuth = false;
        _authentication.userName = "";

    };

    // Make user info available throughout the application
    var _fillAuthData = function () {

        var authData = localStorageService.get('authorizationData');
        if (authData) {
            _authentication.isAuth = true;
            _authentication.userName = authData.userName;
        }

    }

    // Method accessors
    authServiceFactory.saveRegistration = _saveRegistration;
    authServiceFactory.login = _login;
    authServiceFactory.logOut = _logOut;
    authServiceFactory.fillAuthData = _fillAuthData;
    authServiceFactory.authentication = _authentication;

    return authServiceFactory;
}]);