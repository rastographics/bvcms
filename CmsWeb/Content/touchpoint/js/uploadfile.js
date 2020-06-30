$(function () {
    $("#editor-modal,#unlayer-editor-modal,#main").on("click",
        "a.upload-file",
        function (ev) {
            ev.preventDefault();
            $("#upload-file-modal").modal('show');
        });
    $("#upload-file-modal").on("click",
        "a.upload-file",
        function (ev) {
            ev.preventDefault();
            var form = $(this).closest("form")[0];
            var oData = new FormData(form);
            var oReq = new XMLHttpRequest();
            oReq.open("POST", this.href, true);
            oReq.onload = function () {
                if (oReq.status === 200) {
                    $("#urlresult input").val(oReq.responseText);
                } else {
                    $("#uploaderror").innerHTML = "Error " + oReq.status + " occurred when trying to upload your file.";
                    $("#uploaderror").show();
                }
            };
            oReq.send(oData);
        });
    $('.done-upload-link-modal').click(function () {
        $("#urlresult input").select();
        document.execCommand('copy');
        $("#upload-file-modal").modal('hide');
    });
});
