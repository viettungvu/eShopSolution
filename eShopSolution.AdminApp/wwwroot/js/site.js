function GetData(control) {
    var url = $(control).data('url');
    $.get(url).done(function (data) {
        $('.modal-content').html(data);
    });
}

function PostRequest(control) {
    var form = $(this).parents().find('form');
    var action = form.attr('action');
    $.post(action, form).done(function (response) {
        $('.modal').hide();
        ShowAlert(response);
    });
}

function ShowAlert(content) {
    $('.toast.toast-text').html(content);
    setTimeout(function () {
        $('.toast').toast('show');
    }, 200);
}