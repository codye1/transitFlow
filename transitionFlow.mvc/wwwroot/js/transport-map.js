document.addEventListener("DOMContentLoaded", function () {
    const DEFAULT_CENTER = [48.1444, 23.0325];
    const DEFAULT_ZOOM = 14;

    if (!document.getElementById('transport-map')) return;

    const map = L.map('transport-map').setView(DEFAULT_CENTER, DEFAULT_ZOOM);

    L.tileLayer('https://{s}.basemaps.cartocdn.com/light_all/{z}/{x}/{y}{r}.png', {
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
    }).addTo(map);

    const { routes, stops } = window.TransitData || { routes: [], stops: [] };

    function createStopIcon(color) {
        return L.divIcon({
            className: '',
            html: `<div style="width:14px;height:14px;background:${color};border:3px solid white;border-radius:50%;box-shadow:0 2px 6px rgba(0,0,0,0.3);"></div>`,
            iconSize: [14, 14],
            iconAnchor: [7, 7]
        });
    }

    const stopColorMap = {};
    routes.forEach(route => {
        if (route.stops) {
            route.stops.forEach(stopId => {
                stopColorMap[stopId] = route.color || '#3B82F6';
            });
        }
    });
    stops.forEach(stop => {
        const markerColor = stopColorMap[stop.id] || '#6B7280';
        const typeLabel = stop.type === 'combined' ? 'Комбінована' : 'Стандартна';

        L.marker([stop.latitude, stop.longitude], {
            icon: createStopIcon(markerColor)
        })
            .bindPopup(`
            <div style="font-family: var(--font-body); font-size: 14px;">
                <div style="font-weight: bold; color: var(--color-text);">${stop.name}</div>
                <div style="color: var(--color-muted); font-size: 12px; margin-top:2px;">${typeLabel} зупинка</div>
            </div>
        `)
            .addTo(map);
    });
});