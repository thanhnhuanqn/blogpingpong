$(document).ready(function () {

    function removeUnicode(str) {
        str = str.toLowerCase();
        str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
        str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
        str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
        str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
        str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
        str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
        str = str.replace(/đ/g, "d");
        str = str.replace(/-+-/g, "-"); //thay thế 2- thành 1-
        str = str.replace(/^\-+|\-+$/g, "");//cắt bỏ ký tự - ở đầu và cuối chuỗi

        return str;
    }

    $('a[data-post]').click(function (e) {
        e.preventDefault();
        var $this = $(this);
        var message = $this.data('post');
        if (message && !confirm(message)) {
            return;
        }
        var antiForgeryToken = $('#anti-forgery-form input');
        var antiForgeryInput = $('<input type="hidden">').attr("name", antiForgeryToken.attr("name"))
            .val(antiForgeryToken.val());

        $('<form>')
            .attr('method', 'post')
            .attr('action', $this.attr('href'))
            .append(antiForgeryToken)
            .appendTo(document.body)
            .submit();
    });

    $("[data-slug]").each(function () {

        var $this = $(this);

        var $sendSlugFrom = $($this.data('slug'));

        $sendSlugFrom.keyup(function () {
            var slug = $sendSlugFrom.val();
            slug = removeUnicode(slug);
            slug = slug.replace(/[^a-zA-Z0-9\s]/g, "");
            slug = slug.replace(/\s+/g, "-");
            if (slug.charAt(slug.length - 1) == "-")
                slug = slug.sustr(0, slug.length - 1);

            $this.val(slug);
        });

    });

    $("#deleteActionRun").click(function () {
        return confirm("Are you sure you want to delete?");
    });

    $('#select-all').click(function (event) {
        if (this.checked) {
            // Iterate each checkbox
            $(':checkbox').each(function () {
                this.checked = true;
            });
        }
        if (!this.checked) {
            // Iterate each checkbox
            $(':checkbox').each(function () {
                this.checked = false;
            });
        }
    });


});