/**
 * @license Copyright (c) 2003-2013, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.html or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function (config) {
    config.toolbar = 'MyToolbar';

    config.allowedContent = true;
    config.toolbar_MyToolbar =
	[
    	{ name: 'document', items: ['Source', '-', 'Preview'] },
    	{ name: 'clipboard', items: ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-', 'Undo', 'Redo'] },
    	{ name: 'editing', items: ['Find', 'Replace', '-', 'SelectAll', '-', 'SpellChecker', 'Scayt'] },
    	{ name: 'links', items: ['Link', 'Unlink', 'Anchor'] },
    	'/',
    	{ name: 'basicstyles', items: ['Bold', 'Italic', 'Underline', 'Strike', '-', 'RemoveFormat'] },
    	{ name: 'paragraph', items: ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', '-', 'Blockquote', 'CreateDiv', '-', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'] },
    	{ name: 'insert', items: ['Image', 'Flash', 'Table', 'HorizontalRule', 'Smiley', 'SpecialChar'] },
    	'/',
    	{ name: 'styles', items: ['Styles', 'Format', 'Font', 'FontSize'] },
    	{ name: 'colors', items: ['TextColor', 'BGColor'] },
    	{ name: 'tools', items: ['Maximize', 'ShowBlocks', '-', 'About'] }
	];
};
