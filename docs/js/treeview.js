$(function () {
    $('li > ul').each(function () {
        var innerUlContent = $(this);
        var parent = innerUlContent.parent('li');
        //parent.addClass('expandable');
        parent.click(function () {
            $(this).toggleClass('expanded');
            innerUlContent.slideToggle('fast');
        });

        // prevent li clicks to propagate upper than the container ul
        innerUlContent.click(function (event) {
            event.stopPropagation();
        });
    });
    
    /*$('#folderView li').click(function (event) {
        //alert($(this).clone().children().remove().end().text().replace(/\s+/g, ''));
        paneName = $(this).clone().children().remove().end().text().replace(/\s+/g, '');
        alert(paneName);
    });*/
});