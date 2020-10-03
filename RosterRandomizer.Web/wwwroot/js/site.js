// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$("#student-roster").click(function (e) {
    e = e || window.event;
    var tar = $(e.target);
    var card =  getParentCard(tar);
    if (tar.is("button")) {
        // reset above all else
        card.removeClass("absent").removeClass("picked");
    } else {
        toggleCard(card);
    }
});

function getParentCard(chd) {
    var par = chd.parents(".student-card");
    if (par.length < 1) {
        par = chd;
    }
    return par;
}

function toggleCard(crd) {
    if (crd.hasClass("absent")) {
        // do nothing
    } else {
        var chk = crd.find("input:checkbox");
        if (chk.is(":checked")) {
            chk.attr("checked", null);
            crd.removeClass("picked");
        } else {
            chk.attr("checked", "checked");
            crd.addClass("picked");
        }
    }
}