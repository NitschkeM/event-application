/*jslint white: true */

angular.module("eventApp")
.controller("detailsCtrl", function ($scope, $stateParams, $location, $http, $state, $filter, data, eventsService, $uibModal, $log) {
    // Dependencies copied from editCtrl
    // $uibModal & log, copied from modalEx Ctrl

    'use strict';
    $scope.userRship = null; // Probably not needed
    $scope.userRship = data.userRship; // New Rship implementation
    $scope.currentEvent = data.event;
    $scope.modal = {};

    var map = {};
    var marker = {};
    var myLatLng = {
        lat: $scope.currentEvent.posLat,
        lng: $scope.currentEvent.posLng
    };

    // ************************** COMMENT AND REPLY ************************

    // **New Comment**
    $scope.postComment = function (comment) {
        comment.postedTime = new Date();
        comment.eventId = $stateParams.id;
        comment.parentId = null;

        eventsService.createComment(comment)
        .then(function (response) {
            console.log("response");
            $scope.currentEvent.comments.push(response.data);
            $scope.comment = {};
        });
    };

    // **New Reply**
    $scope.postReply = function (reply, parentId) {
        reply.postedTime = new Date();
        reply.eventId = $stateParams.id;
        reply.parentId = parentId;

        eventsService.createComment(reply)
        .then(function (response) {
            console.log("response");

            // find() returns the value of the first element passing the test. 
            // item passed to the function is: the "currentValue". 
            // (I THINK it's the current element being scanned).
            var parent = $scope.currentEvent.comments.find(function (item) {
                return item.commentId === parentId;
            });
            parent.replies.push(response.data);
        });
    };

    // **Delete Comment**
    $scope.deleteComment = function (comment) {
        var answer = confirm("Are you sure you want to delete the comment?");
        if (answer) {
            eventsService.deleteComment(comment.commentId)
            .then(function () {
                $scope.currentEvent.comments.splice($scope.currentEvent.comments.indexOf(comment), 1);
            });
        }
    };

    // **Delete Reply**
    $scope.deleteReply = function (comment, parent) {
        var answer = confirm("Are you sure you want to delete the comment?");
        if (answer) {
            eventsService.deleteComment(comment.commentId)
            .then(function () {
                var indexOfParent = $scope.currentEvent.comments.indexOf(parent);
                $scope.currentEvent.comments[indexOfParent].replies.splice($scope.currentEvent.comments[indexOfParent].replies.indexOf(comment), 1);
                //parent.replies.splice(parent.replies.indexOf(comment), 1); - Did not work, probably manipulates item, not currentEvent.Lists.
            });
        }
    };

    // **Edit Comment**
    $scope.editComment = function (textObj, commentId) {
        // find() returns the value of the first element passing the test. item passed to the function is: the "currentValue" - I THINK it's the current element being scanned.
        var comment = $scope.currentEvent.comments.find(function (item) {
            return item.commentId === commentId;
        });
        comment.commentText = textObj.commentText; // Updates view as well.

        eventsService.editComment(comment);
    };


    // **Edit Reply**
    $scope.editReply = function (textObj, commentId, parent) {
        // Find comment
        var comment = parent.replies.find(function (item) {
            return item.commentId === commentId;
        });
        comment.commentText = textObj.commentText; // Updates view as well.

        eventsService.editComment(comment);
    };



    $scope.modal.open = function (pending, size) {
        if (pending) {
            pendingModal(pending, size);
        }
        else {
            participantModal(pending, size);
        }
    }

    // ************************ TIME-SINCE-FUNC ***********************
    $scope.timeSince = function (date) {

        var seconds = Math.floor((new Date() - new Date(date)) / 1000);

        var interval = Math.floor(seconds / 31536000);

        if (interval > 1) {
            return interval + " years";
        }
        interval = Math.floor(seconds / 2592000);
        if (interval > 1) {
            return interval + " months";
        }
        interval = Math.floor(seconds / 86400);
        if (interval > 1) {
            return interval + " days";
        }
        interval = Math.floor(seconds / 3600);
        if (interval > 1) {
            return interval + " hours";
        }
        interval = Math.floor(seconds / 60);
        if (interval > 1) {
            return interval + " minutes";
        }
        return Math.floor(seconds) + " seconds";
    }

    // ************************** DeleteEvent, OpenClose Event, and JoinLeave Event *************************
    $scope.deleteEvent = function () {
        var answer = confirm("Are you sure you want to delete the event?");
        if (answer) {
            eventsService.deleteEvent($scope.currentEvent.eventId)
            .then(function () {
                $state.go('mainParent.frontpage');
            })
        }
        //**********************************************************
        // Set currentEvent = null
        // Redirect user / inform of success somehow
        // Redirect to a new "Info view?" Your event was sussessfully deleted? 
        //**********************************************************
    };
    // JoinLeave
    $scope.joinLeave = function () {
        if ($scope.userRship === 'participant' || $scope.userRship === 'pending') {
            var answer = confirm("Are you sure you want to leave the event?");
            if (answer) {
                eventsService.joinLeave($stateParams.id, $scope.userRship)
                .then(function (response) {
                    $scope.userRship = response.data;
                });
            }
        }
        else {
            eventsService.joinLeave($stateParams.id, $scope.userRship)
                .then(function (response) {
                    $scope.userRship = response.data;
                });
        }
    }
    // Change Event Status
    $scope.changeStatus = function (eventStatus) {
        //var isOpen = !$scope.currentEvent.open;
        eventsService.changeStatus($stateParams.id, eventStatus)
        .then(function (response) {
            $scope.currentEvent.eventStatus = response.data;
            $log.debug("$scope.currentEvent.eventStatus:");
            $log.debug($scope.currentEvent.eventStatus);
            $log.debug("response.data");
            $log.debug(response.data);
        });
    }

    // ****************************** MAP ******************************

    // Create + Render map
    map = new google.maps.Map(document.getElementById('mapD'), {
        //mapOptions
        zoom: 12,
        center: myLatLng
    });

    // Create + Render marker
    marker = new google.maps.Marker({
        position: myLatLng,
        map: map,
        draggable: false
    });


    // ***************************** Participant Modal ***************************
    //$scope.modal.open = function (size) {
    function participantModal(pending, size) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Client/components/details/particiModal.html',
            controller: 'particModalCtrl',
            size: size,
            resolve: {
                participants: function () {
                    return $scope.currentEvent.participants;
                },
                pending: function () {
                    return $scope.currentEvent.pending;
                },
                userRship: function () {
                    return $scope.userRship;
                }
            }
        });

        modalInstance.result.then(function () {
            $log.info('Modal Closed');
            $scope.currentEvent.numberOfParticipants = $scope.currentEvent.participants.length;
        }, function () {
            $log.info('Modal Dismissed');
            $scope.currentEvent.numberOfParticipants = $scope.currentEvent.participants.length;
        });
    };

    //// ***************************** Pending Modal ***************************
    //function pendingModal(pending, size) {
    //    var modalInstance = $uibModal.open({
    //        animation: true,
    //        templateUrl: 'Client/components/details/particiModal.html',
    //        controller: 'pendingModalCtrl',
    //        size: size,
    //        resolve: {
    //            items: function () {
    //                return $scope.currentEvent.pending;
    //            },
    //            userRship: function () {
    //                return $scope.userRship;
    //            }
    //        }
    //    });

    //    modalInstance.result.then(function (selectedUserName) {
    //        $scope.selectedUserName = selectedUserName;
    //    }, function () {
    //        $log.info('Modal dismissed at: ' + new Date());
    //    });
    //};


    //// ***************************** Modal scope TEST ***************************
    ////$scope.modal.open = function (size) {
    //function participantModal(pending, size) {
    //    var modalInstance = $uibModal.open({
    //        animation: true,
    //        templateUrl: 'Client/components/details/particiModal.html',
    //        controller: 'particModalCtrl',
    //        size: size,
    //        resolve: {
    //            participants: function () {
    //                return $scope.currentEvent.participants;
    //            }
    //        }
    //    });

    //    modalInstance.result.then(function (participants) {
    //        $log.debug("returned from modal below");
    //        $log.debug(participants);
    //        $log.debug("$scope.currentEvent.participants below");
    //        $log.debug($scope.currentEvent.participants);

    //    }, function () {
    //        $log.info('Modal dismissed at: ' + new Date());
    //    });
    //};

});
