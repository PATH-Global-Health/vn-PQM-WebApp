let year = 2020;
let month = 1;
let quarter = 1;
let provinceCode = '79';
let districtCode = '768';

const updateUI = () => {
    let labels = [
        { elementId: 'ageGroupLabel', name: 'Age Group' },
        { elementId: 'genderLabel', name: 'Gender' },
        { elementId: 'keyPopulationLabel', name: 'Key Population' },
        { elementId: 'siteLabel', name: 'Site' }
    ]
    labels.forEach(label => $(`#${label.elementId}`).html(languages.translate('', label.name)));
    subFilter.render('variables');
}

const createAgeGroupChart = () => {
    $.get(`/api/AggregatedValues/Variables?year=${year}&quarter=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}&indicatorGroup=PrEP&groupBy=AgeGroup`,
        function (response) {
            _initAgeGroupChart(response);
            subFilter.ageGroups = response;
        }
    );
}

const createGenderChart = () => {
    $.get(`/api/AggregatedValues/Variables?year=${year}&quarter=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}&indicatorGroup=PrEP&groupBy=Gender`,
        function (response) {
            _initGenderChart(response);
            subFilter.genders = response;
        }
    )
}

const createKeyPopulationsChart = () => {
    $.get(`/api/AggregatedValues/Variables?year=${year}&quarter=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}&indicatorGroup=PrEP&groupBy=KeyPopulation`,
        function (response) {
            _initKeyPopulationsChart(response);
            subFilter.keyPopulations = response;
        }
    )
}

const createClinicsChart = () => {
    $.get(`/api/AggregatedValues/Variables?year=${year}&quarter=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}&indicatorGroup=PrEP&groupBy=Site`,
        function (response) {
            _initClinicsChart(response);
            subFilter.sites = response;
        }
    )
}

const initIndicators = () => {
    let ageGroupQuery = subFilter.variables.filter(v => v.type === 'ageGroups').map(s => s.name.split(':')[1]).join(',');
    let keyPopulationQuery = subFilter.variables.filter(v => v.type === 'keyPopulations').map(s => s.name.split(':')[1]).join(',');
    let genderQuery = subFilter.variables.filter(v => v.type === 'genders').map(s => s.name.split(':')[1]).join(',');
    let siteQuery = subFilter.variables.filter(v => v.type === 'sites').map(s => s.name.split(':')[1]).join(',');
    $.get(`/api/AggregatedValues/IndicatorValues?indicatorGroup=PrEP&year=${year}&quater=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}`
        + `&ageGroups=${ageGroupQuery}&genders=${genderQuery}&keyPopulations=${keyPopulationQuery}&sites=${siteQuery}`, function (data) {
            $('#indicatorPanel').html('');
            data.forEach((indicator, index) => {
                indicatorOverview(indicator, "indicatorPanel");
            });
            window.scroll(0, findPos(document.getElementById("filterPanel")));
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
    languages.addCallback(updateUI);
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