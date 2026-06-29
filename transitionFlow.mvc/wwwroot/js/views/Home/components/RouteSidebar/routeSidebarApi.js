(function (window, $) {
    'use strict';

    const RouteSidebarApi = {
        createRoute: function (routeData) {
            return $.ajax({
                url: '/routes',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(routeData)
            });
        },

        deleteRoute: function (routeId) {
            return $.ajax({
                url: `/routes/${routeId}`,
                type: 'DELETE'
            });
        }
    };

    window.RouteSidebarApi = RouteSidebarApi;
})(window, jQuery);