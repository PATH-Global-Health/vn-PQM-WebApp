let thresholdSettings = [];

const loadThresholdList = () => {
    let token = `Bearer ${getToken()}`;
    $.ajax({
        url: "/api/ThresholdSettings",
        type: "GET",
        beforeSend: function (xhr) { xhr.setRequestHeader('Authorization', token); },
        success: function (data) {
            thresholdSettings = data;
        }
    });
}

const shortLabels = (number) => {
    return kendo.toString((number / 1000), "\\ #") + "k";
}

const initDataChart = (indicator, elementId, queries) => {
    let ageGroupQuery = queries ? queries.filter(v => v.type === 'AgeGroup').map(s => s.id).join(',') : '';
    let keyPopulationQuery = queries ? queries.filter(v => v.type === 'KeyPopulation').map(s => s.id).join(',') : '';
    let genderQuery = queries ? queries.filter(v => v.type === 'Gender').map(s => s.id).join(',') : '';
    let clinnicQuery = queries ? queries.filter(v => v.type === 'Clinnic').map(s => s.id).join(',') : '';
    $(`#${elementId}`).html('');
    let uri = encodeURI(`/api/AggregatedValues/ChartData?year=${year}&quarter=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}`
        + `&ageGroups=${ageGroupQuery}&genders=${genderQuery}&keyPopulations=${keyPopulationQuery}&clinnics=${clinnicQuery}&indicator=${indicator}`);
    $.get(uri,
        function (response) {
            if (response && response.length > 0) {
                let _series = response[0].dataType === 1 ? response.map(s => s.value) : response.map(s => Math.round(s.value * 10000) / 100);
                let labelTemplate = response[0].dataType === 1 ? "#= shortLabels(value) #" : "#= value #%";
                tooltipTemplate = response[0].dataType === 1 ? "#= category #: #= value #" : "#= category #: #= value #%"
                $(`#${elementId}`).kendoChart({
                    seriesDefaults: {
                        type: "line",
                        style: "smooth"
                    },
                    series: [{
                        data: _series,
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
                        labels: {
                            template: labelTemplate
                        },
                        majorGridLines: {
                            visible: false
                        },
                        visible: true
                    },
                    tooltip: {
                        visible: true,
                        template: tooltipTemplate
                    }
                });
            }
        }
    );
}

const trendElement = (trend, indicatorName) => {
    if (!trend) {
        return ``;
    }
    let percent = Math.round(trend.comparePercent * 10000) / 100;
    let _percent = percent * trend.direction;
    let thresholdSetting = thresholdSettings.find(s => s.indicatorName === indicatorName && s.from <= _percent && _percent <= s.to && s.type === 2);
    let color = thresholdSetting != null ? thresholdSetting.colorCode : "black";

    let trendDirection = trend.direction === 1 ?
        `<svg xmlns="http://www.w3.org/2000/svg" color="${color}" width="35" height="35" fill="currentColor" class="bi bi-caret-up-fill" viewBox="0 0 20 20">
                        <path d="M7.247 4.86l-4.796 5.481c-.566.647-.106 1.659.753 1.659h9.592a1 1 0 0 0 .753-1.659l-4.796-5.48a1 1 0 0 0-1.506 0z" />
                     </svg>`
        :
        `<svg xmlns="http://www.w3.org/2000/svg" color="${color}" width="35" height="35" fill="currentColor" class="bi bi-caret-down-fill" viewBox="0 0 20 20">
                         <path d="M7.247 11.14L2.451 5.658C1.885 5.013 2.345 4 3.204 4h9.592a1 1 0 0 1 .753 1.659l-4.796 5.48a1 1 0 0 1-1.506 0z" />
                     </svg>`;
    return `${trendDirection}
            <span style="color: ${color}">${percent}%</span>`;
}

const calValue = (indicatorValue) => {
    let value = indicatorValue.dataType === 1 ? indicatorValue.value
        : (Math.round(((indicatorValue.numerator / indicatorValue.denominator) + Number.EPSILON) * 10000) / 100 + '%');
    return value.toLocaleString('vi-VN');
}

const indicatorOverview = (indicator, parent) => {
    let elementId = encodeURI(indicator.name).split('%').join('');
    let value = indicator.value.dataType === 1 ? indicator.value.value
        : (Math.round(((indicator.value.numerator / indicator.value.denominator) + Number.EPSILON) * 10000) / 100);
    let thresholdSetting = thresholdSettings.find(s => s.indicatorName === indicator.name && s.from <= value && value <= s.to && s.type === 1);
    let color = thresholdSetting != null ? thresholdSetting.colorCode : "black";
    let html =
        `<div class="col-xl-4 col-md-12 px-1" style="overflow: hidden">
            <div class="row d-flex justify-content-center">
                <div id="${elementId}" class="col-xl-10 col-md-12 px-1 indicator-container" onclick="onClickIndicator(this.id)">
                    <div class="d-flex flex-column justify-content-around indicator-infor">
                        <div class="row d-flex justify-content-center indicator-title">${indicator.name}</div>
                        <div class="row d-flex justify-content-center indicator-value">
                            <span style="color: ${color}">${calValue(indicator.value)}</span>
                        </div>
                        <div class="row d-flex justify-content-center indicator-percent">
                            <span class="align-middle">
                                ${trendElement(indicator.trend, indicator.name)}
                            </span>
                        </div>
                        </div>
                        <div id="${elementId}_chart" class="indicator-infor-chart">
                    </div>
                </div>
            </div>
        </div>`
    $(`#${parent}`).append(html);
    initDataChart(indicator.name, `${elementId}_chart`);
}

const onClickIndicator = (id) => {
    for (var i = 0; i < $('.indicator-container').length; i++) {
        $($('.indicator-container')[i]).removeClass("selected-indicator-container");
    }
    $(`#${id}`).addClass("selected-indicator-container")
}

$(document).ready(() => {
    loadThresholdList();
});