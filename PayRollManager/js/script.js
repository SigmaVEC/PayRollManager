function setCookie(value) {
    var d = new Date();
    d.setTime(d.getTime() + (1 * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toUTCString();
    document.cookie = "Token=" + value + "; " + expires + "; path=/";
}
function getCookie(a) {
    //console.log("get Cookie called");
    var ca = document.cookie.split(';');
    console.log(ca);
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) === " ") {
            //console.log(c);
            c = c.substring(1);
            //console.log(c);
        }
        if (c.indexOf(a) == 0) {
            c1 = c.split("=");
            return c1[1];
            // return c.substring(name.length,name.length+ c.length);
        }
    }
    return "";
}
function deleteCookie() {
    var d = new Date();
    d.setTime(d.getTime() + (-2 * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toUTCString();
    document.cookie = "Token=null" + "; " + expires + "; path=/";
}
function setUserName(value) {
    var d = new Date();
    d.setTime(d.getTime() + (1 * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toUTCString();
    document.cookie = "Uname=" + value + "; " + expires + "; path=/";
}
function notify(s) {
    $("#notificationArea").html(s);
    $('#notification').slideDown(300).delay(3000).slideUp(300);
}
/*document.writeln(`<div id="notification" class="container-fluid">
            <div class="row">
                <div class="col-md-4"></div>
                <div class="col-md-4" id="notificationArea"></div>
                <div class="col-md-4"></div>
            </div>
         </div>`);*/
$.get("/api/employeeview", {token:getCookie("Token")}, function (result) {
    // alert(result.message);
    if (result.message != "Success" && !window.location.toString().includes("first.html")) {
        //notify("Please Login");
        window.location = "first.html";
        return;
    }
});