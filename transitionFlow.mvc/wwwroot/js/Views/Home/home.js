import TransitMap from '../../Helpers/transitMap.js';

$(function () {
    window.TransitMapInstance = new TransitMap('transit-map', {
        center: [48.1444, 23.0325],
        zoom: 14
    });

    if (window.TransitData) {
        if (window.TransitData.stops) {
            window.TransitMapInstance.renderStops(window.TransitData.stops, window.TransitData.routes || []);
        }

        if (window.TransitData.routes && window.TransitData.stops) {
            window.TransitMapInstance.renderRoutes(window.TransitData.routes, window.TransitData.stops);
        }
    }

    $('.tab-trigger').on('click', function () {
        const targetTab = $(this).data('tab');

        // Зміна активного стану кнопок
        $('.tab-trigger').removeClass('active');
        $(this).addClass('active');

        // Відображення відповідної панелі сайдбару
        $('.tab-panel').removeClass('active');
        $(`#panel-${targetTab}`).addClass('active');
    });
});