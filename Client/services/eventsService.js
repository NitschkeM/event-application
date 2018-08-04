'use strict';

angular.module('eventApp')

.factory('eventsService', ['$http',
    function ($http) {
        var service = {};

        // Query events
        service.queryEvents = function (queryString) {
            return $http.get('/api/events/', { params: queryString });
        };


        // Get event by Id
        service.getEventById = function (eventId) {
            return $http.get('/api/event/' + eventId);
        };

        // Get userEvents
        service.getUserEvents = function (queryObj) {
            return $http.get('/api/userevents/', { params: queryObj });
        };

        // Get userEvents (Past)
        service.getPastEvents = function (queryObj) {
            return $http.get('/api/userevents/', { params: queryObj });
        };

        // Create event
        service.createEvent = function (newEvent) {
            return $http({
                method: "POST",
                url: "/api/event/",
                data: newEvent
            });
        }

        // Edit event
        service.editEvent = function (event) {
            return $http({
                method: "PUT",
                url: "/api/event/" + event.eventId,
                data: event
            });
        }

        // Delete event
        service.deleteEvent = function (eventId) {
            return $http.delete('/api/event/' + eventId);
        };

        // Join-Leave Event
        service.joinLeave = function (eventId, rShip) {
            return $http({
                method: "PUT",
                url: "/api/participate/" + eventId + "/" + rShip
            });
        }

        // Approve/Reject Pending
        service.actionOnPartic = function (eventId, particName, creatorAction) {
            return $http({
                method: "POST",
                url: "/api/handlepartic/" + eventId + "/" + particName + "/" + creatorAction
            });
        }

        // Change Event Status
        service.changeStatus = function (eventId, eventStatus) {
            return $http({
                method: "PUT",
                url: "/api/changestatus/" + eventId + "/" + eventStatus
            });
        }

        // Create Comment
        service.createComment = function (comment) {
            return $http({
                method: "POST",
                url: "/api/comment/",
                data: comment
            });
        }

        // Edit Comment
        service.editComment = function (comment) {
            return $http({
                method: "PUT",
                url: "/api/comment/" + comment.commentId,
                data: comment
            });
        }

        // Delete Comment
        service.deleteComment = function (commentId) {
            return $http.delete('/api/comment/' + commentId);
        }

        return service;
    }]);
