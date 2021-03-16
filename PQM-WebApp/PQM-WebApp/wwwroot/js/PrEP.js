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
    $.get(`/api/AggregatedValues?year=${year}&quarter=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}&indicatorGroup=PrEP&groupBy=AgeGroup`,
        function (response) {
            _initAgeGroupChart(response);
        }
    );
}

function createGenderChart() {
    $.get(`/api/AggregatedValues?year=${year}&quarter=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}&indicatorGroup=PrEP&groupBy=Sex`,
        function (response) {
            _initGenderChart(response);
        }
    )
}

function createKeyPopulationsChart() {
    $.get(`/api/AggregatedValues?year=${year}&quarter=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}&indicatorGroup=PrEP&groupBy=KeyPopulation`,
        function (response) {
            _initKeyPopulationsChart(response);
        }
    )
}

function createClinicsChart() {
    $.get(`/api/AggregatedValues?year=${year}&quarter=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}&indicatorGroup=PrEP&groupBy=Site`,
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

function initPrEP_NewIndicator(indicator) {
    if (indicator) {
        initDataChart("PrEP%20NEW", "PrEP_NEW_chart", variables);
        $("#PrEP_NEW-value").html(indicator.value.value);
        $("#PrEP_NEW-value").css("color", indicator.value.criticalInfo);
        $("#PrEP_NEW-percent").html(trendElement(indicator.trend));
    }
    else {
        $("#PrEP_NEW-value").html('N/A');
        $("#PrEP_NEW-percent").html('');
        $("#PrEP_NEW_chart").html('');
    }
}

function initPrEP_CurrIndicator(indicator) {
    if (indicator) {
        initDataChart("PrEP%20CURR", "PrEP_CURR_chart", variables);
        $("#PrEP_CURR-value").html(indicator.value.value);
        $("#PrEP_CURR-value").css("color", indicator.value.criticalInfo);
        $("#PrEP_CURR-percent").html(trendElement(indicator.trend));
    } else {
        $("#PrEP_CURR-value").html('N/A');
        $("#PrEP_CURR-percent").html('');
        $("#PrEP_CURR_chart").html('');
    }
}

function initPrEP_3MIndicator(indicator) {
    if (indicator) {
        initDataChart("%PrEP%203M", "pPrEP_3M_chart", variables);
        $("#pPrEP_3M-value").html(indicator.value.dataType === 1 ? indicator.value.value : (Math.round(((indicator.value.numerator / indicator.value.denominator) + Number.EPSILON) * 100) + '%'));
        $("#pPrEP_3M-value").css("color", indicator.value.criticalInfo);
        $("#pPrEP_3M-percent").html(trendElement(indicator.trend));
    } else {
        $("#pPrEP_3M-value").html('N/A');
        $("#pPrEP_3M-percent").html('');
        $("#pPrEP_3M_chart").html('');
    }
}

let firstloadIndicator = true;

function initIndicators() {
    let ageGroupQuery = variables.filter(v => v.type === 'AgeGroup').map(s => s.id).join(',');
    let keyPopulationQuery = variables.filter(v => v.type === 'KeyPopulation').map(s => s.id).join(',');
    let genderQuery = variables.filter(v => v.type === 'Gender').map(s => s.id).join(',');
    let clinnicQuery = variables.filter(v => v.type === 'Clinnic').map(s => s.id).join(',');
    $.get(`/api/PrEP/indicators?year=${year}&quater=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}`
        + `&ageGroups=${ageGroupQuery}&genders=${genderQuery}&keyPopulations=${keyPopulationQuery}&clinnics=${clinnicQuery}`, function (data) {
            let p1 = true;
            let p2 = true;
            let p3 = true;
            if (firstloadIndicator) {
                indicators = data;
                firstloadIndicator = false;
            }
            data.forEach(indicator => {
                switch (indicator.name) {
                    case "PrEP NEW":
                        initPrEP_NewIndicator(indicator);
                        p1 = false;
                        break;
                    case "PrEP CURR":
                        initPrEP_CurrIndicator(indicator);
                        p2 = false;
                        break;
                    case "%PrEP 3M":
                        initPrEP_3MIndicator(indicator);
                        p3 = false;
                        break;
                }
            });
            if (p1) {
                initPrEP_NewIndicator(null);
            };
            if (p2) {
                initPrEP_CurrIndicator(null);
            }
            if (p3) {
                initPrEP_3MIndicator(null);
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
    $('body').append(modal);
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