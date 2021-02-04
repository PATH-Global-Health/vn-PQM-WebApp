function logout() {
    sessionStorage.removeItem('token');
    window.location.href = '/Login';
}

function getToken() {
    return sessionStorage.getItem('token');
}

function setToken(token) {
    sessionStorage.setItem('token', token);
}

function checkAuth() {
    let token = getToken();
    if (!token) {
        window.location.href = `/Login?returnUrl=${window.location.href}`;
    }
}

function onLogin() {
    $.ajax({
        type: 'POST',
        url: 'http://202.78.227.99:31884/api/Users/Login',
        data: JSON.stringify({
            username: $('#username').val(),
            password: $('#password').val()
        }),
        success: function (data) {
            setToken(data.access_token);
            var url = new URL(window.location.href);
            window.location.href = url.searchParams.get("returnUrl") ? url.searchParams.get("returnUrl") : '/Dashboard';
        },
        error: function (xhr, status, error) {
            $('#user-error').html('username or password is incorrect') 
        },
        contentType: "application/json",
        dataType: 'json'
    });
}

$(document).ready(() => {
    if (!location.pathname.endsWith('Login')) {
        checkAuth();
    }
});