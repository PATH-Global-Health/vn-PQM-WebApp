const layout = {
    onLanguageChange: (language) => {
        $('#profileText').html(languages.translate(language, 'Profile'));
        $('#settingText').html(languages.translate(language, 'Setting'));
        $('#logoutText').html(languages.translate(language, 'Log out'));
    }
}

const getLanguages = () => {
    return httpClient.callApi({
        method: 'GET',
        url: '/language/languages.json',
    })
}

const initMultipleLanguage = (lang) => {
    let dfl = getUrlParam('lang');
    if (!dfl) dfl = 'vn';
    lang.forEach(_ => {
        let e = {
            code: _.code,
            name: _.name,
            dictionary: new Map(),
        }
        _.dictionary.forEach(d => e.dictionary.set(d.key, d.value));
        languages._availableLanguages.set(_.code, e);
    })
    languages._language = dfl;
    languages.init('selectLanguage', 'dropup', layout.onLanguageChange);
    languages.onChange(dfl)

}

$(document).ready(() => {
    Promise.all([getLanguages()])
        .then((results) => {
            initMultipleLanguage(results[0].data);
        });
})