/*
 * Using hitor.js from http://www.the-art-of-web.com/javascript/search-highlight to do search keyword highlight
 * 
 */
function highlightSearchKeyword(keywords){
	var searchKeywordHilitor = new Hilitor("content");
	searchKeywordHilitor.apply(keywords);
}

$(document).ready(function(){
	highlightSearchKeyword($("#data_container").data("highlightedkeyword"));
});