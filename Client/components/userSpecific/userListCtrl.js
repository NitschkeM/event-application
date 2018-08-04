angular.module("eventApp")
.controller("userListCtrl", function ($scope, eventsService, $location, $log) {
    //// For navigation objects
    $scope.mySetPath();

    $scope.search_Params = {};

    $scope.list_Info = {
        paging: {
            page: 1,
            itemsPerPage: 10
        }
    };
    // **************** I DONT THINK I COMMENTED THIS OUT **************
    var dateNow = new Date();
    //dateNow.setDate(date.getDate());

    //// Initialize DateTime afterThisDate. ***Currently only used to set initial loadEvents params
    //function initDate() {
    //    var date = new Date();
    //    date.setDate(date.getDate() + 1); // Remove + 1 probably, not needed w/o minimum
    //    $scope.search_Params.afterThisDate = date;
    //};
    //// Call to above
    //initDate();
    // **************** I DONT THINK I COMMENTED THIS OUT **************

    //// DT Picker: Options
    //$scope.dateOptions = {
    //    showWeeks: false

    //};

    //// DT Picker: Values used by. . .
    //$scope.dateOpened = false;
    //$scope.hourStep = 1;
    //$scope.format = "dd-MMM-yyyy";
    //$scope.minuteStep = 1;
    //$scope.showMeridian = false; // Default: False, Cannot put in dateOptions.

    //// DT Picker: On open/click
    //$scope.open = function ($event, opened) {
    //    $event.preventDefault();
    //    $event.stopPropagation();
    //    $scope.dateOpened = true;
    //};

    //// On afterThisDate change
    //$scope.$watch("afterThisDate", function (newDate) {
    //    // read date value
    //    //// LOG
    //    //console.log("newEventStart: " + newEventStart);
    //}, true);


    // Get FutureEvents
    $scope.defaultObj.getFuture = function () {
        $scope.list_Info.paging.page = 1;
        $scope.search_Params.afterThisDate = dateNow;
        $scope.search_Params.beforeThisDate = null;
        loadEvents();
    };

    // Get PastEvents
    $scope.defaultObj.getPast = function () {
        $scope.list_Info.paging.page = 1;
        $scope.search_Params.afterThisDate = null;
        $scope.search_Params.beforeThisDate = dateNow;
        loadEvents();
    };

    // Select Page
    $scope.selectPage = function () {
        loadEvents();
    };

    // SortBy
    $scope.sort = function (sortBy) {
        if (sortBy === $scope.list_Info.paging.sortBy) {
            $scope.list_Info.paging.reverse = !$scope.list_Info.paging.reverse;
        } else {
            $scope.list_Info.paging.sortBy = sortBy;
            $scope.list_Info.paging.reverse = false;
        }
        $scope.list_Info.paging.page = 1;
        loadEvents();
    };

    // Request: GetEvents
    function loadEvents() {
        var queryObj = {};
        //queryObj.afterThisDate = date;
        angular.extend(queryObj, $scope.search_Params, $scope.list_Info.paging);
        $scope.list_Info.events = null;

        eventsService.getUserEvents(queryObj)
        .then(function (response) {
            $scope.list_Info.events = response.data.events;
            $scope.list_Info.paging.totalItems = response.data.count;
            $scope.list_Info.userId = response.data.userId;
        });
    };

    // Initial load if needed
    loadEvents();
    //if (!$scope.list_Info.events) {
    //    loadEvents();
    //};
});
