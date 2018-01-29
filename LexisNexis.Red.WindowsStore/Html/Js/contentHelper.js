function onMouseDown() {
    var jsonObj = "{ type: 'POSITION', value1: " + window.event.clientX + ", value2: " + window.event.clientY + "}";
    window.external.notify(jsonObj);
};

function GetSectionContent() {
    //var selectedText = rangy.getSelection().toString();
    ////var selectedText = rangy.getSelection().toHtml();
    //var result = PopupContextMenu(selectedText);
    //if (result == true) {
    //    var jObj = { type: 'SELECTED', value1: selectedText };
    //    var ToStr = JSON.stringify(jObj);
    //    window.external.notify(ToStr);
    //}

    if ($(window).scrollTop() >= $(document).height() - $(window).height() - 1) {
        var jsonObj = "{ type:'INFINITE',  value1:'DOWN'}";
        window.external.notify(jsonObj);
    }
    else if ($(window).scrollTop() < 1) {
        var jsonObj = "{ type:'INFINITE',  value1:'UP'}";
        window.external.notify(jsonObj);
    }
};

function AppendHtml_Top(html) {
    var topdiv = document.getElementById("mainbody");
    var childArr = topdiv.children;
    if (childArr.length >= 2)
        topdiv.removeChild(childArr[childArr.length - 1]);

    var top = $(document).height();
    $("#mainbody").prepend(html.content);
    FillPageWithBlank(html.selector);
    loadingcollapse("#loadingup");
    var offset = $(document).height() - top;
    $(window).scrollTop(offset);
    $("html,body").animate({ scrollTop: offset }, 500);
};

function AppendHtml_Bottom(html) {
    var topdiv = document.getElementById("mainbody");
    var childArr = topdiv.children;
    if (childArr.length >= 2)
        topdiv.removeChild(childArr[0]);

    var offset = $(window).scrollTop();
    $("#mainbody").append(html.content);
    FillPageWithBlank(html.selector);
    loadingcollapse("#loadingdown");
    $(window).scrollTop(offset);
    $("html,body").animate({ scrollTop: offset }, 500);
};

var currentdiv;
$(window).scroll(function () {
    if ($(window).scrollTop() >= $(document).height() - $(window).height() - 5) {
        var jsonObj = "{ type:'INFINITE',  value1:'DOWN'}";
        window.external.notify(jsonObj);
    }
    else if ($(window).scrollTop() < 1) {
        var jsonObj = "{ type:'INFINITE',  value1:'UP'}";
        window.external.notify(jsonObj);
    }
    else if ($(window).scrollTop() > 1) {
        var childArr = document.getElementById("mainbody").children;
        for (var i = childArr.length - 1; i >= 0; i--) {
            if ($(window).scrollTop() >= pageY(childArr[i])) {
                if (currentdiv != childArr[i]) {
                    currentdiv = childArr[i];
                    var jsonObj = "{ type:'SCROLL',  value1:'" + childArr[i].id + "'}";
                    window.external.notify(jsonObj);
                }
                break;
            }
        }
    }
});

var SelectString = "";
var Ready = true;
function PopupContextMenu(content) {
    if (content == "" || content == undefined || content == null) {
        SelectString = "";
        return false;
    }

    if (SelectString == content.toString()) {
        if (Ready) {
            Ready = false;
            return true;
        }
        else {
            return false;
        }
    }
    else {
        SelectString = content.toString();
        Ready = true;
        return false;
    }
};

function loadingdownvisible() {
    $("#loadingdown").show();
};

function loadingupvisible() {
    $("#loadingup").show();
};

function loadingcollapse(selector) {
    $(selector).hide();
};

function OnLoad(tocId, pageNum) {
    RangyInitail();
    FillPageWithBlank(tocId);
    ListenHref(tocId + " a");
    setInterval('GetSectionContent()', 700);
    if (pageNum == 0) {
        $(window).scrollTop(1);
    }
    else {
        var page = $(" .pagebreak[data-count='" + pageNum + "']")[0];
        if (page != null) {
            ScrollToElement(page);
        }
        else {
            $(window).scrollTop(1);
        }
    }
};

function ListenHref(selector) {
    $(selector).click(function (e) {
        if (this.href == null || this.href == "")
            return false;
        if (this.href.indexOf("about:blank#") == -1) {
            var jObj = { type: 'HREF', value1: this.href };
            var ToStr = JSON.stringify(jObj);
            window.external.notify(ToStr);
            return false;
        }
    });
};

function FillPageWithBlank(selector) {
    if ($(window).height() >= $(selector).height()) {
        $(selector).height($(window).height() + 10);
    }
};

function ScrollToHighlight(searchresult) {
    var hilitor = new Hilitor();
    hilitor.apply(searchresult.keyword);
    if (searchresult.isDocument == true) {
        var elements = $(searchresult.element);
        ScrollToElement(elements[searchresult.index]);
    }
    else {
        ScrollToElement($(".keyword")[0]);
    }
}

function pageY(elem) {
    return elem.offsetParent ? (elem.offsetTop + pageY(elem.offsetParent)) : elem.offsetTop;
}

var currentPage;
function ListenCurrentPageNum() {
    $(window).scroll(function () {
        var pages = $(".pagebreak");
        for (var i = pages.length - 1; i >= 0; i--) {
            if ($(window).scrollTop() >= pageY(pages[i])) {
                if (currentPage != pages[i]) {
                    currentPage = pages[i];
                    var jsonObj = "{ type:'PBO',  value1:'" + pages[i].getAttribute("data-count") + "'}";
                    window.external.notify(jsonObj);
                }
                break;
            }
        }
    });
}

function ScrollToElement(element) {
    if (element != null) {
        var offset = pageY(element);
        var max = $(document).height() - $(window).height() - 10;
        offset = offset > max ? max : offset;
        $(window).scrollTop(1 + offset);
    }
}

function ScrollToLinkContent(id) {
    var element = document.getElementById(id)
    ScrollToElement(element);
}


var highlighter;

function RangyInitail() {
    rangy.init();

    highlighter = rangy.createHighlighter();

    highlighter.addClassApplier(rangy.createClassApplier("highlight", {
        ignoreWhiteSpace: true,
        tagNames: ["span", "a"]
    }));

    highlighter.addClassApplier(rangy.createClassApplier("note", {
        ignoreWhiteSpace: true,
        elementTagName: "a",
        elementProperties: {
            href: "#",
            onclick: function () {
                var highlight = highlighter.getHighlightForElement(this);
                if (window.confirm("Delete this note (ID " + highlight.id + ")?")) {
                    highlighter.removeHighlights([highlight]);
                }
                return false;
            }
        }
    }));

};

function highlightSelectedText() {
    highlighter.highlightSelection("highlight");
    rangy.getSelection().removeAllRanges();
}

function noteSelectedText() {
    highlighter.highlightSelection("note");
    rangy.getSelection().removeAllRanges();
}

function removeHighlightFromSelectedText() {
    highlighter.unhighlightSelection();
}

function getFirstRange() {
    var sel = rangy.getSelection();
    return sel.rangeCount ? sel.getRangeAt(0) : null;
}

function insertNodeAtRange() {
    var range = getFirstRange();
    if (range) {
        var el = document.createElement("img");
        el.setAttribute("class", "annotation");
        el.setAttribute("src", "ms-appx-web:///Assets/annotation.png");
        range.insertNode(el);
    }
}

function insertTagAtRange() {
    var range = getFirstRange();
    if (range) {
        var el = document.createElement("img");
        el.setAttribute("class", "tag");
        el.setAttribute("src", "ms-appx-web:///Assets/AllTag.png");
        range.insertNode(el);
    }
}

function contextMenu() {
    var selectedText = rangy.getSelection().toString();
    var jObj = { type: 'SELECTED', value1: selectedText };
    var ToStr = JSON.stringify(jObj);
    window.external.notify(ToStr);
}