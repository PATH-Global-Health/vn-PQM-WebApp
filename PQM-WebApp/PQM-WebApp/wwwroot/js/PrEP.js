var year = 2020;
var month = '';
var quarter = 1;
var proviceCode = '';
var districtCode = '';

function createAgeGroupChart() {
    $.get(`/api/AggregatedValues?year=${year}&quarter=${quarter}&month=${month}&proviceCode=${proviceCode}&districtCode=${districtCode}&indicatorGroup=PrEP&groupBy=AgeGroup`,
        function (respones) {
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
                    data: respones.map(m => m.value),
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
                    categories: respones.map(m => m.name),
                    majorGridLines: {
                        visible: false
                    }
                },
                seriesClick: function (e) {
                    addVar(`Age group: ${e.category}`)
                }
            });
        }
    );
}

function createGenderChart() {
    $.get(`/api/AggregatedValues?year=${year}&quarter=${quarter}&month=${month}&proviceCode=${proviceCode}&districtCode=${districtCode}&indicatorGroup=PrEP&groupBy=Sex`,
        function (respones) {
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
                    data: respones.map(m => m.value),
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
                    categories: respones.map(m => m.name),
                    majorGridLines: {
                        visible: false
                    }
                },
                seriesClick: function (e) {
                    addVar(`Gender: ${e.category}`)
                }
            });
        }
    )
}

function createKeyPopulationsChart() {
    $.get(`/api/AggregatedValues?year=${year}&quarter=${quarter}&month=${month}&proviceCode=${proviceCode}&districtCode=${districtCode}&indicatorGroup=PrEP&groupBy=KeyPopulation`,
        function (respones) {
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
                    data: respones.map(m => m.value),
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
                    categories: respones.map(m => m.name),
                    majorGridLines: {
                        visible: false
                    }
                },
                seriesClick: function (e) {
                    addVar(`Key population: ${e.category}`)
                }
            });
        }
    )
}

function createClinicsChart() {
    $.get(`/api/AggregatedValues?year=${year}&quarter=${quarter}&month=${month}&proviceCode=${proviceCode}&districtCode=${districtCode}&indicatorGroup=PrEP&groupBy=Site`,
        function (respones) {
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
                    data: respones.map(m => m.value),
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
                    categories: respones.map(m => m.name),
                    majorGridLines: {
                        visible: false
                    }
                },
                seriesClick: function (e) {
                    addVar(`Clinniic: ${e.category}`)
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
        `<svg xmlns="http://www.w3.org/2000/svg" color="${trend.criticalInfo}" width="20" height="20" fill="currentColor" class="bi bi-caret-down-fill" viewBox="0 0 20 20">
                         <path d="M7.247 11.14L2.451 5.658C1.885 5.013 2.345 4 3.204 4h9.592a1 1 0 0 1 .753 1.659l-4.796 5.48a1 1 0 0 1-1.506 0z" />
                     </svg>`;
    return `${trendDirection}
                         ${trend.comparePercent}%
                        `;
}

function initPrEP_NewIndicator(indicator) {
    createPrEP_NEW_chart();
    $("#PrEP_NEW-value").html(indicator.value.value);
    $("#PrEP_NEW-value").css("color", indicator.value.criticalInfo);
    $("#PrEP_NEW-percent").html(trendElement(indicator.trend));
}

function initPrEP_CurrIndicator(indicator) {
    createPrEP_CURR_chart();
    $("#PrEP_CURR-value").html(indicator.value.value);
    $("#PrEP_CURR-value").css("color", indicator.value.criticalInfo);
    $("#PrEP_CURR-percent").html(trendElement(indicator.trend));
}

function initPrEP_3MIndicator(indicator) {
    createPrEP_3M_chart();
    $("#pPrEP_3M-value").html(indicator.value.value);
    $("#pPrEP_3M-value").css("color", indicator.value.criticalInfo);
    $("#pPrEP_3M-percent").html(trendElement(indicator.trend));
}

function initConfigPanel() {
    $("#year-picker").kendoDatePicker({
        value: new Date(),
        format: "yyyy",
        depth: "decade",
        start: "decade"
    });
}

$(document).ready(() => {
    $.get('/api/PrEP/indicators?year=2020&quater=1&provinceCode=79&districtCode=768', function (data) {
        data.forEach(indicator => {
            switch (indicator.name) {
                case "PrEP NEW":
                    initPrEP_NewIndicator(indicator);
                    break;
                case "PrEP_CURR":
                    initPrEP_CurrIndicator(indicator);
                    break;
                case "%PrEP_3M":
                    initPrEP_3MIndicator(indicator);
            }
        });
    });
    initConfigPanel();
    createAgeGroupChart();
    createGenderChart();
    createKeyPopulationsChart();
    createClinicsChart();
});
$(document).bind("kendo:skinChange", createPrEP_NEW_chart);

var configPanelOpen = true;
function openConfig() {
    if (configPanelOpen) {
        $("#configPanel").css("height", "0px");
    } else {
        $("#configPanel").css("height", "155px");
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
    variables.forEach(v => elements += createChip(v));
    $("#variables").html(elements);
}

function addVar(name) {
    let v = variables.find(s => s === name);
    if (!v) {
        variables.push(name);
        updateVariablesComp();
    }
}

function removeVar(name) {
    variables = variables.filter(s => s !== name);
    updateVariablesComp();
}