$(function(){$("#progress").dialog({autoOpen:!1,modal:!0,close:function(){$("#progress").html("<h2>Working...<\/h2>")}}),$(".bt").button(),$("#Send").click(function(){var n=$("#progress"),t;n.dialog("open"),$("#Body").text(CKEDITOR.instances.Body.getData()),t=$(this).closest("form").serialize(),$.post("/Email/QueueEmails",t,function(t){var i,r;if(t=="timeout"){window.location="/Email/Timeout";return}i=t.id,i==0?n.html(t.content):r=window.setInterval(function(){$.post("/Email/TaskProgress/"+i,null,function(t){t.substr(0,20).indexOf("<!--completed-->")>=0&&window.clearInterval(r),n.html(t)})},3e3)})}),$("#TestSend").click(function(){var n=$("#progress"),t;n.dialog("open"),$("#Body").text(CKEDITOR.instances.Body.getData()),t=$(this).closest("form").serialize(),$.post("/Email/TestEmail",t,function(t){if(t=="timeout"){window.location="/Email/Timeout";return}n.html(t)})});$("#textarea.editor").ckeditor({height:400,fullPage:!0,filebrowserUploadUrl:"/Account/CKEditorUpload/",filebrowserImageUploadUrl:"/Account/CKEditorUpload/",toolbar_Full:[["Source"],["Cut","Copy","Paste","PasteText","PasteFromWord","-","SpellChecker","Scayt"],["Undo","Redo","-","Find","Replace","-","SelectAll","RemoveFormat"],"/",["Bold","Italic","Underline","Strike","-","Subscript","Superscript"],["NumberedList","BulletedList","-","Outdent","Indent","Blockquote","CreateDiv"],["JustifyLeft","JustifyCenter","JustifyRight"],["Link","Unlink","Anchor"],["Image","Table","SpecialChar"],"/",["Styles","Format","Font","FontSize"],["TextColor","BGColor"],["Maximize","ShowBlocks","-","About"]]}),$("#CreateVoteTag").live("click",function(n){n.preventDefault(),CKEDITOR.instances.votetagcontent.updateElement();var t=$(this).closest("form").serialize();$.post("/Email/CreateVoteTag",t,function(n){CKEDITOR.instances.votetagcontent.setData(n,function(){CKEDITOR.instances.votetagcontent.setMode("source")})})})});CKEDITOR.on("dialogDefinition",function(n){var e=n.data.name,o=n.data.definition,t,r,u,f,i;e=="link"&&(t=o.getContents("advanced"),t.label="SpecialLinks",t.remove("advCSSClasses"),t.remove("advCharset"),t.remove("advContentType"),t.remove("advStyles"),t.remove("advAccessKey"),t.remove("advName"),t.remove("advLangCode"),t.remove("advTabIndex"),r=t.get("advRel"),r.label="SmallGroup",u=t.get("advTitle"),u.label="Message",f=t.get("advId"),f.label="OrgId/MeetingId",i=t.get("advLangDir"),i.label="Confirmation",i.items[1][0]="Yes, send confirmation",i.items[2][0]="No, do not send confirmation")})