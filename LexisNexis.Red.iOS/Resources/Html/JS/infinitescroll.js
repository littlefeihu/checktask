var isLoading = false;//indicate whether loading content or not, true: content(next page or previous page) is loading, otherwise means no content is loading


String.prototype.trim = function() { 
     return this.replace(/(^\s*)|(\s*$)/mg, ""); 
} 

$(document).ready(function(){
	var scrollToId = $("#data_container").data("scrollToId");
	if(typeof(scrollToId) != "undefined" && scrollToId !=""){
		$(window).scrollTo($("#"+scrollToId));
	}

	$(window).scroll(function(){

		scrollToPBOPageNumber();

 		var curPageDiv = $(".page_container:first");
		$(".page_container").each(function(){
			if($(window).scrollTop() > $(this).offset().top){
				curPageDiv = $(this);
			}
		});
		if(curPageDiv.data("tocid") != $("#data_container").data("highlightedtocid")){
				sendRequest("looseleaf://highlighttoc?tocid="+ curPageDiv.data("tocid"));
				$("#data_container").data("highlightedtocid", curPageDiv.data("tocid"));
		}


		if(isLoading == false){
			if($(window).scrollTop() < 10 && $("#beginLoadingTitle").data("tocid") != "-1"){//scroll to previous page, "-1" means no more pages to which scroll up or down
				isLoading = true;
				showBeginLoading();
				loadingPreviousPage();
				removeInvisiblePage ();
			} else if( ( $(window).scrollTop() + document.body.clientHeight + 10) >= $(document).height()  && $("#endLoadingTitle").data("tocid") != "-1" ){//scroll to next page
				isLoading = true;
				showEndLoading();
				loadingNextPage();
				removeInvisiblePage ();

			}
		}
	});
});


function removeInvisiblePage(){
		//remove invisible page_container to improve performance

		if($(".page_container").length >= 3){
			var toBeRemoved = new Array(), i=0;
			$(".page_container").each(function(){
				if(($(this).offset().top + $(this).height()) < $(window).scrollTop() ){
					toBeRemoved[i++] = $(this);
				}
			});
			if(toBeRemoved.length > 0){
				$("#beginLoadingTitle").data("tocid", $(toBeRemoved[i-1]).data("tocid"));
				$("#beginLoadingTitle").text($(toBeRemoved[i-1]).data("toctitle"));
			}

			for(x in toBeRemoved){
				toBeRemoved[x].remove();
			}


			toBeRemoved = new Array();
			i = 0;
			var bottomOffset = $(window).scrollTop() + $(window).height();
			$(".page_container").each(function(){
				if( $(this).offset().top  > bottomOffset){
					toBeRemoved[i++] = $(this);
				}
			});
			if(toBeRemoved.length > 0){
				$("#endLoadingTitle").data("tocid", $(toBeRemoved[0]).data("tocid"));
				$("#endLoadingTitle").text($(toBeRemoved[0]).data("toctitle"));

			}
			for(x in toBeRemoved){
				toBeRemoved[x].remove();
			}
		}
}

function showEndLoading (){
	$("#endloading").slideToggle(50, function(){
		$(window).scrollTo("max");//scroll to end
	});
}

function loadingNextPage (){
	setTimeout(function(){
		$("#content").append("<div data-tocid='" + $("#endLoadingTitle").data("tocid") + "' data-toctitle='"+ $("#endLoadingTitle").text() +"' class='page_container next_page'></div>");
		$("#endloading").slideToggle();
		sendRequest("looseleaf://loadpage?page=next&tocid=" + $("#endLoadingTitle").data("tocid"));
		}, 
		200);
}

function showBeginLoading (){
	$("#beginloading").slideToggle(50, function(){
		$(window).scrollTo(1);
	});
}

function loadingPreviousPage (){
	setTimeout(function(){
		$("#beginloading").slideToggle(0);
		$("#content").prepend("<div data-tocid='" + $("#beginLoadingTitle").data("tocid") + "' data-toctitle='" + $("#beginLoadingTitle").text().replace("'","\'") + "' class='page_container'></div>");
		$(window).scrollTo(1);
		sendRequest("looseleaf://loadpage?page=previous&tocid="+ $("#beginLoadingTitle").data("tocid"));
		}, 
		500);
}


function appendPageContent(content, position){
	content = content.replace(/##/g, "\"");
	if(position == "next"){
		$("div.page_container:last").html(content);
	}else{//previous
		$("div.page_container:first").html(content);
		$(window).scrollTo($("div.page_container:first").height() - 29);
	}

	isLoading = false;

}

function setLoadingTOCTitle(tocid, title, position){
	if(position == "next"){
		$("#endLoadingTitle").text(title);
		$("#endLoadingTitle").data("tocid", tocid)
	}else if(position == "previous"){
		$("#beginLoadingTitle").text(title);
		$("#beginLoadingTitle").data("tocid", tocid)
	}
}

function scrollToPBOPageNumber(){
	var scrollToPageNum = 0;
	$(".pagebreak").each(function(){
		if(($(this).offset().top + $(this).height()) < $(window).scrollTop() ){
 			scrollToPageNum = $(this).data("count");
		}else{
		   	return false;
		}
	});
	if(scrollToPageNum != 0 && scrollToPageNum != "0" && scrollToPageNum != $("#data_container").data("PBOPageNum")){
		$("#data_container").data("PBOPageNum", scrollToPageNum);
		sendRequest("looseleaf://setCurrentPBOPageNum?pageNum="+ scrollToPageNum);
	}
}

/**
 * Serveral sequential "window.location.href=''" can not be captured by webview delegate
 * Using iframe to fix this issue
 * https://stackoverflow.com/questions/2934789/triggering-shouldstartloadwithrequest-with-multiple-window-location-href-calls
 */
function sendRequest(url){
	iFrame = document.createElement("IFRAME");
	iFrame.setAttribute("src", url);
	document.body.appendChild(iFrame); 
	iFrame.parentNode.removeChild(iFrame);
	iFrame = null;
}