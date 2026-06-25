class TransitMap {
    constructor(elementId, options = {}) {
        this.elementId = elementId;
        this.defaultCenter = options.center || [48.1444, 23.0325];
        this.defaultZoom = options.zoom || 14;
        this.map = null;
        this.stopColorMap = {};
        this._mapClickHandler = null;
        this.pendingMarker = null; 

        this.init();
    }

    init() {
        if ($(`#${this.elementId}`).length === 0) return;

        this.map = L.map(this.elementId).setView(this.defaultCenter, this.defaultZoom);

        L.tileLayer('https://{s}.basemaps.cartocdn.com/light_all/{z}/{x}/{y}{r}.png', {
            attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
        }).addTo(this.map);
    }

    createStopIcon(color) {
        return L.divIcon({
            className: '',
            html: `<div style="width:14px;height:14px;background:${color};border:3px solid white;border-radius:50%;box-shadow:0 2px 6px rgba(0,0,0,0.3);"></div>`,
            iconSize: [14, 14],
            iconAnchor: [7, 7]
        });
    }

    mapRouteColors(routes) {
        this.stopColorMap = {};
        $.each(routes, (index, route) => {
            if (route.stops) {
                $.each(route.stops, (idx, stopId) => {
                    this.stopColorMap[stopId] = route.color || '#3B82F6';
                });
            }
        });
    }

    renderStops(stops, routes = []) {
        if (!this.map) return;

        this.mapRouteColors(routes);

        $.each(stops, (index, stop) => {
            const markerColor = this.stopColorMap[stop.id] || '#6B7280';
            const typeLabel = stop.type === 'combined' ? 'Комбінована' : 'Стандартна';

            L.marker([stop.latitude, stop.longitude], {
                icon: this.createStopIcon(markerColor)
            })
                .bindPopup(`
                <div style="font-family: var(--font-body); font-size: 14px;">
                    <div style="font-weight: bold; color: var(--color-text);">${stop.name}</div>
                    <div style="color: var(--color-muted); font-size: 12px; margin-top:2px;">${typeLabel} зупинка</div>
                </div>
            `)
                .addTo(this.map);
        });
    }

    enableMapClickSelection(onMapClick) {
        this.disableMapClickSelection();
        this.map.getContainer().style.cursor = 'crosshair';

        this._mapClickHandler = (e) => {
            const { lat, lng } = e.latlng;
            const fixedLat = lat.toFixed(6);
            const fixedLon = lng.toFixed(6);

            if (this.pendingMarker) {
                this.pendingMarker.setLatLng([fixedLat, fixedLon]);
            } else {
                this.pendingMarker = L.marker([fixedLat, fixedLon], {
                    icon: this.createStopIcon('#10B981') 
                }).addTo(this.map);
            }

            if (typeof onMapClick === 'function') {
                onMapClick(fixedLat, fixedLon);
            }
        };

        this.map.on('click', this._mapClickHandler);
    }

    disableMapClickSelection() {
        if (this.map) {
            this.map.getContainer().style.cursor = '';
            if (this._mapClickHandler) {
                this.map.off('click', this._mapClickHandler);
                this._mapClickHandler = null;
            }

            if (this.pendingMarker) {
                this.map.removeLayer(this.pendingMarker);
                this.pendingMarker = null;
            }
        }
    }
}

export default TransitMap ;