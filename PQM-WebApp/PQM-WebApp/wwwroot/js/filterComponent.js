let _districts;

const filterDetail = (label, value, tooltip) => {
    return `<div class="card" data-toggle="tooltip" data-placement="top" title="${tooltip}" style="padding: 8px;display: inline;font-size: 17px;background-color: whitesmoke; margin-right: 5px; min-width: max-content">
                <span>${label}:</span>
                <spanl style="font-weight: bold">${value}</span>
            </div>`
}

const filter = {
    onLanguageChange: (language) => {
        filter.buildForm();
        updateFilterDetail();
    },
    buildForm: () => {
        let lang = languages._language;
        $("#filterPanel").html(
        `<div class="col-12" style="padding: 0px !important">
            <div class="card px-2" style="background-color: white">
                <div class="container-fluid"> 
                    <div class="row">
                        <div class="col-lg-1 col-3 d-flex justify-content-center">
                            <img src="/images/usaid-logo.png" style="height: 50px" />
                        </div>
                        <div class="col-lg-10 col-7 d-flex align-items-center" style="overflow: scroll" id="filterDisplay">
                        </div>
                        <div class="col-lg-1 col-2 d-flex flex-row-reverse">
                            <button type="button" class="btn" onclick="openFilterPanel()" id="filter-btn">
                                <svg xmlns="http://www.w3.org/2000/svg" width="25" height="25" fill="currentColor" class="bi bi-filter" viewBox="0 0 16 16">
                                    <path d="M6 10.5a.5.5 0 0 1 .5-.5h3a.5.5 0 0 1 0 1h-3a.5.5 0 0 1-.5-.5zm-2-3a.5.5 0 0 1 .5-.5h7a.5.5 0 0 1 0 1h-7a.5.5 0 0 1-.5-.5zm-2-3a.5.5 0 0 1 .5-.5h11a.5.5 0 0 1 0 1h-11a.5.5 0 0 1-.5-.5z"/>
                                </svg>
                            </button>
                        </div>
                    </div>
                </div>
                <div class="d-block d-sm-none row">
                    <div class="col-12" id="filterDisplayMobile">
                    </div>
                </div>
            </div>
        </div>
        <div class="modal fade" id="filterModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle" aria-hidden="true">
          <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">
              <div class="modal-header">
                <h5 class="modal-title" id="exampleModalLongTitle">
                    ${languages.translate(lang, 'Filter by')}
                </h5>
                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                  <span aria-hidden="true">&times;</span>
                </button>
              </div>
              <div class="modal-body">
                <form>
                  <div class="form-group">
                    <label for="inputProvince">
                        ${languages.translate(lang, 'Province/City')}
                    </label>
                    <select id="inputProvince" class="form-control" onchange="onProvinceChange()" required>
                        <option value="79" selected>Ho Chi Minh City</option>
                        <option value="82">Tien Giang</option>
                        <option value="75">Dong Nai</option>
                        <option value="72">Tay Ninh</option>
                    </select>
                  </div>
                  <div class="form-group" id="inputDistrictContainer">
                    <label for="inputDistrict">
                        ${languages.translate(lang, 'District')}
                    </label>
                    <select id="inputDistrict" class="form-control">
                    </select>
                  </div>
                  <hr/>
                  <div class="form-group">
                    <div class="k-content">
                        <label for="year-picker">
                            ${languages.translate(lang, 'Year')}
                        </label>
                        <input class="form-control" id="year-picker" required />
                    </div>
                  </div>
                  <div class="form-group">
                    <label for="inputQuarter">
                        ${languages.translate(lang, 'Quarter')}
                    </label>
                    <select id="inputQuarter" class="form-control" onchange="onQuarterChange()" required>
                        <option value="1">${languages.translate(lang, 'Quarter')} 1</option>
                        <option value="2">${languages.translate(lang, 'Quarter')} 2</option>
                        <option value="3">${languages.translate(lang, 'Quarter')} 3</option>
                        <option value="4">${languages.translate(lang, 'Quarter')} 4</option>
                    </select>
                  </div>
                  <div class="form-group">
                    <label for="inputMonth">
                        ${languages.translate(lang, 'Month')}
                    </label>
                    <select id="inputMonth" class="form-control">
                    </select>
                  </div>
                </form>
              </div>
              <div class="modal-footer">
                <button type="button" class="btn btn-primary" onclick="onApplyFilter()">
                    ${languages.translate(lang, 'Process')}
                </button>
              </div>
            </div>
          </div>
        </div>`
        );
        $("#year-picker").kendoDatePicker({
            value: year,
            format: "yyyy",
            depth: "decade",
            start: "decade"
        });
        onQuarterChange();
        updateDistrictSelect(_districts);
    }
}

const initFilterPanel = () => {
    Promise.all([loadDistricts('79')])
        .then((res) => {
            _districts = res[0].data;
            filter.buildForm();
            updateFilterDetail();
            window.scroll(0, findPos(document.getElementById("filterPanel")));
        })
    languages.addCallback(filter.onLanguageChange);
}

const onApplyFilter = () => {
    $('#filterModal').modal('hide')
    applyFilter();
}

const openFilterPanel = () => {
    $('#filterModal').modal('show')
}

const loadDistricts = (provinceCode) => {
    return httpClient.callApi({
        method: 'GET',
        url: `/api/Locations/Districts?provinceCode=${provinceCode}`,
    })
}

const updateDistrictSelect = (districts) => {
    if (!districts) return;
    let data = districts.map(u => { text = u.nameWithType, value = u.code });
    $('#inputDistrictContainer')
        .html(`<label for="inputDistrict">${languages.translate('', 'District')}</label>
                        <select id="inputDistrict" class="form-control">
                        </select>`);
    $('#inputDistrict').kendoMultiSelect({
        dataTextField: "nameWithType",
        dataValueField: "code",
        dataSource: districts,
        filter: "contains",
    });
}

const onProvinceChange = () => {
    provinceCode = $('#inputProvince').val();
    loadDistricts(provinceCode).then((response) => {
        _districts = response.data;
        updateDistrictSelect(_districts);
    })
}

const onQuarterChange = () => {
    quarter = $('#inputQuarter').val();
    let from = quarter === "1" ? 1 : quarter === "2" ? 4 : quarter === "3" ? 7 : 10;
    let to = quarter === "1" ? 3 : quarter === "2" ? 6 : quarter === "3" ? 9 : 12;
    let months = [
        {
            name: 'January',
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
    let monthOptions = `<option value=""></option>${months.filter(m => from <= m.value && m.value <= to)
                                                          .map(m => `<option value='${m.value}'>${languages.translate('', m.name)}</option>`).join()}`;
    $('#inputMonth').html(monthOptions);
    $('#inputMonth').val(month);
}

const updateFilterDetail = () => {
    var htmlElement = ``;
    var province = $("#inputProvince option:selected").text()
    if (province && province.length > 0) {
        htmlElement += filterDetail(languages.translate('', 'Province/City'), province, province);
    }
    let districts = $("#inputDistrict").data("kendoMultiSelect").dataItems().map(m => m.nameWithType);
    let districtValue = '';
    let n = 1;
    while (districtValue.length < 25 && n <= districts.length) {
        districtValue = districts.slice(0, n).join(', ');
        n++
    }
    if (n <= districts.length) {
        districtValue = `${districtValue},...`;
    }
    if (districtValue.length > 0) {
        htmlElement += filterDetail(languages.translate('', 'District'), districtValue, districts.join(', '));
    }
    htmlElement += filterDetail(languages.translate('', 'Year'), $('#year-picker').val(), $('#year-picker').val());
    let quarter = $("#inputQuarter option:selected").text();
    htmlElement += filterDetail(languages.translate('', 'Quarter'), quarter, quarter);
    let month = $("#inputMonth option:selected").text();
    if (month.length > 0) {
        htmlElement += filterDetail(languages.translate('', 'Month'), month, month);
    }
    $("#filterDisplay").html(htmlElement);
}