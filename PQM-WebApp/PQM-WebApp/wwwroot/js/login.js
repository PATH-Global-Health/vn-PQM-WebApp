const onLanguageChange = (language) => {
    buildForm();
    insertParam('lang', language);
}

const buildForm = () => {
    let lang = languages._language;
    let _html =
        `<img src="/images/usaid-logo.png" alt="" width="250" height="auto">
        <h1 class="h3 mb-3 font-weight-normal">${languages.translate(lang, 'Please sign in')}</h1>
        <label for="inputUsername" class="sr-only">${languages.translate(lang, 'Username')}</label>
        <input type="text" id="inputUsername" class="form-control" placeholder="${languages.translate(lang, 'Username')}" required autofocus>
        <label for="inputPassword" class="sr-only">${languages.translate(lang, 'Password')}</label>
        <input type="password" id="inputPassword" class="form-control" placeholder="${languages.translate(lang, 'Password')}" required>
        <span id="loginError" style="display: none; color: red">${languages.translate(lang, 'Please check username or password!!')}</span>
        <div class="checkbox mb-3">
            <label>
                <input type="checkbox" value="remember-me" id="inputRememberMe"> ${languages.translate(lang, 'Remember me')}
            </label>
        </div>
        <button class="btn btn-lg btn-primary btn-block" type="submit">${languages.translate(lang, 'Sign in')}</button>`
    $('#loginForm').html(_html);
}

const initMultipleLanguage = () => {
    let dfl = getUrlParam('lang');
    if (!dfl) dfl = 'vn';
    let _lang =
        [
            {
                code: 'vn',
                name: 'Tiếng Việt',
                dictionary: [
                    { key: 'Sign in', value: 'Đăng nhập' },
                    { key: 'Please sign in', value: 'Đăng nhập để tiếp tục' },
                    { key: 'Username', value: 'Tên đăng nhập' },
                    { key: 'Password', value: 'Mật khẩu' },
                    { key: 'Remember me', value: 'Ghi nhớ' },
                    { key: 'Please check username or password!!', value: 'Vui lòng kiểm tra tên đăng nhập và mật khẩu!' },
                    { key: 'Profile', value: 'Tài khoản' },
                    { key: 'Setting', value: 'Cài đặt' },
                    { key: 'Log out', value: 'Đăng xuất' },
                ]
            },
            {
                code: 'en',
                name: 'English',
                dictionary: [
                    { key: 'Sign in', value: 'Sign in' },
                    { key: 'Please sign in', value: 'Please sign in' },
                    { key: 'Username', value: 'Username' },
                    { key: 'Password', value: 'Password' },
                    { key: 'Remember me', value: 'Remember me' },
                    { key: 'Please check username or password!!', value: 'Please check username or password!!' },
                    { key: 'Profile', value: 'Profile' },
                    { key: 'Setting', value: 'Setting' },
                    { key: 'Log out', value: 'Log out' },
                ]
            }
        ];
    _lang.forEach(_ => {
        let e = {
            code: _.code,
            name: _.name,
            dictionary: new Map(),
        }
        _.dictionary.forEach(d => e.dictionary.set(d.key, d.value));
        languages._availableLanguages.set(_.code, e);
    })
    languages._language = dfl;
    languages.init("selectLanguage", "", onLanguageChange);
}

const onLogin = () => {
    httpClient.callApi({
        method: 'POST',
        url: 'https://auth.vkhealth.vn/api/Users/Login',
        data: {
            username: $('#inputUsername').val(),
            password: $('#inputPassword').val()
        }
    }).then((res) => {
        console.log(res);
        setToken(res.data.access_token, $('#inputRememberMe').is(":checked"));
        var url = new URL(window.location.href);
        var returnUrl = url.searchParams.get("returnUrl");
        if (!returnUrl) returnUrl = `/Dashboard`;
        if (returnUrl.includes('?')) {
            returnUrl += `&lang=${languages._language}`;
        } else {
            returnUrl += `?lang=${languages._language}`;
        }
        window.location.href = returnUrl;
    }).catch((error) => {
        $('#loginError').css('display', '')
    });
    return false;
}

$(document).ready(() => {
    initMultipleLanguage();
    buildForm();
});