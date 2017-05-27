// HFD Incidents
// Copyright © 2016 David M. Wilson
// https://twitter.com/dmwilson_dev
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
var initApp;
var google = google || {};

(function () {

    var map;
    var mapDiv;
    var trafficLayer;
    var infoWindow;
    var incidents;
    var markers;
    var jqMapDiv = null;
    var jqLoadingMsg = null;
    var incidentsFirstLoad = true;
    var contentPath = ContentPath || '';
    var webServiceUrl = IncidentsServiceUrl || '';
    var googleMapsKey = GoogleMapsKey || '';
    var centerLatitude = DefaultLatitude || 29.7605;
    var centerLongitude = DefaultLongitude || -95.3666;

    var defaultMapIconImage = contentPath + 'img/red_MarkerBlank.png';

    function initialize() {

        var mapOptions = {
            zoom: 10,
            center: { lat: centerLatitude, lng: centerLongitude }
        };

        mapDiv = document.getElementById('map-canvas');
        map = new google.maps.Map(mapDiv, mapOptions);
        trafficLayer = null;

        jqLoadingMsg = $('#map-data-loading');
        jqMapDiv = $('#map-canvas');

        onWindowResize();
        showLoadingMessage();

        infoWindow = infoWindow || new google.maps.InfoWindow({
            content: '<div id="infoWindowContent"></div>'
        });

        fetchIncidents();

        if (trafficLayer === null) {
            trafficLayer = new google.maps.TrafficLayer();
            trafficLayer.setMap(map);
            setInterval(reloadTiles, 60000);
        }
    }

    function fetchIncidents() {

        showLoadingMessage();

        if (incidentsFirstLoad === true) {
            incidentsFirstLoad = false;
        } else {
            if (ga !== undefined && ga !== null) {
                try {
                    ga('send', 'event', 'activeincidents', 'fetch', { 'nonInteraction': 1 });
                } catch (e) { }
            }
        }

        $.getJSON(webServiceUrl, function (json) {

            incidents = json;
            deleteAllMarkers();
            processAndDisplayIncidents();
            setTimeout(hideLoadingMessage, 100);
            setTimeout(fetchIncidents, 300000);
        });
    }

    function reloadTiles() {
        var tiles = jqMapDiv.find('img');
        for (var i = 0; i < tiles.length; i++) {
            var src = $(tiles[i]).attr('src');
            if (/googleapis.com\/maps\/vt\?pb=/.test(src)) {
                var new_src = src.split('&ts')[0] + '&ts=' + (new Date()).getTime();
                $(tiles[i]).attr('src', new_src);
            }
        }
    }

    function deleteAllMarkers() {

        markers = markers || [];

        if (markers.length > 0) {
            for (var i = 0; i < markers.length; i++) {
                markers[i].setMap(null);
            }
            markers.length = 0;
        }
    }

    function processAndDisplayIncidents() {

        incidents = incidents || [];

        incidents = incidents.sort(function (a, b) { return a.IncidentType.Id - b.IncidentType.Id; });

        for (var i = 0; i < incidents.length; i++) {

            var incident = incidents[i];

            if (incident.Status !== undefined && incident.Status === 'Cleared') {
                continue;
            }

            if (incident.Latitude === null || incident.Latitude === 0 ||
                incident.Longitude === null || incident.Longitude === 0) {
                continue;
            }

            var latLng = new google.maps.LatLng(incident.Latitude, incident.Longitude);
            var marker = new google.maps.Marker({
                position: latLng,
                title: incident.IncidentType.Name,
                map: map,
                optimized: false
            });

            setMarkerIconAndZIndex(marker, incident);
            markers.push(marker);
            bindInfoWindow(marker, map, infoWindow, getInfoWindowContent(incident));
        }
    }

    function bindInfoWindow(marker, map, infowindow, html) {

        google.maps.event.addListener(marker, 'click', function () {

            infowindow.setContent(html);
            infowindow.open(map, this);

            if (ga !== undefined && ga !== null) {
                try {
                    ga('send', 'event', 'mapmarker', 'click', 'activeincidents');
                } catch (e) { }
            }
        });
    }

    function formatDate(date) {

        if (date === undefined || date === '') {
            return '';
        }
        var dateObj = new Date(date);
        return dateObj.toLocaleString();
    }

    function getInfoWindowContent(src) {

        var srcUnits;
        var srcCurrentUnits;

        if (src === undefined || src === null) {
            return '';
        }

        if (src.Units !== undefined && src.Units !== null && src.Units !== '') {
            srcUnits = src.Units.split(';').join(', ');
        } else {
            srcUnits = '';
        }

        if (src.CurrentUnits !== undefined && src.CurrentUnits !== null && src.CurrentUnits !== '') {
            srcCurrentUnits = src.CurrentUnits.split(';').join(', ');
        } else {
            srcCurrentUnits = '';
        }

        var dst = '<div id="infoWindowContent">' +
            '<span class="iw-text-label">Agency: </span> ' + src.IncidentType.Agency.Name + '<br />' +
            '<span class="iw-text-label">Type:</span> ' + src.IncidentType.Name + '<br />';

        if (src.Status !== undefined && src.Status !== null) {
            dst = dst + '<span class="iw-text-label">Status: </span> ' + src.Status + '<br />';
        }

        dst = dst + '<span class="iw-text-label">Address: </span> ' + src.Address + '<br />';

        if (src.CrossStreet !== undefined && src.CrossStreet !== null && src.CrossStreet !== '') {
            dst = dst + '<span class="iw-text-label">Cross Street: </span> ' + src.CrossStreet + '<br />';
        }

        if (src.KeyMap !== undefined && src.KeyMap !== null && src.KeyMap !== '') {
            dst = dst + '<span class="iw-text-label">KeyMap: </span> ' + src.KeyMap + '<br />';
        }

        if (src.AlarmLevel !== undefined && src.AlarmLevel > 0) {
            dst = dst + '<span class="iw-text-label">Alarm Level: </span>' + src.AlarmLevel + '<br />';
        }

        if (src.NumberOfUnits !== undefined && src.NumberOfUnits !== null && src.NumberOfUnits > 0) {
            var unitsLabel;

            if (srcUnits === undefined || srcUnits === null || srcUnits === '') {
                unitsLabel = '# Vehicles';
            } else {
                unitsLabel = '# Units';
            }

            dst = dst + '<span class="iw-text-label">' + unitsLabel + ': </span> ' + src.NumberOfUnits + '<br />';
        }

        if (srcCurrentUnits !== '' && srcCurrentUnits !== srcUnits) {
            dst = dst + '<span class="iw-text-label">Current Units: </span> ' + srcCurrentUnits + '<br />' +
            '<span class="iw-text-label">All Units: </span> ' + srcUnits + '<br />';
        } else if (srcUnits !== null && srcUnits !== '') {
            dst = dst + '<span class="iw-text-label">Units: </span> ' + srcUnits + '<br />';
        }

        if (src.Notes !== undefined && src.Notes !== null && src.Notes !== '') {
            dst = dst + '<span class="iw-text-label">Additional Info: </span> ' + src.Notes + '<br />';
        }

        dst = dst +
            '<span class="iw-text-label">Call Opened: </span> ' + formatDate(src.CallTimeOpened) + '<br />' +
            '<span class="iw-text-label">Retrieved: </span> ' + formatDate(src.RetrievedDT) + '<br />' +
            '<span class="iw-text-label">Updated: </span> ' + formatDate(src.LastSeenDT) + '<br />';

        dst = dst + '</div>';
        return dst;
    }

    function onWindowResize() {
        var viewportHeight = $(window).height();
        var headerHeight = $('#nav-bar-header').height();
        var mapHeight = Math.ceil((viewportHeight - headerHeight) * 0.73);

        if (jqMapDiv !== null) {
            jqMapDiv.height(mapHeight);
        }

        positionLoadingMessage();

        if (map !== undefined && map !== null) {
            var mapCenter = map.getCenter();
            google.maps.event.trigger(map, 'resize');
            map.setCenter(mapCenter);
        }
    }

    function positionLoadingMessage() {
        if (jqMapDiv !== null && jqLoadingMsg !== null) {
            var msgTop = Math.floor(jqMapDiv.position().top + (jqMapDiv.height() / 2) - (jqLoadingMsg.height() / 2));
            var msgLeft = Math.floor(jqMapDiv.position().left + (jqMapDiv.width() / 2) - (jqLoadingMsg.width() / 2));
            jqLoadingMsg.css({ top: msgTop, left: msgLeft, position: 'absolute' });
        }
    }

    function showLoadingMessage() {
        if (jqLoadingMsg !== null && (!jqLoadingMsg.is(':visible'))) {
            jqLoadingMsg.show();
        }
    }

    function hideLoadingMessage() {
        if (jqLoadingMsg !== null && jqLoadingMsg.is(':visible')) {
            jqLoadingMsg.hide();
        }
    }

    function setMarkerIconAndZIndex(marker, incident) {
        var zIndex = 2;

        if (incident === undefined || incident.IncidentType === undefined) {
            return;
        }

        if (!String.prototype.contains) {
            String.prototype.contains = function () {
                return String.prototype.indexOf.apply(this, arguments) !== -1;
            };
        }

        var itype = incident.IncidentType.Name.toUpperCase();
        var typeImage = String.empty;

        if (itype.contains('EMS') || itype.contains('CHECK PATIENT')) {
            typeImage = contentPath + 'img/paleblue_MarkerE.png';
            zIndex = 1;
        }
        else if (itype.match('^CRASH/')
            || itype.match('^TRAFFIC HAZARD/')) {
            typeImage = contentPath + 'img/yellow_MarkerA.png';
        }
        else if (itype.contains('FIRE') && (itype !== 'CHECK FOR FIRE')) {
            typeImage = contentPath + 'img/red_MarkerF.png';
        }
        else if (itype.contains('MOTOR VEHICLE')
            || itype.contains('MOTORCYCLE')
            || itype.contains('PEDESTRIAN')) {
            typeImage = contentPath + 'img/yellow_MarkerA.png';
        }
        else if (itype.contains('ELEVATOR')) {
            typeImage = contentPath + 'img/yellow_MarkerE.png';
        }
        else if (itype.contains('ALARM')
            || itype.contains('SMOKE')) {
            typeImage = contentPath + 'img/orange_MarkerA.png';
        }
        else if (itype.contains('CHEMICAL LEAK')
            || itype.contains('CHEMICAL SPILL')
            || itype.contains('HAZMAT')) {
            typeImage = contentPath + 'img/green_MarkerH.png';
        }
        else if (itype.contains('WATER RESCUE')) {
            typeImage = contentPath + 'img/blue_MarkerR.png';
        }
        else if (itype.contains('HIGH WATER')) {
            typeImage = contentPath + 'img/blue_MarkerW.png';
        }
        else if (itype.contains('ACCIDENT')) {
            typeImage = contentPath + 'img/yellow_MarkerA.png';
        }
        else {
            typeImage = defaultMapIconImage;
        }

        if (typeImage === String.empty) {
            typeImage = defaultMapIconImage;
        }

        var markerIcon = {
            url: typeImage,
            size: new google.maps.Size(20, 34),
            origin: new google.maps.Point(0, 0),
            anchor: new google.maps.Point(10, 34)
        };

        marker.setZIndex(zIndex);
        marker.setIcon(markerIcon);
    }

    function asyncLoadGoogleMaps() {
        var script = document.createElement('script');
        script.type = 'text/javascript';

        script.src = '//maps.googleapis.com/maps/api/js?v=3.exp&libraries=drawing,geometry&callback=initApp&key=' + googleMapsKey;

        document.body.appendChild(script);
    }

    initApp = initialize;

    $(window).on('load', asyncLoadGoogleMaps).on('resize', onWindowResize);

}());
