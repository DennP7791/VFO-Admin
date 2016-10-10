//Auto log out after 19 minutes
var timeoutTimer = setTimeout(function () { window.open("/Account/SessionExpired", "_self"); }, 1140000);
$().ready(function () {
    //Attach timeout reset to mousedown & keydown actions
    $('body').bind('mousedown keydown', function (event) {
        clearTimeout(timeoutTimer);
        timeoutTimer = setTimeout(function () { window.open("/Account/SessionExpired", "_self"); }, 1140000);
    });
});