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

$(document).ready(() => {
    if (!location.pathname.endsWith('Login')) {
        checkAuth();
    }
});