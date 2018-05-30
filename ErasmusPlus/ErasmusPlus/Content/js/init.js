$(function () {
    $("select").each(function() {
        var instance = $(this).select2();
        Erasmus.PageContext.Selects.push(instance);
    });

    $(".datepicker").datepicker({
        autoclose: true,
        format: "yyyy-MM-dd"
    });

    $('input[type="checkbox"], input[type="radio"]').iCheck({
        checkboxClass: "icheckbox_minimal-blue",
        radioClass: "iradio_minimal-blue"
    });

    $("#datemask").inputmask("yyyy-MM-dd", { "placeholder": "yyyy-MM-dd" });
    $("#datemask2").inputmask("yyyy-MM-dd", { "placeholder": "yyyy-MM-dd" });
    $("[data-mask]").inputmask();
});