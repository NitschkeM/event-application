(function (angular) {
    'use strict';

    angular.module('authModule', ['LocalStorageModule'])
    .run(['authService', function (authService) {
        authService.fillAuthData();
    }]);

}(window.angular));