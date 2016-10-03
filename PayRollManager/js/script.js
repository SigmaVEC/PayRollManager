function setCookie(value) {
    var d = new Date();
    d.setTime(d.getTime() + (1 * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toUTCString();
    document.cookie = "Token=" + value + "; " + expires + "; path=/";
}
function getCookie() {
    //console.log("get Cookie called");
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf("Token=") == 0) {
            c1 = c.split("=");
            return c1[1];
            // return c.substring(name.length,name.length+ c.length);
        }
    }
    return "";
}