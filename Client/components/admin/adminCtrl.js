angular.module("eventApp")
.controller("adminCtrl", function ($scope, $location, $state, $timeout, eventsService, usersService, Upload, $log) {

    // Initializing the parameters for the query string.
    $scope.searchParams = {};
    var map = {};
    var circle = {};

    // GetPosition - (Html5 GeoLocationGET)
    function getPosition() {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(positionSuccess, positionError);
        } else {
            // Got no events and no position? Inform user
            if (!$scope.listInfo.events) {
                console.log("Got no events and no position? Inform user");
            };
            // No browser support
            console.log("Geolocation is not supported by this browser.");
            // RenderGeneralMap
            renderGeneral();
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
        // LOG
        console.log("getPosition, success. $scope.position.posLat: " + $scope.position.posLat);
        // renderMap
        renderMap();
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
            console.log("Got no events and no position? Inform user");
        };
        // RenderGeneralMap
        renderGeneral();
    }



    // If list of events does_not_exist...
    if (!$scope.listInfo.events) {
        // set listView paging
        $scope.listInfo.paging.page = 1;
        $scope.listInfo.paging.itemsPerPage = 5;
        $scope.listInfo.paging.sortBy = "eventStart";
        $scope.listInfo.paging.reverse = false;
        $scope.listInfo.paging.totalItems = 0;
        // LOG
        console.log("Events did not exists");
        // Got a position: GetEvents and renderMap
        if ($scope.position.posLat && $scope.position.posLng) {
            // LOG
            console.log("Got a position");
            renderMap();
            $scope.listInfo.paging.page = 1;
            loadEvents();
        } // Got no position, try: getPosition 
        else {
            // LOG
            console.log("Got no position, try: getPosition");
            getPosition();
            // Success: GetEvents and renderMap
            // Fail, inform: Choose a search area and search
        }
    } // list of events_does_exist
    else {
        // LOG
        console.log("list of events_does_exist");
        // Got position: renderMap
        if ($scope.position.posLat && $scope.position.posLng) {
            // LOG
            console.log("Got position: renderMap");
            renderMap();
        } // Got no position, try: getPosition
        else {
            getPosition();

            // ***THIS SHOULD NOT HAPPEN***
            // Map and eventList will be out of sync

            // ALSO: GotList + Something wrong with paging
            // Should not happen, but ***YOU ARE NOT CHECKING FOR IT***

            // LOG
            console.log("Got no position, try: getPosition");
            console.log("BUT: ***THIS SHOULD NOT HAPPEN***");
            // Will:
            // if Got position: renderMap
            // else: renderMap_Generally
        }
    }

    // Reload State (Not app)
    $scope.refreshEvents = function () {
        $state.reload();
    };

    // Search
    $scope.search = function () {
        $scope.listInfo.paging.page = 1;
        console.log($scope.listInfo.paging.itemsPerPage);
        loadEvents();
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

    // Request: GetEvents
    function loadEvents() {
        var queryObj = {};
        angular.extend(queryObj, $scope.searchParams, $scope.listInfo.paging, $scope.position);

        $scope.listInfo.events = null;
        eventsService.queryEvents(queryObj)
        .then(function (response) {
            $scope.listInfo.events = response.data.events;
            $scope.listInfo.paging.totalItems = response.data.count;
        });
    };

    // Delete Event
    $scope.deleteEvent = function (event) {
        eventsService.deleteEvent(event.eventId)
        .then(function () {
            $scope.listInfo.events.splice($scope.listInfo.events.indexOf(event), 1);
        });
    };

    //// genderBtn
    //$scope.checkModel = {
    //    left: "male",
    //    middle: "female",
    //    right: "all"
    //};
    //// genderBtn
    //$scope.checkResults = [];
    //// genderBtn
    //$scope.$watchCollection('checkModel', function () {
    //    $scope.checkResults = [];
    //    angular.forEach($scope.checkModel, function (value, key) {
    //        if (value) {
    //            $scope.checkResults.push(key);
    //        }
    //    });
    //});

    // CurrentLocation Button calls:
    $scope.getPos = function () {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(setCircleAndMapCenter, positionError);
        } else {
            // No browser support
            // LOG
            console.log("Geolocation is not supported by this browser.");
        }
    };


    // Initialize DateTime searchParams.eventStart
    function initDate() {
        var date = new Date();
        date.setDate(date.getDate() + 1);
        $scope.searchParams.eventStart = date;
    };
    // Call to above
    initDate();

    // DT Picker: Minimum selectable date
    $scope.minDate = new Date();
    // DT Picker: Options
    $scope.dateOptions = {
        showWeeks: false
    };
    // DT Picker: On open/click
    $scope.open = function ($event, opened) {
        $event.preventDefault();
        $event.stopPropagation();
        $scope.dateOpened = true;
    };
    // DT Picker: Values used by. . .
    $scope.dateOpened = false;
    $scope.hourStep = 1;
    $scope.format = "dd-MMM-yyyy";
    $scope.minuteStep = 1;
    $scope.showMeridian = false;

    // On searchParams.eventStart change
    $scope.$watch("searchParams.eventStart", function (newEventStart) {
        // read date value
        //// LOG
        //console.log("newEventStart: " + newEventStart);
    }, true);

    // WHY NOT: 
    // Chug all this mapRendering crap into a function that I call when it's appropriate? 
    // Also: Probably no reason for having map/circle on scope.

    function renderGeneral() {
        // LOG
        console.log("renderGeneral");
        $scope.position.posLat = 59;
        $scope.position.posLng = 10;
        renderMap(5);
        // Inform user
    };

    function renderMap(zoom) {
        // LOG
        console.log("renderMap has been called");
        // LatLng object = searchParams
        var myLatLng = new google.maps.LatLng($scope.position.posLat, $scope.position.posLng);

        // Initialize/Render map
        map = new google.maps.Map(document.getElementById('map'), {
            // mapOptions
            zoom: (zoom ? zoom : 8),
            center: myLatLng
        });

        // Initialize circle
        circle = new google.maps.Circle({
            center: myLatLng,
            radius: ($scope.position.radius ? $scope.position.radius : 10000), // Meters
            editable: true,

            strokeColor: '#FF0000',
            strokeOpacity: 0.8,
            strokeWeight: 2,
            fillColor: '#FF0000',
            fillOpacity: 0.10,

            // Render circle
            map: map,
        });

        // Listener: Circle_Position_Change
        circle.addListener('center_changed', function () {

            // set searchParam properties to circle position
            $scope.position.posLat = circle.getCenter().lat();
            $scope.position.posLng = circle.getCenter().lng();

            // LOG: Circle_Position_Change
            console.log("Circle_Position_Change, variables below: ");
            console.log("position.posLat: " + $scope.position.posLat);
            console.log("position.posLng: " + $scope.position.posLng);
        });

        // Listener: Circle_Radius_Change
        circle.addListener('radius_changed', function () {

            // set searchParam radius property to circle radius
            $scope.position.radius = circle.getRadius();

            // LOG: Circle_Radius_Change
            console.log("position.radius: " + $scope.position.radius + " meters");
        });
    };

    // Button currentPosition calls: 
    function setCircleAndMapCenter(position) {
        // Set: SearchParams to position
        $scope.position.posLat = position.coords.latitude;
        $scope.position.posLng = position.coords.longitude;
        // Get 
        map.setCenter({
            lat: $scope.position.posLat,
            lng: $scope.position.posLng
        });
        // Center circle
        circle.setCenter({
            lat: $scope.position.posLat,
            lng: $scope.position.posLng
        });
    };

    // getAllUsers
    $scope.getAllUsers = function () {
        usersService.getAllUsers()
        .then(function (data) {
            console.log(data);
            $scope.users = data.data;
        });
    };

    // queryUsers
    $scope.queryUsers = function () {
        usersService.queryUsers($scope.queryObj)
        .then(function (data) {
            $scope.users = data.data;
        })
    }

    // findUserById
    $scope.getById = function (userId) {
        usersService.getUserById(userId)
            .then(function (data) {
                $scope.users = [];
                $scope.users.push(data.data);
            });
    };

    // findUserByUserName
    $scope.getByName = function (userName) {
        usersService.getUserByUserName(userName)
        .then(function (data) {
            console.log(data.data);
            $scope.users = [];
            $scope.users.push(data.data);
        });
    };

    // deleteUserById
    $scope.deleteUser = function (user) {
        usersService.deleteUserById(user.id)
        .then(function () {
            $scope.users.splice($scope.users.indexOf(user), 1);
        });
    };


    $scope.upload = function (file) {

        if (file && !file.$error) {
            Upload.upload({
                url: '/blobs/uploadDefaultImages/',
                data: {
                    file: file
                }
            }).then(function (response) {
                // Success
                $timeout(function () {
                    $scope.log =
                        'file: '
                        + response.config.data.file.name
                        + ', Response: '
                        + JSON.stringify(response.data)
                        + '\n'
                        + $scope.log;
                    //$scope.userInfo.imageUrl = response.data[0].fileUrl;
                    $log.debug(response.data);
                });
            },
            // Error
            null,
            // Notify callback: Can be called zero or more times to provide a progress indication.
            function (evt) {
                var progressPercentage = parseInt(100.0 * evt.loaded / evt.total);
                $scope.log =
                    'progress: '
                    + progressPercentage
                    + '% '
                    + evt.config.data.file.name
                    + '\n'
                    + $scope.log;
            });
        }
    };


});