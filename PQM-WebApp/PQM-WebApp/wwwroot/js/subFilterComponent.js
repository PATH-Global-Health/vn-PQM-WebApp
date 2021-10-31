const createChip = (name) => {
    return `<div class="chip">
                <span style="font-weight: bold">${name.split(':')[0]}</span>: ${name.split(':')[1]}
                <span class="closebtn" onclick="subFilter.removeVar('${name}')">×</span>
            </div>`;
}

const subFilter = {
    ageGroups: [],
    genders: [],
    keyPopulations: [],
    sites: [],
    variables: [],
    rootElement: '',
    render: (rootElement) => {
        let dropdown =
            `<div class="dropdown">
                <button class="add-filter-label" type="button" id="dropdownMenuButton" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                + ${languages.translate('', 'Add filter')}
                </button>
                <div class="dropdown-menu" aria-labelledby="dropdownMenuButton" style="width: 250px">
                    <form class="px-4 py-3">
                        <div class="form-group">
                            <label for="exampleDropdownFormEmail1">${languages.translate('', 'Variable')}</label>
                            <select class="form-control" onchange="subFilter.onVariableChange()" id="variableSelect">
                                <option value=""></option>
                                <option value="ageGroups">${languages.translate('', 'Age Group')}</option>
                                <option value="genders">${languages.translate('', 'Gender')}</option>
                                <option value="keyPopulations">${languages.translate('', 'Key Population')}</option>
                                <option value="sites">${languages.translate('', 'Site')}</option>
                            </select>
                        </div>
                        <div class="form-group" id="valueComponent">
                            
                        </div>
                        <button type="button" class="btn btn-primary" onclick="subFilter.processFilter()">${languages.translate('', 'Process')}</button>
                    </form>
                </div>
            </div>`;
        subFilter.rootElement = rootElement;
        $(`#${rootElement}`).html(subFilter.variables.map(item => createChip(item.name)).join(''));
        $(`#${rootElement}`).append(dropdown);
    },
    onVariableChange: () => {
        let variable = $('#variableSelect').val();
        let _v = subFilter[variable];
        if (_v) {
            let _html = `<label for="exampleDropdownFormPassword1">${languages.translate('', 'Value')}</label>
                    <select class="form-control" id="valueSelect">
                        ${_v.map(m => `<option value="${m.id}">${m.name}</option>`).join("")}
                    </select>`
            $('#valueComponent').html(_html);
        }
    },
    addVar: (variable) => {
        let v = subFilter.variables.find(s => s.name === variable.name);
        if (!v) {
            subFilter.variables.push(variable);
            subFilter.render(subFilter.rootElement)
            applyFilter();
        }
    },
    removeVar: (name) => {
        subFilter.variables = subFilter.variables.filter(s => s.name !== name);
        subFilter.render(subFilter.rootElement)
        applyFilter();
    },
    processFilter: () => {
        let variable = $('#variableSelect').val();
        let id = $('#valueSelect').val();
        let name = subFilter[variable].find(s => s.id === id).name;
        subFilter.addVar({ name: `${variable}:${name}`, id: id, type: variable })
        subFilter.render(subFilter.rootElement);
    },
}