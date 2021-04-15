let provinceCode = '79';
let districtCode = '';
let year = '2020';
let month = '';
let quarter = '1';
let firstload = true;

function drillDown(groupIndicator) {
    if (groupIndicator !== 'PrEP' && groupIndicator !== 'Testing' && groupIndicator !== 'Treatment') return;
    console.log(`${groupIndicator} is clicked`);
    var win = window.open(`${groupIndicator}?year=${year}&quarter=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}`, '_blank');
    win.focus();
}

function fakeIndicator(name, value, valueColor, info, trend, hrTag) {
    let trendElement = trend !== null ? (trend.direction === 1
        ? `<svg xmlns="http://www.w3.org/2000/svg" color="${trend.color}" width="20" height="20" fill="currentColor" class="bi bi-caret-up-fill" viewBox="0 0 20 20">
                       <path d="M7.247 4.86l-4.796 5.481c-.566.647-.106 1.659.753 1.659h9.592a1 1 0 0 0 .753-1.659l-4.796-5.48a1 1 0 0 0-1.506 0z" />
                   </svg>`
        : `<svg xmlns="http://www.w3.org/2000/svg" color="${trend.color}" width="20" height="20" fill="currentColor" class="bi bi-caret-down-fill" viewBox="0 0 20 20">
                       <path d="M7.247 11.14L2.451 5.658C1.885 5.013 2.345 4 3.204 4h9.592a1 1 0 0 1 .753 1.659l-4.796 5.48a1 1 0 0 1-1.506 0z" />
                   </svg>`) : '';
    let infoElement = info
        ? `<svg xmlns="http://www.w3.org/2000/svg" color="${info}" width="20" height="20" fill="currentColor" class="bi bi-record-fill" viewBox="0 0 20 20">
                       <path fill-rule="evenodd" d="M8 13A5 5 0 1 0 8 3a5 5 0 0 0 0 10z" />
                   </svg>`
        : `<svg width="20" height="20" opacity="0"></svg>`;
    let hrElement = hrTag ? '<hr />' : '';
    return `<div class="d-flex justify-content-between dashboard-indicator">
                        <span class="align-middle">${name}</span>
                        <span class="align-middle" style="color: ${valueColor}; font-size: 23px; font-weight: bold">
                            ${value}
                            ${infoElement}
                            ${trendElement}
                        </span>
                    </div>
                    ${hrElement}`;
}

function createIndicator(indicator, hrTag) {
    let _trendElement = indicator.trend !== null ? (indicator.trend.direction === 1
        ? `<svg xmlns="http://www.w3.org/2000/svg" color="${indicator.trend.criticalInfo}" width="20" height="20" fill="currentColor" class="bi bi-caret-up-fill" viewBox="0 0 20 20">
                       <path d="M7.247 4.86l-4.796 5.481c-.566.647-.106 1.659.753 1.659h9.592a1 1 0 0 0 .753-1.659l-4.796-5.48a1 1 0 0 0-1.506 0z" />
                   </svg>`
        : `<svg xmlns="http://www.w3.org/2000/svg" color="${indicator.trend.criticalInfo ? indicator.trend.criticalInfo : "black"}" width="20" height="20" fill="currentColor" class="bi bi-caret-down-fill" viewBox="0 0 20 20">
                       <path d="M7.247 11.14L2.451 5.658C1.885 5.013 2.345 4 3.204 4h9.592a1 1 0 0 1 .753 1.659l-4.796 5.48a1 1 0 0 1-1.506 0z" />
                   </svg>`) : '';
    let _infoElement = indicator.criticalInfo
        ? `<svg xmlns="http://www.w3.org/2000/svg" color="${indicator.criticalInfo}" width="20" height="20" fill="currentColor" class="bi bi-record-fill" viewBox="0 0 20 20">
                       <path fill-rule="evenodd" d="M8 13A5 5 0 1 0 8 3a5 5 0 0 0 0 10z" />
                   </svg>`
        : `<svg width="20" height="20" opacity="0"></svg>`;
    let _hrElement = hrTag ? '<hr />' : '';
    let value = indicator.value.dataType === 1 ? indicator.value.value : (Math.round(((indicator.value.numerator / indicator.value.denominator) + Number.EPSILON) * 100) + '%');
    return `<div class="d-flex justify-content-between dashboard-indicator">
                        <span class="align-middle">${indicator.name}</span>
                        <span class="align-middle" style="color: ${indicator.value.criticalInfo}; font-size: 23px; font-weight: bold">
                            ${numberWithCommas(value)}
                            ${_infoElement}
                            ${_trendElement}
                        </span>
                    </div>
                    ${_hrElement}`;
}

function initTestingIndicators() {
    $("#testing").html('');
    $.get(`/api/Testing/indicators?year=${year}&quater=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}`, function (data) {
        data.forEach((indicator, idx, array) => {
            $("#testing").append(createIndicator(indicator, idx !== array.length - 1));
        });
    });
}

function initPrEPIndicators() {
    $("#prep").html('');
    $.get(`/api/PrEP/indicators?year=${year}&quater=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}`, function (data) {
        data.forEach((indicator, idx, array) => {
            $("#prep").append(createIndicator(indicator, idx !== array.length - 1));
        });
    });
    //$("#prep").append(fakeIndicator('PrEP_Curr', '1,925', 'green', null, { direction: 1, color: 'green' }, true));
    //$("#prep").append(fakeIndicator('%PrEP_3M', '92%', 'green', null, { direction: 1, color: 'green' }, false));
}

function initTreatmentIndicators() {
    $("#treatment").html('');
    $.get(`/api/Treatment/indicators?year=${year}&quater=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}`, function (data) {
        data.forEach((indicator, idx, array) => {
            $("#treatment").append(createIndicator(indicator, idx !== array.length - 1));
        });
    });
}

function initDrugsIndicators() {
    $("#drugs").append(fakeIndicator('%ARV drug consumption', '98%', 'green', null, { direction: 1, color: 'green' }, true));
    $("#drugs").append(fakeIndicator('%ARV drugs that have MOS', '79%', 'orange', null, { direction: 1, color: 'green' }, false));
}

function initSHIIndicators() {
    $("#shi").append(fakeIndicator('Patients do not </br> have a SHI card', '13', 'red', null, { direction: -1, color: 'green' }, true));
    $("#shi").append(fakeIndicator('TX_Curr using ARV from SHI', '459', 'green', null, { direction: 1, color: 'green' }, true));
    $("#shi").append(fakeIndicator('TX_Curr receiving VL test from SHI', '441', 'green', 'red', { direction: 1, color: 'green' }, true));
    $("#shi").append(fakeIndicator('% of TX_Curr receiving </br> ARV MMD having a SHI card', '97%', 'green', null, { direction: 1, color: 'green' }, false));
}

function initServiceQualityIndicators() {
    $("#service-quality").append(fakeIndicator('% clients know their VL status', '77%', 'red', 'red', { direction: -1, color: 'green' }, true));
    $("#service-quality").append(fakeIndicator('% of clients experience stigma', '08%', 'green', null, { direction: 1, color: 'red' }, true));
    $("#service-quality").append(fakeIndicator('% clients experience IPV/coercion </br> during testing services', '04%', 'green', null, { direction: -1, color: 'green' }, true));
    $("#service-quality").append(fakeIndicator('% of clients seeking services at private clinics', '47%', 'green', null, { direction: -1, color: 'green' }, true));
    $("#service-quality").append(fakeIndicator('%HTS_TST_1st_Time', '04%', 'green', null, { direction: 1, color: 'red' }, false));
}

function initIndicators() {
    console.log('init indicators...');
    $("#testing").html('');
    $("#prep").html('');
    $("#treatment").html('');
    $("#drugs").html('');
    $("#shi").html('');
    $("#service-quality").html('');
    $.get(`/api/AggregatedValues/IndicatorValues?year=${year}&quarter=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}`, function (data) {
        let prep = [];
        let testing = [];
        let treatment = [];
        data.forEach(e => {
            if (e.group === "PrEP") {
                prep.push(e);
            } else if (e.group === "Testing") {
                testing.push(e);
            } else if (e.group === "Treatment") {
                treatment.push(e);
            }
        })
        prep.forEach((indicator, idx, array) => {
            $("#prep").append(createIndicator(indicator, idx !== array.length - 1));
        })
        testing.forEach((indicator, idx, array) => {
            $("#testing").append(createIndicator(indicator, idx !== array.length - 1));
        })
        treatment.forEach((indicator, idx, array) => {
            $("#treatment").append(createIndicator(indicator, idx !== array.length - 1));
        });
        $("#treatment").append('<hr />');
        $("#treatment").append(fakeIndicator('TB_PREW', 'N/A', 'black', null, null, false));
        initDrugsIndicators();
        initSHIIndicators();
        initServiceQualityIndicators();
        fixContainerHeight();
    });
}

function addBoundaries(boundaries, map) {
    boundaries.forEach(m => {
        let polygon = new google.maps.Polygon({
            paths: m,
            strokeColor: '#FF0000',
            strokeOpacity: 0.8,
            strokeWeight: 2,
            fillColor: "#FF0000",
            fillOpacity: 0.05
        });
        polygon.setMap(map);
    });
}

function addMarkerClusterer(map, response) {
    let markers = [];
    for (var i = 0; i < response.data.length; i++) {
        var location = response.data[i];
        if (location.lat !== null && location.lon !== null) {
            for (var j = 0; j < location.count; j++) {
                markers.push(new google.maps.Marker({
                    position: {
                        lat: location.lat,
                        lng: location.lon,
                    },
                    label: i + j + ''
                }))
            }
        }
    }
    new MarkerClusterer(map, markers, {
        imagePath:
            "https://developers.google.com/maps/documentation/javascript/examples/markerclusterer/m",
    });
}

function initMap() {
    let p = getBoundary(provinceCode);
    let n = -1000000;
    let s = 1000000;
    let e = -1000000;
    let w = 1000000;
    p.forEach(ip => ip.forEach(point => {
        n = point.lat > n ? point.lat : n;
        s = point.lat < s ? point.lat : s;
        e = point.lng > e ? point.lng : e;
        w = point.lng < w ? point.lng : w;
    }))

    $.get(`/api/Maps?year=${year}&quarter=${quarter}&month=${month}&provinceCode=${provinceCode}`,
        function (response) {
            console.log("init map...");
            map = new google.maps.Map(document.getElementById('map'), {
                center: { lat: (n + s) / 2, lng: (e + w) / 2 },
                zoom: 10,
                disableDefaultUI: true,
                restriction: {
                    latLngBounds: {
                        north: n + 0.25,
                        south: s - 0.25,
                        east: e + 0.25,
                        west: w - 0.25,
                    },
                },
                fullscreenControl: true,
            });
            addBoundaries(p, map);
            addMarkerClusterer(map, response);
        }
    );
}

function checkURLParams() {
    let url = new URL(window.location.href);
    year = url.searchParams.get("year") ? url.searchParams.get("year") : year;
    quarter = url.searchParams.get("quarter") ? url.searchParams.get("quarter") : quarter;
    month = url.searchParams.get("month") ? url.searchParams.get("month") : month;
    provinceCode = url.searchParams.get("provinceCode") ? url.searchParams.get("provinceCode") : provinceCode;
    districtCode = url.searchParams.get("districtCode") ? url.searchParams.get("districtCode") : districtCode;
    console.log(`${year} - ${quarter} - ${month} - ${provinceCode} - ${districtCode}`);
}

const loadThresholdList = () => {
    let token = `Bearer ${getToken()}`;
    $.ajax({
        url: "/api/ThresholdSettings",
        type: "GET",
        beforeSend: function (xhr) { xhr.setRequestHeader('Authorization', token); },
        success: function (data) {
            thresholdSettings = data;
            initIndicators();
        }
    });
}


const fixContainerHeight = () => {
    let containerHeight = $('.dashboard-container').height();
    let testing = $('#testing-container');
    let prep = $('#prep-container');
    let treatment = $('#treatment-container');
    let gap = containerHeight - testing.outerHeight(true) - prep.outerHeight(true) - treatment.outerHeight(true);
    let m = gap % 3;
    let g = (gap - m) / 3;
    if (g < 0) {
        return;
    }
    testing.height(testing.height() + g);
    prep.height(prep.height() + g);
    treatment.height(treatment.height() + g + m);
    let drug = $('#drug-container');
    let shi = $('#shi-container');
    let sq = $('#service-quality-container');
    gap = containerHeight - drug.outerHeight(true) - shi.outerHeight(true) - sq.outerHeight(true);
    m = gap % 3;
    g = (gap - m) / 3;
    drug.height(drug.height() + g);
    shi.height(shi.height() + g);
    sq.height(sq.height() + g + m);
}

$(window).resize(() => {
    fixContainerHeight();
});

$(document).ready(() => {
    checkURLParams();
    initFilterPanel();
    loadThresholdList();
}); 

const applyFilter = () => {
    provinceCode = $('#inputProvince').val();
    districtCode = $('#inputDistrict').val();
    year = $('#year-picker').val();
    quarter = $('#inputQuarter').val();
    month = $('#inputMonth').val();
    console.log(`filter with: province - ${provinceCode}; district - ${districtCode}; year - ${year}; quarter - ${quarter}; month - ${month}`);
    initIndicators();
    initMap();
    updateFilterDetail();
    return true;
}
