// Please note that $uibModalInstance represents a modal window (instance) dependency.
// It is not the same as the $uibModal service used above.

angular.module('eventApp')
    .controller('advSearchCtrl', function ($scope, $uibModalInstance, sParamsCopy, tags, $timeout, $log) {

        // ****************I HAVE NOT TESTED IF THIS CTRL HAS ACCESS TO LIST SCOPE *********************
        // Not using scope allows for: 
        // Confirm - Cancel (change of mind)
        // ---Earlier I explicitly set $scope as scope for advanced search modal.
        // This was before I gave it a Ctrl
        // *********************************************************************************************

        $scope.advObj = {
            searchParams: sParamsCopy,
            tags: tags,
            tagsCopy: angular.copy(tags),
            btnActive: {},
            btnClass:{}
            //isDefined: {
            //    gender: angular.isDefined($scope.advObj.searchParams.gender)
            //}
        };

        //$scope.$watch('advObj.searchParams.gender', function () {
        //    $scope.advObj.isDefined.gender = angular.isDefined($scope.advObj.searchParams.gender),
        //    $log.debug($scope.searchParams.gender);
        //});

        // ********************* $scope.advObj.btnActive.Param != Param*****************************
        function trueFalse(param) {
            $scope.advObj.btnActive[param] = angular.isDefined($scope.advObj.btnActive[param]) ? $scope.advObj.btnActive[param] : false;
            $scope.advObj.btnActive[param] = !$scope.advObj.btnActive[param];
        };

        $scope.advObj.btnColorFunc = function (param, fromBtn) {
            //$log.debug(param, fromBtn);
            if (fromBtn) {
                trueFalse(param);
                //$log.debug('Line that runs after trueFalse call');
            }
            //if ($scope.advObj.searchParams[param] || $scope.advObj.searchParams[param] === 0) { // I taught ageMin === 0 would eval to false with other, but guess not? ..
            if ($scope.advObj.searchParams[param] && $scope.advObj.searchParams[param].length !== 0) { // I taught ageMin === 0 would eval to false with other, but guess not? ..
                $scope.advObj.btnClass[param] = 'btn-success';
                $scope.advObj.btnActive[param] = true;
            }
            else {
                $scope.advObj.btnClass[param] = $scope.advObj.btnActive[param] ? 'btn-warning' : 'btn-primary';
            }
            //$log.debug($scope.advObj.btnClass);
        }
        
        $scope.advObj.btnColorFunc('ageMin', false);
        $scope.advObj.btnColorFunc('ageMax', false);
        $scope.advObj.btnColorFunc('cPartMin', false);
        $scope.advObj.btnColorFunc('cPartMax', false);
        $scope.advObj.btnColorFunc('approvalReq', false);
        $scope.advObj.btnColorFunc('gender', false);
        $scope.advObj.btnColorFunc('tags', false);
        $scope.advObj.btnColorFunc('dateAfter', false);
        $scope.advObj.btnColorFunc('dateBefore', false);
        $scope.advObj.btnColorFunc('timeAfter', false);
        $scope.advObj.btnColorFunc('timeBefore', false);
        //$scope.advObj.trueFalse = function (value) {
        //    $scope.advObj.btnActive[value] = angular.isDefined($scope.advObj.btnActive[value]) ? $scope.advObj.btnActive[value] : false;
        //    $scope.advObj.btnActive[value] = !$scope.advObj.btnActive[value];
        //};
        // ********************* is value defined? *****************************
        //$scope.advObj.isDefined = function (value) {
        //    return angular.isDefined(value);
        //};

        // ************************* Tags *********************************

        $scope.advObj.addTag = function (tag) {
            $scope.advObj.searchParams.tags.push(tag.name);
            $scope.advObj.tags.splice($scope.advObj.tags.indexOf(tag), 1);

            $log.debug($scope.advObj.tags);
            $log.debug($scope.advObj.searchParams.tags);
        }

        $scope.advObj.addCustom = function (customTag) {
            var newTag = { name: customTag };
            $scope.advObj.searchParams.tags.push(newTag.name);
            $scope.advObj.customTag = null;

            $log.debug($scope.advObj.searchParams.tags);
        };

        $scope.advObj.removeTag = function (tagName) {
            $scope.advObj.searchParams.tags.splice($scope.advObj.searchParams.tags.indexOf(tagName), 1);

            var existingTag = $scope.advObj.tagsCopy.find(function (tag) {
                return tag.name === tagName;
            });
            if (existingTag) {
                $scope.advObj.tags.push(existingTag);
            }

            $log.debug($scope.advObj.tags);
            $log.debug($scope.advObj.searchParams.tags);
        };

        // ****************************** Date-Time Picker******************************************
        
        // If date/time exists, initialize pickers, else null.
        $scope.advObj.searchParams.dateAfter = $scope.advObj.searchParams.dateAfter ? new Date($scope.advObj.searchParams.dateAfter) : null;
        $scope.advObj.searchParams.dateBefore = $scope.advObj.searchParams.dateBefore ? new Date($scope.advObj.searchParams.dateBefore) : null;
        $scope.advObj.searchParams.timeAfter = $scope.advObj.searchParams.timeAfter ? new Date($scope.advObj.searchParams.timeAfter) : null;
        $scope.advObj.searchParams.timeBefore = $scope.advObj.searchParams.timeBefore ? new Date($scope.advObj.searchParams.timeBefore) : null;
        // UI-Bootstrap docs: If the model value is updated (i.e. via Date.prototype.setDate), 
        // you must update the model value by breaking the reference by modelValue = new Date(modelValue) in order to have the timepicker update.
        // Disable picker will set value to null, and be valid.

        $scope.afDatePicker = {
            dateOptions: {
                minDate: new Date()
            }
        }
        $scope.beDatePicker = {
            dateOptions: {
                //minDate: Later than afDate if exists
            }
        }
        $scope.afTimePicker = {
            timeOptions: {
                //readonlyInput: false,
                isMeridian: false,
                showSpinners: false
            }
        };
        $scope.beTimePicker = {
            timeOptions: {
                //readonlyInput: false,
                isMeridian: false,
                showSpinners: false
                //min: Later than afTime if exists
            }
        };

        $scope.openCalendar = function (picker) {
            //$scope.picker.isOpen = true;
            $scope[picker].isOpen = true;
        };


        // ****************************** ClearParams - Search - Cancel ******************************************
        $scope.clearParams = function () {
            var eligibleTemp = $scope.advObj.searchParams.eligibleOnly;
            $scope.advObj.searchParams = {};
            $scope.advObj.searchParams.tags = [];
            $scope.advObj.searchParams.eligibleOnly = eligibleTemp;
            $scope.advObj.btnActive = {};

            $scope.advObj.btnClass.ageMin = 'btn-primary';
            $scope.advObj.btnClass.ageMax = 'btn-primary';
            $scope.advObj.btnClass.cPartMin = 'btn-primary';
            $scope.advObj.btnClass.cPartMax = 'btn-primary';
            $scope.advObj.btnClass.approvalReq = 'btn-primary';
            $scope.advObj.btnClass.gender = 'btn-primary';
            $scope.advObj.btnClass.tags = 'btn-primary';
            $scope.advObj.btnClass.dateAfter = 'btn-primary';
            $scope.advObj.btnClass.dateBefore = 'btn-primary';
            $scope.advObj.btnClass.timeAfter = 'btn-primary';
            $scope.advObj.btnClass.timeBefore = 'btn-primary';
        }
        $scope.search = function () {
            //$uibModalInstance.close($scope.selected.userName);
            $uibModalInstance.close($scope.advObj.searchParams);
        };

        $scope.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };
    });



////********************* Gender Btns *********************
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



////********************* Date-Time_Picker *********************

//// DT Picker: Options
//$scope.dateOptions = {
//    showWeeks: false,
//    minDate: new Date()
//};
//// DT Picker: On open/click
//$scope.open = function ($event, opened) {
//    $event.preventDefault();
//    $event.stopPropagation();
//    $scope.dateOpened = true;
//};
//// DT Picker: Values used by. . .
//$scope.dateOpened = false;
//$scope.hourStep = 1;
//$scope.format = "dd-MMM-yyyy";
//$scope.minuteStep = 1;
//$scope.showMeridian = false;

//// On searchParams.eventStart change
//$scope.$watch("searchParams.eventStart", function (newEventStart) {
//    // read date value
//    //// LOG
//    //console.log("newEventStart: " + newEventStart);
//}, true);
////********************* Date-Time_Picker - END *********************