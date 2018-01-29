/*execute javascript which required to be excuted after document is ready*/
$(function(){
	var readyScript = $("#document_ready_script_container").text() ;
	if(readyScript != "#DOCUMENT_READY_SCRIPT#"){
		eval(readyScript);
	}
});