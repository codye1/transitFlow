import api from './vehicleSidebarApi.js';
import validator from './vehicleSidebarValidator.js';
import Modal from '../../../../helpers/ModalManager.js';
import { showApiErrors } from '../../../../helpers/showApiErrors.js';

$(function () {
    'use strict';

    const getMap = () => window.TransitMapInstance;

    $(document).on('click', '.vehicle-item', function (e) {
        if ($(e.target).closest('.js-vehicle-route-select, .js-vehicle-status-select, .btn-delete-vehicle').length) {
            return;
        }

        const map = getMap();
        const vehicleId = $(this).data('id');
        if (map && vehicleId !== undefined) {
            map.focusOnVehicle(vehicleId);
        }
    });

    $(document).on('click', '.btn-delete-vehicle', function (e) {
        e.stopPropagation();
        const $item = $(this).closest('.vehicle-item');
        const vehicleId = $item.data('id');
        const plateNumber = $item.find('.vehicle-plate').text().trim();

        if (!confirm(`Видалити транспортний засіб [${plateNumber}]?`)) return;

        api.deleteVehicle(vehicleId)
            .done(() => {
                const map = getMap();
                if (map && typeof map.removeVehicle === 'function') {
                    map.removeVehicle(vehicleId);
                }

                if (window.TransitData && Array.isArray(window.TransitData.vehicles)) {
                    window.TransitData.vehicles = window.TransitData.vehicles.filter(v => Number(v.id ?? v.Id) !== Number(vehicleId));
                }

                $item.remove();
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

    $(document).on('change', '.js-vehicle-route-select', function (e) {
        e.preventDefault();
        const $select = $(this);
        const vehicleId = Number($select.data('id'));
        const routeId = $select.val() ? Number($select.val()) : null;

        $select.prop('disabled', true);

        api.updateRoute(vehicleId, routeId)
            .done(() => {
                if (window.TransitData && window.TransitData.vehicles) {
                    const vehicle = window.TransitData.vehicles.find(v => v.id === vehicleId);
                    if (vehicle) {
                        vehicle.routeId = routeId;
                    }

                    if (window.TransitMapInstance && typeof window.TransitMapInstance.renderAndAnimateVehicles === 'function') {
                        window.TransitMapInstance.renderAndAnimateVehicles(
                            window.TransitData.vehicles,
                            window.TransitData.routes,
                            window.TransitData.stops
                        );
                    }
                }
            })
            .fail((xhr) => {
                if (xhr.status === 401) {
                    window.location.href = '/login';
                    return;
                }
                if (xhr.status === 400 && xhr.responseJSON?.errors) {
                    const message = xhr.responseJSON.errors.routeId?.[0] || 'Помилка оновлення маршруту для транспорту';
                    alert(message);
                    return;
                }
                alert('Помилка оновлення маршруту для транспорту');
            })
            .always(() => {
                $select.prop('disabled', false);
            });
    });

    $(document).on('change', '.js-vehicle-status-select', function (e) {
        e.preventDefault();
        const $select = $(this);
        const vehicleId = Number($select.data('id'));
        const nextStatus = $select.val();

        $select.prop('disabled', true);

        api.updateStatus(vehicleId, nextStatus)
            .done(() => {
                const $indicator = $select.closest('.vehicle-item').find('.status-indicator');
                $indicator.attr('class', `status-indicator status-${nextStatus}`);

                if (window.TransitData && window.TransitData.vehicles) {
                    const vehicle = window.TransitData.vehicles.find(v => v.id === vehicleId);
                    if (vehicle) {
                        vehicle.status = nextStatus;
                    }

                    if (window.TransitMapInstance && typeof window.TransitMapInstance.renderAndAnimateVehicles === 'function') {
                        window.TransitMapInstance.renderAndAnimateVehicles(
                            window.TransitData.vehicles,
                            window.TransitData.routes,
                            window.TransitData.stops
                        );
                    }
                }
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
        if (!Modal) {
            console.error('Global Modal manager library initialization instances not found.');
            return;
        }

        Modal.open('Новий транспортний засіб', '#tpl-add-vehicle', {
            ...validator.vehicleFormRules,
            showErrors: function (errorMap, errorList) {
                this.defaultShowErrors();

                const $form = $(this.currentForm);
                const $submitBtn = $form.find('#btn-submit-vehicle');

                if (this.numberOfInvalids() > 0) {
                    $submitBtn.prop('disabled', true);
                } else {
                    $submitBtn.prop('disabled', false);
                }
            },
            submitHandler: (form) => {
                const $form = $(form);

                const vehicleData = {
                    plateNumber: $form.find('#vehicle-plate').val().trim(),
                    type: $form.find('#vehicle-type').val(),
                    model: $form.find('#vehicle-model').val().trim(),
                    capacity: parseInt($form.find('#vehicle-capacity').val(), 10),
                    routeId: $form.find('#vehicle-route').val() || null,
                    status: $form.find('#vehicle-status').val()
                };

                const $submitBtn = $form.find('#btn-submit-vehicle');
                $submitBtn.prop('disabled', true).addClass('loading');

                api.createVehicle(vehicleData)
                    .done((html) => {
                        const $newItem = $(html);
                        $('.vehicle-list').append($newItem);

                        const vehicleId = Number($newItem.data('id'));
                        const routeIdRaw = $newItem.data('route-id');
                        const routeId = routeIdRaw === '' || routeIdRaw === null || routeIdRaw === undefined ? null : Number(routeIdRaw);
                        const vehicleModel = {
                            id: vehicleId,
                            plateNumber: String($newItem.data('plate-number') || ''),
                            type: String($newItem.data('type') || ''),
                            status: String($newItem.data('status') || ''),
                            model: String($newItem.data('model') || ''),
                            routeId: routeId,
                            capacity: Number($newItem.data('capacity') || 0),
                            createdById: Number($newItem.data('created-by-id') || 0)
                        };

                        if (window.TransitData && Array.isArray(window.TransitData.vehicles)) {
                            window.TransitData.vehicles.push(vehicleModel);
                        }

                        const map = getMap();
                        if (map && typeof map.addVehicle === 'function') {
                            map.addVehicle(vehicleModel);
                        }

                        Modal.close();
                    })
                    .fail((xhr) => {
                        if (xhr.status === 401) {
                            window.location.href = '/login';
                            return;
                        }
                            console.log('API Errors:', xhr.responseJSON);

                        if (xhr.status === 400 && xhr.responseJSON?.errors) {
                            showApiErrors($form, xhr.responseJSON.errors);
                            return;
                        }
                        alert('Помилка збереження нового транспортного засобу');
                    })
                    .always(() => {
                        $submitBtn.prop('disabled', false).removeClass('loading');
                    });
            }
        });

        const $modalBody = $('#modal-body-content');

        $modalBody.on('click', '#js-close-vehicle-modal', function (e) {
            e.preventDefault();
            Modal.close();
        });
    }

    $('#open-vehicle-modal-btn').on('click', function () {
        openAddVehicleModal();
    });
});