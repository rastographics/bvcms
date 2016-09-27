$(function () {
    $('.clickDate').editable({
        mode: 'popup',
        type: 'text',
        url: '/TransactionHistory/Edit/',
        params: function (params) {
            var data = {};
            data['id'] = params.pk;
            data['value'] = params.value;
            return data;
        }
    });

    $("body").on("click", 'a.deltran', function (ev) {
        ev.preventDefault();
        var url = $(this).attr("href");
        swal({
            title: "Are you sure?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-danger",
            confirmButtonText: "Yes, delete it!",
            closeOnConfirm: false
        },
          function () {
              $.post(url, {}, function (ret) {
                  $("#history").replaceWith(ret);
                  swal({
                      title: "Deleted!",
                      type: "success"
                  });
              });
          });
        return false;
    });

    $("body").on("click", '#deleteall', function (ev) {
        ev.preventDefault();
        var url = $(this).attr("href");
        swal({
            title: "Are you sure you want to delete all transactions?",
            text: "This will not affect attendance.",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-danger",
            confirmButtonText: "Yes, delete them all!",
            closeOnConfirm: false
        },
          function () {
              $.post(url, {}, function () {
                  $("#history").replaceWith('');
                  swal({
                      title: "Deleted!",
                      text: "You will need to refresh your organization page to see the changes there.",
                      type: "success"
                  });
              });
          });
        return false;
    });

    $("body").on("click", '#repair', function (ev) {
        ev.preventDefault();
        var url = $(this).attr("href");
        swal({
            title: "Are you sure you want to repair transactions?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-warning",
            confirmButtonText: "Yes, Repair them",
            closeOnConfirm: false
        },
          function () {
              $.post(url, {}, function (ret) {
                  $("#history").replaceWith(ret);
                  swal({
                      title: "Repaired",
                      type: "success"
                  });
              });
          });
        return false;
    });
});
