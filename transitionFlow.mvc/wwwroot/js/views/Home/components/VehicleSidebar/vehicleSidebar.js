$(function () {
    'use strict';

    const Api = window.VehicleSidebarApi;
    const Validator = window.VehicleSidebarValidator;
    const ModalManager = window.Modal;

    $(document).on('click', '.btn-delete-vehicle', function (e) {
        e.stopPropagation();
        const $item = $(this).closest('.vehicle-item');
        const vehicleId = $item.data('id');
        const plateNumber = $item.find('.vehicle-plate').text().trim();

        if (!confirm(`Видалити транспортний засіб [${plateNumber}]?`)) return;

        Api.deleteVehicle(vehicleId)
            .done(() => {
                window.location.reload();
            })
            .fail((xhr) => {
                if (xhr.status === 401) {
                    window.location.href = '/login';
                    return;
                }
                if (xhr.status === 403) {
                    alert('Немає прав для видалення цього транспорту');
                    return;
                }
                alert('Помилка видалення транспортного засобу');
            });
    });

    $(document).on('change', '.js-vehicle-route-select', function () {
        const $select = $(this);
        const vehicleId = $select.data('id');
        const routeId = $select.val();

        $select.prop('disabled', true);

        Api.updateRoute(vehicleId, routeId)
            .done(() => {
                window.location.reload();
            })
            .fail((xhr) => {
                if (xhr.status === 401) {
                    window.location.href = '/login';
                    return;
                }
                alert('Помилка оновлення маршруту для транспорту');
                window.location.reload();
            })
            .always(() => {
                $select.prop('disabled', false);
            });
    });

    $(document).on('change', '.js-vehicle-status-select', function () {
        const $select = $(this);
        const vehicleId = $select.data('id');
        const nextStatus = $select.val();

        $select.prop('disabled', true);

        Api.updateStatus(vehicleId, nextStatus)
            .done(() => {
                const $indicator = $select.closest('.vehicle-item').find('.status-indicator');
                $indicator.attr('class', `status-indicator status-${nextStatus}`);
            })
            .fail((xhr) => {
                if (xhr.status === 401) {
                    window.location.href = '/login';
                    return;
                }
                alert('Помилка зміни статусу транспорту');
            })
            .always(() => {
                $select.prop('disabled', false);
            });
    });

    function openAddVehicleModal() {
        if (!ModalManager) {
            console.error('Global Modal manager library initialization instances not found.');
            return;
        }

        ModalManager.open('Новий транспортний засіб', '#tpl-add-vehicle', function ($form) {
            const $submitBtn = $form.find('#btn-submit-vehicle');

            $form.validate({
                ...Validator.vehicleFormRules,
                onkeyup: function () { this.checkForm() ? $submitBtn.prop('disabled', false) : $submitBtn.prop('disabled', true); },
                onclick: function () { this.checkForm() ? $submitBtn.prop('disabled', false) : $submitBtn.prop('disabled', true); },
                submitHandler: function (form, e) {
                    e.preventDefault();

                    const vehicleData = {
                        plateNumber: $form.find('#vehicle-plate').val().trim(),
                        type: $form.find('#vehicle-type').val(),
                        model: $form.find('#vehicle-model').val().trim(),
                        capacity: parseInt($form.find('#vehicle-capacity').val(), 10),
                        routeId: $form.find('#vehicle-route').val() || null,
                        status: $form.find('#vehicle-status').val()
                    };

                    $submitBtn.prop('disabled', true).addClass('loading');

                    Api.createVehicle(vehicleData)
                        .done(() => {
                            ModalManager.close();
                            window.location.reload();
                        })
                        .fail((xhr) => {
                            if (xhr.status === 401) {
                                window.location.href = '/login';
                                return;
                            }
                            alert('Помилка збереження нового транспортного засобу');
                        })
                        .always(() => {
                            $submitBtn.prop('disabled', false).removeClass('loading');
                        });
                }
            });

            $submitBtn.prop('disabled', !$form.valid());

            $form.on('click', '#js-close-vehicle-modal', function (e) {
                e.preventDefault();
                ModalManager.close();
            });
        });
    }

    $('#open-vehicle-modal-btn').on('click', function () {
        openAddVehicleModal();
    });
});