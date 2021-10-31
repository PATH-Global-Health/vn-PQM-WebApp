const languages = {
    _availableLanguages: new Map(),
    _language: 'vn',
    _callback: [],
    init: (rootElement, direction, callback) => {
        let array = Array.from(languages._availableLanguages, ([name, value]) => ({ name, value }));
        let d = languages._availableLanguages.get(languages._language).name;
        let _html =
            `<div class="btn-group ${direction}" style="width: 160px">
            <button class="btn dropdown-toggle" type="button" id="dropdownMenuButton" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                <svg width="20" height="20" viewBox="0 0 24 24"><path d="M11.99 2C6.47 2 2 6.48 2 12s4.47 10 9.99 10C17.52 22 22 17.52 22 12S17.52 2 11.99 2zm6.93 6h-2.95a15.65 15.65 0 00-1.38-3.56A8.03 8.03 0 0118.92 8zM12 4.04c.83 1.2 1.48 2.53 1.91 3.96h-3.82c.43-1.43 1.08-2.76 1.91-3.96zM4.26 14C4.1 13.36 4 12.69 4 12s.1-1.36.26-2h3.38c-.08.66-.14 1.32-.14 2s.06 1.34.14 2H4.26zm.82 2h2.95c.32 1.25.78 2.45 1.38 3.56A7.987 7.987 0 015.08 16zm2.95-8H5.08a7.987 7.987 0 014.33-3.56A15.65 15.65 0 008.03 8zM12 19.96c-.83-1.2-1.48-2.53-1.91-3.96h3.82c-.43 1.43-1.08 2.76-1.91 3.96zM14.34 14H9.66c-.09-.66-.16-1.32-.16-2s.07-1.35.16-2h4.68c.09.65.16 1.32.16 2s-.07 1.34-.16 2zm.25 5.56c.6-1.11 1.06-2.31 1.38-3.56h2.95a8.03 8.03 0 01-4.33 3.56zM16.36 14c.08-.66.14-1.32.14-2s-.06-1.34-.14-2h3.38c.16.64.26 1.31.26 2s-.1 1.36-.26 2h-3.38z"></path></svg>
                <span style="margin: 0px 15px 0px 15px" id="_selectedLanguage">
                    ${d}
                </span>
            </button>
            <div class="dropdown-menu" aria-labelledby="dropdownMenuButton">
                ${array.map(m =>
                    `<a class="dropdown-item" href="#" onclick="languages.onChange('${m.name}', true)">
                        <span>${m.value.name}</span>
                    </a>`
                ).join('')}
            </div>
        </div>`;
        languages._callback.push(callback);
        $(`#${rootElement}`).html(_html)
    },
    onChange: (language, setUrl) => {
        if (setUrl) {
            insertParam('lang', language);
        }
        let display = languages._availableLanguages.get(language);
        $('#_selectedLanguage').html(display.name);
        languages._language = language;
        languages._callback.forEach(f => {
            try {
                f(language)
            }
            catch (err) {
                console.log(err);
            }
        });
    },
    addCallback: (callback) => {
        languages._callback.push(callback);
    },
    translate: (language, key) => {
        language = language && language.length > 0 ? language : languages._language;
        let curr = languages._availableLanguages.get(language);
        if (curr) {
            let str = curr.dictionary.get(key);
            if (str) return str;
        }
        return key;
    },
}