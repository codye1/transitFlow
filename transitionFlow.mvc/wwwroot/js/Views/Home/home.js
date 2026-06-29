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

    function switchTab(tabName, updateUrl = true) {
        const $targetTrigger = $(`.tab-trigger[data-tab="${tabName}"]`);
        const $targetPanel = $(`#panel-${tabName}`);

        if ($targetTrigger.length === 0) return;

        $('.tab-trigger').removeClass('active');
        $targetTrigger.addClass('active');

        $('.tab-panel').removeClass('active');
        $targetPanel.addClass('active');

        if (updateUrl) {
            history.pushState(null, null, `#${tabName}`);
        }
    }

    $('.tab-trigger').on('click', function () {
        const targetTab = $(this).data('tab');
        switchTab(targetTab);
    });

    const currentHash = window.location.hash.replace('#', '');
    if (currentHash) {
        switchTab(currentHash, false);
    } else {
        switchTab('routes', false);
    }

    window.addEventListener('popstate', function () {
        const hash = window.location.hash.replace('#', '') || 'routes';
        switchTab(hash, false);
    });
});