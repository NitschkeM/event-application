/*jslint white: true */

(function (angular) {
    'use strict';
    // 'ui.bootstrap.datetimepicker',
    // Remove ngResource
    angular.module("eventApp", ['ui.router', 'ngResource', 'ui.bootstrap', 'ngAnimate', 'xeditable', 'angular-loading-bar', 'authModule', 'ngFileUpload', 'ui.bootstrap.datetimepicker'])
        .constant("baseUrl", "/api/events/") // Is probably not used anymore
        .constant("commentUrl", "/api/comments/") // Can probably remove
        .run(function (editableOptions) {
            editableOptions.theme = "bs3";
        })
        .config(function ($httpProvider) {
            $httpProvider.interceptors.push('authInterceptorService');
        });

    //.run(function (editableOptions, $anchorScroll, $window) {
    //    editableOptions.theme = "bs3";
    //    // hack to scroll to top when navigating to new URLS but not back/forward
    //    var wrap = function (method) {
    //        var orig = $window.window.history[method];
    //        $window.window.history[method] = function () {
    //            var retval = orig.apply(this, Array.prototype.slice.call(arguments));
    //            $anchorScroll();
    //            return retval;
    //        };
    //    };
    //    wrap('pushState');
    //    wrap('replaceState');
    //})
    //.config(function ($httpProvider, $logProvider) {
    //    $httpProvider.interceptors.push('authInterceptorService');
    //    $logProvider.debugEnabled(true);
    //});

}(window.angular));

// https://ng-perf.com/ 
// Angular’s watcher functions for ng-bind and text nodes ({{expression}}) put binding information inside the respective DOM elements using jQuery .data method. 
// This is unfortunately a very expensive operation that impacts both load times and time taken to delete nodes. 
    // Fortunately this information is not used in actual binding and is mostly for debugging purposes. 
        // If you are using 1.2, you can load the following snippet before your app.js loads and magically you will see load time reduced by as much as 50%!
//config(['$routeProvider', '$compileProvider', function($routeProvider, $compileProvider) {
//    //configure routeProvider as usual
//    $compileProvider.debugInfoEnabled(false);
//}]



// http://stackoverflow.com/questions/16589853/ng-app-vs-data-ng-app-what-is-the-difference

// The difference is simple - there is absolutely no difference between the two 
// except that certain HTML5 validators will throw an error on a property like ng-app, 
// but they don't throw an error for anything prefixed with data-, like data-ng-app.
//***********************************************************************************
// The differences lies in the fact that custom data-*attributes are valid in the HTML5 specification. 
// So if you need your markup to be validated, you should use them rather than the ng attributes.