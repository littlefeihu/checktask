/*
 * Using hitor.js from http://www.the-art-of-web.com/javascript/search-highlight to do search keyword highlight
 * 
 */

 $(document).ready(function(){
	highlightSearchKeyword(keyword);
});

function highlightSearchKeyword(keywords){
	var searchKeywordHilitor = new Hilitor();
	searchKeywordHilitor.apply(keywords);
}

function scrollToSearchContentPositon(searchHeader) {

    var domObj = $("h1,h2,h3,h4");
    for(var i=0; i<domObj.length; i++) {
	  var title = domObj[i].innerText.toLowerCase();
	  var index = title.indexOf(searchHeader.toLowerCase());
	  if (index == 0) {
	        var offset = pageY(domObj[i]);
	        var max = $(document).height() - document.body.clientHeight - 1;
	        offset = offset > max ? max : offset;
	        $(window).scrollTop(1 + offset);

	        var result = "{scrollToSearchContentPositon:"+ offset+"}";
	        return result;
	   }
	}
}

function pageY(element) {
    return element.offsetParent ? (element.offsetTop + pageY(element.offsetParent)) : element.offsetTop;
}

function scrollToSearchHeaderPositon(searchHeader) {
    var array = searchHeader.split(",");
    var header = array[0].toLowerCase();
    var index = parseInt(array[1]);
    var domObjects = $(header);
    var domObj = domObjects[index];
    if (domObj!= null) {
	    var offset = domObj.offsetTop;
	    var max = $(document).height() - document.body.clientHeight - 1;
		offset = offset > max ? max : offset;
		$(window).scrollTop(1 + offset);

		var result = "{scrollToSearchContentPositon:"+ offset + "}";
		 return result;
	 }
}