angular.module("eventApp")
.controller("editCtrl", function ($scope, $stateParams, $location, $state, $timeout, data, tags, eventsService, defaultImgs, Upload, $log, uibDateParser) {
    // For navigation objects
    $scope.mySetPath();

    // added this to detailsCtrl as well
    //$scope.editObj.currentEvent = {};
    var map = {};
    var marker = {};
    $scope.editObj = {
        currentEvent: {},
        defaultImgs: defaultImgs,
        tags: tags,
        defaultSelected: defaultImgs[0],
        btnActive: {},
        btnClass:{}
    }

    //// Not important?
    //$log.debug($scope.editObj.tags);

    //// Not important?
    //$scope.$watch('editObj.defaultSelected', function () {
    //    $log.debug($scope.editObj.defaultSelected);
    //    //$log.info($scope.editObj.defaultSelected[0]);
    //    //$log.info($scope.editObj.defaultSelected[0].name);
    //    //$log.info($scope.editObj.defaultSelected[0].imageUrl);
    //});


    // ********************* $scope.advObj.btnActive.Param != Param*****************************
    function trueFalse(param) {
        $scope.editObj.btnActive[param] = angular.isDefined($scope.editObj.btnActive[param]) ? $scope.editObj.btnActive[param] : false;
        $scope.editObj.btnActive[param] = !$scope.editObj.btnActive[param];
    };

    $scope.editObj.btnColorFunc = function (param, fromBtn) {
        //$log.debug(param, fromBtn);
        if (fromBtn) {
            trueFalse(param);
            //$log.debug('Line that runs after trueFalse call');
        }
        //if ($scope.advObj.searchParams[param] || $scope.advObj.searchParams[param] === 0) { // I taught ageMin === 0 would eval to false with other, but guess not? ..
        if ($scope.editObj.currentEvent[param] && $scope.editObj.currentEvent[param].length !== 0) { // I taught ageMin === 0 would eval to false with other, but guess not? ..
            $scope.editObj.btnClass[param] = 'btn-success';
            $scope.editObj.btnActive[param] = true;
        }
        else {
            $scope.editObj.btnClass[param] = $scope.editObj.btnActive[param] ? 'btn-warning' : 'btn-primary';
        }
        //$log.debug($scope.advObj.btnClass);
    }

    // ************************* Tags *********************************

    $scope.editObj.addTag = function (tag) {
        $scope.editObj.currentEvent.tags.push(tag);
        $scope.editObj.tags.splice($scope.editObj.tags.indexOf(tag), 1);

        $log.debug($scope.editObj.tags);
        $log.debug($scope.editObj.currentEvent.tags);
    };

    $scope.editObj.addCustom = function (customTag) {
        var newTag = { name: customTag };
        $scope.editObj.currentEvent.tags.push(newTag);
        $scope.editObj.customTag = null;

        $log.debug($scope.editObj.currentEvent.tags);
    };

    $scope.editObj.removeTag = function (addedTag) {
        $scope.editObj.currentEvent.tags.splice($scope.editObj.currentEvent.tags.indexOf(addedTag), 1);
        if (angular.isDefined(addedTag.id)) {
            $scope.editObj.tags.push(addedTag);
        }
    };

    // ************************* Cancel - Create - Update - RedirectTimer - UploadFile *********************************

    // Cancel
    $scope.editObj.cancelEdit = function () {
        $state.go("mainParent.frontpage");
    }

    // Create Event
    $scope.editObj.createEvent = function (newEvent) {
        newEvent.eventStart = combineDT($scope.editObj.pickers.startDate, $scope.editObj.pickers.startTime);
        newEvent.eventStatus = 'open';
        if ($scope.editObj.useImage === 'default') {
            newEvent.pictureId = $scope.editObj.defaultSelected.pictureId;
        }
        eventsService.createEvent(newEvent)
            .then(function (resp) {
                if ($scope.editObj.useImage === 'upload') {
                    upload(resp.data.event.eventId);
                }
                startTimer(); // Just redirect
            });
    }


    // Update Event
    $scope.editObj.saveEdit = function (event) {
        event.eventStart = combineDT($scope.editObj.pickers.startDate, $scope.editObj.pickers.startTime);
        if ($scope.editObj.useImage === 'default') {
            event.pictureId = $scope.editObj.defaultSelected.pictureId;
        }
        eventsService.editEvent(event)
            .then(function (resp) {
                if ($scope.editObj.useImage === 'upload') {
                    upload(event.eventId);
                }
                startTimer(); // Just redirect
            });
    }

    // Redirect 1 sec delay
    var startTimer = function () {
        var timer = $timeout(function () {
            $timeout.cancel(timer);
            $scope.editObj.currentEvent = {};
            // LOG
            console.log("editObj.currentEvent below");
            console.log($scope.editObj.currentEvent);
            $state.go("mainParent.userList");
        }, 1000);
    }
    // Upload file
    var upload = function (eventId) {
        if ($scope.editObj.file && !$scope.editObj.file.$error) {
            $log.debug('Upload called, passed eventId below:');
            $log.debug(eventId);
            Upload.upload({
                url: '/blobs/uploadEventImage/' + eventId,
                data: {
                    file: $scope.editObj.file
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

    // ****************************** Date-Time Picker******************************************

    $scope.editObj.pickers = {
        isOpen: false,
        openCalendar: function () {
            $scope.editObj.pickers.isOpen = true;
        },
        dateOptions: {

        },
        timeOptions: {
            showSpinners: false,
            isMeridian:false
        }
    }
    $scope.combineDT = function (date, time) {
        $scope.combinedDT = new Date(date.getFullYear(), date.getMonth(), date.getDate(), time.getHours(), time.getMinutes());
        $log.debug('datePassed below:');
        $log.debug(date);
        $log.debug('timePassed below:');
        $log.debug(time);
        $log.debug('combined below:');
        $log.debug($scope.combinedDT);
    };
    var combineDT = function (date, time) {
        return new Date(date.getFullYear(), date.getMonth(), date.getDate(), time.getHours(),time.getMinutes(),0,0);
    }
    var initDateTime = function () {
        var time = new Date(0, 0, 0, 19, 0, 0, 0);
        $scope.editObj.pickers.startDate = new Date();
        $scope.editObj.pickers.startTime = new Date(0, 0, 0, 19, 0, 0, 0);
    }

    //$scope.dtPicker = {
    //    dt: new Date(),
    //    timepickerOptions: {
    //        showMeridian: false
    //    }
    //};

    //$scope.openCalendar = function (e) {
    //    $scope.dtPicker.open = true;
    //};

    // ****************************** GEO-LOCATION ******************************************
    // Html5 GeoLocationGET
    $scope.getLocation = function () {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(showPosition, showError);
        } else {
            // No browser support
            console.log("Geolocation is not supported by this browser.");
        }
    }

    // Html5 GeoLocationGET Success
    function showPosition(position) {
        var myLat = position.coords.latitude;
        var myLng = position.coords.longitude;

        $scope.editObj.currentEvent.posLat = myLat;
        $scope.editObj.currentEvent.posLng = myLng;

        map.setCenter({ lat: myLat, lng: myLng });
        marker.setPosition({ lat: myLat, lng: myLng });
    }
    // Html5 GeoLocationGET Error
    function showError(error) {
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
    }
    // ****************************** MAP ******************************************
    function mapInit() {
        // Initial markerPosition and mapCenter
        var myLatLng = new google.maps.LatLng($scope.editObj.currentEvent.posLat, $scope.editObj.currentEvent.posLng);

        // Initialize map
        map = new google.maps.Map(document.getElementById('mapE'), {
            //mapOptions
            zoom: 12,
            center: myLatLng
        });

        // Initialize marker
        marker = new google.maps.Marker({
            position: myLatLng,
            map: map,
            draggable: true
        });

        // EventListener on marker-drag-end
        marker.addListener('dragend', function () {
            // set editObj.currentEvent properties to marker position
            $scope.editObj.currentEvent.posLat = marker.getPosition().lat();
            $scope.editObj.currentEvent.posLng = marker.getPosition().lng();
            $log.info('editObj.currentEvent.posLat;');
            $log.info($scope.editObj.currentEvent.posLat);
            $log.debug('editObj.currentEvent.posLng:');
            $log.debug($scope.editObj.currentEvent.posLng);
        });
    }

    // Resize Map on Window Resize
    google.maps.event.addDomListener(window, "resize", function () {
        var center = map.getCenter(); // Change to posLat-posLng? 
        google.maps.event.trigger(map, "resize");
        map.setCenter(center);
    });

    // ********************** PAGE INIT (event + map) **********************************
    // Placed at bottom, else: $scope.getLocation is not a function.
    //if ($location.path().indexOf("/edit/") == 1) {
    if ($state.includes("mainParent.edit")) {
        $scope.editObj.currentEvent = data.event;
        $scope.editObj.useImage = $scope.editObj.defaultImgs.find(function (item) { return item.pictureId === $scope.editObj.currentEvent.pictureId; }) ? 'existing' : 'default';
        // Set DT pickers values (Should I copy to avoid referencing same object?)
        $scope.editObj.pickers.startDate = angular.copy($scope.editObj.currentEvent.eventStart);
        $scope.editObj.pickers.startTime = angular.copy($scope.editObj.currentEvent.eventStart);
        mapInit();
        //$scope.editObj.existingId = $scope.editObj.currentEvent.pictureId;
        //$scope.editObj.existingUrl = $scope.editObj.currentEvent.imageUrl;
        $log.debug("**********$scope.editObj.useImage Below****************");
        $log.debug($scope.editObj.useImage);
        $log.debug("**********$scope.editObj.currentEvent.pictureId Below****************");
        $log.debug($scope.editObj.currentEvent.pictureId);
        $log.debug("**********$scope.editObj.currentEvent Below****************");
        $log.debug($scope.editObj.currentEvent);
    } else {
        $scope.editObj.currentEvent = {};
        //$scope.editObj.currentEvent.gender = "all";
        $scope.editObj.currentEvent.posLat = 10;
        $scope.editObj.currentEvent.posLng = 15;
        $scope.editObj.currentEvent.tags = [];
        $scope.editObj.currentEvent.approvalReq = false;
        $scope.editObj.useImage = 'default';
        initDateTime();
        mapInit();
        $scope.getLocation();
        //var date = new Date();
        //date.setDate(date.getDate() + 1);
        //$log.debug("date:" + date);
        //$scope.editObj.currentEvent.eventStart = date;
        //$scope.editObj.currentEvent.eventEnd = date;
    }
    $scope.editObj.btnColorFunc('ageMin', false);
    $scope.editObj.btnColorFunc('ageMax', false);
    $scope.editObj.btnColorFunc('partMin', false);
    $scope.editObj.btnColorFunc('partMax', false);
    $scope.editObj.btnColorFunc('gender', false);
    $scope.editObj.btnColorFunc('tags', false);
});

//// ****************************** GENDER BTNS******************************************
//// GenderBtn
//$scope.checkModel = {
//    left: "male",
//    middle: "female",
//    right: "all"
//};
//// GenderBtn
//$scope.checkResults = [];
//// GenderBtn
//$scope.$watchCollection('checkModel', function () {
//    $scope.checkResults = [];
//    angular.forEach($scope.checkModel, function (value, key) {
//        if (value) {
//            $scope.checkResults.push(key);
//        }
//    });
//});


//// Create_or_Edit Event
//$scope.editObj.saveEdit = function (event, file) {
//    $scope.editObj.currentEvent.open = angular.isDefined($scope.editObj.currentEvent.open) ? $scope.editObj.currentEvent.open : true;
//    if ($scope.editObj.useImage === 'exisiting') {
//        updateEvent(event);
//    }
//    else if ($scope.editObj.useImage === 'default') {
//        event.pictureId = $scope.editObj.defaultSelected.pictureId;
//        if (angular.isDefined(event.eventId)) {
//            updateEvent(event);
//        } else {
//            createEvent(event);
//        }
//    }
//    else if ($scope.editObj.useImage === 'upload') {
//        if (file) {
//            Upload.upload({
//                url: '/blobs/upload/',
//                data: {
//                    file: file,
//                    event: event
//                }
//            }).then(function (resp) {
//                // Success
//                $log.debug("*****Resp****");
//                $log.debug(resp);
//                $log.debug("*****Resp.data****");
//                $log.debug(resp.data);
//                event.pictureId = resp.data[0].pictureId;
//                if (angular.isDefined(event.eventId)) {
//                    updateEvent(event);
//                } else {
//                    createEvent(event);
//                }
//            });
//        }
//        else {
//            // Inform: Please upload a file.
//            $log.debug("Please upload a file");
//        }
//    }
//    else {
//        // catch error
//        $log.debug("Error: Something is wrong");
//    }
//}

//// Create Event
//var createEvent = function (newEvent) {
//    eventsService.createEvent(newEvent)
//        .then(function (resp) {
//            startTimer();
//        });
//};

//// Edit Event
//var updateEvent = function (event) {
//    eventsService.editEvent(event)
//    .then(function (resp) {
//        startTimer();
//    });
//}