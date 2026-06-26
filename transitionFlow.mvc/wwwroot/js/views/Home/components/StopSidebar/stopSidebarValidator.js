window.StopSidebarValidator = {
    stopFormRules: {
        rules: {
            name: {
                required: true
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
                required: "Будь ласка, введіть назву зупинки."
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