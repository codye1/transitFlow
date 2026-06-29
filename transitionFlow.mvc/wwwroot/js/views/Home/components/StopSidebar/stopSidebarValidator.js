
(function (window) {
    'use strict';

    const StopSidebarValidator = {
        stopFormRules: {
            rules: {
                name: {
                    required: true,
                    minlength: 3
                },
                latitude: {
                    required: true,
                    number: true
                },
                longitude: {
                    required: true,
                    number: true
                }
            },
            messages: {
                name: {
                    required: "Будь ласка, введіть назву зупинки.",
                    minlength: "Назва повинна містити мінімум 3 символи." 
                },
                latitude: {
                    required: "Координата широти обов'язкова.",
                    number: "Введіть коректне число."
                },
                longitude: {
                    required: "Координата довготи обов'язкова.",
                    number: "Введіть коректне число."
                }
            }
        }
    };

    window.StopSidebarValidator = StopSidebarValidator;
})(window);