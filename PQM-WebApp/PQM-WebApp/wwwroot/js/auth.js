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
        logout();
    }
}

$(document).ready(() => {
    checkAuth();
});