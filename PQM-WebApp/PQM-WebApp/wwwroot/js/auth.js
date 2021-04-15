const logout = () => {
    sessionStorage.removeItem('token');
    localStorage.removeItem('token');
    window.location.href = '/Login';
}

const checkAuth = () => {
    let token = getToken();
    if (!token) {
        window.location.href = `/Login?returnUrl=${window.location.href}`;
    } else {
        $("body").css("display", "");
    }
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
        window.location.href = url.searchParams.get("returnUrl") ? url.searchParams.get("returnUrl") : '/Dashboard';
    }).catch((error) => {
        $('#loginError').css('display', '')
    });
    return false;
}

$(document).ready(() => {
    if (!location.pathname.endsWith('Login')) {
        checkAuth();
    }
});