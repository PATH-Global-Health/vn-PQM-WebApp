const openCreateSettingModal = () => {
    $('#create-setting-modal-body').html(
        `<form>
            <div class="form-group">
                <label for="type">Loại ngưỡng:</label>
                <select id="type" class="form-control" required>
                    <option value="1" selected>Giá trị</option>
                    <option value="2">Mũi tên</option>
                </select>
            </div>
            <div class="form-group">
                <label>Tỉnh thành</label>
                <select id="inputProvince" class="form-control" onchange="onProvinceChange()" required>
                    <option value="79" selected>Thành phố Hồ Chí Minh</option>
                    <option value="82">Tỉnh Tiền Giang</option>
                    <option value="75">Tỉnh Đồng Nai</option>
                    <option value="72">Tỉnh Tây Ninh</option>
                </select>
            </div>
            <div class="form-group" id="inputDistrictContainer">
                <label for="inputDistrict">Quận huyện</label>
                <input id="inputDistrict" class="form-control" />
            </div>
            <div class="form-group">
                <label for="inputIndicator">Chỉ số</label>
                <input id="inputIndicator" class="form-control" />
            </div>
            <div iclass="form-group">
                <label>Từ</label>
                <input id="inputFrom" type="number" class="form-control">
            </div>
            <div class="form-group">
                <label>Đến</label>
                <input id="inputTo" type="number" class="form-control">
            </div>
            <div class="form-group">
                <label>Ngưỡng màu</label>
                <select id="inputColor" class="form-control" required>
                    <option value="#FF0000" selected style="color: #FF0000">Đỏ</option>
                    <option value="#FFD700" selected style="color: #FFD700">Vàng</option>
                    <option value="#228B22" selected style="color: #228B22">Xanh</option>
                </select>
            </div>
        </form>`)
    initIndicatorSelect();
    $('#create-setting-modal').modal('show');
    onProvinceChange();
}

const onProvinceChange = () => {
    provinceCode = $('#inputProvince').val();
    $.get(`/api/Locations/Districts?provinceCode=${provinceCode}`,
        function (districts) {
            $('#inputDistrictContainer')
                .html(`<label for="inputDistrict">Quận huyện</label>
                        <select id="inputDistrict" class="form-control">
                        </select>`);
            $('#inputDistrict').kendoMultiSelect({
                dataTextField: "nameWithType",
                dataValueField: "code",
                dataSource: districts,
                filter: "contains",
            });
        }
    );
}

let provinces = [
    {
        text: "Thành phố Hồ Chí Minh",
        value: "79",
    },
    {
        text: "Tỉnh Tiền Giang",
        value: "82",
    },
    {
        text: "Tỉnh Đồng Nai",
        value: "75",
    },
    {
        text: "Tỉnh Tây Ninh",
        value: "72",
    },
];

const initProvinceSelect = () => {
    $('#inputProvince').kendoMultiSelect({
        dataTextField: "text",
        dataValueField: "value",
        dataSource: data,
        index: 0,
    });
}

const initIndicatorSelect = () => {
    $('#inputIndicator').html('');
    let data = [
        {
            text: "TX_New",
            value: "TX_New",
        },
        {
            text: "TX_Curr",
            value: "TX_Curr",
        },
        {
            text: "MMD",
            value: "MMD",
        },
        {
            text: "TX_ML",
            value: "TX_ML",
        },
        {
            text: "% VL unsupressed",
            value: "% VL unsupressed",
        },
        {
            text: "PrEP NEW",
            value: "PrEP NEW",
        },
        {
            text: "PrEP CURR",
            value: "PrEP CURR",
        },
        {
            text: "%PrEP 3M",
            value: "%PrEP 3M",
        },
        {
            text: "HTS_TST_Positive",
            value: "HTS_TST_Positive",
        },
        {
            text: "HTS_TST_Recency",
            value: "HTS_TST_Recency",
        },
        {
            text: "POS_TO_ART",
            value: "POS_TO_ART",
        },
    ];
    $('#inputIndicator').kendoDropDownList({
        dataTextField: "text",
        dataValueField: "value",
        dataSource: data,
    });
}

const loadThresholdList = () => {
    let token = `Bearer ${getToken()}`;
    $.ajax({
        url: "/api/ThresholdSettings",
        type: "GET",
        beforeSend: function (xhr) { xhr.setRequestHeader('Authorization', token); },
        success: function (data) {
            let rows = data
                .map(m =>
                        `<tr>
                            <td>${m.username}</td>
                            <td>${m.type === 1 ? "Giá trị" : "Mũi tên"}</td>
                            <td>${m.provinceCode}</td>
                            <td>${m.districtCode}</td>
                            <td>${m.indicatorName}</td>
                            <td>${m.from}</td>
                            <td>${m.to}</td>
                            <td style="color: ${m.colorCode}">${m.colorCode}</td>
                            <td>
                                <button class="btn btn-outline-warning" style="margin-right: 5px">
                                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-pen" viewBox="0 0 16 16">
                                      <path d="m13.498.795.149-.149a1.207 1.207 0 1 1 1.707 1.708l-.149.148a1.5 1.5 0 0 1-.059 2.059L4.854 14.854a.5.5 0 0 1-.233.131l-4 1a.5.5 0 0 1-.606-.606l1-4a.5.5 0 0 1 .131-.232l9.642-9.642a.5.5 0 0 0-.642.056L6.854 4.854a.5.5 0 1 1-.708-.708L9.44.854A1.5 1.5 0 0 1 11.5.796a1.5 1.5 0 0 1 1.998-.001zm-.644.766a.5.5 0 0 0-.707 0L1.95 11.756l-.764 3.057 3.057-.764L14.44 3.854a.5.5 0 0 0 0-.708l-1.585-1.585z"/>
                                    </svg>
                                </button>
                                <button class="btn btn-outline-danger">
                                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-trash" viewBox="0 0 16 16">
                                      <path d="M5.5 5.5A.5.5 0 0 1 6 6v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5zm2.5 0a.5.5 0 0 1 .5.5v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5zm3 .5a.5.5 0 0 0-1 0v6a.5.5 0 0 0 1 0V6z"/>
                                      <path fill-rule="evenodd" d="M14.5 3a1 1 0 0 1-1 1H13v9a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V4h-.5a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1H6a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1h3.5a1 1 0 0 1 1 1v1zM4.118 4 4 4.059V13a1 1 0 0 0 1 1h6a1 1 0 0 0 1-1V4.059L11.882 4H4.118zM2.5 3V2h11v1h-11z"/>
                                    </svg>
                                </button>
                            </td>
                        </tr>`);
            $('#threshold-setting-list').html(rows);
        }
    });
}

const createThreshold = () => {
    let token = `Bearer ${getToken()}`;
    $.ajax({
        url: "/api/ThresholdSettings",
        type: "POST",
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(
            {
                districtCode: $('#inputDistrict').val().join(','),
                provinceCode: $('#inputProvince').val(),
                indicatorName: $('#inputIndicator').val(),
                colorCode: $('#inputColor').val(),
                from: Number.parseInt($('#inputFrom').val()),
                to: Number.parseInt($('#inputTo').val()),
                type: Number.parseInt($('#type').val())
            }),
        beforeSend: function (xhr) { xhr.setRequestHeader('Authorization', token); },
        success: function (data) {
            loadThresholdList();
            $('#create-setting-modal').modal('hide');
        }
    });
}

$(document).ready(() => {
    loadThresholdList();
});