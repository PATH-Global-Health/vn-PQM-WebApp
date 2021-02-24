let year = 2020;
let month = '';
let quarter = 1;
let provinceCode = '79';
let districtCode = '768';
let firstload = true;
let ageGroups = [];
let keyPopulations = [];
let genders = [];
let clinnics = [];

function openModal(indicatorName) {
    openIndicatorModal(indicatorName, year, quarter, month, provinceCode, districtCode);
}

function createAgeGroupChart() {
    $.get(`/api/AggregatedValues?year=${year}&quarter=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}&indicatorGroup=PrEP&groupBy=AgeGroup`,
        function (response) {
            ageGroups = response;
            $("#age-group-chart").kendoChart({
                theme: "bootstrap",
                legend: {
                    visible: false
                },
                seriesDefaults: {
                    type: "bar"
                },
                series: [{
                    name: "Age Group",
                    data: response.map(m => m.value),
                    color: "#62666e",
                }],
                valueAxis: {
                    max: 75,
                    line: {
                        visible: false
                    },
                    minorGridLines: {
                        visible: true
                    },
                    labels: {
                        rotation: "auto"
                    }
                },
                categoryAxis: {
                    categories: response.map(m => m.name),
                    majorGridLines: {
                        visible: false
                    }
                },
                seriesClick: function (e) {
                    let a = ageGroups.find(ag => ag.name === e.category);
                    addVar({name: `Age group: ${e.category}`, id: a.id, type: 'AgeGroup'});
                }
            });
        }
    );
}

function createGenderChart() {
    $.get(`/api/AggregatedValues?year=${year}&quarter=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}&indicatorGroup=PrEP&groupBy=Sex`,
        function (response) {
            genders = response;
            $("#gender-chart").kendoChart({
                theme: "bootstrap",
                legend: {
                    visible: false
                },
                seriesDefaults: {
                    type: "bar"
                },
                series: [{
                    name: "Gender",
                    data: response.map(m => m.value),
                    color: "#62666e",
                }],
                valueAxis: {
                    max: 155,
                    line: {
                        visible: false
                    },
                    minorGridLines: {
                        visible: true
                    },
                    labels: {
                        rotation: "auto"
                    }
                },
                categoryAxis: {
                    categories: response.map(m => m.name),
                    majorGridLines: {
                        visible: false
                    }
                },
                seriesClick: function (e) {
                    let a = genders.find(g => g.name === e.category);
                    addVar({ name: `Gender: ${e.category}`, id: a.id, type: 'Gender' });
                }
            });
        }
    )
}

function createKeyPopulationsChart() {
    $.get(`/api/AggregatedValues?year=${year}&quarter=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}&indicatorGroup=PrEP&groupBy=KeyPopulation`,
        function (response) {
            keyPopulations = response;
            $("#key-populations-chart").kendoChart({
                theme: "bootstrap",
                legend: {
                    visible: false
                },
                seriesDefaults: {
                    type: "bar"
                },
                series: [{
                    name: "Key Populations",
                    data: response.map(m => m.value),
                    color: "#62666e",
                }],
                valueAxis: {
                    max: 115,
                    line: {
                        visible: false
                    },
                    minorGridLines: {
                        visible: true
                    },
                    labels: {
                        rotation: "auto"
                    }
                },
                categoryAxis: {
                    categories: response.map(m => m.name),
                    majorGridLines: {
                        visible: false
                    }
                },
                seriesClick: function (e) {
                    let a = keyPopulations.find(g => g.name === e.category);
                    addVar({ name: `Key population: ${e.category}`, id: a.id, type: 'KeyPopulation' });
                }
            });
        }
    )
}

function createClinicsChart() {
    $.get(`/api/AggregatedValues?year=${year}&quarter=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}&indicatorGroup=PrEP&groupBy=Site`,
        function (response) {
            clinnics = response;
            $("#clinics-chart").kendoChart({
                theme: "bootstrap",
                legend: {
                    visible: false
                },
                seriesDefaults: {
                    type: "bar"
                },
                series: [{
                    name: "Clinic",
                    data: response.map(m => m.value),
                    color: "#62666e",
                }],
                valueAxis: {
                    max: 75,
                    line: {
                        visible: false
                    },
                    minorGridLines: {
                        visible: true
                    },
                    labels: {
                        rotation: "auto"
                    }
                },
                categoryAxis: {
                    labels: {
                        visual: function (e) {
                            var rect = new kendo.geometry.Rect(e.rect.origin, [e.rect.size.width, 250]);
                            var layout = new kendo.drawing.Layout(rect, {
                                orientation: "vertical",
                                alignContent: "center"
                            });
                            var words = e.text.split(" ");
                            for (var i = 0; i < words.length; i++) {
                                layout.append(new kendo.drawing.Text(words[i]));
                            }
                            layout.reflow();
                            return layout;
                        }
                    },  
                    categories: response.map(m => m.name),
                    majorGridLines: {
                        visible: false
                    }
                },
                seriesClick: function (e) {
                    let a = clinnics.find(g => g.name === e.category);
                    addVar({ name: `Clinnic: ${e.category}`, id: a.id, type: 'Clinnic' });
                }
            });
        }
    )
}

function createPrEP_NEW_chart() {
    $("#PrEP_NEW_chart").kendoChart({
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

function createPrEP_CURR_chart() {
    $("#PrEP_CURR_chart").kendoChart({
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

function createPrEP_3M_chart() {
    $("#pPrEP_3M_chart").kendoChart({
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

function initPrEP_NewIndicator(indicator) {
    if (indicator) {
        createPrEP_NEW_chart();
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
        createPrEP_CURR_chart();
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
        createPrEP_3M_chart();
        $("#pPrEP_3M-value").html(indicator.value.dataType === 1 ? indicator.value.value : (Math.round(((indicator.value.numerator / indicator.value.denominator) + Number.EPSILON) * 100) + '%'));
        $("#pPrEP_3M-value").css("color", indicator.value.criticalInfo);
        $("#pPrEP_3M-percent").html(trendElement(indicator.trend));
    } else {
        $("#pPrEP_3M-value").html('N/A');
        $("#pPrEP_3M-percent").html('');
        $("#pPrEP_3M_chart").html('');
    }
}

function initConfigPanel() {
    $("#year-picker").kendoDatePicker({
        value: year,
        format: "yyyy",
        depth: "decade",
        start: "decade"
    });
}

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
    $('body').append(modal);
});
$(document).bind("kendo:skinChange", createPrEP_NEW_chart);

var configPanelOpen = true;
function openConfig() {
    if (configPanelOpen) {
        $("#configPanel").css("height", "0px");
    } else {
        $("#configPanel").css("height", "185px");
    }

    configPanelOpen = !configPanelOpen;
}

let variables = [];

function createChip(name) {
    return `<div class="chip">
                <span style="font-weight: bold">${name.split(':')[0]}</span>: ${name.split(':')[1]}
                <span class="closebtn" onclick="removeVar('${name}')">×</span>
            </div>`;
}

function updateVariablesComp() {
    let elements = '';
    variables.forEach(v => elements += createChip(v.name));
    $("#variables").html(elements);
}

function addVar(variable) {
    let v = variables.find(s => s.name === variable.name);
    if (!v) {
        variables.push(variable);
        updateVariablesComp();
        applyFilter();
    }
}

function removeVar(name) {
    variables = variables.filter(s => s.name !== name);
    updateVariablesComp();
    applyFilter();
}

function onProvinceChange() {
    provinceCode = $('#inputProvince').val();
    $.get(`/api/Locations/Districts?provinceCode=${provinceCode}`,
        function (districts) {
            let options = districts.map(d => `<option value='${d.code}'>${d.nameWithType}</option>`).join();
            $('#inputDistrict').html(options);
            if (firstload) {
                firstload = false;
                $('#inputDistrict').val(districtCode);
            }
        }
    );
}

function onQuarterChange() {
    quarter = $('#inputQuarter').val();
    let from = quarter === "1" ? 1 : quarter === "2" ? 4 : quarter === "3" ? 7 : 10;
    let to = quarter === "1" ? 3 : quarter === "2" ? 6 : quarter === "3" ? 9 : 12;
    let months = [
        {
            name: 'Janaury',
            value: 1
        },
        {
            name: 'February',
            value: 2
        },
        {
            name: 'March',
            value: 3
        },
        {
            name: 'April',
            value: 4
        },
        {
            name: 'May',
            value: 5
        },
        {
            name: 'June',
            value: 6
        },
        {
            name: 'July',
            value: 7
        },
        {
            name: 'August',
            value: 8
        },
        {
            name: 'September',
            value: 9
        },
        {
            name: 'October',
            value: 10
        },
        {
            name: 'November',
            value: 11
        },
        {
            name: 'December',
            value: 12
        }
    ];
    let monthOptions = `<option value="">-- select month --</option>${months.filter(m => from <= m.value && m.value <= to).map(m => `<option value='${m.value}'>${m.name}</option>`).join()}`;
    $('#inputMonth').html(monthOptions);
}

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