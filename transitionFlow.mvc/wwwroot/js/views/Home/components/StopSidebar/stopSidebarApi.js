const stopSidebarApi = {
    createStop: function (stopData) {
        return $.ajax({
            url: '/stops',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(stopData),
            dataType: 'html'
        });
    },

    updateStop: function (stopId, updateData) {
        return $.ajax({
            url: `/stops/${stopId}`,
            type: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(updateData)
        });
    },

    deleteStop: function (stopId) {
        return $.ajax({
            url: `/stops/${stopId}`,
            type: 'DELETE'
        });
    }
};

export default stopSidebarApi;