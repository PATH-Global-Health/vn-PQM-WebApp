let modal =
`<div class="modal fade" id="indicator-modal" tabindex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle" aria-hidden="true">
  <div class="modal-dialog modal-lg modal-dialog-centered" role="document">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title" id="indicator-modal-title"></h5>
        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
          <span aria-hidden="true">&times;</span>
        </button>
      </div>
      <div class="modal-body" id="indicator-modal-body">
        
      </div>
    </div>
  </div>
</div>`;

$('body').append(modal);

const openIndicatorModal = (indicator, year, quarter, month, province, district) => {
    console.log(indicator, year, quarter, month, province, district);
    $('#indicator-modal-title').html(indicator);
    $('#indicator-modal').modal();
};