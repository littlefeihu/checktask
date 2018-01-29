<!--        mouse up-->
function customMouseUp () {

     window.alert("handleAhref");

    if(event.button==2) {
	    var jsonObj = "{value1: " + window.event.clientX + ", value2: " + window.event.clientY + "}";
	    window.dictWebView.DictionaryMouseUp_(jsonObj);
     }else {
         handleAhref();
     }
}

function handleAhref() {
	$("a").click(function(e) {
	    if (this.href == null || this.href == "")
            return;

        

        e.preventDefault();   

        var jsonObj = this.href;
        window.dictWebView.ClickAHref_(jsonObj);
	})
}

$(function () {
    $('a.something').on("click", function (e) {
        e.preventDefault();
        window.dictWebView.ClickAHref("onclick");
    });
})



