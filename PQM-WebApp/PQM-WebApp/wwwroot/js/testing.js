function createAgeGroupChart() {
    $("#age-group-chart").kendoChart({
        theme: "bootstrap",
        legend: {
            visible: false
        },
        seriesDefaults: {
            type: "bar"
        },
        series: [{
            name: "Age Group",
            data: [10, 70, 43, 25, 12, 5, 2, 4],
            color: "#62666e",
        }],
        valueAxis: {
            max: 75,
            line: {
                visible: false
            },
            minorGridLines: {
                visible: true
            },
            labels: {
                rotation: "auto"
            }
        },
        categoryAxis: {
            categories: ["15-19", "20-24", "25-29", "30-34", "35-39", "40-44", "45-49", "50+"],
            majorGridLines: {
                visible: false
            }
        },
        seriesClick: function (e) {
            addVar(`Age group: ${e.category}`)
        }
    });
}

function createGenderChart() {
    $("#gender-chart").kendoChart({
        theme: "bootstrap",
        legend: {
            visible: false
        },
        seriesDefaults: {
            type: "bar"
        },
        series: [{
            name: "Gender",
            data: [20, 150],
            color: "#62666e",
        }],
        valueAxis: {
            max: 155,
            line: {
                visible: false
            },
            minorGridLines: {
                visible: true
            },
            labels: {
                rotation: "auto"
            }
        },
        categoryAxis: {
            categories: ["Female", "Male"],
            majorGridLines: {
                visible: false
            }
        },
        seriesClick: function (e) {
            addVar(`Gender: ${e.category}`)
        }
    });
}

function createKeyPopulationsChart() {
    $("#key-populations-chart").kendoChart({
        theme: "bootstrap",
        legend: {
            visible: false
        },
        seriesDefaults: {
            type: "bar"
        },
        series: [{
            name: "Key Populations",
            data: [3, 105, 0, 60, 0, 0, 0],
            color: "#62666e",
        }],
        valueAxis: {
            max: 115,
            line: {
                visible: false
            },
            minorGridLines: {
                visible: true
            },
            labels: {
                rotation: "auto"
            }
        },
        categoryAxis: {
            categories: ["FSWs", "MSM", "PWID", "SDCs", "TCMT", "TGW", "Others"],
            majorGridLines: {
                visible: false
            }
        },
        seriesClick: function (e) {
            addVar(`Key population: ${e.category}`)
        }
    });
}

function createHTS_TEST_POS_chart() {
    $("#HTS_TEST_POS_chart").kendoChart({
        seriesDefaults: {
            type: "area",
            area: {
                line: {
                    style: "smooth"
                }
            },
        },
        series: [{
            data: [24, 15, 10, 23, 12, 14, 16, 17, 16, 18, 23, 25],
            color: "#62666e"
        }],
        categoryAxis: {
            title: {
            },
            majorGridLines: {
                visible: false
            },
            majorTicks: {
                visible: false
            }
        },
        valueAxis: {
            max: 25,
            title: {
            },
            majorGridLines: {
                visible: false
            },
            visible: false
        }
    });
}

function createHTS_TEST_Recency_chart() {
    $("#HTS_TEST-Recency_chart").kendoChart({
        seriesDefaults: {
            type: "area",
            area: {
                line: {
                    style: "smooth"
                }
            },
        },
        series: [{
            data: [0.507, 3.943, 2.848, 0.284, 7.263, 4.801, 7.890, 4.238, 9, 4.552, 15.855, 25],
            color: "#62666e"
        }],
        categoryAxis: {
            title: {
            },
            majorGridLines: {
                visible: false
            },
            majorTicks: {
                visible: false
            }
        },
        valueAxis: {
            max: 25,
            title: {
            },
            majorGridLines: {
                visible: false
            },
            visible: false
        }
    });
}

function createHTS_TEST_POS_refer_chart() {
    $("#HTS_TEST_POS-refer_chart").kendoChart({
        seriesDefaults: {
            type: "area",
            area: {
                line: {
                    style: "smooth"
                }
            },
        },
        series: [{
            data: [24, 15, 10, 23, 12, 14, 16, 17, 16, 18, 23, 25],
            color: "#62666e"
        }],
        categoryAxis: {
            title: {
            },
            majorGridLines: {
                visible: false
            },
            majorTicks: {
                visible: false
            }
        },
        valueAxis: {
            max: 25,
            title: {
            },
            majorGridLines: {
                visible: false
            },
            visible: false
        }
    });
}

function trendElement(trend) {
    let trendDirection = trend.direction === 1 ?
        `<svg xmlns="http://www.w3.org/2000/svg" color="${trend.criticalInfo}" width="35" height="35" fill="currentColor" class="bi bi-caret-up-fill" viewBox="0 0 20 20">
                        <path d="M7.247 4.86l-4.796 5.481c-.566.647-.106 1.659.753 1.659h9.592a1 1 0 0 0 .753-1.659l-4.796-5.48a1 1 0 0 0-1.506 0z" />
                     </svg>`
        :
        `<svg xmlns="http://www.w3.org/2000/svg" color="${trend.criticalInfo}" width="20" height="20" fill="currentColor" class="bi bi-caret-down-fill" viewBox="0 0 20 20">
                         <path d="M7.247 11.14L2.451 5.658C1.885 5.013 2.345 4 3.204 4h9.592a1 1 0 0 1 .753 1.659l-4.796 5.48a1 1 0 0 1-1.506 0z" />
                     </svg>`;
    return `${trendDirection}
                         ${trend.comparePercent}%
                        `;
}

function initHTS_TEST_POSIndicator(indicator) {
    createHTS_TEST_POS_chart();
    $("#HTS_TEST_POS-value").html(indicator.value.value);
    $("#HTS_TEST_POS-value").css("color", indicator.value.criticalInfo);
    $("#HTS_TEST_POS-percent").html(trendElement(indicator.trend));
}

function initHTS_TEST_RecencyIndicator(indicator) {
    createHTS_TEST_Recency_chart();
    $("#HTS_TEST-Recency-value").html(indicator.value.value);
    $("#HTS_TEST-Recency-value").css("color", indicator.value.criticalInfo);
    $("#HTS_TEST-Recency-percent").html(trendElement(indicator.trend));
}

function initHTS_TEST_POS_referIndicator(indicator) {
    createHTS_TEST_POS_refer_chart();
    $("#HTS_TEST_POS-refer-value").html(indicator.value.value);
    $("#HTS_TEST_POS-refer-value").css("color", indicator.value.criticalInfo);
    $("#HTS_TEST_POS-refer-percent").html(trendElement(indicator.trend));
}

function initConfigPanel() {
    $("#year-picker").kendoDatePicker({
        value: new Date(),
        format: "yyyy",
        depth: "decade",
        start: "decade"
    });
}

$(document).ready(() => {
    $.get('/Testing/indicators', function (data) {
        data.forEach(indicator => {
            switch (indicator.name) {
                case "HTS_TEST_POS":
                    initHTS_TEST_POSIndicator(indicator);
                    break;
                case "HTS_TEST-Recency":
                    initHTS_TEST_RecencyIndicator(indicator);
                    break;
                case "HTS_TEST_POS successfully refer to OPC":
                    initHTS_TEST_POS_referIndicator(indicator);
            }
        });
    });
    initConfigPanel();
    createAgeGroupChart();
    createGenderChart();
    createKeyPopulationsChart();
});

var configPanelOpen = true;
function openConfig() {
    if (configPanelOpen) {
        $("#configPanel").css("height", "0px");
    } else {
        $("#configPanel").css("height", "200px");
    }

    configPanelOpen = !configPanelOpen;
}

let variables = [];

function createChip(name) {
    return `<div class="chip">
                <span style="font-weight: bold">${name.split(':')[0]}</span>: ${name.split(':')[1]}
                <span class="closebtn" onclick="removeVar('${name}')">×</span>
            </div>`;
}

function updateVariablesComp() {
    let elements = '';
    variables.forEach(v => elements += createChip(v));
    $("#variables").html(elements);
}

function addVar(name) {
    let v = variables.find(s => s === name);
    if (!v) {
        variables.push(name);
        updateVariablesComp();
    }
}

function removeVar(name) {
    variables = variables.filter(s => s !== name);
    updateVariablesComp();
}

function getHCMboundaries() {
    return JSON.parse(`[{"lng":106.55529022,"lat":10.65239429},{"lng":106.55593872,"lat":10.65696144},{"lng":106.54891205,"lat":10.65744114},{"lng":106.54927063,"lat":10.65965176},{"lng":106.55049896,"lat":10.65906525},{"lng":106.55163574,"lat":10.66426468},{"lng":106.55323792,"lat":10.66613388},{"lng":106.55358887,"lat":10.66853714},{"lng":106.55208588,"lat":10.67063904},{"lng":106.55319214,"lat":10.67186737},{"lng":106.55352783,"lat":10.67536926},{"lng":106.55157471,"lat":10.67943573},{"lng":106.55052185,"lat":10.68543911},{"lng":106.54763031,"lat":10.68676472},{"lng":106.54742432,"lat":10.68698311},{"lng":106.53559113,"lat":10.6856575},{"lng":106.53404999,"lat":10.68789387},{"lng":106.52891541,"lat":10.70250225},{"lng":106.52724457,"lat":10.70925808},{"lng":106.52844238,"lat":10.71496964},{"lng":106.53022766,"lat":10.7172842},{"lng":106.5202179,"lat":10.72367764},{"lng":106.5109787,"lat":10.71797085},{"lng":106.50934601,"lat":10.71717739},{"lng":106.50167847,"lat":10.72650909},{"lng":106.49967957,"lat":10.7249918},{"lng":106.49578094,"lat":10.72762871},{"lng":106.49157715,"lat":10.72433376},{"lng":106.48212433,"lat":10.73545647},{"lng":106.47451019,"lat":10.74544334},{"lng":106.47351074,"lat":10.74640179},{"lng":106.47227478,"lat":10.74545765},{"lng":106.46369171,"lat":10.75651646},{"lng":106.47219849,"lat":10.76306343},{"lng":106.48714447,"lat":10.77430725},{"lng":106.50457001,"lat":10.78895473},{"lng":106.50574493,"lat":10.79374599},{"lng":106.52006531,"lat":10.85225105},{"lng":106.52580261,"lat":10.87364769},{"lng":106.52589417,"lat":10.87363148},{"lng":106.53499603,"lat":10.87186909},{"lng":106.52880859,"lat":10.84750462},{"lng":106.54463959,"lat":10.84318352},{"lng":106.54503632,"lat":10.84479046},{"lng":106.55594635,"lat":10.84302807},{"lng":106.56239319,"lat":10.8447237},{"lng":106.56324005,"lat":10.84138107},{"lng":106.56458282,"lat":10.84076595},{"lng":106.56924438,"lat":10.84281731},{"lng":106.57575989,"lat":10.84261513},{"lng":106.5765686,"lat":10.83837986},{"lng":106.5790863,"lat":10.83550262},{"lng":106.58200073,"lat":10.83277416},{"lng":106.58781433,"lat":10.83281231},{"lng":106.59095001,"lat":10.83183479},{"lng":106.59307861,"lat":10.8293848},{"lng":106.59516907,"lat":10.82979202},{"lng":106.59375,"lat":10.82784843},{"lng":106.59029388,"lat":10.82330418},{"lng":106.58195496,"lat":10.81565666},{"lng":106.58337402,"lat":10.81531334},{"lng":106.58376312,"lat":10.813797},{"lng":106.58493042,"lat":10.81401062},{"lng":106.58602905,"lat":10.81311893},{"lng":106.58456421,"lat":10.80675793},{"lng":106.58351898,"lat":10.80636787},{"lng":106.58289337,"lat":10.79732227},{"lng":106.58473969,"lat":10.79723167},{"lng":106.58448029,"lat":10.79580212},{"lng":106.58548737,"lat":10.79572773},{"lng":106.58542633,"lat":10.79458332},{"lng":106.58473969,"lat":10.7947216},{"lng":106.58446503,"lat":10.79075336},{"lng":106.58561707,"lat":10.79061413},{"lng":106.58563995,"lat":10.7894659},{"lng":106.58687592,"lat":10.789258},{"lng":106.58668518,"lat":10.788167},{"lng":106.5890274,"lat":10.78735065},{"lng":106.58808899,"lat":10.7817173},{"lng":106.58883667,"lat":10.77914715},{"lng":106.58816528,"lat":10.77795887},{"lng":106.58522797,"lat":10.77499199},{"lng":106.58115387,"lat":10.77515697},{"lng":106.57688904,"lat":10.77120876},{"lng":106.57582092,"lat":10.76924419},{"lng":106.57383728,"lat":10.76825047},{"lng":106.5696106,"lat":10.76859474},{"lng":106.56759644,"lat":10.76493931},{"lng":106.56800842,"lat":10.76433277},{"lng":106.56776428,"lat":10.76292706},{"lng":106.5734024,"lat":10.76039696},{"lng":106.56799316,"lat":10.7549448},{"lng":106.55942535,"lat":10.74898338},{"lng":106.55889893,"lat":10.74458122},{"lng":106.55934143,"lat":10.7422533},{"lng":106.55830383,"lat":10.73947334},{"lng":106.55883789,"lat":10.73647499},{"lng":106.56772614,"lat":10.73435116},{"lng":106.57407379,"lat":10.7327261},{"lng":106.57433319,"lat":10.72763252},{"lng":106.57725525,"lat":10.73055363},{"lng":106.5816803,"lat":10.73325634},{"lng":106.58194733,"lat":10.72943687},{"lng":106.59033203,"lat":10.72692776},{"lng":106.60089874,"lat":10.72231483},{"lng":106.60009766,"lat":10.71793842},{"lng":106.60394287,"lat":10.71733665},{"lng":106.60280609,"lat":10.71496201},{"lng":106.60356903,"lat":10.71326256},{"lng":106.60311127,"lat":10.71156216},{"lng":106.60379028,"lat":10.71084976},{"lng":106.60440826,"lat":10.70431232},{"lng":106.59780884,"lat":10.70182419},{"lng":106.59892273,"lat":10.69865036},{"lng":106.60146332,"lat":10.69512367},{"lng":106.60481262,"lat":10.69569683},{"lng":106.60959625,"lat":10.6954031},{"lng":106.61489105,"lat":10.69776058},{"lng":106.61812592,"lat":10.69773197},{"lng":106.61856079,"lat":10.69903088},{"lng":106.62071228,"lat":10.69977474},{"lng":106.62373352,"lat":10.69298267},{"lng":106.6240387,"lat":10.6924715},{"lng":106.62611389,"lat":10.69368362},{"lng":106.62548828,"lat":10.69449806},{"lng":106.64221191,"lat":10.7277174},{"lng":106.64344025,"lat":10.72768116},{"lng":106.64553833,"lat":10.72908592},{"lng":106.65010071,"lat":10.7287302},{"lng":106.64945984,"lat":10.73034382},{"lng":106.65610504,"lat":10.73220634},{"lng":106.66575623,"lat":10.73421001},{"lng":106.66616058,"lat":10.73211479},{"lng":106.67171478,"lat":10.73624992},{"lng":106.67362213,"lat":10.73738766},{"lng":106.67601013,"lat":10.73535538},{"lng":106.68184662,"lat":10.73451614},{"lng":106.68282318,"lat":10.73945999},{"lng":106.68606567,"lat":10.73994446},{"lng":106.68769836,"lat":10.74026871},{"lng":106.69197083,"lat":10.74156475},{"lng":106.69497681,"lat":10.73613834},{"lng":106.69355011,"lat":10.73314476},{"lng":106.69062042,"lat":10.73081398},{"lng":106.69013214,"lat":10.72525787},{"lng":106.68886566,"lat":10.72119904},{"lng":106.68962097,"lat":10.7188158},{"lng":106.68849182,"lat":10.71605206},{"lng":106.68886566,"lat":10.71210194},{"lng":106.6857605,"lat":10.71165943},{"lng":106.68395996,"lat":10.71340752},{"lng":106.68122864,"lat":10.71452999},{"lng":106.6787262,"lat":10.71166515},{"lng":106.67819977,"lat":10.70982838},{"lng":106.67665863,"lat":10.70970821},{"lng":106.67444611,"lat":10.70510578},{"lng":106.6733017,"lat":10.70408916},{"lng":106.67459869,"lat":10.7022028},{"lng":106.67333221,"lat":10.69235611},{"lng":106.67570496,"lat":10.68928337},{"lng":106.67620087,"lat":10.68540764},{"lng":106.6795578,"lat":10.68246365},{"lng":106.68148804,"lat":10.67757416},{"lng":106.68117523,"lat":10.67573833},{"lng":106.67896271,"lat":10.67211342},{"lng":106.67877197,"lat":10.66833973},{"lng":106.67892456,"lat":10.66447353},{"lng":106.68037415,"lat":10.66291904},{"lng":106.68429565,"lat":10.66133881},{"lng":106.68532562,"lat":10.65672874},{"lng":106.68574524,"lat":10.65327835},{"lng":106.68509674,"lat":10.65099239},{"lng":106.67906952,"lat":10.64561272},{"lng":106.67420197,"lat":10.64449501},{"lng":106.67282104,"lat":10.64265251},{"lng":106.66960144,"lat":10.63920212},{"lng":106.66814423,"lat":10.63912201},{"lng":106.66662598,"lat":10.64030266},{"lng":106.66609192,"lat":10.63847256},{"lng":106.66564941,"lat":10.63964367},{"lng":106.66494751,"lat":10.63886738},{"lng":106.66337585,"lat":10.64013672},{"lng":106.66247559,"lat":10.63937569},{"lng":106.66016388,"lat":10.63981438},{"lng":106.66077423,"lat":10.64197731},{"lng":106.65694427,"lat":10.64355087},{"lng":106.65627289,"lat":10.63836288},{"lng":106.65401459,"lat":10.63956928},{"lng":106.6529541,"lat":10.63798904},{"lng":106.65352631,"lat":10.63333321},{"lng":106.6529541,"lat":10.63246822},{"lng":106.64865875,"lat":10.63275433},{"lng":106.64620972,"lat":10.63421822},{"lng":106.64380646,"lat":10.62958241},{"lng":106.64099121,"lat":10.62666512},{"lng":106.63885498,"lat":10.62749863},{"lng":106.63484192,"lat":10.62543392},{"lng":106.63214874,"lat":10.62821579},{"lng":106.63020325,"lat":10.62669468},{"lng":106.62966156,"lat":10.62893581},{"lng":106.63085938,"lat":10.63019466},{"lng":106.62963867,"lat":10.63164234},{"lng":106.62937164,"lat":10.63531685},{"lng":106.62950897,"lat":10.63633728},{"lng":106.63117981,"lat":10.63680363},{"lng":106.63058472,"lat":10.63905144},{"lng":106.62963104,"lat":10.63944435},{"lng":106.62994385,"lat":10.6404295},{"lng":106.62949371,"lat":10.64203835},{"lng":106.62722015,"lat":10.64240265},{"lng":106.6258316,"lat":10.63972378},{"lng":106.62415314,"lat":10.63941765},{"lng":106.62120056,"lat":10.64095306},{"lng":106.62186432,"lat":10.64410877},{"lng":106.62434387,"lat":10.64448929},{"lng":106.62394714,"lat":10.64635181},{"lng":106.62479401,"lat":10.64742565},{"lng":106.61896515,"lat":10.64901733},{"lng":106.61902618,"lat":10.64960384},{"lng":106.61734009,"lat":10.650177},{"lng":106.61778259,"lat":10.65167141},{"lng":106.61684418,"lat":10.65217876},{"lng":106.61675262,"lat":10.65331268},{"lng":106.61567688,"lat":10.65335178},{"lng":106.61557007,"lat":10.65411091},{"lng":106.61419678,"lat":10.65384674},{"lng":106.61083984,"lat":10.6568985},{"lng":106.60594177,"lat":10.65802479},{"lng":106.60541534,"lat":10.65877151},{"lng":106.60151672,"lat":10.65105057},{"lng":106.60064697,"lat":10.65073204},{"lng":106.59963989,"lat":10.65258789},{"lng":106.59535217,"lat":10.6537838},{"lng":106.59564972,"lat":10.65109634},{"lng":106.59515381,"lat":10.65099239},{"lng":106.59177399,"lat":10.65007687},{"lng":106.5904541,"lat":10.65307045},{"lng":106.58744812,"lat":10.6528101},{"lng":106.58413696,"lat":10.65368176},{"lng":106.58332062,"lat":10.65540314},{"lng":106.57975006,"lat":10.65610218},{"lng":106.57791901,"lat":10.65326977},{"lng":106.57802582,"lat":10.65186787},{"lng":106.5795517,"lat":10.65186787},{"lng":106.57939148,"lat":10.65033436},{"lng":106.57700348,"lat":10.64886761},{"lng":106.57514954,"lat":10.64986324},{"lng":106.57339478,"lat":10.6498127},{"lng":106.57351685,"lat":10.65415668},{"lng":106.56904602,"lat":10.65651131},{"lng":106.56629181,"lat":10.65643883},{"lng":106.56529999,"lat":10.65229321},{"lng":106.56323242,"lat":10.65219784},{"lng":106.56315613,"lat":10.65143681},{"lng":106.55903625,"lat":10.65185356},{"lng":106.5592041,"lat":10.64991379},{"lng":106.55854797,"lat":10.64862728},{"lng":106.55788422,"lat":10.64855385},{"lng":106.55744934,"lat":10.650383},{"lng":106.5555954,"lat":10.6509037},{"lng":106.55529022,"lat":10.65239429}]`);
}

function getFakeLocations() {
    return [
        { lat: 10.746455, lng: 106.516 },
        { lat: 10.748455, lng: 106.515 },
        { lat: 10.750455, lng: 106.514 },
        { lat: 10.7406455, lng: 106.513 },
        { lat: 10.749455, lng: 106.512 },
        { lat: 10.743455, lng: 106.511 },
        { lat: 10.742455, lng: 106.51 },
        { lat: 10.741455, lng: 106.509 },
        { lat: 10.756455, lng: 106.508 },
        { lat: 10.753455, lng: 106.507 },
        { lat: 10.754455, lng: 106.506 },
        { lat: 10.755455, lng: 106.505 },
        { lat: 10.751455, lng: 106.504 },
        { lat: 10.752455, lng: 106.503 },
        { lat: 10.742455, lng: 106.502 },
        { lat: 10.738455, lng: 106.501 }
    ];
}

function addBoundaries(map) {
    let HCMboundaries = getHCMboundaries();
    let hcmPolygon = new google.maps.Polygon({
        paths: HCMboundaries,
        strokeColor: '#FF0000',
        strokeOpacity: 0.8,
        strokeWeight: 2,
        fillColor: "#FF0000",
        fillOpacity: 0.05,
    });
    hcmPolygon.setMap(map)
}

function addMarkerClusterer(map) {
    let markers = getFakeLocations().map((location, i) => {
        return new google.maps.Marker({
            position: location,
            label: i + '',
        });
    });
    new MarkerClusterer(map, markers, {
        imagePath:
            "https://developers.google.com/maps/documentation/javascript/examples/markerclusterer/m",
    });
}

function initMap() {
    console.log("init map...");
    map = new google.maps.Map(document.getElementById('map'), {
        center: { lat: 10.746455, lng: 106.5735963 },
        zoom: 11,
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
    addMarkerClusterer(map);
}