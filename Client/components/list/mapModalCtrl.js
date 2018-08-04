// Please note that $uibModalInstance represents a modal window (instance) dependency.
// It is not the same as the $uibModal service used above.

// $scope.$watch() should replace the need for events

angular.module('eventApp')
    .controller('mapModalCtrl', function ($scope, $uibModalInstance, items, $timeout) {

        // ****************I HAVE NOT TESTED IF THIS CTRL HAS ACCESS TO LIST SCOPE *********************
        // Not using scope allows for: 
        // Confirm - Cancel (change of mind)
        // ---Earlier I explicitly set $scope as scope for advanced search modal.
        // This was before I gave it a Ctrl

        var map = {};
        var circle = {};


        var timer = $timeout(function () {
            if (items.posLat && items.posLng) {
                renderMap();
            }
            else {
                renderGeneral();
            }
        }, 500);

        function renderGeneral() {
            // LOG
            console.log("renderGeneral");
            items.posLat = 59;
            items.posLng = 10;
            renderMap(5);
            // Inform user
        };

        function renderMap(zoom) {
            // LOG
            console.log("renderMap has been called");
            // LatLng object = searchParams
            var myLatLng = new google.maps.LatLng(items.posLat, items.posLng);

            // Initialize/Render map
            map = new google.maps.Map(document.getElementById('map'), {
                // mapOptions
                zoom: (zoom ? zoom : 8),
                center: myLatLng
            });

            // Initialize circle
            circle = new google.maps.Circle({
                center: myLatLng,
                radius: items.radius,
                editable: true,

                strokeColor: '#FF0000',
                strokeOpacity: 0.8,
                strokeWeight: 2,
                fillColor: '#FF0000',
                fillOpacity: 0.10,

                map: map // Render circle
            });

            // Listener: Circle_Position_Change
            circle.addListener('center_changed', function () {

                // set searchParam properties to circle position
                items.posLat = circle.getCenter().lat();
                items.posLng = circle.getCenter().lng();

                // LOG: Circle_Position_Change
                console.log("Circle_Position_Change, variables below: ");
                console.log("position.posLat: " + items.posLat);
                console.log("position.posLng: " + items.posLng);
            });

            // Listener: Circle_Radius_Change
            circle.addListener('radius_changed', function () {

                // set searchParam radius property to circle radius
                items.radius = circle.getRadius();

                // LOG: Circle_Radius_Change
                console.log("position.radius: " + items.radius + " meters");
            });

        };
        // Resize map on window resize
        google.maps.event.addDomListener(window, "resize", function () {
            var center = map.getCenter(); // Change to posLat-posLng? 
            google.maps.event.trigger(map, "resize");
            map.setCenter(center);
        });


        $scope.ok = function () {
            //$uibModalInstance.close($scope.selected.userName);
            $uibModalInstance.close(items);
        };

        $scope.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };
    });




//// Only used by "showMap Button"
//$scope.mapFunc = {
//    //render: renderMap,
//    resize: function () {
//        var timer = $timeout(function () {
//            var center = map.getCenter(); // Change to posLat-posLng? 
//            google.maps.event.trigger(map, 'resize');
//            map.setCenter(center);
//        }, 500)
//    }
//}


//// CurrentLocation Button calls:
//$scope.getPos = function () {
//    if (navigator.geolocation) {
//        navigator.geolocation.getCurrentPosition(setCircleAndMapCenter, positionError);
//    } else {
//        // No browser support
//        // LOG
//        console.log("Geolocation is not supported by this browser.");
//    }
//};