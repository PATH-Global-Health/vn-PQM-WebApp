function logout() {
    sessionStorage.removeItem('token');
    localStorage.removeItem('token');
    window.location.href = '/Login';
}

function getToken() {
    return sessionStorage.getItem('token') !== null ? sessionStorage.getItem('token') : localStorage.getItem('token');
}

function setToken(token, remember) {
    if (remember) {
        localStorage.setItem('token', token);
    } else {
        sessionStorage.setItem('token', token);
    }
}

function checkAuth() {
    let token = getToken();
    if (!token) {
        window.location.href = `/Login?returnUrl=${window.location.href}`;
    } else {
        $("body").css("display", "");
    }
}

function onLogin() {
    $.ajax({
        type: 'POST',
        url: 'https://auth.vkhealth.vn/api/Users/Login',
        data: JSON.stringify({
            username: $('#inputUsername').val(),
            password: $('#inputPassword').val()
        }),
        success: function (data) {
            setToken(data.access_token, $('#inputRememberMe').is(":checked"));
            var url = new URL(window.location.href);
            window.location.href = url.searchParams.get("returnUrl") ? url.searchParams.get("returnUrl") : '/Dashboard';
        },
        error: function (xhr, status, error) {
            $('#loginError').css('display','') 
        },
        contentType: "application/json",
        dataType: 'json'
    });
    return false;
}

$(document).ready(() => {
    if (!location.pathname.endsWith('Login')) {
        checkAuth();
    }
});