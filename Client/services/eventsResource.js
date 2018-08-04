angular.module("eventApp")
.factory("eventsResource", function ($resource, baseUrl) {
    // ** This service is beeing gradually removed from the application **

    // "@-EventId" -> value will be extracted from data object passed when calling method.
    return $resource(baseUrl + ":id", { id: "@eventId" },
        {
            create: { method: "POST" },
            save: { method: "PUT" },
            query: { method: "GET", params: {}, isArray: false}
        });

});





//(function (eventApp) {
//    'use strict';


//    eventApp.factory('eventService', function ($q, $http, $resource, baseUrl) {

//        return {
//            getAllEvents: getAllEvents
//        };

//        function getAllEvents() {
//            return $http({
//                method: 'GET',
//                url: baseUrl
//            })
//            .then(sendResponseData)
//            .catch(sendGetEventsError)
//        }

//        function sendResponseData(response) {
//            return response.data;
//        }

//        function sendGetEventsError(response) {
//            return $q.reject('Error: ' + response.status);
//        }


//    });
//}(angular.module("eventApp")));