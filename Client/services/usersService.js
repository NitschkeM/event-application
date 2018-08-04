'use strict';

angular.module('eventApp')

.factory('usersService', ['$http',
    function ($http) {
        var service = {};

        // Get All Users
        service.getAllUsers = function () {
            return $http.get('/api/accounts/users/');
        };

        // Query Users
        service.queryUsers = function (queryString) {
            return $http.get('/api/accounts/query/', { params: queryString });
        };

        // Get user by Id
        service.getUserById = function (userId) {
            return $http.get('/api/accounts/user/' + userId);
        };

        // Get user by UserName
        service.getUserByUserName = function (userName) {
            return $http.get('/api/accounts/user/' + userName);
        };

        // Delete user by Id
        service.deleteUserById = function (userId) {
            return $http.delete('/api/accounts/user/' + userId);
        };

        // Get info on current user
        service.getUserInfo = function () {
            return $http.get('/api/accounts/userinfo/');
        };

        // Edit aboutMe
        // Add/Remove Participant
        service.editAboutme = function (aboutText) {
            return $http({
                method: "PUT",
                url: "/api/accounts/editAboutMe/" + aboutText
            });
        }


        return service;
    }]);