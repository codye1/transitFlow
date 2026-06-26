window.RouteSidebarValidator = {
    initRouteCustomRules: function () {
        if (typeof $.validator !== "undefined" && !$.validator.methods.minStops) {
            $.validator.addMethod("minStops", function (value, element, minCount) {
                const listSelector = $(element).data('stops-list-target') || '#selected-stops-list';
                const stopsCount = $(listSelector).find('.selected-stop-item').length;
                return stopsCount >= minCount;
            }, "Необхідно додати мінімум {0} зупинки.");
        }
    },

    routeFormRules: {
        rules: {
            number: {
                required: true
            },
            name: {
                required: true
            },
            color: {
                required: true
            }
        },
        messages: {
            number: {
                required: "Будь ласка, введіть номер маршруту."
            },
            name: {
                required: "Будь ласка, введіть назву маршруту."
            },
            color: {
                required: "Оберіть колір мітки."
            }
        }
    }
};