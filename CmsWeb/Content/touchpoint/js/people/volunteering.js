$(function () {

    var getSubmitDialog = function () {
        return $('#dialogHolder');
    };

    $('.showSubmitDialog').click(function (ev) {        
        ev.preventDefault();
        var id = $(this).attr('data-cid');
        var type = $(this).attr('data-ctype');
        var submitDialog = getSubmitDialog();
        $.post('/Volunteering/DialogSubmit/' + id + '?type=' + type, null, function (data) {
            $('.modal-content', submitDialog).html(data);
            submitDialog.modal();
        });
    });

    $(document).on("click", "#submitCheck #btnFinalSubmit", function (ev) {
        ev.preventDefault;
        $("#btnFinalSubmit").val("Please Wait...");
        $('#submitCheck').submit(function () {
            $("#btnFinalSubmit").attr('disabled', true)
        });
    });

    

    $('.showCreateDialog').click(function (ev) {
        ev.preventDefault();
        var id = $(this).attr('data-pid');
        var type = $(this).attr('data-ctype');
        var submitDialog = getSubmitDialog();
        $.post('/Volunteering/DialogType/' + id + '?type=' + type, null, function (data) {
            $('.modal-content', submitDialog).html(data);
            $('.modal-title', submitDialog).text('Select Check Type');
            submitDialog.modal();
        });
    });

    $('.showEditDialog').click(function (ev) {
        ev.preventDefault();
        var id = $(this).attr('data-cid');
        var type = $(this).attr('data-ctype');
        var submitDialog = getSubmitDialog();
        $.post('/Volunteering/DialogEdit/' + id + '?type=' + type, null, function (data) {
            $('.modal-content', submitDialog).html(data);
            submitDialog.modal();
        });
    });

    $('.showDeleteDialog').click(function (ev) {
        ev.preventDefault();
        var id = $(this).attr('data-cid');

        swal({
            title: 'Delete Check',
            type: 'warning',
            showCancelButton: true,
            confirmButtonClass: 'btn-danger',
            confirmButtonText: 'Yes, delete it!',
            closeOnConfirm: false
        },
            function () {
                $.post('/Volunteering/DeleteCheck/' + id, null, function (ret) {
                    if (ret && ret.error)
                        swal('Error!', ret.error, 'error');
                    else {
                        swal({
                            title: 'Deleted!',
                            type: 'success'
                        },
                            function () {
                                document.location.reload();
                            });
                    }
                });
            });
    });

    $('.deleteVolDocument').click(function (e) {
        e.preventDefault();        
        var form = $(this).parents('form');        
        swal({
            title: 'Are you sure?',
            type: 'warning',
            showCancelButton: true,
            confirmButtonClass: 'btn-danger',
            confirmButtonText: 'Yes, delete it!',
            closeOnConfirm: false
        },
            function () {                
                form.submit();
            }
        );        
    });

    $('#documents a.editable').editable({
        mode: 'inline',
        type: 'text',
        url: '/Volunteering/EditForm/'
    });

    $.InitFunctions.Editable = function () {
        $("#ev-form a.editable").editable();
        $("#ev-form a.editable-bit").editable({ type: 'checklist', mode: 'popup', source: { 'True': 'True' }, emptytext: 'False' });
    };
    $.InitFunctions.Editable();
    $.InitFunctions.ExtraEditable();
});

function confirmDelete(ev) {
    ev.preventDefault();
    
}
