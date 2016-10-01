$(document).ready(function () {
    var clickcount = 0;
    $('.toggle-btn').click(function () {
        $('#navigation-bar').toggleClass('hidden');
        if (clickcount % 2 == 0) {
            $('#content').css('width', '100%');
        } else {
            $('#content').css('width', '80%');
        }
        clickcount++;
    });
});