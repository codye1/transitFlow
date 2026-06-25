import TransitMap from '../../Helpers/transitMap.js';

$(function () {
    let debounceTimer = null;
    let savedStopState = null;
    let temporaryCoords = null;

    const transitMap = new TransitMap('transit-map', {
        center: [48.1444, 23.0325],
        zoom: 14
    });

    transitMap.renderStops(window.TransitData.stops);

    $(document).on('click', '.btn-delete-stop', function () {
        const stopId = $(this).data('id');

        if (!confirm('Видалити зупинку?')) return;

        $.ajax({
            url: `/stops/${stopId}`,
            type: 'DELETE',
            success: () => {
                window.location.reload();
            },
            error: (xhr) => {
                if (xhr.status === 401) {
                    window.location.href = '/login';
                    return;
                }
                if (xhr.status === 403) {
                    alert('Немає прав для видалення цієї зупинки');
                    return;
                }
                alert('Помилка видалення зупинки');
            }
        });
    });

    function openAddStopModal() {
        window.Modal.open('Нова зупинка', '#tpl-add-stop', function ($form) {
            const stopData = {
                name: $form.find('#stop-name').val(),
                type: $form.find('#stop-type').val(),
                latitude: parseFloat($form.find('#stop-lat').val()),
                longitude: parseFloat($form.find('#stop-lon').val())
            };

            const $submitBtn = $form.find('#btn-submit-stop');
            $submitBtn.prop('disabled', true).addClass('loading');

            $.ajax({
                url: '/stops',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(stopData),
                success: () => {
                    savedStopState = null;
                    window.location.reload();
                },
                error: (xhr) => {
                    if (xhr.status === 401) {
                        window.location.href = '/login';
                        return;
                    }
                    alert('Помилка збереження зупинки');
                    $submitBtn.prop('disabled', false).removeClass('loading');
                }
            });
        });

        const $modalBody = $('#modal-body-content');
        const $submitBtn = $modalBody.find('#btn-submit-stop');
        const $suggestions = $modalBody.find('#search-suggestions');
        const $searchIcon = $modalBody.find('.search-icon');
        const $spinnerIcon = $modalBody.find('.spinner-icon');

        if (savedStopState) {
            $modalBody.find('#stop-name').val(savedStopState.name);
            $modalBody.find('#stop-type').val(savedStopState.type);
            $modalBody.find('#address-search').val(savedStopState.addressSearch || '');
            if (savedStopState.latitude) $modalBody.find('#stop-lat').val(savedStopState.latitude);
            if (savedStopState.longitude) $modalBody.find('#stop-lon').val(savedStopState.longitude);
        }

        function validateForm() {
            const name = $modalBody.find('#stop-name').val().trim();
            const lat = $modalBody.find('#stop-lat').val().trim();
            const lon = $modalBody.find('#stop-lon').val().trim();
            $submitBtn.prop('disabled', !(name && lat && lon));
        }

        validateForm();

        $modalBody.on('input', '#stop-name, #stop-lat, #stop-lon', validateForm);

        function fetchSuggestions(query) {
            if (!query.trim()) {
                $suggestions.addClass('hidden').empty();
                return;
            }

            $searchIcon.addClass('hidden');
            $spinnerIcon.removeClass('hidden');

            const url = `https://nominatim.openstreetmap.org/search?q=${encodeURIComponent(query)}&format=json&limit=6&accept-language=uk`;

            $.getJSON(url, function (data) {
                $suggestions.empty().removeClass('hidden');

                if (data.length === 0) {
                    $suggestions.append('<div class="suggestion-item text-muted">Нічого не знайдено</div>');
                } else {
                    $.each(data, function (idx, item) {
                        const displayName = item.display_name.split(',')[0];
                        const $btn = $(`
                            <button type="button" class="suggestion-item">
                                <span class="pin">📍</span>
                                <div class="text-block">
                                    <span class="main-text">${displayName}</span>
                                    <span class="full-text">${item.display_name}</span>
                                </div>
                            </button>
                        `);

                        $btn.on('click', function () {
                            const lat = parseFloat(item.lat).toFixed(6);
                            const lon = parseFloat(item.lon).toFixed(6);

                            $modalBody.find('#stop-lat').val(lat);
                            $modalBody.find('#stop-lon').val(lon);
                            $modalBody.find('#address-search').val(displayName);

                            $suggestions.addClass('hidden').empty();
                            validateForm();
                        });

                        $suggestions.append($btn);
                    });
                }
            }).always(function () {
                $searchIcon.removeClass('hidden');
                $spinnerIcon.addClass('hidden');
            });
        }

        $modalBody.on('input', '#address-search', function () {
            const val = $(this).val();
            clearTimeout(debounceTimer);

            if (val.trim().length >= 3) {
                debounceTimer = setTimeout(() => fetchSuggestions(val), 400);
            } else {
                $suggestions.addClass('hidden').empty();
            }
        });

        $modalBody.on('click', '#btn-search-trigger', function () {
            clearTimeout(debounceTimer);
            fetchSuggestions($modalBody.find('#address-search').val());
        });

        $modalBody.on('click', '#btn-pick-on-map', function () {
            savedStopState = {
                name: $modalBody.find('#stop-name').val(),
                type: $modalBody.find('#stop-type').val(),
                addressSearch: $modalBody.find('#address-search').val()
            };

            window.Modal.close();

            temporaryCoords = null;
            $('#banner-text').text('Натисніть на карту, щоб обрати координати зупинки');
            $('#btn-banner-confirm').addClass('hidden');
            $('#map-selection-banner').removeClass('hidden');

            transitMap.enableMapClickSelection(function (lat, lon) {
                temporaryCoords = { lat, lon };
                $('#banner-text').text(`Точка обрана: ${lat}, ${lon}`);
                $('#btn-banner-confirm').removeClass('hidden');
            });
        });
    }

    $('#btn-banner-confirm').on('click', function () {
        if (temporaryCoords && savedStopState) {
            savedStopState.latitude = temporaryCoords.lat;
            savedStopState.longitude = temporaryCoords.lon;
        }
        $('#map-selection-banner').addClass('hidden');
        transitMap.disableMapClickSelection();
        openAddStopModal();
    });

    $('#btn-banner-cancel').on('click', function () {
        $('#map-selection-banner').addClass('hidden');
        transitMap.disableMapClickSelection();
        temporaryCoords = null;
        openAddStopModal();
    });

    $('#open-modal-btn').on('click', function () {
        savedStopState = null;
        temporaryCoords = null;
        openAddStopModal();
    });
});