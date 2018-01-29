<!--        mouse drag-->
 var currentDivID = '';
$(document).ready(function (){
	$(window).scroll(function(){
		var scrollTop = $(this).scrollTop();
		var scrollHeight = $(document).height();
		var windowHeight = $(this).height();

		if (scrollTop!=0 && scrollTop!=(scrollHeight-windowHeight)) {
		    var childElements = document.getElementById("content_body").children;
		    var iStart = childElements.length-1;

            for (var i=iStart; i >= 0; i--) {   
                var elementID = childElements[i].id;
                if (elementID=='pageSeparator' 
                  ||elementID=='div_nextload'
                  ||elementID=='div_preload') {
                    continue;
                }

                var divHeight = childElements[i].scrollHeight;
                var offsetTop = childElements[i].offsetTop;
                var offset = scrollTop-offsetTop;
                if (offset<divHeight && offset>0 && currentDivID.localeCompare(childElements[i].id)!=0) {

//	                  var jsonObj = "{offset: " + (scrollTop-offsetTop)
//	                                +", divHeight: " + divHeight
//	                                +", scrollTop: " + scrollTop
//	                	            +", offsetTop: " + offsetTop +"\n"
//					                +", currentDivID: " + currentDivID +"\n"
//					                +", childElements["+i+"].id: " + childElements[i].id
//					                + "}";
                    

                    currentDivID = childElements[i].id;
					var jsonObj = "{TOCNodeID:" + currentDivID + "}";
			        window.native.CustomScrollDrag_(jsonObj);
			        break;
                }
            }             
        }
	});
});

<!--        mouse up-->
function customMouseUp () {
    if(event.button==2) {
	    var jsonObj = "{value1: " + window.event.clientX + ", value2: " + window.event.clientY + "}";
	    window.native.CustomMouseUp_(jsonObj);
	    //rectsForSelection();
     }else {
         handleAhref();
     }
}

function customDobleClick(){
    var selectObj = ((window.getSelection)?window.getSelection():document.getSelection());
    var rect = selectObj.getBoundingClientRect();
    var jsonObj = "{value1: " + rect.left + ", value2: " +rect.top +", value3: "+ rect.width + ", value4: " + rect.height + "}";
    window.native.CustomMouseUp_(selectObj);
}

<!--        selection-->
function rectsForSelection() {
    var selectObj = ((window.getSelection)?window.getSelection():document.getSelection());
    var rect = selectObj.getBoundingClientRect();
    var jsonObj = "{value1: " + rect.left + ", value2: " +rect.top +", value3: "+ rect.width + ", value4: " + rect.height + "}";
    window.native.CustomMouseUp_(jsonObj);
}

<!--        next page load-->
function addNextPageLoadingDiv (html) {
    var divhtml = "<div id='div_nextload' class='div_nextloading' style='display:'''>"
                    +"<br>"
                    +"<img src='loading_gif.gif' width=18pt height=18pt/>"
                    +"<br>"
                    +"<span style='color:#949494;font-size:14px;font-weight:normal'>"
                    +html
                    +"</span>"
                    +"<br>"
                    +"<div id='nextPageFrame'></div>"
                    +"</div>";
     $("#content_body").append(divhtml);
}

function appendHtml_NextPage (html) {
   removeLoadingDiv();
   addNextPageSeparator();

   $("#content_body").append(html);

   var result = "{appendHtml_NextPage: 'append html finished'}";
   return result;
}

function addNextPageSeparator() {
    $("#content_body").append("<div id='pageSeparator' class='separator_div'>");
}

function ScrollTo(offset, time, status) {
    if (status == 0) {
        offset = $(window).scrollTop() + offset;
    }
    $("html,body").animate({ scrollTop: offset }, time);
}

<!--        previous page load-->
function addPrePageLoadingDiv (html) {
    var divhtml = "<div id='div_preload' class='div_preloading' style='display:'''>"
                    +"<div id='prePageFrame'></div>"
                    +"<br>"
                    +"<img src='loading_gif.gif' width=18pt height=18pt/>"
                    +"<br>"
                    +"<span style='color:#949494;font-size:14px;font-weight:normal'>"
                    +html+""
                    +"</span>"
                    +"<br>"
                    +"<br>"
                    +"</div>";
     $("#content_body").prepend(divhtml);
}

function prependHtml_PrePage (html) {
    removeLoadingDiv();
    addPrePageSeparator();

    var top = $(document).height();

    $("#content_body").prepend(html);

    var offset = $(document).height() - top - 100;
    ScrollTo(offset+100, 100, 1);
    ScrollTo(offset, 400, 1);

<!--            $(window).scrollbottom();-->
   var result = "{prependHtml_PrePage: 'append html finished'}";
   return result;
}

function addPrePageSeparator() {
     $("#content_body").prepend("<div id='pageSeparator' class='separator_div'>");
}

function removeLoadingDiv() {
    $("#div_nextload").remove();
    $("#div_preload").remove();
}

function setBodyFontSize(fontSize) {
    $("#content_body").style.fontSize = fontsize+"pt";
}

function handleAhref() {
	$("a").click(function(e) {
	    if (this.href == null || this.href == "" || 
	    this.href.indexOf("#FOOT")>=0|| this.href.indexOf("#")>=0)  // this.href.indexOf("about:blank#")
            return;

        e.preventDefault();   

        var jsonObj = this.href;
        alert(jsonObj);
        window.native.ClickAHref(jsonObj);
	})
}

$(function () {
    $('a.something').on("click", function (e) {
        e.preventDefault();
        window.native.ClickAHref("onclick");
    });
});

//infinite scroll
function listenCurrentPageNum() {
    var pages = $(".pagebreak");
    for (var i = pages.length - 1; i >= 0; i--) {
        var offset = $(window).scrollTop()-pages[i].offsetTop;
        var offsetBottom = $(window).scrollTop()-(pages[i].offsetTop+pages[i].scrollHeight);
        if ( offset>=0) {
            var value = pages[i].getAttribute("data-count");
            var jsonObj = "{listenCurrentPageNum: " + value + "}";
            return jsonObj;
        }
    }
}

function scrollToSearchPage(pageNumber){
    var pages = $(".pagebreak");
    for (var i = pages.length - 1; i >= 0; i--) {
        var value = pages[i].getAttribute("data-count");
        if (value.toString() == pageNumber) {
            var offset = pages[i].offsetTop;
            ScrollTo(offset, 50, 1);
            break;
        }
    }

}

