$(function() {
    $('body').on('click', 'a.exportquery', function(ev) {
        ev.preventDefault();
// ReSharper disable once UseOfImplicitGlobalInFunctionScope
        BootstrapDialog.show({
            message: $('<textarea style="overflow: auto;word-wrap: none;white-space: pre;width:100%;height:350px;word-wrap:initial;padding: 5px;"></textarea>').load(this.href),
            size: BootstrapDialog.SIZE_WIDE,
            closable: true,
            buttons: [{
                    label: 'Close',
                    action: function(dialogItself) {
                        dialogItself.close();
                    }
                }
            ]
        });
    });
});
