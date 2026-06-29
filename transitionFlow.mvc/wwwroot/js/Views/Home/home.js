import TransitMap from '../../Helpers/transitMap.js';

$(function () {
    window.TransitMapInstance = new TransitMap('transit-map', {
        center: [48.1444, 23.0325],
        zoom: 14
    });

    async function setupMapData() {
        if (!window.TransitData) return;

        if (window.TransitData.stops) {
            window.TransitMapInstance.renderStops(window.TransitData.stops, window.TransitData.routes || []);
        }

        if (window.TransitData.routes && window.TransitData.stops) {
            await window.TransitMapInstance.renderRoutes(window.TransitData.routes, window.TransitData.stops);
        }

        if (window.TransitData.vehicles) {
            window.TransitMapInstance.renderAndAnimateVehicles(
                window.TransitData.vehicles,
                window.TransitData.routes || [],
                window.TransitData.stops || []
            );
        }
    }

    setupMapData();

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