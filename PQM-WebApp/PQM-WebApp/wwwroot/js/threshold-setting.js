const openCreateSettingModal = () => {
    $('#create-setting-modal-body').html(
        `<form>
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
            $('#inputDistrict').kendoDropDownList({
                optionLabel: "-- Chọn quận/huyện --",
                dataTextField: "nameWithType",
                dataValueField: "code",
                dataSource: districts,
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
            text: "Interruption in Treatment",
            value: "Interruption in Treatment",
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
            text: "HTS Positive",
            value: "HTS Positive",
        },
        {
            text: "%HIV+ referred",
            value: "%HIV+ referred",
        },
        {
            text: "%HTS recent",
            value: "%HTS recent",
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
                            <td>${m.provinceCode}</td>
                            <td>${m.districtCode}</td>
                            <td>${m.indicatorName}</td>
                            <td>${m.from}</td>
                            <td>${m.to}</td>
                            <td style="color: ${m.colorCode}">${m.colorCode}</td>
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
                districtCode: $('#inputDistrict').val(),
                provinceCode: $('#inputProvince').val(),
                indicatorName: $('#inputIndicator').val(),
                colorCode: $('#inputColor').val(),
                from: Number.parseInt($('#inputFrom').val()),
                to: Number.parseInt($('#inputTo').val())
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