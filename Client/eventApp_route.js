
(function (eventApp) {

    var config = function ($stateProvider, $locationProvider, $urlRouterProvider) {

        $locationProvider.html5Mode(true);

        $stateProvider
            // mainParent
            .state('mainParent', {
                url: '/main',
                abstract: true,
                templateUrl: '/Client/components/mainParent/mainParent.html'
            })

            // frontpage (child)
            .state('mainParent.frontpage', {
                url: '/frontpage',
                templateUrl: '/Client/components/frontPage/frontView.html',
                controller: 'frontPageCtrl'
            })

            // list (child)
            .state('mainParent.list', {
                url: '/list',
                templateUrl: '/Client/components/list/listView.html',
                controller: 'listCtrl'
            })

            // userList (child)
            .state('mainParent.userList', {
                url: '/userList',
                templateUrl: '/Client/components/userSpecific/userListView.html',
                controller: 'userListCtrl'
            })

            // create (child)
            .state('mainParent.create', {
                url: '/create',
                templateUrl: '/Client/components/edit/editView.html',
                resolve: {
                    data: function () {
                        return null;
                    },
                    defaultImgs: function ($http) {
                        return $http.get('api/defaultimgs/')
                        .then(function (resp) {
                            return resp.data.defaultImgs;
                        });
                    },
                    tags: function ($http) {
                        return $http.get('api/tags/')
                        .then(function (resp) {
                            return resp.data.tags;
                        });
                    }
                },
                controller: 'editCtrl'
            })
            // edit (child)
            .state('mainParent.edit', {
                url: '/edit/{id}',
                templateUrl: '/Client/components/edit/editView.html',
                resolve: {
                    data: function ($http, $stateParams) {
                        // Get event by Id
                        return $http.get('/api/event/' + $stateParams.id)
                        .then(function (resp) {
                            return resp.data;
                        });
                    },
                    defaultImgs: function ($http) {
                        return $http.get('api/defaultimgs/')
                        .then(function (resp) {
                            return resp.data.defaultImgs;
                        });
                    },
                    tags: function ($http) {
                        return $http.get('api/tags/')
                        .then(function (resp) {
                            return resp.data.tags;
                        });
                    }
                },
                controller: 'editCtrl'
            });

        // details
        $stateProvider.state('details', {
            url: '/details/{id}',
            templateUrl: '/Client/components/details/detailsView.html',
            resolve: {
                data: function ($http, $stateParams) {
                    // Get event by Id
                    return $http.get('/api/event/' + $stateParams.id)
                    .then(function (resp) {
                        return resp.data;
                    });
                }
            },
            controller: 'detailsCtrl'
        });

        // profile
        $stateProvider.state('profile', {
            url: '/profile',
            templateUrl: '/Client/components/userSpecific/profileView.html',
            resolve: {
                userInfo: function (usersService) {
                    return usersService.getUserInfo()
                    .then(function (response) {
                        console.log(response);
                        console.log(response.data);
                        return response.data;
                    });
                }
            },
            controller: 'profileCtrl'
        });

        // upload
        $stateProvider.state('upload', {
            url: '/upload',
            templateUrl: '/Client/fileUploadModule/uploadView.html',
            controller: 'uploadCtrl'
        });

        // login
        $stateProvider.state('login', {
            url: '/login',
            controller: 'loginCtrl',
            templateUrl: '/Client/authModule/views/loginView.html'
        });

        // signup
        $stateProvider.state('signup', {
            url: '/signup',
            controller: 'signupCtrl',
            templateUrl: '/Client/authModule/views/signupView.html'
        });

        // about
        $stateProvider.state('about', {
            url: '/about',
            templateUrl: '/Client/components/about_contact/aboutView.html',
            controller: 'aboutContactCtrl'
        });

        // contact
        $stateProvider.state('contact', {
            url: '/contact',
            templateUrl: '/Client/components/about_contact/contactView.html',
            controller: 'aboutContactCtrl'
        });

        // admin
        $stateProvider.state('admin', {
            url: '/admin',
            templateUrl: '/Client/components/admin/adminView.html',
            controller: 'adminCtrl'
        });

        // empty **** Probably not needed? ****
        $stateProvider.state('empty', {
            url: '',
            templateUrl: '/Client/components/frontPage/frontView.html',
            controller: 'frontPageCtrl'
        });

        $urlRouterProvider.otherwise('/main/frontpage');
        //$routeProvider.otherwise({
        //    redirectTo: '/list'
        //});

    };

    eventApp.config(config);

}(angular.module('eventApp')));


//Errors happen in no-man’s-land
//You have a resolve that makes an AJAX $http call. Eventually it will fail. Where will you handle the error? You don’t have a controller yet to manage things at this point.

//Make sure to either have some generic error handling or make your resolves always return some value, even on errors, and then handle those situations in your controller.