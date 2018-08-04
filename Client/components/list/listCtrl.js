angular.module("eventApp") // $state & $location not used ?
.controller("listCtrl", function ($scope, $location, $state, $timeout, eventsService, $uibModal, $log) {
    //// For navigation objects
    $scope.mySetPath();

    //$scope.defaultObj.searchParams = {};
    $scope.searchModal = {};
    $scope.mapModal = {};

    // Initialize eligibleOnly if not defined.
    // Does not deal with: User queries w/o auth-> logIn => allways false.
    // Could: onLogin -> Set to true. 
    if (!angular.isDefined($scope.defaultObj.searchParams.eligibleOnly)) {
        $scope.defaultObj.searchParams.eligibleOnly = $scope.authentication.isAuth ? true : false;
    }

    //// eligibleBtns calls:
    //$scope.eligibleOnly = function (trueFalse) {
    //    $scope.defaultObj.searchParams.eligibleOnly = trueFalse;
    //    $scope.search();
    //};
    // eligibleBtns calls:
    $scope.eligibleOnly = function () {
        $scope.search();
    };

    // Set ListView Paging
    function setPaging() {
        $scope.listInfo.paging.page = 1;
        $scope.listInfo.paging.itemsPerPage = 10;
        $scope.listInfo.paging.sortBy = "eventStart";
        $scope.listInfo.paging.reverse = false;
        $scope.listInfo.paging.totalItems = 0;
    };


    // GetPosition - (Html5 GeoLocationGET)
    function getPosition() {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(positionSuccess, positionError);
        } else {
            // Got no events and no position? Inform user
            if (!$scope.listInfo.events) {
                console.log("Got no events and no position: Inform user");
            };
            // No browser support
            console.log("Geolocation is not supported by this browser.");
        }
    };

    // GetPosition: Success - (Html5 GeoLocationGET)
    function positionSuccess(position) {
        // Set: SearchParams to position
        $scope.position.posLat = position.coords.latitude;
        $scope.position.posLng = position.coords.longitude;

        // Got no events? GetEvents
        if (!$scope.listInfo.events) {
            $scope.listInfo.paging.page = 1;
            loadEvents();
        };
        $log.debug("getPosition, success. $scope.position.posLat: " + $scope.position.posLat);                  // log: "getPosition, success. $scope.position.posLat: " + $scope.position.posLat 
    }

    // GetPosition: Error - (Html5 GeoLocationGET)
    function positionError(error) {
        switch (error.code) {
            case error.permission_denied:
                console.log("user denied the request for geolocation.");
                break;
            case error.position_unavailable:
                console.log("location information is unavailable.");
                break;
            case error.timeout:
                console.log("the request to get user location timed out.");
                break;
            case error.unknown_error:
                console.log("an unknown error occurred.");
                break;
        }
        // Got no events and no position? Inform user
        if (!$scope.listInfo.events) {
            $scope.listInfo.paging.page = 1;
            $log.debug("Got no events and no position? Inform user");                                           // log: Got no events and no position? Inform user
        };
    }

    // If list of events does_not_exist... ***This can proably be made simpler***
    if (!$scope.listInfo.events) {
        // set listView paging
        setPaging();
        $log.debug("Events did not exist");                                                                     // log: Events did not exits
        // Got a position: GetEvents
        if ($scope.position.posLat && $scope.position.posLng) {
            $log.debug("Got a position");                                                                       // log: Get a position
            loadEvents();
        } // Got no position, try: getPosition 
        else {
            $log.debug("Got no position, try: getPosition");                                                    // log Got no position, try: getPosition
            getPosition();
            // Success: GetEvents
            // Fail, inform: Choose a search area and search
        }
    } // list of events_does_exist
    else if (!$scope.position.posLat || !$scope.position.posLng) {
        $log.debug("List exists. NO position. Try: GetPosition. **THIS SHOULD NOT HAPPEN**");                    // log list of events_does_exist

        getPosition();

        // ***THIS SHOULD NOT HAPPEN*** Map and eventList will be out of sync ***YOU ARE NOT CHECKING FOR IT***
    }

    // ********************* SortBy & Paging ********************* 
    // Set Events Per Page
    $scope.setPageSize = function (number) {
        $scope.listInfo.paging.itemsPerPage = number;
    };

    // SortBy
    $scope.sort = function (sortBy) {
        if (sortBy === $scope.listInfo.paging.sortBy) {
            $scope.listInfo.paging.reverse = !$scope.listInfo.paging.reverse;
        } else {
            $scope.listInfo.paging.sortBy = sortBy;
            $scope.listInfo.paging.reverse = false;
        }
        $scope.listInfo.paging.page = 1;
        loadEvents();
    };

    // Select Page
    $scope.selectPage = function () {
        loadEvents();
    };

    // ********************* Search & loadEvents ********************* 
    // Should not have to exist on scope, not with current advSearch setup.
    $scope.search = function () {
        $scope.listInfo.paging.page = 1;
        console.log($scope.listInfo.paging.itemsPerPage);
        loadEvents();
    };

    function loadEvents() {
        var queryObj = {};
        angular.extend(queryObj, $scope.defaultObj.searchParams, $scope.listInfo.paging, $scope.position);
        // LOG
        $log.debug(queryObj);
        eventsService.queryEvents(queryObj)
        .then(function (response) {
            $scope.listInfo.events = null; // Better to null it after success right?
            $scope.listInfo.events = response.data.events;
            $scope.listInfo.paging.totalItems = response.data.count;
        });
    };

    // ********************* ADVANCED SEARCH MODAL *********************
    $scope.defaultObj.searchModal.open = function (size) {

        var searchModalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Client/components/list/advancedSearch.html',
            controller: 'advSearchCtrl',
            //scope: $scope,
            size: size,
            windowClass: 'advModal', // For styling I guess
            resolve: {
                sParamsCopy: function () {
                    return angular.copy($scope.defaultObj.searchParams);
                },
                tags: function ($http) {
                    return $http.get('/api/tags/')
                    .then(function (resp) {
                        return resp.data.tags;
                    });
                }
            }
        });

        searchModalInstance.result.then(function (result) {
            $scope.defaultObj.searchParams = result;
            $scope.search();
            $log.info(result);
        }, function () {
            $log.info('Modal dismissed at: ' + new Date());
        });
    };

    // ********************* ADVANCED SEARCH MODAL - END *********************
    // ***************************************************************************
    // ***************************************************************************
    // Should I do: scope: {}, for these modals, to give them an isolated scope? 
    // Do they currently prototypically inherit? I believe they already have an "isolated" / out of flow scope? 
    // ***************************************************************************
    // ***************************************************************************

    // ************************** MAP MODAL **********************************
    $scope.defaultObj.mapModal.open = function (size) {

        var mapModalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Client/components/list/map.html',
            controller: 'mapModalCtrl',
            size: size,
            windowClass: 'mapModal', // For styling I guess
            resolve: {
                items: function () {
                    return $scope.position;
                }
            }
        });

        mapModalInstance.result.then(function (position) {
            $scope.position.posLat = position.posLat;
            $scope.position.posLng = position.posLng;
            $scope.position.radius = position.radius;
            $scope.search();
            $log.info(position);                                                                                    // log: Modal Result
        }, function () {
            $log.info('Modal dismissed at: ' + new Date());                                                         // log: Modal dismissed at: time
        });
    };

    // ************************** MAP MODAL - END *****************************
});