let ageGroups, keyPopulations, genders, provinces, indicators = [];

const loadAgeGroups = () => {
    return httpClient.callApi({
        method: 'GET',
        url: 'https://pqm.bakco.vn/api/AgeGroups?pageIndex=0&pageSize=2147483647',
    })
}

const loadKeyPopulations = () => {
    return httpClient.callApi({
        method: 'GET',
        url: 'https://pqm.bakco.vn/api/KeyPopulations?pageIndex=0&pageSize=2147483647',
    })
}

const loadGenders = () => {
    return httpClient.callApi({
        method: 'GET',
        url: 'https://pqm.bakco.vn/api/Genders?pageIndex=0&pageSize=2147483647',
    })
}

const loadProvinces = () => {
    return httpClient.callApi({
        method: 'GET',
        url: 'https://pqm.bakco.vn/api/Locations/Provinces?pageIndex=0&pageSize=2147483647',
    })
}

const loadDistricts = (provinceCode) => {
    return httpClient.callApi({
        method: 'GET',
        url: `https://pqm.bakco.vn/api/Locations/Districts?provinceCode=${provinceCode}`,
    })
}

const loadSites = (districtId) => {
    return httpClient.callApi({
        method: 'GET',
        url: `https://pqm.bakco.vn/api/Locations/Sites?districtId=${districtId}`,
    })
}

const loadIndicators = () => {
    return httpClient.callApi({
        method: 'GET',
        url: 'https://pqm.bakco.vn/api/Indicators?pageIndex=0&pageSize=2147483647',
    })
}

const buildGrid = (aggregatedValues) => {
    $("#grid").kendoGrid({
        dataSource: {
            data: aggregatedValues,
            schema: {
                model: {
                    fields: {
                        period: { type: "string" },
                        year: { type: "number" },
                        quarter: { type: "number" },
                        month: { type: "number" },
                        day: { type: "number" },
                        ageGroup: { type: "string" },
                        gender: { type: "string" },
                        keyPopulation: { type: "string" },
                        site: { type: "string" },
                        value: { type: "number" },
                    }
                }
            },
            pageSize: 20
        },
        sortable: true,
        scrollable: {
            virtual: "columns"
        },
        width: 1000,
        groupable: true,
        aggregate: [
            { field: "value", aggregate: "sum" },
        ],
        columns: [
            { field: "period", width: "100px" },
            { field: "year", width: "100px" },
            { field: "quarter", width: "100px" },
            { field: "month", width: "100px" },
            { field: "day", width: "100px" },
            { field: "ageGroup", width: "100px" },
            { field: "gender", width: "100px" },
            { field: "keyPopulation", width: "150px" },
            { field: "site", width: "150px" },
            {
                field: "value", width: "150px"
            },
        ]
    });
}

const loadAggregatedValue = () => {
    let pageIndex = 0;
    let pageSize = 2147483647;
    httpClient.callApi({
        method: 'GET',
        url: `https://pqm.bakco.vn/api/AggregatedValues?pageIndex=${pageIndex}&pageSize=${pageSize}`,
    }).then((res) => {
        let data = res.data.map(m => {
            return {
                id: m.id,
                ageGroup: m.ageGroup.name,
                keyPopulation: m.keyPopulation.name,
                gender: m.gender.name,
                site: m.site.name,
                period: m.periodType,
                year: m.year,
                quarter: m.quarter,
                month: m.month,
                day: m.day,
                value: m.dataType === 1 ? m.numerator.toLocaleString() : m.numerator / m.denominator,
            };
        });
        buildGrid(data);
        console.log(data);
    }).catch((error) => {
    });
}

const loadData = () => {
    Promise.all([loadAgeGroups(), loadGenders(), loadKeyPopulations(), loadProvinces(), loadIndicators()])
        .then((results) => {
            ageGroups = results[0].data;
            genders = results[1].data;
            keyPopulations = results[2].data;
            provinces = results[3].data;
            indicators = results[4].data;
        });
}

$(document).ready(() => {
    loadData();
    loadAggregatedValue();
})
