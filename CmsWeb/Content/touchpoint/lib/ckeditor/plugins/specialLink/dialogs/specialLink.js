CKEDITOR.dialog.add('specialLinkDialog', function(editor) {
  return {
    title: 'Create Special Link',
    minWidth: 400,
    minHeight: 200,
    contents: [
      {
        id: 'tab-basic',
        label: 'Create Special Link',
        elements: [
          {
            type: 'select',
            id: 'special-link-type',
            label: 'Type',
            items: [
              ['<not set>', ''],
              ['Register Link', 'registerlink'],
              ['Register Link 2', 'registerlink2'],
              ['Regrets Link', 'regretslink'],
              ['RSVP Link', 'rsvplink'],
              ['Send Link', 'sendlink'],
              ['Send Link 2', 'sendlink2'],
              ['Support Link', 'supportlink'],
              ['Vote Link', 'votelink'],
              ['Master Link (NOT SUPPORTED YET)', 'masterlink'],
            ],
            'default': '',
            validate: function() {
              if (!this.getValue()) {
                editor.showNotification('Type must be set.');
                return false;
              }
            },
            setup: function(element) {
              var hrefValue = element.getAttribute('href').replace('https://', '');
              var isValidValue = this.items.map(function(x) { return x[1]; }).some(function(item) {
                return item === hrefValue;
              })

              if (isValidValue) {
                this.setValue(hrefValue);
              }
              else {
                this.setValue('');
              }
            },
            commit: function(element) {
              element.setAttribute('href', 'https://' + this.getValue());
              if (element.getText() === '') {
                element.setText(this.getValue());
              }
            }
          },

          {
            type: 'text',
            id: 'special-link-orgid',
            label: 'OrgId / Meeting Id',
            validate: function() {
              if (!this.getValue()) {
                editor.showNotification('OrgId / Meeting Id cannot be empty.');
                return false;
              }

              var val = parseInt(this.getValue(), 10) || 0;
              if (val <= 0) {
                editor.showNotification('OrgId / Meeting Id must be a positive number.');
                return false;
              }
            },
            setup: function(element) {
              this.setValue(element.getAttribute('lang'));
            },
            commit: function(element) {
              element.setAttribute('lang', this.getValue());
            }
          },

          {
            type: 'text',
            id: 'special-link-message',
            label: 'Message',
            setup: function(element) {
              this.setValue(element.getAttribute('title'));
            },
            commit: function(element) {
              var val = this.getValue();
              if (val) {
                element.setAttribute('title', this.getValue());
              }
            }
          },

          {
            type: 'text',
            id: 'special-link-small-group',
            label: 'Small Group',
            setup: function(element) {
              var smallGroupVal = '';
              var relValue = element.getAttribute('rel');
              if (relValue !== 'nofollow') {
                smallGroupVal = relValue;
              }
              this.setValue(smallGroupVal);
            },
            commit: function(element) {
              var val = this.getValue();
              if (val) {
                element.setAttribute('rel', this.getValue());
              }
            }
          },

          {
            type: 'select',
            id: 'special-link-confirmation',
            label: 'Confirmation',
            items: [
              ['<not set>', ''],
              ['Yes, send confirmation', 'ltr'],
              ['No, do not send confirmation', 'rtl']
            ],
            'default': '',
            setup: function(element) {
              var dirValue = element.getAttribute('dir');
              var isValidValue = this.items.some(function(item) {
                return (item === dirValue);
              });

              if (isValidValue) {
                this.setValue(dirValue);
              }
              else {
                this.setValue('');
              }
            },
            commit: function(element) {
              var val = this.getValue();
              if (val) {
                element.setAttribute('dir', this.getValue());
              }
            }
          },

          {
            type: 'checkbox',
            id: 'special-link-in-new-tab',
            label: 'Open in new tab',
            setup: function(element) {
              this.setValue(element.getAttribute('target') === '_blank');
            },
            commit: function(element) {
              var val = this.getValue();
              if (val) {
                element.setAttribute('target', val);
              }
            }
          }
        ]
      }
    ],

    onShow: function() {
      var selection = editor.getSelection();
      var element = selection.getStartElement();
      var selectedText = selection.getSelectedText();

      if (element) {
        element = element.getAscendant('a', true);
      }

      if (!element || element.getName() !== 'a') {
        element = editor.document.createElement('a');
        element.setText(selectedText);
        this.insertMode = true;
      }
      else {
        this.insertMode = false;
      }

      this.element = element;
      if (!this.insertMode) {
        this.setupContent(this.element);
      }
    },

    onOk: function() {
      var dialog = this;
      var specialLink = this.element;

      this.commitContent(specialLink);

      if (this.insertMode) {
        editor.insertElement(specialLink);
      }
    }
  };
});
