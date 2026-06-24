// Global configuration to always attach the Bearer token to API requests

const API_BASE_URL = "https://localhost:7094";

$.ajaxPrefilter(function (options, originalOptions, jqXHR) {
    if (options.url.startsWith('/')) {
        options.url = API_BASE_URL + options.url;
    }
});

$.ajaxSetup({
    beforeSend: function (xhr) {
        const token = localStorage.getItem("accessToken");
        if (token) {
            xhr.setRequestHeader("Authorization", "Bearer " + token);
        }
    }
});

// The Interceptor: Catches 401 errors globally and handles refreshing
$(document).ajaxError(function (event, jqXHR, ajaxSettings, thrownError) {
    if (jqXHR.status !== 401 || ajaxSettings.url.includes('/auth/login') || ajaxSettings.url.includes('/auth/refresh')) {
        return;
    }

    const deferred = $.Deferred();

    $.ajax({
        url: '/auth/refresh',
        type: 'POST',
        contentType: 'application/json',
        xhrFields: {
            withCredentials: true 
        },
        success: function (response) {
            localStorage.setItem("accessToken", response.accessToken);

            ajaxSettings.headers = ajaxSettings.headers || {};
            ajaxSettings.headers["Authorization"] = "Bearer " + response.accessToken;

            $.ajax(ajaxSettings).done(deferred.resolve).fail(deferred.reject);
        },
        error: function () {
            localStorage.removeItem("accessToken");
            window.location.href = '/login';
            deferred.reject();
        }
    });

    return deferred.promise();
});