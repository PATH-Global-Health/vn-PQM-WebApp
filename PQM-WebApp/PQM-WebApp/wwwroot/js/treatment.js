let year = 2020;
let month = '';
let quarter = 1;
let provinceCode = '79';
let districtCode = '768';
let firstload = true;

function createAgeGroupChart() {
    $.get(`/api/AggregatedValues?year=${year}&quarter=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}&indicatorGroup=Treatment&groupBy=AgeGroup`,
        function (response) {
            _initAgeGroupChart(response);
        }
    );
}

function createGenderChart() {
    $.get(`/api/AggregatedValues?year=${year}&quarter=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}&indicatorGroup=Treatment&groupBy=Gender`,
        function (response) {
            _initGenderChart(response);
        }
    )
}

function createKeyPopulationsChart() {
    $.get(`/api/AggregatedValues?year=${year}&quarter=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}&indicatorGroup=Treatment&groupBy=KeyPopulation`,
        function (response) {
            _initKeyPopulationsChart(response)
        }
    )
}

function createClinicsChart() {
    $.get(`/api/AggregatedValues?year=${year}&quarter=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}&indicatorGroup=Treatment&groupBy=Site`,
        function (response) {
            _initClinicsChart(response);
        }
    )
}

function trendElement(trend) {
    if (!trend || trend.direction === 0) return "";
    let trendDirection = trend.direction === 1 ?
        `<svg xmlns="http://www.w3.org/2000/svg" color="${trend.criticalInfo}" width="35" height="35" fill="currentColor" class="bi bi-caret-up-fill" viewBox="0 0 20 20">
                        <path d="M7.247 4.86l-4.796 5.481c-.566.647-.106 1.659.753 1.659h9.592a1 1 0 0 0 .753-1.659l-4.796-5.48a1 1 0 0 0-1.506 0z" />
                     </svg>`
        :
        `<svg xmlns="http://www.w3.org/2000/svg" color="${trend.criticalInfo}" width="35" height="35" fill="currentColor" class="bi bi-caret-down-fill" viewBox="0 0 20 20">
                         <path d="M7.247 11.14L2.451 5.658C1.885 5.013 2.345 4 3.204 4h9.592a1 1 0 0 1 .753 1.659l-4.796 5.48a1 1 0 0 1-1.506 0z" />
                     </svg>`;
    return `${trendDirection}
                         ${(Math.round(trend.comparePercent * 10000) / 100).toLocaleString('vi-VN')}%
                        `;
}

function initTX_CurrIndicator(indicator) {
    if (indicator) {
        initDataChart("TX_CURR", "TX_Curr_chart", variables);
        $("#TX_Curr-value").html(indicator.value.value.toLocaleString('vi-VN'));
        $("#TX_Curr-value").css("color", indicator.value.criticalInfo);
        $("#TX_Curr-percent").html(trendElement(indicator.trend));
    }
    else {
        $("#TX_Curr-value").html('N/A');
        $("#TX_Curr-percent").html('');
        $("#TX_Curr_chart").html('');
    }
}

function initTX_NewIndicator(indicator) {
    if (indicator) {
        initDataChart("TX_NEW", "TX_New_chart", variables);
        $("#TX_New-value").html(indicator.value.value.toLocaleString('vi-VN'));
        $("#TX_New-value").css("color", indicator.value.criticalInfo);
        $("#TX_New-percent").html(trendElement(indicator.trend));
    } else {
        $("#TX_New-value").html('N/A');
        $("#TX_New-percent").html('');
        $("#TX_New_chart").html('');
    }
}

function initMMDIndicator(indicator) {
    if (indicator) {
        initDataChart("MMD","MMD_chart")
        $("#MMD-value").html(indicator.value.dataType === 1 ? indicator.value.value.toLocaleString('vi-VN') : (Math.round(((indicator.value.numerator / indicator.value.denominator) + Number.EPSILON) * 100) + '%').toLocaleString('vi-VN'));
        $("#MMD-value").css("color", indicator.value.criticalInfo);
        $("#MMD-percent").html(trendElement(indicator.trend));
    } else {
        $("#MMD-value").html('N/A');
        $("#MMD-percent").html('');
        $("#MMD_chart").html('');
    }
}

function initITIndicator(indicator) {
    if (indicator) {
        initDataChart("Interruption%20in%20Treatment", "IT_chart")
        $("#IT-value").html(indicator.value.dataType === 1 ? indicator.value.value.toLocaleString('vi-VN') : (Math.round(((indicator.value.numerator / indicator.value.denominator) + Number.EPSILON) * 100) + '%').toLocaleString('vi-VN'));
        $("#IT-value").css("color", indicator.value.criticalInfo);
        $("#IT-percent").html(trendElement(indicator.trend));
    } else {
        $("#IT-value").html('N/A');
        $("#IT-percent").html('');
        $("#IT_chart").html('');
    }
}

function initpVLIndicator(indicator) {
    if (indicator) {
        initDataChart("%%20VL%20unsupressed", "pVL_chart")
        $("#pVL-value").html(indicator.value.dataType === 1 ? indicator.value.value : (customRound(indicator.value.numerator, indicator.value.denominator) * 100).toLocaleString('vi-VN') + '%');
        $("#pVL-value").css("color", indicator.value.criticalInfo);
        $("#pVL-percent").html(trendElement(indicator.trend));
    } else {
        $("#pVL-value").html('N/A');
        $("#pVL-percent").html('');
        $("#pVL_chart").html('');
    }
}

function initTB_PREWIndicator(indicator) {
    if (indicator) {
        initDataChart("TB_PREW", "TB_PREW_chart")
        $("#TB_PREW-value").html(indicator.value.dataType === 1 ? indicator.value.value : (Math.round(((indicator.value.numerator / indicator.value.denominator) + Number.EPSILON) * 100) + '%'));
        $("#TB_PREW-value").css("color", indicator.value.criticalInfo);
        $("#TB_PREW-percent").html(trendElement(indicator.trend));
    } else {
        $("#TB_PREW-value").html('N/A');
        $("#TB_PREW-percent").html('');
        $("#TB_PREW_chart").html('');
    }
}

function initIndicators() {
    let ageGroupQuery = variables.filter(v => v.type === 'AgeGroup').map(s => s.id).join(',');
    let keyPopulationQuery = variables.filter(v => v.type === 'KeyPopulation').map(s => s.id).join(',');
    let genderQuery = variables.filter(v => v.type === 'Gender').map(s => s.id).join(',');
    let clinnicQuery = variables.filter(v => v.type === 'Clinnic').map(s => s.id).join(',');
    $.get(`/api/AggregatedValues/IndicatorValues?year=${year}&quarter=${quarter}&month=${month}&indicatorGroup=Treatment&provinceCode=${provinceCode}&districtCode=${districtCode}`
        + `&ageGroups=${ageGroupQuery}&genders=${genderQuery}&keyPopulations=${keyPopulationQuery}&clinnics=${clinnicQuery}`, function (data) {
            let p1 = true;
            let p2 = true;
            let p3 = true;
            let p4 = true;
            let p5 = true;
            let p6 = true;
            data.forEach(indicator => {
                switch (indicator.name) {
                    case "TX_Curr":
                        initTX_CurrIndicator(indicator);
                        p1 = false;
                        break;
                    case "TX_New":
                        initTX_NewIndicator(indicator);
                        p2 = false;
                        break;
                    case "MMD":
                        initMMDIndicator(indicator);
                        p3 = false;
                        break;
                    case "Interruption in Treatment":
                        initITIndicator(indicator);
                        p4 = false;
                        break;
                    case "% VL unsupressed":
                        initpVLIndicator(indicator);
                        p5 = false;
                        break;
                    case "TB_PREW":
                        initTB_PREWIndicator(indicator);
                        p6 = false;
                        break;
                }
            });
            if (p1) {
                initTX_CurrIndicator(null);
            };
            if (p2) {
                initPrEP_CurrIndicator(null);
            }
            if (p3) {
                initMMDIndicator(null);
            }
            if (p4) {
                initITIndicator(null);
            };
            if (p5) {
                initpVLIndicator(null);
            }
            if (p6) {
                initTB_PREWIndicator(null);
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