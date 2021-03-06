$(function () {
    $("#BreakOffFrom,#BreakOffTo,#CutFrom,#fromTime,#toTime").attr({ class: "form-control", placeholder: "Time" });

    $("#Save").click(function () {
        if ($("#BreakOffFrom").val() == "" || $("#BreakOffTo").val() == "") {
            alert("DateFrom and DateTo is required.");
            return false;
        }
        var dateFrom = new Date($("#BreakOffFrom").val());
        var dateTo = new Date($("#BreakOffTo").val());
        if (dateFrom >= dateTo) {
            alert("From date cannot be equal or greater than to date.");
            return false;
        }
        if (!checkTime(dateFrom)) {
            alert("Our normal working hours are 9 AM to 6 PM!");
            return false;
        }
        if (!checkTime(dateTo)) {
            alert("Our normal working hours are 9 AM to 6 PM!");
            return false;
        }
        $("#bfInfo").submit();
    });

    $("#BreakOffFrom,#BreakOffTo").datetimepicker({
        weekStart: 1,
        todayBtn: 1,
        autoclose: 1,
        todayHighlight: true,
        forceParse: 0,
        showMeridian: 0,
        minView: 0,
        minuteStep: 60,
        format:"mm/dd/yyyy hh:ii"
    });

    $("#fromTime,#toTime").datetimepicker({
        weekStart: 1,
        todayBtn: 1,
        autoclose: 1,
        todayHighlight: true,
        forceParse: 0,
        showMeridian: 1,
        minView: 2,
        format: "mm/dd/yyyy"
    });

    function checkTime(time) {
        var ar = ['9:00', '18:00'];
        var current = parseInt(time.getHours()) * 60 + parseInt(time.getMinutes());
        var ar_begin = ar[0].split(':');
        var ar_end = ar[1].split(':');
        var b = parseInt(ar_begin[0]) * 60 + parseInt(ar_begin[1]);
        var e = parseInt(ar_end[0]) * 60 + parseInt(ar_end[1]);
        if (current >= b && current <= e) return true;
        else return false;
    }


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

        var $action = "BreakOffManager/Search";
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
        var $href = "BreakOffManager/Export";
        $(this).attr("href", $href + "?from=" + $("#fromTime").val() + "&to=" + $("#toTime").val() + "&status=" + $("#status").val());
    });

    $("#breakOffMan table td a").click(function () {
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