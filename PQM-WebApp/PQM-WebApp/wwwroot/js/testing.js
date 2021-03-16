let year = 2020;
let month = '';
let quarter = 1;
let provinceCode = '79';
let districtCode = '768';
let firstload = true;
let indicators = [];

function openModal(indicatorName) {
    let value = indicators.find(f => f.name == indicatorName).value;
    openIndicatorModal(indicatorName, year, quarter, month, provinceCode, districtCode, value);
}

function createAgeGroupChart() {
    $.get(`/api/AggregatedValues?year=${year}&quarter=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}&indicatorGroup=Testing&groupBy=AgeGroup`,
        function (response) {
            _initAgeGroupChart(response);
        }
    );
}

function createGenderChart() {
    $.get(`/api/AggregatedValues?year=${year}&quarter=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}&indicatorGroup=Testing&groupBy=Sex`,
        function (response) {
            _initGenderChart(response);
        }
    )
}

function createKeyPopulationsChart() {
    $.get(`/api/AggregatedValues?year=${year}&quarter=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}&indicatorGroup=Testing&groupBy=KeyPopulation`,
        function (response) {
            _initKeyPopulationsChart(response);
        }
    )
}

function createClinicsChart() {
    $.get(`/api/AggregatedValues?year=${year}&quarter=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}&indicatorGroup=Testing&groupBy=Site`,
        function (response) {
            _initClinicsChart(response);
        }
    )
}

function trendElement(trend) {
    let trendDirection = trend.direction === 1 ?
        `<svg xmlns="http://www.w3.org/2000/svg" color="${trend.criticalInfo}" width="35" height="35" fill="currentColor" class="bi bi-caret-up-fill" viewBox="0 0 20 20">
                        <path d="M7.247 4.86l-4.796 5.481c-.566.647-.106 1.659.753 1.659h9.592a1 1 0 0 0 .753-1.659l-4.796-5.48a1 1 0 0 0-1.506 0z" />
                     </svg>`
        :
        `<svg xmlns="http://www.w3.org/2000/svg" color="${trend.criticalInfo}" width="35" height="35" fill="currentColor" class="bi bi-caret-down-fill" viewBox="0 0 20 20">
                         <path d="M7.247 11.14L2.451 5.658C1.885 5.013 2.345 4 3.204 4h9.592a1 1 0 0 1 .753 1.659l-4.796 5.48a1 1 0 0 1-1.506 0z" />
                     </svg>`;
    return `${trendDirection}
                         ${trend.comparePercent}%
                        `;
}

function initHTS_TEST_POSIndicator(indicator) {
    if (indicator) {
        initDataChart("HTS%20Positive", "HTS_TEST_POS_chart", variables);
        $("#HTS_TEST_POS-value").html(indicator.value.value);
        $("#HTS_TEST_POS-value").css("color", indicator.value.criticalInfo);
        $("#HTS_TEST_POS-percent").html(trendElement(indicator.trend));
    }
    else {
        $("#HTS_TEST_POS-value").html('N/A');
        $("#HTS_TEST_POS-percent").html('');
        $("#HTS_TEST_POS_chart").html('');
    }
}

function init_pHTSreferredIndicator(indicator) {
    if (indicator) {
        initDataChart("%25HIV%2B%20referred", "HTS_TEST-Recency_chart", variables);
        $("#HTS_TEST-Recency-value").html(indicator.value.dataType === 1 ? indicator.value.value : (Math.round(((indicator.value.numerator / indicator.value.denominator) + Number.EPSILON) * 100) + '%'));
        $("#HTS_TEST-Recency-value").css("color", indicator.value.criticalInfo);
        $("#HTS_TEST-Recency-percent").html(trendElement(indicator.trend));
    } else {
        $("#HTS_TEST-Recency-value").html('N/A');
        $("#HTS_TEST-Recency-percent").html('');
        $("#HTS_TEST-Recency_chart").html('');
    }
}

function init_pHTSrecentIndicator(indicator) {
    if (indicator) {
        initDataChart("%HTS%20recent", "HTS_TEST_POS-refer_chart", variables);
        $("#HTS_TEST_POS-refer-value").html(indicator.value.dataType === 1 ? indicator.value.value : (Math.round(((indicator.value.numerator / indicator.value.denominator) + Number.EPSILON) * 100) + '%'));
        $("#HTS_TEST_POS-refer-value").css("color", indicator.value.criticalInfo);
        $("#HTS_TEST_POS-refer-percent").html(trendElement(indicator.trend));
    } else {
        $("#HTS_TEST_POS-refer-value").html('N/A');
        $("#HTS_TEST_POS-refer-percent").html('');
        $("#HTS_TEST_POS-refer").html('');
    }
}

function initIndicators() {
    let ageGroupQuery = variables.filter(v => v.type === 'AgeGroup').map(s => s.id).join(',');
    let keyPopulationQuery = variables.filter(v => v.type === 'KeyPopulation').map(s => s.id).join(',');
    let genderQuery = variables.filter(v => v.type === 'Gender').map(s => s.id).join(',');
    let clinnicQuery = variables.filter(v => v.type === 'Clinnic').map(s => s.id).join(',');
    $.get(`/api/Testing/indicators?year=${year}&quater=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}`
        + `&ageGroups=${ageGroupQuery}&genders=${genderQuery}&keyPopulations=${keyPopulationQuery}&clinnics=${clinnicQuery}`, function (data) {
            let p1 = true;
            let p2 = true;
            let p3 = true;
            data.forEach(indicator => {
                switch (indicator.name) {
                    case "HTS Positive":
                        initHTS_TEST_POSIndicator(indicator);
                        p1 = false;
                        break;
                    case "%HIV+ referred":
                        init_pHTSreferredIndicator(indicator);
                        p2 = false;
                        break;
                    case "%HTS recent":
                        init_pHTSrecentIndicator(indicator);
                        p3 = false;
                        break;
                }
            });
            if (p1) {
                initHTS_TEST_POSIndicator(null);
            };
            if (p2) {
                init_pHTSreferredIndicator(null);
            };
            if (p3) {
                init_pHTSrecentIndicator(null);
            };
            if (variables.length === 0) {
                this.indicators = data
            }
        });
}

function checkURLParams() {
    let url = new URL(window.location.href);
    year = url.searchParams.get("year");
    quarter = url.searchParams.get("quarter");
    month = url.searchParams.get("month");
    provinceCode = url.searchParams.get("provinceCode");
    districtCode = url.searchParams.get("districtCode");
    console.log(month);
    $('#inputQuarter').val(quarter);
    $('#inputMonth').val(month);
}

$(document).ready(() => {
    checkURLParams();
    initIndicators();
    initFilterPanel();
    createAgeGroupChart();
    createGenderChart();
    createKeyPopulationsChart();
    createClinicsChart();
});

function applyFilter() {
    provinceCode = $('#inputProvince').val();
    districtCode = $('#inputDistrict').val();
    year = $('#year-picker').val();
    quarter = $('#inputQuarter').val();
    month = $('#inputMonth').val();
    initIndicators();
    createAgeGroupChart();
    createGenderChart();
    createKeyPopulationsChart();
    createClinicsChart();
    updateFilterDetail();
}