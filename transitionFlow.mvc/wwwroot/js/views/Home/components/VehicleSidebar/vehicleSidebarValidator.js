(function (window) {
    'use strict';

    const VehicleSidebarValidator = {
        vehicleFormRules: {
            rules: {
                plateNumber: {
                    required: true,
                    minlength: 3,
                    maxlength: 15
                },
                type: {
                    required: true
                },
                model: {
                    required: true,
                    minlength: 2
                },
                capacity: {
                    required: true,
                    digits: true,
                    min: 1
                }
            },
            messages: {
                plateNumber: {
                    required: "Будь ласка, вкажіть держномер",
                    minlength: "Номер повиннен мати не менше 3 символів"
                },
                type: {
                    required: "Оберіть тип транспорту"
                },
                model: {
                    required: "Вкажіть модель транспорту",
                    minlength:"Модель повинна мати не менше 2 символів"
                },
                capacity: {
                    required: "Вкажіть місткість",
                    digits: "Місткість має бути цілим числом",
                    min: "Місткість повинна бути не менше 1"
                }
            },
            errorClass: "form-error-text", 
            errorElement: "span"
        }
    };

    window.VehicleSidebarValidator = VehicleSidebarValidator;
})(window);