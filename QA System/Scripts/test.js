$(document).ready(function () {
    clearMsgBox();
    InitStatusButton();
    //Found Test Issue
    InitFoundIssueTable();

    //Save Code Evalueate
    $("#drpCodeEvaluate").change(function () {
        SaveTestCodeEvaluate($("#drpCodeEvaluate option:selected").val());
    })

    //Test Comments
    $("#divAddComments").hide();
})

function clearMsgBox() {
    $("#Msgbox").removeClass().html("");
}

function InitStatusButton()
{    
    var buttonIn = $("#statusInProgress");
    var buttonFound = $("#statusFoundIssue");
    var buttonTested = $("#statusTested");
    
    var txtStatus = $("#txtStatus");
    buttonIn.hide();
    buttonFound.hide();
    buttonTested.hide();
    
    if (txtStatus.val() == "InProgress") {
        buttonTested.show();
        buttonFound.show();
    }
    else if (txtStatus.val() == "FoundIssue") {
        buttonTested.show();
    }
    else {
        buttonIn.show();
    }
}

function InitFoundIssueTable()
{
    var isHasFoundIssue = $("#txtHasFoundIssue").val();
    var txtStatus = $("#txtStatus").val();
    var divFoundIssue = $("#divFoundIssue");
    if (isHasFoundIssue == "True" || txtStatus == "FoundIssue") {
        divFoundIssue.show();
        $(".tableClass tr:not(.trTitle):odd").addClass("odd");
        $(".tableClass tr:not(.trTitle):even").addClass("even");
    }
    else {
        divFoundIssue.hide();
    }
}

//Test Status 
function saveTestStatus(statusValue){
    $.messager.progress();
    clearMsgBox();
    var testReportID = $("#txtTestReportID").val();
    $('#formTest').form('submit', {
        url: "../Test/SaveTestStatus?testReportID=" + testReportID + "&statusValue=" + statusValue,
        onSubmit: function () {
            if (testReportID == '' || statusValue == '') {
                $.messager.progress('close');	// hide progress bar while the form is invalid
                return false;
            }
            return true;
        },
        success: function (msg) {
            var resultJson = eval("(" + msg + ")");            
            if (resultJson.IsSuccess) {
                $("#txtStatus").val(resultJson.TestStatus);
                $("#spanStatus").html(resultJson.TestStatusDispalyName);
                $("#Msgbox").removeClass().addClass("successMsg").append("Success to update Test Status!");
                InitStatusButton();
                InitFoundIssueTable();
            }
            else {
                $("#Msgbox").removeClass().addClass("errorMsg").append(resultJson.msg);
            }
            $.messager.progress('close');	// hide progress bar while the form is invalid
        }
    });
}

//Test Result
function saveTestResult(resultValue)
{
    $.messager.progress();
    clearMsgBox();
    var testReportID = $("#txtTestReportID").val();
    $('#formTest').form('submit', {
        url: "../Test/SaveTestResult?testReportID=" + testReportID + "&resultValue=" + resultValue,
        onSubmit: function () {
            if (testReportID == '' || resultValue == '') {
                $.messager.progress('close');	// hide progress bar while the form is invalid
                return false;
            }
            return true;
        },
        success: function (msg) {
            var resultJson = eval("(" + msg + ")");
            if (resultJson.IsSuccess) {
                $("#spanResult").html(resultJson.TestResultDisplayName);
                $("#Msgbox").removeClass().addClass("successMsg").append("Success to update Test Result!");
                InitStatusButton();
            }
            else {
                $("#Msgbox").removeClass().addClass("errorMsg").append(resultJson.msg);
            }
            $.messager.progress('close');	// hide progress bar while the form is invalid
        }
    });
}

//Code Evalute
function SaveTestCodeEvaluate(codeValue) {
    $.messager.progress();
    clearMsgBox();
    var testReportID = $("#txtTestReportID").val();
    $('#formTest').form('submit', {
        url: "../Test/SaveTestCodeEvaluate?testReportID=" + testReportID + "&codeValue=" + codeValue,
        onSubmit: function () {
            if (testReportID == '' || codeValue == '') {
                $.messager.progress('close');	// hide progress bar while the form is invalid
                return false;
            }
            return true;
        },
        success: function (msg) {
            var resultJson = eval("(" + msg + ")");
            if (resultJson.IsSuccess) {                
                $("#Msgbox").removeClass().addClass("successMsg").append("Success to update CodeEvaluate!");
                InitStatusButton();
                $("html, body").animate({ scrollTop: $('#Msgbox').offset().top });
            }
            else {
                $("#Msgbox").removeClass().addClass("errorMsg").append(resultJson.msg);
            }
            $.messager.progress('close');	// hide progress bar while the form is invalid
        }
    });
}

//Test Comments
$("#addComment").click(function () {
    $("#divAddComments").show();
    $(this).hide();
})
$("#cancelComment").click(function () {
    $("#divAddComments").hide();
    $("#addComment").show();
})
$("#saveComment").click(function () {
    saveTestComments();
});
function saveTestComments() {
    clearMsgBox();
    var testReportID = $("#txtTestReportID").val();
    var testComments = $("#txtComments").val();
    $.ajax({
        type: 'POST',
        url: '../Test/SaveTestComments/',
        data: { testReportID: testReportID, testComments: testComments },
        error: function (xhr) {
            alert('Error: ' + xhr.statusText);
        },
        success: function (result) {
            var resultJson = eval("(" + result + ")");
            if (resultJson.IsSuccess) {
                location.reload();
            }
            else {
                $("#Msgbox").removeClass().addClass("errorMsg").append(resultJson.msg);
            }
        }
    });
}

// add Issue begin
$("#addIssue").click(function () {
    $('#fm').form('clear');
    $("#testReportID").val($("#txtTestReportID").val());
    addIssue();
});

function addIssue() {
    //$("html, body").animate({ scrollTop: $('#addIssue').offset().top });
    $('#dlgAddIssue').dialog({ title: 'Add New Issue' }).dialog('open');    
    $('#issueFile').MultiFile({ list: '#issueFile-list' });
    $("#issueFile-list").html("");    
}

//Show Edit Issue 
function EditIssue(testIssueID) {
    addIssue();
    $.ajax({
        type: 'POST',
        url: '../Test/GetTestIssue/',
        data: { testIssueID: testIssueID },
        error: function (xhr) {
            alert('Error: ' + xhr.statusText);
        },
        success: function (result) {
            if (result != null)
            {
                $("#testReportID").val($("#txtTestReportID").val());
                $("#txtIssueDes").val(result.Description);
                $("#testIssueID").val(result.TestIssueID);
                //Show attachment
                for(var i=0; i<result.TestIssueFiles.length; i++)
                {                                        
                    showAttachmentList(result.TestIssueFiles[i].TestIssueFileID, result.TestIssueFiles[i].TestIssueFileName, result.TestIssueFiles[i].TestIssueFileExt, true, "issueFile-list");
                }                
            }
            $('#issueFile').MultiFile({ list: '#issueFile-list' });
        }
    });
}

// Save Test Issue
function SaveIssue() {
    $.messager.progress();
    clearMsgBox();
    var txtIssueDes = $("#txtIssueDes").val();
    var txtTestReportID = $("#testReportID").val();
    $('#fm').form('submit', {
        url: "../Test/SaveTestIssue",
        onSubmit: function () {
            if (txtIssueDes == '' || txtTestReportID == '') {
                $.messager.progress('close');	// hide progress bar while the form is invalid
                $("#Msgbox").removeClass().addClass("errorMsg").append("Please check data.");
                return false;
            }
            return true;
        },
        success: function (msg) {
            var resultJson = eval("(" + msg + ")");
            if (resultJson.IsSuccess) {
                $('#dlgAddIssue').dialog('close');
                location.reload();
            }
            else {
                $("#Msgbox").removeClass().addClass("errorMsg").append(resultJson.errorMsg);
            }
            $.messager.progress('close');	// hide progress bar while the form is invalid
        }
    });
}

//add Issue end