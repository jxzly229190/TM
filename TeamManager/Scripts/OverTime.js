$(function () {
    $("#OnDate,#Hours").attr({ class: "form-control", placeholder: "On" });
    $("#fromTime,#toTime").attr({ class: "form-control", placeholder: "Time" });

    $("#Save").click(function () {
        if ($("#OnDate").val() == "" || $("#Hours").val() == "") {
            alert("The date and last is required.");
            return false;
        }
        $("#ovInfo").submit();
    });

    $("#OnDate,#fromTime,#toTime").datetimepicker({
        weekStart: 1,
        todayBtn: 1,
        autoclose: 1,
        todayHighlight: true,
        forceParse: 0,
        showMeridian: 1,
        minView: 2,
        format: "mm/dd/yyyy"
    });

    function getMonday(offset) {
        var d = new Date();
        var date = new Date(d - ((d.getDay() || 7) - 1 - (offset || 0) * 7) * 864E5);
        var year = date.getFullYear();
        var month = ("0" + (date.getMonth() + 1)).slice(-2);
        var day = ("0" + date.getDate()).slice(-2);
        return month + "/" + day + "/" + year;
    }

    function getFirstDay() {
        var myDate = new Date();
        var year = myDate.getFullYear();
        var month = myDate.getMonth() + 1;
        if (month < 10) {
            month = "0" + month;
        }
        return month + "/" + "01/" + year;
    }

    function getLastDay() {
        var myDate = new Date();
        var year = myDate.getFullYear();
        var month = myDate.getMonth() + 1;
        if (month < 10) {
            month = "0" + month;
        }

        myDate = new Date(year, month, 0);
        return month + "/" + myDate.getDate() + "/" + year;
    }

    $("#searchForm").submit(function (e) {
        if ($("#fromTime").val() == "" || $("#toTime").val() == "") {
            alert("Please input from date and to date.");
            return false;
        }
        var from = new Date($("#fromTime").val());
        var to = new Date($("#toTime").val());
        if (from > to) {
            alert('From date cannot be greater than to date.');
            return false;
        }

        var $action = "OverTimeManager/Search";
        $(this).attr("action", $action + "?from=" + $("#fromTime").val() + "&to=" + $("#toTime").val() + "&status=" + $("#status").val());
    });

    $("#Week").click(function () {
        $("#fromTime").val(getMonday(0));
        $("#toTime").val(getMonday(1));
    });

    $("#Month").click(function () {
        $("#fromTime").val(getFirstDay());
        $("#toTime").val(getLastDay());
    });

    $("#search a").click(function () {
        var $href = "OverTimeManager/Export";
        $(this).attr("href", $href + "?from=" + $("#fromTime").val() + "&to=" + $("#toTime").val() + "&status=" + $("#status").val());
    });

    $("#overTimeMan table td a").click(function () {
        var $href = $(this).attr("href");
        if ($href.toString().toString().indexOf("?") >= 0) {
            //return false;
        }
        else {
            $(this).attr("href", $href + "?from=" + $("#fromTime").val() + "&to=" + $("#toTime").val() + "&status=" + $("#status").val());
        }
    });

    $("#status,#emailTo,#emailCc,#subject").attr({ class: "form-control" });
});