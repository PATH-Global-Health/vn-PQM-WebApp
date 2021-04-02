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
            console.log(response);
            _initAgeGroupChart(response);

        }
    );
}

function createGenderChart() {
    $.get(`/api/AggregatedValues?year=${year}&quarter=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}&indicatorGroup=Testing&groupBy=Gender`,
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
    if (!trend) {
        return '';
    }
    let trendDirection = trend.direction === 1 ?
        `<svg xmlns="http://www.w3.org/2000/svg" color="${trend.criticalInfo}" width="35" height="35" fill="currentColor" class="bi bi-caret-up-fill" viewBox="0 0 20 20">
                        <path d="M7.247 4.86l-4.796 5.481c-.566.647-.106 1.659.753 1.659h9.592a1 1 0 0 0 .753-1.659l-4.796-5.48a1 1 0 0 0-1.506 0z" />
                     </svg>`
        :
        `<svg xmlns="http://www.w3.org/2000/svg" color="${trend.criticalInfo}" width="35" height="35" fill="currentColor" class="bi bi-caret-down-fill" viewBox="0 0 20 20">
                         <path d="M7.247 11.14L2.451 5.658C1.885 5.013 2.345 4 3.204 4h9.592a1 1 0 0 1 .753 1.659l-4.796 5.48a1 1 0 0 1-1.506 0z" />
                     </svg>`;
    return `${trendDirection}
            ${Math.round(trend.comparePercent * 10000)/100}%`;
}

function initHTS_TEST_POSIndicator(indicator) {
    if (indicator) {
        initDataChart("HTS_TST_Positive", "HTS_TST_Positive_chart", variables);
        $("#HTS_TST_Positive-value").html(indicator.value.value.toLocaleString('vi-VN'));
        $("#HTS_TST_Positive-value").css("color", indicator.value.criticalInfo);
        $("#HTS_TST_Positive-percent").html(trendElement(indicator.trend));
    }
    else {
        $("#HTS_TST_Positive-value").html('N/A');
        $("#HTS_TST_Positive-percent").html('');
        $("#HTS_TST_Positive_chart").html('');
    }
}

function init_pHTSreferredIndicator(indicator) {
    if (indicator) {
        initDataChart("HTS_TST_Recency", "HTS_TST_Recency_chart", variables);
        $("#HTS_TST_Recency-value").html(indicator.value.dataType === 1 ? indicator.value.value : (Math.round(((indicator.value.numerator / indicator.value.denominator) + Number.EPSILON) * 100) + '%'));
        $("#HTS_TST_Recency-value").css("color", indicator.value.criticalInfo);
        $("#HTS_TST_Recency-percent").html(trendElement(indicator.trend));
    } else {
        $("#HTS_TST_Recency-value").html('N/A');
        $("#HTS_TST_Recency-percent").html('');
        $("#HTS_TST_Recency_chart").html('');
    }
}

function init_pHTSrecentIndicator(indicator) {
    if (indicator) {
        initDataChart("POS_TO_ART", "POS_TO_ART_chart", variables);
        $("#POS_TO_ART-value").html(indicator.value.dataType === 1 ? indicator.value.value : (Math.round(((indicator.value.numerator / indicator.value.denominator) + Number.EPSILON) * 100) + '%'));
        $("#POS_TO_ART-value").css("color", indicator.value.criticalInfo);
        $("#POS_TO_ART-percent").html(trendElement(indicator.trend));
    } else {
        $("#POS_TO_ART-value").html('N/A');
        $("#POS_TO_ART-percent").html('');
        $("#POS_TO_ART_chart").html('');
    }
}

function initIndicators() {
    let ageGroupQuery = variables.filter(v => v.type === 'AgeGroup').map(s => s.id).join(',');
    let keyPopulationQuery = variables.filter(v => v.type === 'KeyPopulation').map(s => s.id).join(',');
    let genderQuery = variables.filter(v => v.type === 'Gender').map(s => s.id).join(',');
    let clinnicQuery = variables.filter(v => v.type === 'Clinnic').map(s => s.id).join(',');
    $.get(`/api/AggregatedValues/IndicatorValues?year=${year}&quarter=${quarter}&month=${month}&indicatorGroup=Testing&provinceCode=${provinceCode}&districtCode=${districtCode}`
        + `&ageGroups=${ageGroupQuery}&genders=${genderQuery}&keyPopulations=${keyPopulationQuery}&clinnics=${clinnicQuery}`, function (data) {
            let p1 = true;
            let p2 = true;
            let p3 = true;
            data.forEach(indicator => {
                switch (indicator.name) {
                    case "HTS_TST_Positive":
                        initHTS_TEST_POSIndicator(indicator);
                        p1 = false;
                        break;
                    case "HTS_TST_Recency":
                        init_pHTSreferredIndicator(indicator);
                        p2 = false;
                        break;
                    case "POS_TO_ART":
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