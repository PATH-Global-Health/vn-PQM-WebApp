{
    var configPanelOpen = true;

    function initConfigPanel() {
        let _html =
        `<div class="col-12 dashboard-container">
            <div class="card dashboard-card" style="background-color: white; padding: 20px">
                <form>
                    <div class="row">
                        <div class="col">
                            <div class="form-group">
                                <label for="inputProvince">Province/City</label>
                                <select id="inputProvince" class="form-control" onchange="onProvinceChange()" required>
                                    <option value="79" selected>Ho Chi Minh City</option>
                                </select>
                            </div>
                        </div>
                        <div class="col">
                            <div class="form-group">
                                <label for="inputDistrict">District</label>
                                <select id="inputDistrict" class="form-control" required>
                                </select>
                            </div>
                        </div>
                        <div class="col">
                            <div class="k-content">
                                <label for="year-picker">Year</label>
                                <input class="form-control" id="year-picker" required />
                            </div>
                        </div>
                        <div class="col">
                            <div class="form-group">
                                <label for="inputQuarter">Quarter</label>
                                <select id="inputQuarter" class="form-control" onchange="onQuarterChange()" required>
                                    <option value="1">Quarter 1</option>
                                    <option value="2">Quarter 2</option>
                                    <option value="3">Quarter 3</option>
                                    <option value="4">Quarter 4</option>
                                </select>
                            </div>
                        </div>
                        <div class="col">
                            <div class="form-group">
                                <label for="inputMonth">Month</label>
                                <select id="inputMonth" class="form-control">
                                </select>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col d-flex justify-content-end">
                            <button type="button" class="btn btn-info" onclick="applyFilter()">Process</button>
                        </div>
                    </div>
                </form>
            </div>
        </div>`
        $("#configPanel").html(_html);
        $("#year-picker").kendoDatePicker({
            value: year,
            format: "yyyy",
            depth: "decade",
            start: "decade"
        });
    }

    function openConfig() {
        if (configPanelOpen) {
            $("#configPanel").css("height", "0px");
        } else {
            $("#configPanel").css("height", "200px");
        }

        configPanelOpen = !configPanelOpen;
    }

    function onProvinceChange() {
        provinceCode = $('#inputProvince').val();
        $.get(`/api/Locations/Districts?provinceCode=${provinceCode}`,
            function (districts) {
                let data = districts.map(u => { text = u.nameWithType, value = u.code });
                $('#inputDistrict').kendoMultiSelect({
                    dataTextField: "nameWithType",
                    dataValueField: "code",
                    dataSource: districts,
                    filter: "contains",
                });
                if (firstload) {
                    firstload = false;
                    var multiselect = $("#inputDistrict").data("kendoMultiSelect");

                    multiselect.value(districtCode.split(','));
                    multiselect.trigger("change");
                }
            }
        );
    }

    function onQuarterChange() {
        quarter = $('#inputQuarter').val();
        let from = quarter === "1" ? 1 : quarter === "2" ? 4 : quarter === "3" ? 7 : 10;
        let to = quarter === "1" ? 3 : quarter === "2" ? 6 : quarter === "3" ? 9 : 12;
        let months = [
            {
                name: 'Janaury',
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
        let monthOptions = `<option value=""></option>${months.filter(m => from <= m.value && m.value <= to).map(m => `<option value='${m.value}'>${m.name}</option>`).join()}`;
        $('#inputMonth').html(monthOptions);
        $('#inputMonth').val(month);
    }
}