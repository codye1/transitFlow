import TransitMap from '../Helpers/transitMap.js';

$(function () {
    if (window.ComponentStopsData) {
        const transitMap = new TransitMap('transit-map', {
            center: [48.1444, 23.0325],
            zoom: 14
        });
        transitMap.renderStops(window.ComponentStopsData);
    }
});