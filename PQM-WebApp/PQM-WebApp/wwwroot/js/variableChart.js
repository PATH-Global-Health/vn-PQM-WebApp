let ageGroups = [];
let keyPopulations = [];
let genders = [];
let clinnics = [];
let variables = [];

const createChip = (name) => {
    return `<div class="chip">
                <span style="font-weight: bold">${name.split(':')[0]}</span>: ${name.split(':')[1]}
                <span class="closebtn" onclick="removeVar('${name}')">×</span>
            </div>`;
}

const updateVariablesComp = () => {
    let elements = '';
    variables.forEach(v => elements += createChip(v.name));
    $("#variables").html(elements);
}

const addVar = (variable) => {
    let v = variables.find(s => s.name === variable.name);
    if (!v) {
        variables.push(variable);
        updateVariablesComp();
        applyFilter();
    }
}

const removeVar = (name) => {
    variables = variables.filter(s => s.name !== name);
    updateVariablesComp();
    applyFilter();
}


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
        },
        seriesClick: function (e) {
            let a = ageGroups.find(ag => ag.name === e.category);
            addVar({ name: `Age group: ${e.category}`, id: a.id, type: 'AgeGroup' });
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
        },
        seriesClick: function (e) {
            let a = genders.find(g => g.name === e.category);
            addVar({ name: `Gender: ${e.category}`, id: a.id, type: 'Gender' });
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
        },
        seriesClick: function (e) {
            let a = keyPopulations.find(g => g.name === e.category);
            addVar({ name: `Key population: ${e.category}`, id: a.id, type: 'KeyPopulation' });
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
        },
        seriesClick: function (e) {
            let a = clinnics.find(g => g.name === e.category);
            addVar({ name: `Clinnic: ${e.category}`, id: a.id, type: 'Clinnic' });
        }
    });
}

const initDataChart = (indicator, elementId, queries) => {
    let ageGroupQuery = queries ? queries.filter(v => v.type === 'AgeGroup').map(s => s.id).join(',') : '';
    let keyPopulationQuery = queries ? queries.filter(v => v.type === 'KeyPopulation').map(s => s.id).join(',') : '';
    let genderQuery = queries ? queries.filter(v => v.type === 'Gender').map(s => s.id).join(',') : '';
    let clinnicQuery = queries ? queries.filter(v => v.type === 'Clinnic').map(s => s.id).join(',') : '';
    $(`#${elementId}`).html('');
    $.get(`/api/AggregatedValues/ChartData?year=${year}&quarter=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}`
        + `&ageGroups=${ageGroupQuery}&genders=${genderQuery}&keyPopulations=${keyPopulationQuery}&clinnics=${clinnicQuery}&indicator=${indicator}`,
        function (response) {
            $(`#${elementId}`).kendoChart({
                seriesDefaults: {
                    type: "line",
                    style: "smooth"
                },
                series: [{
                    data: response.map(s => s.value),
                    color: "#62666e",
                    tooltip: {
                        visible: true
                    }
                }],
                categoryAxis: {
                    categories: response.map(s => s.name),
                    majorGridLines: {
                        visible: false
                    },
                    majorTicks: {
                        visible: false
                    }
                },
                valueAxis: {
                    title: {
                    },
                    majorGridLines: {
                        visible: false
                    },
                    visible: true
                },
                tooltip: {
                    visible: true,
                    format: "{0}%",
                    template: "#= category #: #= value #"
                }
            });
        }
    );
}