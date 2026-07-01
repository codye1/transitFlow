const routeSidebarValidator = {
    initRouteCustomRules: function () {
        if (typeof $.validator !== "undefined" && !$.validator.methods.minStops) {
            $.validator.addMethod("minStops", function (value, element, minCount) {
                const listSelector = $(element).data('stops-list-target') || '#selected-stops-list';
                console.log($(listSelector).find('.selected-stop-item').length);
                return $(listSelector).find('.selected-stop-item').length >= minCount;
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
            },
            selectedStops: {
                minStops: 2
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
            },
            selectedStops: {
                minStops: "Необхідно додати мінімум 2 зупинки."
            }
        }
    }
};

export default routeSidebarValidator;