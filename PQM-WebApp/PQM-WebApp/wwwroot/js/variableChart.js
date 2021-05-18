let ageGroups = [];
let keyPopulations = [];
let genders = [];
let clinnics = [];

const _initAgeGroupChart = (response, htmlElement) => {
    console.log('here');
    ageGroups = response;
    $(htmlElement ? `#${htmlElement}` : "#age-group-chart").kendoChart({
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
            tooltip: {
                visible: true
            }
        }],
        valueAxis: {
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
        }
    });
}

const _initGenderChart = (response, htmlElement) => {
    genders = response;
    $(htmlElement ? `#${htmlElement}` : "#gender-chart").kendoChart({
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
            tooltip: {
                visible: true
            }
        }],
        valueAxis: {
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
        }
    });
}

const _initKeyPopulationsChart = (response, htmlElement) => {
    keyPopulations = response;
    $(htmlElement ? `#${htmlElement}` : "#key-populations-chart").kendoChart({
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
            tooltip: {
                visible: true
            }
        }],
        valueAxis: {
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
        }
    });
}

const _initClinicsChart = (response, htmlElement) => {
    clinnics = response;
    $(htmlElement ? `#${htmlElement}` : "#clinics-chart").kendoChart({
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
            tooltip: {
                visible: true
            }
        }],
        valueAxis: {
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
            categories: response.map(m => m.code),
            majorGridLines: {
                visible: false
            }
        }
    });
}

