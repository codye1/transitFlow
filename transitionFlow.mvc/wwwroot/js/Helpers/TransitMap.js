class TransitMap {
    constructor(elementId, options = {}) {
        this.elementId = elementId;
        this.defaultCenter = options.center || [48.1444, 23.0325];
        this.defaultZoom = options.zoom || 14;
        this.map = null;
        this.stopColorMap = {};
        this._mapClickHandler = null;
        this.pendingMarker = null;
        this.markers = {};
        this.routeLines = {}; 
        this.previewPolyline = null;
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
        this.markers = {};

        $.each(stops, (index, stop) => {
            const markerColor = this.stopColorMap[stop.id] || '#6B7280';
            const typeLabel = stop.type === 'combined' ? 'Комбінована' : 'Стандартна';

            const marker = L.marker([stop.latitude, stop.longitude], {
                icon: this.createStopIcon(markerColor)
            })
                .bindPopup(`
                <div style="font-family: var(--font-body); font-size: 14px;">
                    <div class="popup-stop-name" style="font-weight: bold; color: var(--color-text);">${stop.name}</div>
                    <div style="color: var(--color-muted); font-size: 12px; margin-top:2px;">${typeLabel} зупинка</div>
                </div>
            `)
                .addTo(this.map);

            this.markers[stop.id] = marker;
        });
    }

    async fetchOSRMRoute(coords) {
        if (coords.length < 2) return null;

        const coordString = coords.map(c => `${c[1]},${c[0]}`).join(';');
        const url = `https://router.project-osrm.org/route/v1/driving/${coordString}?overview=full&geometries=geojson`;

        try {
            const response = await fetch(url);
            if (!response.ok) throw new Error('OSRM network response was not ok');

            const data = await response.json();
            if (data.routes && data.routes.length > 0) {
                return data.routes[0].geometry.coordinates.map(c => [c[1], c[0]]);
            }
        } catch (error) {
            console.error("Failed to fetch road path from OSRM:", error);
        }
        return null; 
    }

    async renderRoutes(routes, stops) {
        if (!this.map) return;

        $.each(this.routeLines, (id, line) => {
            this.map.removeLayer(line);
        });
        this.routeLines = {};

        for (const route of routes) {
            if (!route.stops || route.stops.length < 2) continue;
            const stopCoords = [];
            for (const stopId of route.stops) {
                const targetStop = stops.find(s => s.id === stopId);
                if (targetStop) {
                    stopCoords.push([targetStop.latitude, targetStop.longitude]);
                }
            }

            if (stopCoords.length < 2) continue;

            let pathPoints = await this.fetchOSRMRoute(stopCoords);
            if (!pathPoints) {
                pathPoints = stopCoords;
            }

            const polyline = L.polyline(pathPoints, {
                color: route.color || '#3B82F6',
                weight: 4,
                opacity: 0.85
            }).addTo(this.map);

            this.routeLines[route.id] = polyline;
        }
    }

    updateStopPopup(id, newName) {
        const marker = this.markers[id];
        if (!marker) return;

        const popup = marker.getPopup();
        if (popup) {
            const currentContent = popup.getContent();
            const $html = $(`<div>${currentContent}</div>`);
            $html.find('.popup-stop-name').text(newName);
            marker.setPopupContent($html.html());
        }
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
    enableRouteStopsSelection(allStops, currentStopIds, onStopSelected) {
        this.disableRouteStopsSelection();
        this._allStops = allStops;
        this._selectedStopIds = [...currentStopIds];

        // Зберігаємо початкові іконки маркерів, щоб повернути їх назад потім
        this._originalIcons = {};

        this.updatePreviewRoute();

        $.each(this.markers, (stopId, marker) => {
            const numericId = Number(stopId);

            if (marker.getPopup()) {
                marker._savedPopup = marker.getPopup();
                marker.unbindPopup();
            }

            // Зберігаємо оригінальну іконку маркера
            this._originalIcons[numericId] = marker.options.icon;

            // Якщо маркер вже був обраний до переходу на карту — підсвічуємо його відразу
            if (this._selectedStopIds.includes(numericId)) {
                marker.setIcon(this.createHighlightIcon('#10B981')); // зелений активний колір
            }

            marker._routeSelectHandler = async (e) => {
                if (e && e.originalEvent) e.originalEvent.stopPropagation();

                if (!this._selectedStopIds.includes(numericId)) {
                    this._selectedStopIds.push(numericId);
                    marker.setIcon(this.createHighlightIcon('#10B981')); // Підсвічуємо при виборі
                } else {
                    this._selectedStopIds = this._selectedStopIds.filter(id => id !== numericId);
                    // Повертаємо дефолтну іконку
                    marker.setIcon(this._originalIcons[numericId]);
                }

                await this.updatePreviewRoute();

                if (typeof onStopSelected === 'function') {
                    onStopSelected(this._selectedStopIds);
                }
            };

            marker.on('click', marker._routeSelectHandler);
        });
    }

    // Допоміжний метод для створення підсвіченої іконки (більший розмір + ефект пульсації/обводки)
    createHighlightIcon(color) {
        return L.divIcon({
            className: '',
            html: `<div style="width:20px;height:20px;background:${color};border:4px solid white;border-radius:50%;box-shadow:0 0 12px ${color}, 0 2px 6px rgba(0,0,0,0.4);transition: all 0.2s ease;"></div>`,
            iconSize: [20, 20],
            iconAnchor: [10, 10]
        });
    }

    async updatePreviewRoute() {
        if (this.previewPolyline) {
            this.map.removeLayer(this.previewPolyline);
            this.previewPolyline = null;
        }

        if (!this._selectedStopIds || this._selectedStopIds.length < 2) return;

        const stopCoords = [];
        for (const stopId of this._selectedStopIds) {
            const targetStop = this._allStops.find(s => s.id === stopId || s.Id === stopId);
            if (targetStop) {
                stopCoords.push([targetStop.latitude || targetStop.Latitude, targetStop.longitude || targetStop.Longitude]);
            }
        }

        if (stopCoords.length < 2) return;

        let pathPoints = await this.fetchOSRMRoute(stopCoords);
        if (!pathPoints) {
            pathPoints = stopCoords;
        }

        this.previewPolyline = L.polyline(pathPoints, {
            color: '#10B981', 
            weight: 5,
            opacity: 0.75,
            dashArray: '10, 10' 
        }).addTo(this.map);
    }

    disableRouteStopsSelection() {
        $.each(this.markers, (stopId, marker) => {
            const numericId = Number(stopId);

            if (marker._routeSelectHandler) {
                marker.off('click', marker._routeSelectHandler);
                delete marker._routeSelectHandler;
            }

            if (marker._savedPopup) {
                marker.bindPopup(marker._savedPopup);
                delete marker._savedPopup;
            }

            // Відновлюємо початковий вигляд усіх маркерів
            if (this._originalIcons && this._originalIcons[numericId]) {
                marker.setIcon(this._originalIcons[numericId]);
            }
        });

        if (this.previewPolyline) {
            this.map.removeLayer(this.previewPolyline);
            this.previewPolyline = null;
        }

        this._selectedStopIds = [];
        this._allStops = [];
        this._originalIcons = {};
    }

    focusOnPoint(id, lat, lon, zoom = 16) {
        if (!this.map) return;

        this.map.flyTo([lat, lon], zoom, {
            animate: true,
            duration: 1.2
        });

        if (this.markers[id]) {
            this.markers[id].openPopup();
        }
    }

    focusOnRoute(routeId) {
        if (!this.map) return;

        const polyline = this.routeLines[routeId];
        if (polyline) {
            const bounds = polyline.getBounds();

            this.map.flyToBounds(bounds, {
                animate: true,
                duration: 1.2,
                padding: [50, 50] 
            });
        } else {
            console.warn(`Route line with ID ${routeId} not found on the map.`);
        }
    }
}

export default TransitMap;