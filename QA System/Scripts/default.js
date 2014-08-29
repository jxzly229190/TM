
function showAttachmentList(fileID, fileName, fileExt, showRemove, contorlID) {
    
    var remove = "<img src='../images/delete-sm.png' style='cursor:pointer;border:0;' onmouseover=\"this.src=\'../images/delete-ovr-sm.png\'\" onmouseout=\"this.src=\'../images/delete-sm.png\'\"/>";
    
    var a = '<span class="MultiFile-remove" id="' + fileID + '" onclick="removeAttachment(this)">' + remove + '</span>';
    
    var r = '<div class="MultiFile-label"></div>';
    if (showRemove) {
        var b = '<span class="MultiFile-title"><a href=\"../Attachment/' + fileID + "." + fileExt + "\" target=\"_blank\">" + fileName + '</a></span>';
        if (contorlID != ""){
            $("#" + contorlID).append($(r).append($(a), ' ', $(b)));
        }
        else {
            $("#" + fileID).append($(r).append($(a), ' ', $(b)));
        }
    }
    else {
        var b = '<span class="MultiFile-title"><img src="../Images/attach-document.png" /> &nbsp;<a href=\"../Attachment/' + fileID + "." + fileExt + "\" target=\"_blank\">" + fileName + '</a></span>';
        if (contorlID != "") {
            $("#" + contorlID).append($(r).append($(b)));
        }
        else {
            $("#" + fileID).append($(r).append($(b)));
        }
    }

}

function removeAttachment(attli)
{
    if (confirm("Are you sure want to delete this document?")) {
        var id = $(attli).attr("id");
        $(attli).parent(".MultiFile-label").hide();
        $.ajax({
            async: false,
            url: '../Request/RequestTestFileDelete',
            data: { fileID: id },
            dataType: "json",
            success: function () {                
                $(".warning").html("Success!"); 
            },
            error: function (a) { alert(a); }
        });
    }
}