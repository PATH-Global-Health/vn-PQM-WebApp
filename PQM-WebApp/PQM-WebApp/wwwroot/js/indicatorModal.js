let modal =
`<div class="modal fade" id="indicator-modal" tabindex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle" aria-hidden="true">
  <div class="modal-dialog modal-lg modal-dialog-centered" role="document">
    <div class="modal-content">
      <div class="modal-body" id="indicator-modal-body" style="margin: 10px">
        <div class="row">
         <div class="col-4 d-flex align-items-center" style="padding: 0px; background-color: whitesmoke">
            <div class="col-12 indicator-container" style="margin: 45px; max-width: 420px">
                <div class="d-flex flex-column justify-content-around indicator-infor">
                    <div class="row d-flex justify-content-center indicator-title" id="_title"></div>
                    <div class="row d-flex justify-content-center indicator-value">
                        <span id="_value"></span>
                    </div>
                    <div class="row d-flex justify-content-center indicator-percent">
                        <span class="align-middle" id="_percent">

                        </span>
                    </div>
                </div>
                <div id="_chart" class="indicator-infor-chart">

                </div>
            </div>
         </div>
         <div class="col-8" style="padding: 0px">
            <div class="col-12 d-flex justify-content-around" style="padding: 0px">
                <div class="col-6" style="padding: 0px">
                    <div class="col-12 variable-containter">
                        <div class="row" style="padding-left: 15px; padding-top: 15px">
                            <h4>Age Group</h4>
                        </div>
                        <div id="_age-group-chart"></div>
                    </div>
                </div>
                <div class="col-6" style="padding: 0px">
                    <div class="col-12 variable-containter">
                        <div class="row" style="padding-left: 15px; padding-top: 15px">
                            <h4>Gender</h4>
                        </div>
                        <div id="_gender-chart"></div>
                    </div>
                </div>
             </div>
             <div class="col-12 d-flex justify-content-around" style="padding: 0px">
                <div class="col-6" style="padding: 0px">
                    <div class="col-12 variable-containter">
                        <div class="row" style="padding-left: 15px; padding-top: 15px">
                            <h4>Key Populations</h4>
                        </div>
                        <div id="_key-populations-chart"></div>
                    </div>
                </div>
                <div class="col-6" style="padding: 0px">
                    <div class="col-12 variable-containter">
                        <div class="row" style="padding-left: 15px; padding-top: 15px">
                            <h4>Clinics</h4>
                        </div>
                        <div id="_clinics-chart"></div>
                    </div>
                </div>
             </div>
         </div>
        </div>
      </div>
    </div>
  </div>
</div>`;

$('body').append(modal);


const openIndicatorModal = (indicator, year, quarter, month, province, district, value) => {
    let _indicator = encodeURI(indicator);
    initDataChart(_indicator, "_chart");
    $('#indicator-modal-title').html(indicator);
    $('#_title').html(indicator);
    $('#_value').html(value.dataType === 1 ? value.value : (Math.round(((value.numerator / value.denominator) + Number.EPSILON) * 100) + '%'));
    $("#_value").css("color", value.criticalInfo);
    $('#indicator-modal').modal();
    $.get(`/api/AggregatedValues?year=${year}&quarter=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}&indicator=${_indicator}&groupBy=AgeGroup`,
        function (response) {
            _initAgeGroupChart(response, '_age-group-chart');
        }
    );
    $.get(`/api/AggregatedValues?year=${year}&quarter=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}&indicator=${_indicator}&groupBy=Sex`,
        function (response) {
            _initGenderChart(response, '_gender-chart');
        }
    );
    $.get(`/api/AggregatedValues?year=${year}&quarter=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}&indicator=${_indicator}&groupBy=KeyPopulation`,
        function (response) {
            _initKeyPopulationsChart(response, '_key-populations-chart');
        }
    );
    $.get(`/api/AggregatedValues?year=${year}&quarter=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}&indicator=${_indicator}&groupBy=Site`,
        function (response) {
            _initClinicsChart(response, '_clinics-chart');
        }
    );
};