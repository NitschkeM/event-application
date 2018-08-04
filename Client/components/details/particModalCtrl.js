// Please note that $uibModalInstance represents a modal window (instance) dependency.
// It is not the same as the $uibModal service used above.

angular.module('eventApp')
    .controller('particModalCtrl', function ($scope, $uibModalInstance, eventsService, $stateParams, $log, participants, pending, userRship) {

        $scope.partic = {
            participants: participants,
            pending: pending,
            userRship: userRship
        };

        $scope.partic.creatorAction = function (user, creatorAction) {
            // Reject Pending
            if (creatorAction === 'reject') {
                var answer = confirm("Are you sure you want to reject the request?");
                if (answer === true) {
                    eventsService.actionOnPartic($stateParams.id, user.userName, creatorAction)
                        .then(function () {
                            $scope.partic.pending.splice($scope.partic.pending.indexOf(user), 1);
                        });
                }
            }
                // Accept Pending
            else if (creatorAction === 'accept') {
                eventsService.actionOnPartic($stateParams.id, user.userName, creatorAction)
                    .then(function () {
                        $scope.partic.pending.splice($scope.partic.pending.indexOf(user), 1);
                        $scope.partic.participants.push(user);

                    });
            }
                // Remove Participant
            else if (creatorAction === 'remove') {
                var answer = confirm("Are you sure you want to reject the request?");

                if (answer === true) {
                    eventsService.actionOnPartic($stateParams.id, user.userName, creatorAction)
                        .then(function () {
                            $scope.partic.participants.splice($scope.partic.participants.indexOf(user), 1);
                        });
                }
            }
                // Catch.. (So above to case?) 
            else {
                $log.debug('Something is wrong, creatorAction is not recognized.')
            }
        }

        $scope.ok = function () {
            $uibModalInstance.close($scope.partic.participants, $scope.partic.pending);
        };

        //$scope.cancel = function () {
        //    $uibModalInstance.dismiss('cancel');
        //};

    });

//angular.module('eventApp')
//    .controller('particModalCtrl', function ($scope, $uibModalInstance, $log, participants) {

//        $scope.participants = participants;
//        //$scope.pending = pending;
//        //$log.debug($scope.pending);

//        //$scope.selected = {
//        //    userName: $scope.items[0].userName
//        //};

//        $scope.remove = function (user) {
//            $scope.participants.splice($scope.participants.indexOf(user), 1);
//        };


//        $scope.ok = function () {
//            $uibModalInstance.close($scope.participants);
//        };

//        $scope.cancel = function () {
//            $uibModalInstance.dismiss('cancel');
//        };

//    });
