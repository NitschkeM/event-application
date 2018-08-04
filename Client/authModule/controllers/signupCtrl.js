'use strict';
angular.module("authModule")
.controller('signupCtrl', ['$scope', '$location', '$timeout', 'authService', function ($scope, $location, $timeout, authService) {

    $scope.savedSuccessfully = false;
    $scope.message = "";

    $scope.registration = {
        userName: "",
        password: "",
        confirmPassword: "",
        gender: "male",
        email: ""
    };
    // Registration
    $scope.signUp = function () {

        authService.saveRegistration($scope.registration).then(function (response) {
            // Registration Success
            $scope.savedSuccessfully = true;
            $scope.message = "User has been registered successfully, you will be redicted to login page in 2 seconds.";
            startTimer();

        },
         function (response) {
             // Registration Error
             var errors = [];
             for (var key in response.data.modelState) {
                 for (var i = 0; i < response.data.modelState[key].length; i++) {
                     errors.push(response.data.modelState[key][i]);
                 }
             }
             $scope.message = "Failed to register user due to:" + errors.join(' ');
         });
    };

    // On registration success: redirect to login in 2 sec
    var startTimer = function () {
        var timer = $timeout(function () {
            $timeout.cancel(timer);
            $location.path('/login');
        }, 2000);
    }

    // **********datePicker start**********
    $scope.initDate = function () {
        var date = new Date();
        date.setDate(date.getDate() - 7000);
        //$scope.newUser = {};
        $scope.registration.dateOfBirth = date;
    };
    $scope.initDate();


    // $scope.minDate = new Date();

    $scope.dateOptions = {
        showWeeks: false
    };

    $scope.open = function ($event, opened) {
        $event.preventDefault();
        $event.stopPropagation();
        $scope.dateOpened = true;
    };

    $scope.dateOpened = false;
    $scope.hourStep = 1;
    $scope.format = "dd-MMM-yyyy";
    $scope.minuteStep = 1;

    $scope.showMeridian = false;

    $scope.$watch("date", function (date) {
        // read date value
    }, true);
    // **********datePicker end**********


}]);