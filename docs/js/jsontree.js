function JsonTreeBuilder() {

    this.build = function (jsonDoc) {
        // build ul for the json
        var tree = generateTree(jsonDoc);
        // and make it expandable/collapsible
        activateTree(tree);

        // wrap with container and return
        return $('<div id="jsonTree"/>').append(tree);
    };

    var generateTree = function (data) {
        if (typeof (data) == 'object' && data != null) {
            var ul = $('<ul>');
            for (var i in data) {
                var li = $('<li>');
                ul.append(li.text(i).append(generateTree(data[i])));
            }
            return ul;
        } else {
            var v = (data == undefined) ? '[empty]' : data;
            var textNode = document.createTextNode(' : ' + v);
            return textNode;
        }
    };

    var activateTree = function (tree) {
        // find every ul that is child of li and make the li (the parent) expandable so it will be able to hide/show the ul (the content) by click
        $('li > ul', tree).each(function () {
            var innerUlContent = $(this);
            var parent = innerUlContent.parent('li');
            parent.addClass('expandable');
            parent.click(function () {
                $(this).toggleClass('expanded');
                innerUlContent.slideToggle('fast');
            });

            // prevent li clicks to propagate upper than the container ul
            innerUlContent.click(function (event) {
                event.stopPropagation();
            });
        });

        // start with the tree collapsed.
        $('ul', tree).hide();
    };
}