/*jslint white: true */
'use strict';
angular.module("eventApp")
.controller('profileCtrl', ['$scope', 'Upload', '$timeout', 'userInfo', 'usersService', '$log', function ($scope, Upload, $timeout, userInfo, usersService, $log) {
    // Sample service endpoint for storage emulator: http://127.0.0.1:10000/devstoreaccount1/secondcontainer/52.jpg

    //$scope.$watch('files', function () {
    //    // If $scope.files changes, it uploads $scope.files.
    //    $scope.upload($scope.files);
    //});
    //$scope.$watch('file', function () {
    //    $log.debug($scope.file);
    //    // If file $scope.file changes to != null
    //    // Set $scope.files = an array containing $scope.file
    //    // This will change $scope.files, and upload $scope.files.
    //    //if ($scope.file != null) {
    //    //    $scope.files = [$scope.file];
    //    //}
    //});

    //$scope.$watch('userInfo.aboutMe', function () {
    //    // If $scope.files changes, it uploads $scope.files.
    //    $log($scope.userInfo.aboutMe)
    //});


    $scope.log = '';
    // Set userInfo to resolved value
    $scope.userInfo = userInfo;

    $scope.editAbout = function (data) {
        usersService.editAboutme(data.aboutText)
        .then(function (response) {
            userInfo.aboutMe = data.aboutText;
            //userInfo.aboutMe = response.data;
            $log.debug(response);
        });
    }

    $scope.fileDropped = function (fileD, filesD, eventD, rejectedFilesD) {
        $log.debug('fileD below:');
        $log.debug(fileD);
        $log.debug('filesD below:');
        $log.debug(filesD);
        $log.debug('eventD below:');
        $log.debug(eventD);
        $log.debug('rejectedFilesD below:');
        $log.debug(rejectedFilesD);
        $log.debug('$scope.file below:');
        $log.debug($scope.file);
    };


    $scope.upload = function (file) {

        if (file && !file.$error) {
            var isDefault = false
            Upload.upload({
                url: '/blobs/uploadUserImage/',
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
                    $scope.userInfo.imageUrl = response.data[0].fileUrl;
                    $log.debug(response.data[0].fileUrl);
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
}]);


/*jslint white: true */
//'use strict';
//angular.module("uploadModule")
//.controller('uploadCtrl', ['$scope', 'Upload', '$timeout', function ($scope, Upload, $timeout) {
//    $scope.upload = function (dataUrl, name) {
//        Upload.upload({
//            url: '/blobs/upload/',
//            data: {
//                file: Upload.dataUrltoBlob(dataUrl, name)
//            }
//        }).then(function (response) {
//            $timeout(function () {
//                $scope.result = response.data;
//            });
//        }, function (response) {
//            if (response.status > 0) {
//                $scope.errorMsg = response.status
//                + ': ' + response.data;
//            }
//        }, function (evt) {
//            $scope.progress = parseInt(100.0 * evt.loaded / evt.total);
//        });
//    };
//}]);