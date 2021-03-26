﻿let provinceCode = '79';
let districtCode = '';
let year = '2020';
let month = '';
let quarter = '1';
let firstload = true;
let thresholdSettings = [];

function drillDown(groupIndicator) {
    if (groupIndicator !== 'PrEP' && groupIndicator !== 'Testing' && groupIndicator !== 'Treatment') return;
    console.log(`${groupIndicator} is clicked`);
    var win = window.open(`${groupIndicator}?year=${year}&quarter=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}`, '_blank');
    win.focus();
}

function fakeIndicator(name, value, valueColor, info, trend, hrTag) {
    let trendElement = trend.direction === 1
        ? `<svg xmlns="http://www.w3.org/2000/svg" color="${trend.color}" width="20" height="20" fill="currentColor" class="bi bi-caret-up-fill" viewBox="0 0 20 20">
                       <path d="M7.247 4.86l-4.796 5.481c-.566.647-.106 1.659.753 1.659h9.592a1 1 0 0 0 .753-1.659l-4.796-5.48a1 1 0 0 0-1.506 0z" />
                   </svg>`
        : `<svg xmlns="http://www.w3.org/2000/svg" color="${trend.color}" width="20" height="20" fill="currentColor" class="bi bi-caret-down-fill" viewBox="0 0 20 20">
                       <path d="M7.247 11.14L2.451 5.658C1.885 5.013 2.345 4 3.204 4h9.592a1 1 0 0 1 .753 1.659l-4.796 5.48a1 1 0 0 1-1.506 0z" />
                   </svg>`;
    let infoElement = info
        ? `<svg xmlns="http://www.w3.org/2000/svg" color="${info}" width="20" height="20" fill="currentColor" class="bi bi-record-fill" viewBox="0 0 20 20">
                       <path fill-rule="evenodd" d="M8 13A5 5 0 1 0 8 3a5 5 0 0 0 0 10z" />
                   </svg>`
        : `<svg width="20" height="20" opacity="0"></svg>`;
    let hrElement = hrTag ? '<hr />' : '';
    return `<div class="d-flex justify-content-between dashboard-indicator">
                        <span class="align-middle">${name}</span>
                        <span class="align-middle" style="color: ${valueColor}; font-size: 23px; font-weight: bold">
                            ${value}
                            ${infoElement}
                            ${trendElement}
                        </span>
                    </div>
                    ${hrElement}`;
}

function createIndicator(indicator, hrTag) {
    let _trendElement = indicator.trend !== null ? (indicator.trend.direction === 1
        ? `<svg xmlns="http://www.w3.org/2000/svg" color="${indicator.trend.criticalInfo}" width="20" height="20" fill="currentColor" class="bi bi-caret-up-fill" viewBox="0 0 20 20">
                       <path d="M7.247 4.86l-4.796 5.481c-.566.647-.106 1.659.753 1.659h9.592a1 1 0 0 0 .753-1.659l-4.796-5.48a1 1 0 0 0-1.506 0z" />
                   </svg>`
        : `<svg xmlns="http://www.w3.org/2000/svg" color="${indicator.trend.criticalInfo ? indicator.trend.criticalInfo : "black"}" width="20" height="20" fill="currentColor" class="bi bi-caret-down-fill" viewBox="0 0 20 20">
                       <path d="M7.247 11.14L2.451 5.658C1.885 5.013 2.345 4 3.204 4h9.592a1 1 0 0 1 .753 1.659l-4.796 5.48a1 1 0 0 1-1.506 0z" />
                   </svg>`) : '';
    let _infoElement = indicator.criticalInfo
        ? `<svg xmlns="http://www.w3.org/2000/svg" color="${indicator.criticalInfo}" width="20" height="20" fill="currentColor" class="bi bi-record-fill" viewBox="0 0 20 20">
                       <path fill-rule="evenodd" d="M8 13A5 5 0 1 0 8 3a5 5 0 0 0 0 10z" />
                   </svg>`
        : `<svg width="20" height="20" opacity="0"></svg>`;
    let _hrElement = hrTag ? '<hr />' : '';
    return `<div class="d-flex justify-content-between dashboard-indicator">
                        <span class="align-middle">${indicator.name}</span>
                        <span class="align-middle" style="color: ${indicator.value.criticalInfo}; font-size: 23px; font-weight: bold">
                            ${indicator.value.dataType === 1 ? indicator.value.value : (Math.round(((indicator.value.numerator / indicator.value.denominator) + Number.EPSILON) * 100) + '%')}
                            ${_infoElement}
                            ${_trendElement}
                        </span>
                    </div>
                    ${_hrElement}`;
}

function initTestingIndicators() {
    $("#testing").html('');
    $.get(`/api/Testing/indicators?year=${year}&quater=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}`, function (data) {
        data.forEach((indicator, idx, array) => {
            $("#testing").append(createIndicator(indicator, idx !== array.length - 1));
        });
    });
}

function initPrEPIndicators() {
    $("#prep").html('');
    $.get(`/api/PrEP/indicators?year=${year}&quater=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}`, function (data) {
        data.forEach((indicator, idx, array) => {
            $("#prep").append(createIndicator(indicator, idx !== array.length - 1));
        });
    });
    //$("#prep").append(fakeIndicator('PrEP_Curr', '1,925', 'green', null, { direction: 1, color: 'green' }, true));
    //$("#prep").append(fakeIndicator('%PrEP_3M', '92%', 'green', null, { direction: 1, color: 'green' }, false));
}

function initTreatmentIndicators() {
    $("#treatment").html('');
    $.get(`/api/Treatment/indicators?year=${year}&quater=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}`, function (data) {
        data.forEach((indicator, idx, array) => {
            $("#treatment").append(createIndicator(indicator, idx !== array.length - 1));
        });
    });
}

function initDrugsIndicators() {
    $("#drugs").append(fakeIndicator('%ARV drug consumption', '98%', 'green', null, { direction: 1, color: 'green' }, true));
    $("#drugs").append(fakeIndicator('%ARV drugs that have MOS', '79%', 'orange', null, { direction: 1, color: 'green' }, false));
}

function initSHIIndicators() {
    $("#shi").append(fakeIndicator('Patients do not </br> have a SHI card', '13', 'red', null, { direction: -1, color: 'green' }, true));
    $("#shi").append(fakeIndicator('TX_Curr using ARV from SHI', '459', 'green', null, { direction: 1, color: 'green' }, true));
    $("#shi").append(fakeIndicator('TX_Curr receiving VL test from SHI', '441', 'green', 'red', { direction: 1, color: 'green' }, true));
    $("#shi").append(fakeIndicator('% of TX_Curr receiving </br> ARV MMD having a SHI card', '97%', 'green', null, { direction: 1, color: 'green' }, false));
}

function initServiceQualityIndicators() {
    $("#service-quality").append(fakeIndicator('% clients know their VL status', '77%', 'red', 'red', { direction: -1, color: 'green' }, true));
    $("#service-quality").append(fakeIndicator('% of clients experience stigma', '08%', 'green', null, { direction: 1, color: 'red' }, true));
    $("#service-quality").append(fakeIndicator('% clients experience IPV/coercion </br> during testing services', '04%', 'green', null, { direction: -1, color: 'green' }, true));
    $("#service-quality").append(fakeIndicator('% of clients seeking services at private clinics', '47%', 'green', null, { direction: -1, color: 'green' }, true));
    $("#service-quality").append(fakeIndicator('%HTS_TST_1st_Time', '04%', 'green', null, { direction: 1, color: 'red' }, false));
}

function initIndicators() {
    console.log('init indicators...');
    $("#testing").html('');
    $("#prep").html('');
    $("#treatment").html('');
    $("#drugs").html('');
    $("#shi").html('');
    $("#service-quality").html('');
    $.get(`/api/AggregatedValues/IndicatorValues?year=${year}&quarter=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}`, function (data) {
        let prep = [];
        let testing = [];
        let treatment = [];
        data.forEach(e => {
            if (e.group === "PrEP") {
                prep.push(e);
            } else if (e.group === "Testing") {
                testing.push(e);
            } else if (e.group === "Treatment") {
                treatment.push(e);
            }
        })
        console.log(prep);
        console.log(testing);
        console.log(treatment);
        prep.forEach((indicator, idx, array) => {
            $("#prep").append(createIndicator(indicator, idx !== array.length - 1));
        })
        testing.forEach((indicator, idx, array) => {
            $("#testing").append(createIndicator(indicator, idx !== array.length - 1));
        })
        treatment.forEach((indicator, idx, array) => {
            $("#treatment").append(createIndicator(indicator, idx !== array.length - 1));
        })
        initDrugsIndicators();
        initSHIIndicators();
        initServiceQualityIndicators();
    });
}

function getHCMboundaries() {
    return JSON.parse(`[{"lng":106.96268463,"lat":10.43849468},{"lng":106.97260284,"lat":10.43316078},{"lng":106.98229218,"lat":10.42474556},{"lng":106.98529816,"lat":10.4182024},{"lng":106.98561096,"lat":10.41319561},{"lng":106.98096466,"lat":10.40523052},{"lng":106.97621918,"lat":10.39969254},{"lng":106.97055054,"lat":10.39481354},{"lng":106.94264984,"lat":10.38544941},{"lng":106.91846466,"lat":10.37398052},{"lng":106.89455414,"lat":10.3650856},{"lng":106.87587738,"lat":10.36071873},{"lng":106.86758423,"lat":10.35986996},{"lng":106.85276794,"lat":10.35833168},{"lng":106.83459473,"lat":10.35422611},{"lng":106.82253265,"lat":10.3774128},{"lng":106.81508636,"lat":10.39125824},{"lng":106.80444336,"lat":10.40364456},{"lng":106.77774048,"lat":10.43307114},{"lng":106.77225494,"lat":10.44035816},{"lng":106.76315308,"lat":10.45245361},{"lng":106.75685883,"lat":10.46256828},{"lng":106.75231171,"lat":10.47235394},{"lng":106.74539185,"lat":10.4884367},{"lng":106.74358368,"lat":10.49762726},{"lng":106.74401855,"lat":10.4999876},{"lng":106.7484436,"lat":10.51856136},{"lng":106.74909973,"lat":10.53071404},{"lng":106.74417877,"lat":10.55299854},{"lng":106.74293518,"lat":10.55956364},{"lng":106.74196625,"lat":10.56537151},{"lng":106.74263,"lat":10.57101822},{"lng":106.74528503,"lat":10.57567596},{"lng":106.74121857,"lat":10.57910633},{"lng":106.73812866,"lat":10.57842541},{"lng":106.73670959,"lat":10.57692528},{"lng":106.73513794,"lat":10.57758904},{"lng":106.7350235,"lat":10.57893467},{"lng":106.73892212,"lat":10.5814085},{"lng":106.73948669,"lat":10.5827837},{"lng":106.73897552,"lat":10.58509445},{"lng":106.7370224,"lat":10.58633423},{"lng":106.73522186,"lat":10.58590126},{"lng":106.73495483,"lat":10.58269978},{"lng":106.7327652,"lat":10.58020306},{"lng":106.72871399,"lat":10.57995701},{"lng":106.72660828,"lat":10.58429718},{"lng":106.72718048,"lat":10.58550167},{"lng":106.72554779,"lat":10.58515549},{"lng":106.72328949,"lat":10.58806896},{"lng":106.72357178,"lat":10.59109592},{"lng":106.72350311,"lat":10.5935936},{"lng":106.7202301,"lat":10.59630871},{"lng":106.72054291,"lat":10.59771538},{"lng":106.72229004,"lat":10.5995369},{"lng":106.71966553,"lat":10.60265923},{"lng":106.71880341,"lat":10.60590076},{"lng":106.72231293,"lat":10.6073122},{"lng":106.72386932,"lat":10.61190033},{"lng":106.72662354,"lat":10.61115265},{"lng":106.72859192,"lat":10.60868645},{"lng":106.72904205,"lat":10.60923195},{"lng":106.73153687,"lat":10.60883236},{"lng":106.73282623,"lat":10.60990143},{"lng":106.73152924,"lat":10.61217308},{"lng":106.72907257,"lat":10.61211681},{"lng":106.72763824,"lat":10.61568737},{"lng":106.72790527,"lat":10.6167202},{"lng":106.72930908,"lat":10.61647987},{"lng":106.72996521,"lat":10.62036324},{"lng":106.73186493,"lat":10.62274551},{"lng":106.73155975,"lat":10.62365341},{"lng":106.72836304,"lat":10.62432098},{"lng":106.72905731,"lat":10.62808704},{"lng":106.72981262,"lat":10.62858009},{"lng":106.73167419,"lat":10.62771511},{"lng":106.73331451,"lat":10.62884045},{"lng":106.73116302,"lat":10.63293839},{"lng":106.73142242,"lat":10.63391685},{"lng":106.72962952,"lat":10.636446},{"lng":106.72559357,"lat":10.63822365},{"lng":106.72489929,"lat":10.64009857},{"lng":106.7232132,"lat":10.6414299},{"lng":106.72258759,"lat":10.6458683},{"lng":106.72129059,"lat":10.64624882},{"lng":106.71875763,"lat":10.64558506},{"lng":106.71204376,"lat":10.65030003},{"lng":106.7075119,"lat":10.64988232},{"lng":106.70483398,"lat":10.65092564},{"lng":106.70396423,"lat":10.65317345},{"lng":106.70186615,"lat":10.6530838},{"lng":106.7009201,"lat":10.65394974},{"lng":106.69730377,"lat":10.6519022},{"lng":106.69621277,"lat":10.6498003},{"lng":106.69133759,"lat":10.64849567},{"lng":106.69049072,"lat":10.64884949},{"lng":106.69036102,"lat":10.65137291},{"lng":106.68843842,"lat":10.65341568},{"lng":106.68764496,"lat":10.65572929},{"lng":106.68532562,"lat":10.65672874},{"lng":106.68574524,"lat":10.65327835},{"lng":106.68509674,"lat":10.65099239},{"lng":106.67906952,"lat":10.64561272},{"lng":106.67420197,"lat":10.64449501},{"lng":106.67282104,"lat":10.64265251},{"lng":106.66960144,"lat":10.63920212},{"lng":106.66814423,"lat":10.63912201},{"lng":106.66662598,"lat":10.64030266},{"lng":106.66609192,"lat":10.63847256},{"lng":106.66564941,"lat":10.63964367},{"lng":106.66494751,"lat":10.63886738},{"lng":106.66337585,"lat":10.64013672},{"lng":106.66247559,"lat":10.63937569},{"lng":106.66016388,"lat":10.63981438},{"lng":106.66077423,"lat":10.64197731},{"lng":106.65694427,"lat":10.64355087},{"lng":106.65627289,"lat":10.63836288},{"lng":106.65401459,"lat":10.63956928},{"lng":106.6529541,"lat":10.63798904},{"lng":106.65352631,"lat":10.63333321},{"lng":106.6529541,"lat":10.63246822},{"lng":106.64865875,"lat":10.63275433},{"lng":106.64620972,"lat":10.63421822},{"lng":106.64380646,"lat":10.62958241},{"lng":106.64099121,"lat":10.62666512},{"lng":106.63885498,"lat":10.62749863},{"lng":106.63484192,"lat":10.62543392},{"lng":106.63214874,"lat":10.62821579},{"lng":106.63020325,"lat":10.62669468},{"lng":106.62966156,"lat":10.62893581},{"lng":106.63085938,"lat":10.63019466},{"lng":106.62963867,"lat":10.63164234},{"lng":106.62937164,"lat":10.63531685},{"lng":106.62950897,"lat":10.63633728},{"lng":106.63117981,"lat":10.63680363},{"lng":106.63058472,"lat":10.63905144},{"lng":106.62963104,"lat":10.63944435},{"lng":106.62994385,"lat":10.6404295},{"lng":106.62949371,"lat":10.64203835},{"lng":106.62722015,"lat":10.64240265},{"lng":106.6258316,"lat":10.63972378},{"lng":106.62415314,"lat":10.63941765},{"lng":106.62120056,"lat":10.64095306},{"lng":106.62186432,"lat":10.64410877},{"lng":106.62434387,"lat":10.64448929},{"lng":106.62394714,"lat":10.64635181},{"lng":106.62479401,"lat":10.64742565},{"lng":106.61896515,"lat":10.64901733},{"lng":106.61902618,"lat":10.64960384},{"lng":106.61734009,"lat":10.650177},{"lng":106.61778259,"lat":10.65167141},{"lng":106.61684418,"lat":10.65217876},{"lng":106.61675262,"lat":10.65331268},{"lng":106.61567688,"lat":10.65335178},{"lng":106.61557007,"lat":10.65411091},{"lng":106.61419678,"lat":10.65384674},{"lng":106.61083984,"lat":10.6568985},{"lng":106.60594177,"lat":10.65802479},{"lng":106.60541534,"lat":10.65877151},{"lng":106.60151672,"lat":10.65105057},{"lng":106.60064697,"lat":10.65073204},{"lng":106.59963989,"lat":10.65258789},{"lng":106.59535217,"lat":10.6537838},{"lng":106.59564972,"lat":10.65109634},{"lng":106.59515381,"lat":10.65099239},{"lng":106.59177399,"lat":10.65007687},{"lng":106.5904541,"lat":10.65307045},{"lng":106.58744812,"lat":10.6528101},{"lng":106.58413696,"lat":10.65368176},{"lng":106.58332062,"lat":10.65540314},{"lng":106.57975006,"lat":10.65610218},{"lng":106.57791901,"lat":10.65326977},{"lng":106.57802582,"lat":10.65186787},{"lng":106.5795517,"lat":10.65186787},{"lng":106.57939148,"lat":10.65033436},{"lng":106.57700348,"lat":10.64886761},{"lng":106.57514954,"lat":10.64986324},{"lng":106.57339478,"lat":10.6498127},{"lng":106.57351685,"lat":10.65415668},{"lng":106.56904602,"lat":10.65651131},{"lng":106.56629181,"lat":10.65643883},{"lng":106.56529999,"lat":10.65229321},{"lng":106.56323242,"lat":10.65219784},{"lng":106.56315613,"lat":10.65143681},{"lng":106.55903625,"lat":10.65185356},{"lng":106.5592041,"lat":10.64991379},{"lng":106.55854797,"lat":10.64862728},{"lng":106.55788422,"lat":10.64855385},{"lng":106.55744934,"lat":10.650383},{"lng":106.5555954,"lat":10.6509037},{"lng":106.55529022,"lat":10.65239429},{"lng":106.55593872,"lat":10.65696144},{"lng":106.54891205,"lat":10.65744114},{"lng":106.54927063,"lat":10.65965176},{"lng":106.55049896,"lat":10.65906525},{"lng":106.55163574,"lat":10.66426468},{"lng":106.55323792,"lat":10.66613388},{"lng":106.55358887,"lat":10.66853714},{"lng":106.55208588,"lat":10.67063904},{"lng":106.55319214,"lat":10.67186737},{"lng":106.55352783,"lat":10.67536926},{"lng":106.55157471,"lat":10.67943573},{"lng":106.55052185,"lat":10.68543911},{"lng":106.54763031,"lat":10.68676472},{"lng":106.54742432,"lat":10.68698311},{"lng":106.53559113,"lat":10.6856575},{"lng":106.53404999,"lat":10.68789387},{"lng":106.52891541,"lat":10.70250225},{"lng":106.52724457,"lat":10.70925808},{"lng":106.52844238,"lat":10.71496964},{"lng":106.53022766,"lat":10.7172842},{"lng":106.5202179,"lat":10.72367764},{"lng":106.5109787,"lat":10.71797085},{"lng":106.50934601,"lat":10.71717739},{"lng":106.50167847,"lat":10.72650909},{"lng":106.49967957,"lat":10.7249918},{"lng":106.49578094,"lat":10.72762871},{"lng":106.49157715,"lat":10.72433376},{"lng":106.48212433,"lat":10.73545647},{"lng":106.47451019,"lat":10.74544334},{"lng":106.47351074,"lat":10.74640179},{"lng":106.47227478,"lat":10.74545765},{"lng":106.46369171,"lat":10.75651646},{"lng":106.47219849,"lat":10.76306343},{"lng":106.48714447,"lat":10.77430725},{"lng":106.50457001,"lat":10.78895473},{"lng":106.50574493,"lat":10.79374599},{"lng":106.52006531,"lat":10.85225105},{"lng":106.52580261,"lat":10.87364769},{"lng":106.52589417,"lat":10.87363148},{"lng":106.53219604,"lat":10.89753532},{"lng":106.53292847,"lat":10.90035534},{"lng":106.53160095,"lat":10.90264511},{"lng":106.4960022,"lat":10.9208374},{"lng":106.48851776,"lat":10.92452717},{"lng":106.48738861,"lat":10.92506123},{"lng":106.4535141,"lat":10.94188023},{"lng":106.44372559,"lat":10.94937515},{"lng":106.44029236,"lat":10.95210171},{"lng":106.4260788,"lat":10.96306133},{"lng":106.41144562,"lat":10.97384357},{"lng":106.37297058,"lat":10.96974373},{"lng":106.36173248,"lat":10.98468399},{"lng":106.35667419,"lat":10.99161911},{"lng":106.36030579,"lat":10.99466324},{"lng":106.35996246,"lat":10.99531841},{"lng":106.36206818,"lat":10.99624157},{"lng":106.36325073,"lat":10.99766827},{"lng":106.36360168,"lat":10.99726772},{"lng":106.36695099,"lat":10.99870491},{"lng":106.37725067,"lat":11.00042725},{"lng":106.3807373,"lat":10.9984045},{"lng":106.39203644,"lat":10.99979115},{"lng":106.4052887,"lat":11.00379848},{"lng":106.40633392,"lat":11.00621319},{"lng":106.41052246,"lat":11.00741386},{"lng":106.41281891,"lat":11.00893593},{"lng":106.41127777,"lat":11.01317501},{"lng":106.40910339,"lat":11.01472473},{"lng":106.40953064,"lat":11.01785946},{"lng":106.40891266,"lat":11.02243042},{"lng":106.41138458,"lat":11.02925491},{"lng":106.41330719,"lat":11.03193569},{"lng":106.41517639,"lat":11.03415108},{"lng":106.41622162,"lat":11.03410912},{"lng":106.41812897,"lat":11.03601456},{"lng":106.42008209,"lat":11.03616428},{"lng":106.42219543,"lat":11.04146576},{"lng":106.42163086,"lat":11.04197598},{"lng":106.42273712,"lat":11.04277325},{"lng":106.42221069,"lat":11.04454041},{"lng":106.4233551,"lat":11.04541016},{"lng":106.42259216,"lat":11.04658604},{"lng":106.42430878,"lat":11.04711151},{"lng":106.42432404,"lat":11.04778385},{"lng":106.42034912,"lat":11.05192184},{"lng":106.42063141,"lat":11.05303383},{"lng":106.42132568,"lat":11.05528355},{"lng":106.42053986,"lat":11.05823421},{"lng":106.42227173,"lat":11.0591383},{"lng":106.42069244,"lat":11.06109619},{"lng":106.42157745,"lat":11.06189537},{"lng":106.4201889,"lat":11.06336403},{"lng":106.42108154,"lat":11.06458378},{"lng":106.42073059,"lat":11.06513596},{"lng":106.42179871,"lat":11.06629086},{"lng":106.42575073,"lat":11.06655884},{"lng":106.42523193,"lat":11.06881142},{"lng":106.42735291,"lat":11.07401466},{"lng":106.42602539,"lat":11.07794571},{"lng":106.42654419,"lat":11.08413792},{"lng":106.42597198,"lat":11.09161949},{"lng":106.43084717,"lat":11.092062},{"lng":106.4315567,"lat":11.09829044},{"lng":106.43228912,"lat":11.09829235},{"lng":106.43309021,"lat":11.10030174},{"lng":106.43384552,"lat":11.10022259},{"lng":106.43740845,"lat":11.10990143},{"lng":106.43781281,"lat":11.1121664},{"lng":106.44238281,"lat":11.11583996},{"lng":106.44478607,"lat":11.12676239},{"lng":106.4484024,"lat":11.13087082},{"lng":106.44577789,"lat":11.13583183},{"lng":106.45089722,"lat":11.13643742},{"lng":106.45631409,"lat":11.14148712},{"lng":106.45828247,"lat":11.14795971},{"lng":106.45774841,"lat":11.152915},{"lng":106.45633698,"lat":11.15340233},{"lng":106.45269012,"lat":11.15144634},{"lng":106.45181274,"lat":11.15198231},{"lng":106.45146179,"lat":11.15348244},{"lng":106.45276642,"lat":11.15817738},{"lng":106.45614624,"lat":11.16030979},{"lng":106.45967102,"lat":11.1583643},{"lng":106.46385193,"lat":11.16001034},{"lng":106.46629333,"lat":11.15721703},{"lng":106.4709549,"lat":11.15525436},{"lng":106.47116852,"lat":11.15363598},{"lng":106.46723938,"lat":11.14788532},{"lng":106.47228241,"lat":11.14334393},{"lng":106.47945404,"lat":11.14227581},{"lng":106.48242188,"lat":11.14249992},{"lng":106.48840332,"lat":11.14437008},{"lng":106.49146271,"lat":11.14450836},{"lng":106.49491119,"lat":11.14361858},{"lng":106.50146484,"lat":11.14015388},{"lng":106.50615692,"lat":11.14203072},{"lng":106.50764465,"lat":11.14196968},{"lng":106.50859833,"lat":11.1392498},{"lng":106.51171112,"lat":11.13490105},{"lng":106.51373291,"lat":11.12879276},{"lng":106.51531219,"lat":11.12633705},{"lng":106.51737213,"lat":11.12421513},{"lng":106.52135468,"lat":11.12180328},{"lng":106.52617645,"lat":11.12147331},{"lng":106.52696991,"lat":11.12033272},{"lng":106.52687073,"lat":11.11788559},{"lng":106.52780151,"lat":11.11462498},{"lng":106.52602386,"lat":11.10527802},{"lng":106.52416229,"lat":11.10372353},{"lng":106.5165329,"lat":11.10161018},{"lng":106.5161972,"lat":11.10080433},{"lng":106.51744843,"lat":11.09751129},{"lng":106.52316284,"lat":11.08986378},{"lng":106.52443695,"lat":11.08954239},{"lng":106.52729034,"lat":11.09165955},{"lng":106.53016663,"lat":11.09240627},{"lng":106.5326004,"lat":11.09204483},{"lng":106.53413391,"lat":11.09020138},{"lng":106.53305817,"lat":11.08732128},{"lng":106.52606201,"lat":11.07976913},{"lng":106.52383423,"lat":11.07366276},{"lng":106.52522278,"lat":11.06979275},{"lng":106.52971649,"lat":11.06565762},{"lng":106.53168488,"lat":11.06127644},{"lng":106.53431702,"lat":11.05754757},{"lng":106.54626465,"lat":11.05213261},{"lng":106.54988861,"lat":11.04628468},{"lng":106.55878448,"lat":11.04554939},{"lng":106.56461334,"lat":11.04493427},{"lng":106.56757355,"lat":11.04544353},{"lng":106.56842041,"lat":11.04661942},{"lng":106.56900787,"lat":11.05075264},{"lng":106.57167816,"lat":11.05429459},{"lng":106.57266235,"lat":11.05462742},{"lng":106.57962799,"lat":11.04769802},{"lng":106.58115387,"lat":11.04162025},{"lng":106.5827713,"lat":11.03821468},{"lng":106.58648682,"lat":11.03640652},{"lng":106.59104156,"lat":11.03690529},{"lng":106.59281158,"lat":11.04323578},{"lng":106.59618378,"lat":11.04575348},{"lng":106.60022736,"lat":11.04474831},{"lng":106.60299683,"lat":11.04110622},{"lng":106.60329437,"lat":11.03957367},{"lng":106.5955658,"lat":11.02997684},{"lng":106.59519196,"lat":11.02565098},{"lng":106.59751129,"lat":11.01683235},{"lng":106.59931183,"lat":11.01463699},{"lng":106.60834503,"lat":11.01183701},{"lng":106.6158905,"lat":11.0114851},{"lng":106.61750031,"lat":11.01102161},{"lng":106.62005615,"lat":11.0081625},{"lng":106.62130737,"lat":11.00367737},{"lng":106.62101746,"lat":11.00187969},{"lng":106.61668396,"lat":10.99404144},{"lng":106.61582184,"lat":10.99090195},{"lng":106.61708832,"lat":10.98809814},{"lng":106.62195587,"lat":10.98477173},{"lng":106.62506104,"lat":10.98107719},{"lng":106.62792969,"lat":10.9794836},{"lng":106.63691711,"lat":10.97761059},{"lng":106.64331055,"lat":10.9809351},{"lng":106.64602661,"lat":10.98156071},{"lng":106.64839935,"lat":10.98082161},{"lng":106.65019226,"lat":10.97298336},{"lng":106.64888,"lat":10.96921921},{"lng":106.64883423,"lat":10.96434021},{"lng":106.65205383,"lat":10.95275021},{"lng":106.65224457,"lat":10.9473381},{"lng":106.65132904,"lat":10.94247723},{"lng":106.65007782,"lat":10.93789864},{"lng":106.65042877,"lat":10.93391609},{"lng":106.65323639,"lat":10.9293108},{"lng":106.65723419,"lat":10.92593575},{"lng":106.66480255,"lat":10.92188263},{"lng":106.66790771,"lat":10.92094231},{"lng":106.68076324,"lat":10.923522},{"lng":106.68489075,"lat":10.92113972},{"lng":106.69002533,"lat":10.91112614},{"lng":106.69274902,"lat":10.90417194},{"lng":106.6945343,"lat":10.89624405},{"lng":106.69381714,"lat":10.89348125},{"lng":106.69149017,"lat":10.88936901},{"lng":106.68901825,"lat":10.88497162},{"lng":106.68930817,"lat":10.88119125},{"lng":106.69688416,"lat":10.87129593},{"lng":106.69986725,"lat":10.86843395},{"lng":106.70280457,"lat":10.86696148},{"lng":106.71322632,"lat":10.86588955},{"lng":106.7155838,"lat":10.86853313},{"lng":106.71492004,"lat":10.87223148},{"lng":106.71539307,"lat":10.87276077},{"lng":106.72353363,"lat":10.87184715},{"lng":106.72124481,"lat":10.8763628},{"lng":106.71528625,"lat":10.87747288},{"lng":106.71405792,"lat":10.87696648},{"lng":106.71380615,"lat":10.87786388},{"lng":106.71259308,"lat":10.87794971},{"lng":106.71379852,"lat":10.87914944},{"lng":106.71795654,"lat":10.88085461},{"lng":106.71658325,"lat":10.88471127},{"lng":106.71753693,"lat":10.8878336},{"lng":106.71560669,"lat":10.887743},{"lng":106.71447754,"lat":10.88678932},{"lng":106.71382141,"lat":10.88752747},{"lng":106.71450043,"lat":10.88816833},{"lng":106.71310425,"lat":10.88988972},{"lng":106.71761322,"lat":10.89638042},{"lng":106.72640991,"lat":10.89070511},{"lng":106.72754669,"lat":10.89013386},{"lng":106.72846222,"lat":10.89092541},{"lng":106.72924805,"lat":10.88943386},{"lng":106.73244476,"lat":10.89064026},{"lng":106.73131561,"lat":10.8859539},{"lng":106.73668671,"lat":10.88458538},{"lng":106.73420715,"lat":10.88051987},{"lng":106.73815155,"lat":10.87915993},{"lng":106.73966217,"lat":10.88103199},{"lng":106.74118805,"lat":10.88135529},{"lng":106.74299622,"lat":10.88031864},{"lng":106.74504852,"lat":10.88158226},{"lng":106.74568176,"lat":10.88077736},{"lng":106.74468994,"lat":10.88007259},{"lng":106.74718475,"lat":10.87738514},{"lng":106.74849701,"lat":10.8782692},{"lng":106.74942017,"lat":10.87678528},{"lng":106.75043488,"lat":10.87722492},{"lng":106.75136566,"lat":10.87548447},{"lng":106.7464447,"lat":10.87387753},{"lng":106.74704742,"lat":10.87207031},{"lng":106.7460022,"lat":10.87172985},{"lng":106.74793243,"lat":10.86929798},{"lng":106.74790192,"lat":10.8683691},{"lng":106.74871063,"lat":10.86563492},{"lng":106.75052643,"lat":10.86402607},{"lng":106.75122833,"lat":10.8644352},{"lng":106.75176239,"lat":10.86336994},{"lng":106.75463867,"lat":10.86631012},{"lng":106.76221466,"lat":10.86726284},{"lng":106.76328278,"lat":10.86993313},{"lng":106.76039124,"lat":10.87238789},{"lng":106.76180267,"lat":10.87439537},{"lng":106.76133728,"lat":10.87455368},{"lng":106.76192474,"lat":10.87556076},{"lng":106.76141357,"lat":10.87851048},{"lng":106.76196289,"lat":10.87871075},{"lng":106.76052856,"lat":10.8812542},{"lng":106.76455688,"lat":10.88485432},{"lng":106.76625061,"lat":10.88915539},{"lng":106.76689148,"lat":10.89153862},{"lng":106.76769257,"lat":10.89162445},{"lng":106.76727295,"lat":10.89249706},{"lng":106.76843262,"lat":10.89233589},{"lng":106.76843262,"lat":10.89360428},{"lng":106.76967621,"lat":10.8938446},{"lng":106.77554321,"lat":10.88721561},{"lng":106.77661896,"lat":10.88508797},{"lng":106.78053284,"lat":10.88149929},{"lng":106.78208923,"lat":10.8764019},{"lng":106.78191376,"lat":10.87396526},{"lng":106.77994537,"lat":10.86838722},{"lng":106.78775787,"lat":10.86721039},{"lng":106.78803253,"lat":10.87037277},{"lng":106.78892517,"lat":10.87195206},{"lng":106.79216766,"lat":10.87197495},{"lng":106.7949295,"lat":10.87353802},{"lng":106.79750824,"lat":10.87322235},{"lng":106.7986145,"lat":10.872015},{"lng":106.80128479,"lat":10.87359619},{"lng":106.80441284,"lat":10.87370396},{"lng":106.80524445,"lat":10.87504578},{"lng":106.80857086,"lat":10.87302685},{"lng":106.81279755,"lat":10.8788271},{"lng":106.81381226,"lat":10.87842369},{"lng":106.81630707,"lat":10.88135242},{"lng":106.81729889,"lat":10.88133144},{"lng":106.81780243,"lat":10.88422775},{"lng":106.81894684,"lat":10.8844471},{"lng":106.81934357,"lat":10.88592434},{"lng":106.82007599,"lat":10.88646221},{"lng":106.8208847,"lat":10.88575268},{"lng":106.82183075,"lat":10.88600445},{"lng":106.82492065,"lat":10.88821602},{"lng":106.82376099,"lat":10.89202404},{"lng":106.82512665,"lat":10.89292336},{"lng":106.82508087,"lat":10.89380932},{"lng":106.82582092,"lat":10.89369392},{"lng":106.82678223,"lat":10.89473915},{"lng":106.82804108,"lat":10.89422989},{"lng":106.82971191,"lat":10.89479542},{"lng":106.83392334,"lat":10.89829731},{"lng":106.84024048,"lat":10.89892864},{"lng":106.84301758,"lat":10.89128685},{"lng":106.84358978,"lat":10.88396454},{"lng":106.8459549,"lat":10.88228989},{"lng":106.85106659,"lat":10.87628269},{"lng":106.85076141,"lat":10.87308121},{"lng":106.84673309,"lat":10.86487675},{"lng":106.84957123,"lat":10.85624123},{"lng":106.85668945,"lat":10.84267807},{"lng":106.87164307,"lat":10.82619095},{"lng":106.87273407,"lat":10.82500172},{"lng":106.88011932,"lat":10.81694031},{"lng":106.88132477,"lat":10.81423092},{"lng":106.88191986,"lat":10.80848885},{"lng":106.88097382,"lat":10.80474186},{"lng":106.87821198,"lat":10.80054379},{"lng":106.8653183,"lat":10.78910923},{"lng":106.86473083,"lat":10.78592014},{"lng":106.86611176,"lat":10.78279209},{"lng":106.87218475,"lat":10.77763748},{"lng":106.8742218,"lat":10.77422905},{"lng":106.8745575,"lat":10.76952076},{"lng":106.87258911,"lat":10.76633263},{"lng":106.86807251,"lat":10.76439762},{"lng":106.86495972,"lat":10.76453209},{"lng":106.86302948,"lat":10.76538181},{"lng":106.85695648,"lat":10.7717371},{"lng":106.85192108,"lat":10.77464294},{"lng":106.84824371,"lat":10.77538395},{"lng":106.84568787,"lat":10.7751255},{"lng":106.84033966,"lat":10.7733078},{"lng":106.83167267,"lat":10.76485157},{"lng":106.8300705,"lat":10.76191044},{"lng":106.82808685,"lat":10.76014996},{"lng":106.82434082,"lat":10.75955772},{"lng":106.82314301,"lat":10.76159},{"lng":106.82362366,"lat":10.76660156},{"lng":106.82608795,"lat":10.77394104},{"lng":106.8248291,"lat":10.77861309},{"lng":106.82201385,"lat":10.78080463},{"lng":106.81954193,"lat":10.78093338},{"lng":106.81639862,"lat":10.77990532},{"lng":106.81008911,"lat":10.7746172},{"lng":106.80557251,"lat":10.7700882},{"lng":106.80329895,"lat":10.76251888},{"lng":106.79915619,"lat":10.75899315},{"lng":106.79125214,"lat":10.75560379},{"lng":106.77622223,"lat":10.74871922},{"lng":106.76773834,"lat":10.74192715},{"lng":106.75384521,"lat":10.72482491},{"lng":106.75200653,"lat":10.7210989},{"lng":106.75095367,"lat":10.71372795},{"lng":106.7519455,"lat":10.70497799},{"lng":106.75730896,"lat":10.69409084},{"lng":106.76012421,"lat":10.69108677},{"lng":106.77097321,"lat":10.6800909},{"lng":106.77217865,"lat":10.67751503},{"lng":106.77275848,"lat":10.67629242},{"lng":106.78279877,"lat":10.67305946},{"lng":106.7922287,"lat":10.66731262},{"lng":106.79718781,"lat":10.66215897},{"lng":106.80708313,"lat":10.6574564},{"lng":106.80897522,"lat":10.65498447},{"lng":106.8140564,"lat":10.64054871},{"lng":106.82392883,"lat":10.62904263},{"lng":106.8266449,"lat":10.6278038},{"lng":106.83761597,"lat":10.62704277},{"lng":106.84036255,"lat":10.63562202},{"lng":106.84280396,"lat":10.63802052},{"lng":106.85153961,"lat":10.63978958},{"lng":106.86004639,"lat":10.63655472},{"lng":106.86156464,"lat":10.6376543},{"lng":106.86373901,"lat":10.6414814},{"lng":106.86497498,"lat":10.64229965},{"lng":106.8699646,"lat":10.64317322},{"lng":106.87185669,"lat":10.64698601},{"lng":106.87290955,"lat":10.64808464},{"lng":106.87490845,"lat":10.64837742},{"lng":106.88296509,"lat":10.64740467},{"lng":106.88506317,"lat":10.64025593},{"lng":106.88956451,"lat":10.63754559},{"lng":106.89273071,"lat":10.6322031},{"lng":106.89839172,"lat":10.62934208},{"lng":106.90338898,"lat":10.62479973},{"lng":106.90561676,"lat":10.62077999},{"lng":106.90969849,"lat":10.6100235},{"lng":106.91136932,"lat":10.6082716},{"lng":106.91391754,"lat":10.6075058},{"lng":106.91819763,"lat":10.6099081},{"lng":106.92063904,"lat":10.61004162},{"lng":106.92653656,"lat":10.60348129},{"lng":106.93278503,"lat":10.60189629},{"lng":106.93458557,"lat":10.60076427},{"lng":106.93836212,"lat":10.59924507},{"lng":106.94138336,"lat":10.59668827},{"lng":106.94685364,"lat":10.59725761},{"lng":106.95626068,"lat":10.59395599},{"lng":106.96067047,"lat":10.59354115},{"lng":106.96320343,"lat":10.59145069},{"lng":106.96855927,"lat":10.58940411},{"lng":106.97458649,"lat":10.58236885},{"lng":106.97988892,"lat":10.58173275},{"lng":106.98386383,"lat":10.57943439},{"lng":106.98705292,"lat":10.58784676},{"lng":106.98604584,"lat":10.59602547},{"lng":106.99060822,"lat":10.60069656},{"lng":106.99069214,"lat":10.60488796},{"lng":106.99214935,"lat":10.60569096},{"lng":106.99633026,"lat":10.60545635},{"lng":106.9980545,"lat":10.60766315},{"lng":107.00066376,"lat":10.6070919},{"lng":107.00257874,"lat":10.6086359},{"lng":107.00215149,"lat":10.61042309},{"lng":107.00292206,"lat":10.61089611},{"lng":107.00655365,"lat":10.60983849},{"lng":107.00794983,"lat":10.61326122},{"lng":107.01248169,"lat":10.61009884},{"lng":107.01744843,"lat":10.60652065},{"lng":107.01905823,"lat":10.60438728},{"lng":107.0222702,"lat":10.59508324},{"lng":107.02659607,"lat":10.58845997},{"lng":107.02700806,"lat":10.58634186},{"lng":107.0266571,"lat":10.58347702},{"lng":107.02423096,"lat":10.57854366},{"lng":107.01194763,"lat":10.56070232},{"lng":107.01073456,"lat":10.55782986},{"lng":107.01068878,"lat":10.55461502},{"lng":107.01228333,"lat":10.55036736},{"lng":107.0147171,"lat":10.54794788},{"lng":107.02500916,"lat":10.54590702},{"lng":107.02727509,"lat":10.5420599},{"lng":107.02709961,"lat":10.53779697},{"lng":107.02404022,"lat":10.53382015},{"lng":107.01863098,"lat":10.52905178},{"lng":107.01248169,"lat":10.52054691},{"lng":107.0094986,"lat":10.51411915},{"lng":107.00696564,"lat":10.51051426},{"lng":107.00574493,"lat":10.5093689},{"lng":107.00019073,"lat":10.50711155},{"lng":106.9973526,"lat":10.50321579},{"lng":106.99820709,"lat":10.47863674},{"lng":106.99658203,"lat":10.46049023},{"lng":106.99147034,"lat":10.45094681},{"lng":106.98085785,"lat":10.44734478},{"lng":106.9661026,"lat":10.44648933},{"lng":106.96268463,"lat":10.43849468}]`);
}

function addBoundaries(map) {
    let HCMboundaries = getHCMboundaries();
    let hcmPolygon = new google.maps.Polygon({
        paths: HCMboundaries,
        strokeColor: '#FF0000',
        strokeOpacity: 0.8,
        strokeWeight: 2,
        fillColor: "#FF0000",
        fillOpacity: 0.05
    });
    hcmPolygon.setMap(map);
}

function addMarkerClusterer(map, response) {
    let markers = [];
    for (var i = 0; i < response.data.length; i++) {
        var location = response.data[i];
        if (location.lat !== null && location.lon !== null) {
            for (var j = 0; j < location.count; j++) {
                markers.push(new google.maps.Marker({
                    position: {
                        lat: location.lat,
                        lng: location.lon,
                    },
                    label: i + j + ''
                }))
            }
        }
    }
    new MarkerClusterer(map, markers, {
        imagePath:
            "https://developers.google.com/maps/documentation/javascript/examples/markerclusterer/m",
    });
}

function initMap() {
    $.get(`/api/Maps?year=${year}&quarter=${quarter}&month=${month}&provinceCode=${provinceCode}`,
        function (response) {
            console.log("init map...");
            map = new google.maps.Map(document.getElementById('map'), {
                center: { lat: 10.73699533, lng: 106.6997183 },
                zoom: 9,
                disableDefaultUI: true,
                restriction: {
                    latLngBounds: {
                        north: 11.2,
                        south: 10.3,
                        east: 107.1,
                        west: 106.33,
                    },
                },
                fullscreenControl: true,
            });
            addBoundaries(map);
            addMarkerClusterer(map, response);
        }
    );
}

function initPersonTypeChart() {
    $.get(`/api/AggregatedValues?year=${year}&quarter=${quarter}&month=${month}&provinceCode=${provinceCode}&districtCode=${districtCode}&indicatorGroup=Testing&groupBy=KeyPopulation`,
        function (response) {
            $("#person-type-char").kendoChart({
                theme: "bootstrap",
                series: [{
                    data: response.map(m => m.value)
                }],
                categoryAxis: {
                    categories: response.map(m => m.name)
                }
            });
        });
}

function initHTSchart() {
    $("#HTS-chart").kendoChart({
        theme: "bootstrap",
        legend: {
            position: "bottom"
        },
        series: [
            {
                type: "column",
                data: [1.5, 1, 2.3, 2.4, 6, 10],
                name: "%HIV+ referred",
            },
            {
                type: "column",
                data: [0, 0, 0.2, 0, 0.3, 0.5],
                name: "%HTS recent",
            },
            {
                type: "line",
                data: [210, 200, 205, 206, 180, 150],
                name: "HTS Positive",
                color: "#ec5e0a",
                axis: "mpg"
            }
        ],
        valueAxes: [
            {
                title: { text: "%" },
                min: 0,
                max: 13
            },
            {
                name: "mpg",
                color: "#ec5e0a"
            }
        ],
        categoryAxis: {
            categories: ["Jun 20", "Jul 20", "Aug 20", "Sep 20", "Oct 20", "Nov 20"],
            axisCrossingValues: [10, 0]
        }
    });
}

function checkURLParams() {
    let url = new URL(window.location.href);
    year = url.searchParams.get("year") ? url.searchParams.get("year") : year;
    quarter = url.searchParams.get("quarter") ? url.searchParams.get("quarter") : quarter;
    month = url.searchParams.get("month") ? url.searchParams.get("month") : month;
    provinceCode = url.searchParams.get("provinceCode") ? url.searchParams.get("provinceCode") : provinceCode;
    districtCode = url.searchParams.get("districtCode") ? url.searchParams.get("districtCode") : districtCode;
    $('#inputQuarter').val(quarter);
    $('#inputMonth').val(month);
    console.log(`${year} - ${quarter} - ${month} - ${provinceCode} - ${districtCode}`);
}

const loadThresholdList = () => {
    let token = `Bearer ${getToken()}`;
    $.ajax({
        url: "/api/ThresholdSettings",
        type: "GET",
        beforeSend: function (xhr) { xhr.setRequestHeader('Authorization', token); },
        success: function (data) {
            thresholdSettings = data;
            initIndicators();
        }
    });
}

$(document).ready(() => {
    checkURLParams();
    initFilterPanel();
    loadThresholdList();
    initPersonTypeChart();
    initHTSchart();
});

function applyFilter() {
    provinceCode = $('#inputProvince').val();
    districtCode = $('#inputDistrict').val();
    year = $('#year-picker').val();
    quarter = $('#inputQuarter').val();
    month = $('#inputMonth').val();
    console.log(`filter with: province - ${provinceCode}; district - ${districtCode}; year - ${year}; quarter - ${quarter}; month - ${month}`);
    initIndicators();
    initPersonTypeChart();
    initMap();
    updateFilterDetail();
    return true;
}
