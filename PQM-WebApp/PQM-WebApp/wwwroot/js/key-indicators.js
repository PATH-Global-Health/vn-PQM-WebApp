var configPanelOpen = true;
function openConfig() {
    if (configPanelOpen) {
        $("#configPanel").css("height", "0px");
    } else {
        $("#configPanel").css("height", "200px");
    }

    configPanelOpen = !configPanelOpen;
}

function initChart() {
    for (var i = 1; i < 7; i++) {
        $(`#chart-${i}`).kendoChart({
            seriesDefaults: {
                type: "area",
                area: {
                    line: {
                        style: "smooth"
                    }
                },
            },
            series: [{
                data: [24, 15, 10, 23, 12, 14, 16, 17, 16, 18, 23, 25],
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
    initConfigPanel();
    initChart();
}); 