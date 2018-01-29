// Namespace
var android = {};
android.red = {};

android.red.getSelection = function(purpose, saveSelection)
{
	var sel = window.getSelection();
	var text = sel.toString();

	var l = 0, t = 0, r = 0, b = 0;
	if (sel.rangeCount)
	{
		range = sel.getRangeAt(0).cloneRange();
		if (range.getClientRects)
		{
			rects = range.getClientRects();
			if (rects.length > 0)
			{
				rect = rects[0];
			}
			l = rect.left;
			t = rect.top;
			r = rect.right;
			b = rect.bottom;
		}
	}

	if(saveSelection)
	{
		android.red.saveSelection(purpose);
	}

	RedController.jsLog("Selected: " + purpose + ", " + text + ", " + l + ", " + t + ", " + r + ", " + b);
	RedController.Selected(purpose, text, l, t, r, b);
};

android.red.currentPageId = null;
android.red.onscroll = function()
{
	var winTop = $(window).scrollTop();

	// Toc switch
	$(".tocpagediv").each(function(){
		var page = $(this);

		var pageTop = page.offset().top;
		var pageHeight = page.outerHeight();
		var pageBottom = pageTop + pageHeight;

		if(pageTop <= winTop && pageBottom >= winTop)
		{
			var pageId = page.attr("id");
			if(pageId != android.red.currentPageId)
			{
				android.red.currentPageId = pageId;
				android.red.currentPboPageId = null;
				RedController.jsLog("Scroll to :" + pageId);
				RedController.ScrollToPage(pageId, null);
			}

			android.red.locatePboPage();

			// Stop search current page;
			// We found now;
			return false;
		}
	});
}

android.red.currentPboPageId = "";
android.red.locatePboPage = function()
{
	var winTop = $(window).scrollTop();
	if(!android.red.currentPageId)
	{
		return;
	}

	var lastPboPage = "";
	var bodyBoderTopWidth = android.red.getBodyBoderTopWidth();
	$("#" + android.red.currentPageId + " .pagebreak").each(function(){
		var pboPage = $(this);
		if(pboPage.offset().top - 10 >= winTop)
		{
			// Current Pbo Page is exceed the top of the screen;
			// Search finished;
			return false;
		}

		lastPboPage = pboPage.attr("data-count");
	});

	if(lastPboPage != android.red.currentPboPageId)
	{
		android.red.currentPboPageId = lastPboPage;
		RedController.ScrollToPage(android.red.currentPageId, lastPboPage);
	}
}

android.red.boundallowance = 20;
android.red.reachwebviewbound = function(delta)
{
	RedController.jsLog("enter js reachwebviewbound");
	var winTop = $(window).scrollTop();
	var winHeight = RedController.GetWebViewHeight();
	var winBottom = winTop + winHeight;
	var docHeight = $(document).height();

	// Scroll bound detection
	RedController.jsLog("winTop=" + winTop + ";winBottom=" + winBottom + ";docHeight=" + docHeight + ";delta=" + delta + ";");
	if(winTop <= android.red.boundallowance && winBottom >= $(document).height() - android.red.boundallowance)
	{
		// Reach both
		if(delta == 0)
		{
			return;
		}

		RedController.ScrollReachBound(delta);
	}
	else if(winTop <= android.red.boundallowance)
	{
		// Reach top
		if(delta >= 0)
		{
			return;
		}

		RedController.ScrollReachBound(delta);
	}
	else if(winBottom >= $(document).height() - android.red.boundallowance)
	{
		// Reach bottom
		if(delta <= 0)
		{
			return;
		}

		RedController.ScrollReachBound(delta);
	}
}

android.red.pagebreaker = "<div style='margin-left:-5px;margin-right:-5px' class='pagebreakdiv'><div style='width:100%;height:2px;background:#E1E1E1'></div><div style='width:100%;height:13px;background:#EEEEEE'></div></div>";

android.red.showtoploading = function(tocTitle)
{
	//$("#pagecontainer").prepend("<div id='current_loading_break'>" + $("#loading_template").html() + "</div>");
	$("#top_loading_stub").show();
	//$("#title_of_previous_toc").text(tocTitle);
	//scrollBy(0, -1 * $("#top_loading_stub").outerHeight());
}

android.red.prependpage = function(pageContentId, pageid)
{
	var removedPageIdList = removeDownOutSidePages();

	$("#pagecontainer").prepend(RedController.GetCachedLongStringParam(pageContentId));
	$(android.red.pagebreaker).insertAfter("#" + pageid);
	$("#top_loading_stub").hide();
	RedController.jsLog("pageid: " + pageid);
	scrollTo(
		0,
		$("#" + pageid).offset().top + $("#" + pageid).outerHeight() - 50);
	
	RedController.jsLog("offset.top:" + $("#" + pageid).offset().top);
	RedController.jsLog("Height:" + $("#" + pageid).outerHeight());
	RedController.jsLog("prependpage end");
	RedController.OnLoadingPageCompleted(pageid, removedPageIdList, $(document).height());
};

android.red.showbottomloading = function(tocTitle)
{
	$("#bottom_loading_stub").show();
	//$("#title_of_next_toc").text(tocTitle);
	scrollBy(0, 200);
}

android.red.appendpage = function(pageContentId, pageid)
{
	var removedPageIdList = removeUpOutSidePages();

	$("#pagecontainer").append(RedController.GetCachedLongStringParam(pageContentId));
	$(android.red.pagebreaker).insertBefore("#" + pageid);
	$("#bottom_loading_stub").hide();
	scrollBy(0, 30);

	RedController.jsLog("offset.top:" + $("#" + pageid).offset().top);
	RedController.jsLog("Height:" + $("#" + pageid).outerHeight());
	RedController.jsLog("appendpage end");
	RedController.OnLoadingPageCompleted(pageid, removedPageIdList, $(document).height());
};

var removeUpOutSidePages = function()
{
	var winTop = $(window).scrollTop();
	var winHeight = RedController.GetWebViewHeight();

	var pagesAndBreaks = $("#pagecontainer").children();
	var outsidePageAndBreaksIndex = new Array();
	var outsidePageIdList = "";
	for(var i = 0; i < pagesAndBreaks.length; ++i)
	{
		var item = $(pagesAndBreaks[i]);
		var itemTop = item.offset().top;
		var itemHeight = item.outerHeight();
		var itemBottom = itemTop + itemHeight;

		if(winTop - itemBottom > winHeight)
		{
			outsidePageAndBreaksIndex.push(item);
			if(item.attr("class") == "tocpagediv")
			{
				outsidePageIdList += item.attr("id") + ";";
			}
		}
		else
		{
			if(item.attr("class") == "pagebreakdiv")
			{
				outsidePageAndBreaksIndex.push(item);
			}

			break;
		}
	}

	if(outsidePageAndBreaksIndex.length == 0)
	{
		return null;
	}

	var lastItem = outsidePageAndBreaksIndex[outsidePageAndBreaksIndex.length - 1];
	var scrollBack = lastItem.offset().top + lastItem.outerHeight();
	for(var i = 0; i < outsidePageAndBreaksIndex.length; ++i)
	{
		RedController.jsLog("Remove :" + outsidePageAndBreaksIndex[i].attr("class"));
		outsidePageAndBreaksIndex[i].remove();
	}

	scrollTo(0, winTop - scrollBack);
	return outsidePageIdList
}

var removeDownOutSidePages = function()
{
	var winTop = $(window).scrollTop();
	var winHeight = RedController.GetWebViewHeight();
	var winBottom = winTop + winHeight;

	var pagesAndBreaks = $("#pagecontainer").children();
	var outsidePageAndBreaksIndex = new Array();
	var outsidePageIdList = "";
	for(var i = pagesAndBreaks.length - 1; i >= 0; --i)
	{
		var item = $(pagesAndBreaks[i]);
		var itemTop = item.offset().top;
		var itemHeight = item.outerHeight();
		var itemBottom = itemTop + itemHeight;

		if(itemTop - winBottom > winHeight)
		{
			outsidePageAndBreaksIndex.push(item);
			if(item.attr("class") == "tocpagediv")
			{
				outsidePageIdList += item.attr("id") + ";";
			}
		}
		else
		{
			if(item.attr("class") == "pagebreakdiv")
			{
				outsidePageAndBreaksIndex.push(item);
			}

			break;
		}
	}

	if(outsidePageAndBreaksIndex.length == 0)
	{
		return null;
	}

	for(var i = 0; i < outsidePageAndBreaksIndex.length; ++i)
	{
		RedController.jsLog("Remove :" + outsidePageAndBreaksIndex[i].attr("class"));
		outsidePageAndBreaksIndex[i].remove();
	}

	return outsidePageIdList
}

android.red.hilitor = null;
android.red.applyHighLight = function()
{
	var keywords = RedController.GetHighLightKeywords();
	RedController.jsLog("Highlight keywords: " + keywords);
	if(!keywords)
	{
		android.red.hilitor.remove();
		return;
	}

	android.red.hilitor.apply(keywords);
}

android.red.getBodyBoderTopWidth = function()
{
	return parseInt($("body").css('border-top-width').replace("px", ""));
}

android.red.scrollByNavigation = function()
{
	var scrollOp = JSON.parse(RedController.GetScrollOp());
	if(scrollOp.type == "top")
	{
		android.red.deferredScrollToJQElement();
	}
	else if(scrollOp.type == "highlight")
	{
		var searchHighLight = $("#" + scrollOp.tocid + " " + HilitorParas.Tag + "." + HilitorParas.Class + ":first");
		if(searchHighLight.length > 0)
		{
			if(scrollOp.headtype)
			{
				if(scrollOp.headtype.toUpperCase() !== "TOPOFDOCUMENT")
				{
					var head = $("#" + scrollOp.tocid + " " + scrollOp.headtype + ":eq(" + scrollOp.headsequence + ")");
					if(head.length > 0)
					{
						android.red.deferredScrollToJQElement(head);
						return;
					}
				}
			}
			else
			{
				android.red.deferredScrollToJQElement(searchHighLight);
				return;
			}
		}

		android.red.deferredScrollToJQElement();
	}
	else if(scrollOp.type == "pbopage")
	{
		var page = $("#" + scrollOp.tocid + " .pagebreak[data-count='" + scrollOp.pagenum + "']");
		if(page.length > 0)
		{
			android.red.deferredScrollToJQElement(page);
		}
		else
		{
			android.red.deferredScrollToJQElement();
		}
	}
	else if(scrollOp.type == "refpt")
	{
		var refpt = $("#" + scrollOp.tocid + " [id='" + scrollOp.refptid + "']");
		if(refpt.length > 0)
		{
			refpt.removeClass("hiddendiv");
			android.red.deferredScrollToJQElement(refpt);
		}
		else
		{
			android.red.deferredScrollToJQElement();
		}
	}
}

android.red.scrollByIndex = function()
{
	var scrollOp = JSON.parse(RedController.GetIndexScrollOp());
	if(scrollOp.type == "top")
	{
		android.red.deferredScrollToJQElement();
	}
	else if(scrollOp.type == "index")
	{
		var found = false;
		var firstTitle = $(".firsttitle").filter(function() {
			return $(this).html().toUpperCase() === scrollOp.title.toUpperCase();
		}).first();
		if(firstTitle.length > 0)
		{
			android.red.deferredScrollToJQElement(firstTitle);
		}
		else
		{
			android.red.deferredScrollToJQElement();
		}
	}
}

android.red.removeHighLight = function()
{
	android.red.hilitor.remove();
}

android.red.deferredScrollToJQElement = function(jqelement)
{
	if(jqelement)
	{
		setTimeout(function(){
			$.scrollTo(jqelement, 0);
		}, 200);
	}
	else
	{
		setTimeout(function(){
			$.scrollTo(0, 0);
		}, 200);
	}
}

android.red.lastSelection = null;
android.red.lastSelectionPurpose = null;
android.red.saveSelection = function(purpose)
{
    android.red.lastSelection = rangy.serializeSelection();
    android.red.lastSelectionPurpose = purpose;
}

android.red.removeSelection = function(removeCurrentSelection)
{
	if(removeCurrentSelection)
	{
		window.getSelection().removeAllRanges();
	}

    android.red.lastSelection = null;
    android.red.lastSelectionPurpose = null;
}

android.red.restoreSelection = function(notifyApp)
{
	if(android.red.lastSelection)
	{
		rangy.deserializeSelection(android.red.lastSelection);

		if(notifyApp)
		{
			android.red.getSelection(android.red.lastSelectionPurpose);
		}
	}
}

android.red.onresize = function()
{
	android.red.restoreSelection(true);
}

android.red.onselectionchange = function()
{
	var sel = window.getSelection();
	if(sel.toString().length > 0)
	{
		return;
	}

	if(!android.red.lastSelection)
	{
		return;
	}

	android.red.restoreSelection();
}