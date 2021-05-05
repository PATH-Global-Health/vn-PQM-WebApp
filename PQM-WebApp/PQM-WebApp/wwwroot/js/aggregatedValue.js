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

const buildGrid = (aggregatedValues, total) => {
    $('#grid').html('');
    $("#grid").kendoGrid({
        toolbar: [
            { template: kendo.template($("#template1").html()) },
            { template: kendo.template($("#template2").html()) },
            { template: kendo.template($("#template3").html()) },
        ],
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
                        indicator: { type: "string" },
                        unsolvedDimValue: { type: "number" },
                    }
                },
            },
            pageSize: 20,
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
        pageable: {
            pageSizes: [20, 50, 100, 200]
        },
        columns: [
            { field: "period", width: "100px" },
            { field: "year", width: "100px" },
            { field: "quarter", width: "100px" },
            { field: "month", width: "100px" },
            { field: "day", width: "100px" },
            { field: "indicator", width: "100px" },
            { field: "ageGroup", width: "100px" },
            { field: "gender", width: "100px" },
            { field: "keyPopulation", width: "150px" },
            { field: "site", width: "150px" },
            {
                field: "value", width: "150px"
            },
        ]
    });
    const grid = $('#grid').data('kendoGrid');
    const pager = grid.pager;
    pager.totalPage = totalPage;
    pager.bind('change', (e) => {
        loadAggregatedValue(pager.page(), pager.pageSize());
    });
    const pageSizesDdl = $(pager.element).find("[data-role='dropdownlist']").data("kendoDropDownList");
    pageSizesDdl.bind("change", function (ev) {
        console.log(pager.pageSize());
    });
}

const loadAggregatedValue = (pageIndex, pageSize) => {
    httpClient.callApi({
        method: 'GET',
        url: `/api/AggregatedValues?pageIndex=${pageIndex}&pageSize=${pageSize}`,
    }).then((res) => {
        let data = res.data.data.map(m => {
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
                indicator: m.indicator.name,
                value: m.dataType === 1 ? m.numerator.toLocaleString() : m.numerator / m.denominator,
                unsolvedDimValue: m.unsolvedDimValues.count,
            };
        });
        buildGrid(data, res.data.total);
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

const onImportExcel = () => {
    let upload = $("#file").data("kendoUpload"),
        excelfile = upload.getFiles();
    let formData = new FormData();
    formData.append("file", excelfile[0].rawFile, excelfile[0].name);
    httpClient.callApi({
        method: "POST",
        contentType: "multipart/form-data",
        url: "/api/AggregatedValues/ImportByExcel",
        data: formData
    }).then((res) => {
        $('#importExcelModal').modal('hide');
        loadAggregatedValue(0, 200);
    })
    return false;
}

const importExcel = () => {
    $('#importExcelModal').html(`
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Import Excel</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <div class="demo-section k-content">
                        <input name="file" id="file" type="file" aria-label="file" />
                        <p style="padding-top: 1em; text-align: right">

                        </p>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                    <button type="button" class="k-button k-primary" onclick="onImportExcel()">Submit</button>
                </div>
            </div>
        </div>
    `)
    $("#file").kendoUpload();
    $('#importExcelModal').modal('show')
    return false;
}

const populateData = () => {
    httpClient.callApi({
        method: "POST",
        url: "/api/AggregatedValues/PopulateData?all=true&makeDeletion=true",
    }).then((res) => {
        alert('Populate Data is done')
    })
}

const clearAll = () => {
    httpClient.callApi({
        method: "GET",
        url: "/api/AggregatedValues/ClearAll",
    }).then((res) => {
        alert('Clear all is done');
        loadAggregatedValue(0, 200);
    })
}

$(document).ready(() => {
    loadData();
    loadAggregatedValue(0, 200);
})
