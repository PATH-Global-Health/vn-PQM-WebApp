let year = 2020;
let month = '';
let quarter = 1;
let provinceCode = '79';
let districtCode = '768';
let firstload = true;

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

function createHTS_TEST_POS_chart() {
    $("#HTS_TEST_POS_chart").kendoChart({
        seriesDefaults: {
            type: "area",
            area: {
                line: {
                    style: "smooth"
                }
            },
        },
        series: [{
            data: [0.507, 1.943, 2.848, 0.284, 3.263, 4.801, 6.890, 8.238, 9, 4.552, 15.855, 25],
            color: "#62666e"
        }],
        categoryAxis: {
            title: {
            },
            majorGridLines: {
                visible: false
            },
            majorTicks: {
                visible: false
            }
        },
        valueAxis: {
            max: 25,
            title: {
            },
            majorGridLines: {
                visible: false
            },
            visible: false
        }
    });
}

function createHTS_TEST_Recency_chart() {
    $("#HTS_TEST-Recency_chart").kendoChart({
        seriesDefaults: {
            type: "area",
            area: {
                line: {
                    style: "smooth"
                }
            },
        },
        series: [{
            data: [0.507, 3.943, 2.848, 0.284, 7.263, 4.801, 7.890, 4.238, 9, 4.552, 15.855, 25],
            color: "#62666e"
        }],
        categoryAxis: {
            title: {
            },
            majorGridLines: {
                visible: false
            },
            majorTicks: {
                visible: false
            }
        },
        valueAxis: {
            max: 25,
            title: {
            },
            majorGridLines: {
                visible: false
            },
            visible: false
        }
    });
}

function createHTS_TEST_POS_refer_chart() {
    $("#HTS_TEST_POS-refer_chart").kendoChart({
        seriesDefaults: {
            type: "area",
            area: {
                line: {
                    style: "smooth"
                }
            },
        },
        series: [{
            data: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 25, 25],
            color: "#62666e"
        }],
        categoryAxis: {
            title: {
            },
            majorGridLines: {
                visible: false
            },
            majorTicks: {
                visible: false
            }
        },
        valueAxis: {
            max: 25,
            title: {
            },
            majorGridLines: {
                visible: false
            },
            visible: false
        }
    });
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
        createHTS_TEST_POS_chart();
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
        createHTS_TEST_Recency_chart();
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
        createHTS_TEST_POS_refer_chart();
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
    console.log(month);
    $('#inputQuarter').val(quarter);
    $('#inputMonth').val(month);
}

$(document).ready(() => {
    checkURLParams();
    initIndicators();
    initConfigPanel();
    createAgeGroupChart();
    createGenderChart();
    createKeyPopulationsChart();
    createClinicsChart();

    onProvinceChange();
    onQuarterChange();
});

function applyFilter() {
    provinceCode = $('#inputProvince').val();
    districtCode = $('#inputDistrict').val();
    year = $('#year-picker').val();
    quarter = $('#inputQuarter').val();
    month = $('#inputMonth').val();
    console.log(`filter with: province - ${provinceCode}; district - ${districtCode}; year - ${year}; quarter - ${quarter}; month - ${month}`);
    initIndicators();
    createAgeGroupChart();
    createGenderChart();
    createKeyPopulationsChart();
    createClinicsChart();
}