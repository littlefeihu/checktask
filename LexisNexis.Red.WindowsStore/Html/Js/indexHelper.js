function ListenHref() {
    $('a').click(function (e) {
        if (this.href == null || this.href == "")
            return false;
        if (this.href.indexOf("about:blank#") == -1) {
            var jObj = { type: 'HREF', value1: this.href };
            var ToStr = JSON.stringify(jObj);
            window.external.notify(ToStr);
            return false;
        }
    });
}

function GotoElementWithIndex(indexName) {
    $('.firsttitle').each(function () {
        if ($.trim(this.innerText).toUpperCase() == indexName.toString().toUpperCase()) {
            ScrollToElement(this);
            return false;
        }
    });
}

function ScrollToElement(element) {
    if (element != null) {
        var offset = pageY(element);
        var max = $(document).height() - $(window).height() - 10;
        offset = offset > max ? max : offset;
        $(window).scrollTop(offset);
    }
}

function pageY(elem) {
    return elem.offsetParent ? (elem.offsetTop + pageY(elem.offsetParent)) : elem.offsetTop;
}