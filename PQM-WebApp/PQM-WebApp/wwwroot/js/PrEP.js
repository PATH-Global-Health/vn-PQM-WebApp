let year = 2020;
let month = '';
let quarter = 1;
let provinceCode = '79';
let districtCode = '768';


const createAgeGroupChart = () => {
    $.get(`/api/AggregatedValues?year=${year}&quarter=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}&indicatorGroup=PrEP&groupBy=AgeGroup`,
        function (response) {
            _initAgeGroupChart(response);
        }
    );
}

const createGenderChart = () => {
    $.get(`/api/AggregatedValues?year=${year}&quarter=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}&indicatorGroup=PrEP&groupBy=Gender`,
        function (response) {
            _initGenderChart(response);
        }
    )
}

const createKeyPopulationsChart = () => {
    $.get(`/api/AggregatedValues?year=${year}&quarter=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}&indicatorGroup=PrEP&groupBy=KeyPopulation`,
        function (response) {
            _initKeyPopulationsChart(response);
        }
    )
}

const createClinicsChart = () => {
    $.get(`/api/AggregatedValues?year=${year}&quarter=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}&indicatorGroup=PrEP&groupBy=Site`,
        function (response) {
            _initClinicsChart(response);
        }
    )
}

const initIndicators = () => {
    $('#indicatorPanel').html('');
    let ageGroupQuery = variables.filter(v => v.type === 'AgeGroup').map(s => s.id).join(',');
    let keyPopulationQuery = variables.filter(v => v.type === 'KeyPopulation').map(s => s.id).join(',');
    let genderQuery = variables.filter(v => v.type === 'Gender').map(s => s.id).join(',');
    let clinnicQuery = variables.filter(v => v.type === 'Clinnic').map(s => s.id).join(',');
    $.get(`/api/AggregatedValues/IndicatorValues?indicatorGroup=PrEP&year=${year}&quater=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}`
        + `&ageGroups=${ageGroupQuery}&genders=${genderQuery}&keyPopulations=${keyPopulationQuery}&clinnics=${clinnicQuery}`, function (data) {
            data.forEach((indicator, index) => {
                indicatorOverview(indicator, "indicatorPanel");
            });
        });
}

const checkURLParams = () => {
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
});

const applyFilter = () => {
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