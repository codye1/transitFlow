const vehicleSidebarApi = {
    createVehicle: function (vehicleData) {
        return $.ajax({
            url: '/vehicles',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(vehicleData)
        });
    },

    updateRoute: function (vehicleId, routeId) {
        return $.ajax({
            url: `/vehicles/${vehicleId}`,
            method: 'PATCH',
            contentType: 'application/json',
            data: JSON.stringify({
                routeId: routeId ? parseInt(routeId, 10) : null
            })
        });
    },

    updateStatus: function (vehicleId, status) {
        return $.ajax({
            url: `/vehicles/${vehicleId}`,
            method: 'PATCH',
            contentType: 'application/json',
            data: JSON.stringify({
                status: status
            })
        });
    },

    deleteVehicle: function (vehicleId) {
        return $.ajax({
            url: `/vehicles/${vehicleId}`,
            method: 'DELETE'
        });
    }
};

export default vehicleSidebarApi;