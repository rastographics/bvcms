CKEDITOR.plugins.add('specialLink', {
  icons: 'specialLink',
  init: function(editor) {
    editor.addCommand('specialLink', new CKEDITOR.dialogCommand('specialLinkDialog'));
    editor.ui.addButton('specialLink', {
      label: 'Insert Special Link',
      command: 'specialLink',
      toolbar: 'insert'
    });

    CKEDITOR.dialog.add('specialLinkDialog', this.path + 'dialogs/specialLink.js');
  }
});
