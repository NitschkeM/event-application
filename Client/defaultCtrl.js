// authService added as dependency for the logout function I think
angular.module("eventApp").controller("defaultCtrl", function ($scope, $location, eventsResource, $state, authService, $http, $log) {

    // For navigation objects *****Switch to prototypical inheritance*****
    // This should not be needed w/ $state? 
    $scope.mySetPath = function () {
        $scope.currentPath = $location.path();
        $log.info($scope.currentPath);
    }


    // But, why is it an object - instead of an array - becuse in listCtrl.js it resolves a query and adds result as eventArray.events.
    // **Not really used atm**
    $scope.eventArray = {};

    $scope.defaultObj = {
        searchModal: {},
        mapModal: {},
        getFuture: {},
        getPast: {},
        //saveEdit: {},
        //cancelEdit: {},
        currentEvent: {},
        searchParams: {
            tags:[]
        }
    };

    // For list->details->list navigation
    $scope.listInfo = {
        paging: {}
    };
    //$scope.searchParams = {};
    // For list
    $scope.position = {
        radius: 10000 // Meters
    };

    $scope.reflowObj = {
        forceReflow: function () {
            // Could change this condition to a variable I manage, if this becomes problematic.
            if ($location.path() === '/main/frontpage') {
                var textCreate = document.getElementById('jumboTextCreate');
                var textFind = document.getElementById('jumboTextFind');
                var textMy = document.getElementById('jumboTextMy');
                var textGeneral = document.getElementById('jumboTextGeneral');
                textCreate.style.animation = '0';
                textFind.style.animation = '0';
                textMy.style.animation = '0';
                textGeneral.style.animation = '0';
                console.log(textCreate.offsetHeight);
                console.log(textFind.offsetHeight);
                console.log(textMy.offsetHeight);
                console.log(textGeneral.offsetHeight);
            }
        }
    }

    // from tutorials indexController *****NOTE: Change path*****
    $scope.logOut = function () {
        authService.logOut();
        $state.go("mainParent.frontpage");
        //$location.path('/home');
    }


    // ***From: authService.js***  
    // _authentication.isAuth = true;
    // _authentication.userName = loginData.userName;
    $scope.authentication = authService.authentication;
});



//// **Add Participant**
//$scope.addParticipant = function () {
//    var currentId = $stateParams["id"];
//    $http({
//        method: "POST",
//        url: "/api/events/" + currentId
//    });
//}