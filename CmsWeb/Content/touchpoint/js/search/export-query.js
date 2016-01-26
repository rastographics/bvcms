$(function () {
    function SelectText(element) {
        var doc = document
            , text = doc.getElementById(element)
            , range, selection
        ;
        if (doc.body.createTextRange) {
            range = document.body.createTextRange();
            range.moveToElementText(text);
            range.select();
        } else if (window.getSelection) {
            selection = window.getSelection();
            range = document.createRange();
            range.selectNodeContents(text);
            selection.removeAllRanges();
            selection.addRange(range);
        }
    }
    $('body').on('click', 'a.exportquery', function (ev) {
        ev.preventDefault();
        // ReSharper disable once UseOfImplicitGlobalInFunctionScope
        BootstrapDialog.show({
            message: $('<pre id="code" style="font-family: Consolas,Courier;' +
                    'overflow: auto;word-wrap: none;white-space: pre;' +
                    'word-wrap:initial; padding: 5px;"></pre>')
                .load(this.href),
            size: 'size-wide',
            closable: true,
            buttons: [{
                label: 'Select All',
                action: function (dialog) {
                    SelectText('code');
                }
            }, {
                label: 'Close',
                action: function (dialogItself) {
                    dialogItself.close();
                }
            }]
        });
    });
});
