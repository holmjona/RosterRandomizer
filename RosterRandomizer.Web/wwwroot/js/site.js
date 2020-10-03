// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var currentCard = null;

$("#student-roster").mouseup(function (e) {
    e = e || window.event;
    var tar = $(e.target);
    var card = getParentCard(tar);
    if (tar.is("button")) {
        // reset above all else
        //card.removeClass("absent").removeClass("picked");
        changeCard(card, true,true);
    } else {
        //toggleCard(card);
        updateStudent(card);
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

function updateStudent(crd) {
    if (crd.hasClass("absent")) {
        // do nothing
    } else {
        var isAbsent = crd.hasClass("absent");
        changeCard(crd,false,!isAbsent);
    }
}

function changeCard(crd, reset, present) {
    if (typeof reset === "undefined") reset = false;

    if (typeof present === "undefined") {
        present = !crd.hasClass("absent");
    }

    var idParts = crd[0].id.split("-");
    var studId = idParts[1];
    var chk = crd.find("input:checkbox");

    $.ajax({
        url: "UpdateStudent",
        method: "post",
        data: {
            code: myCode,
            studentid: studId,
            isSelected: !chk.is(":checked"),
            inClass: present,
            reset: reset
        },
        success: function (newCrd) {
            //alert(crd + "\n\n " +newCrd);
            crd.replaceWith(newCrd);
        },
        error: function (err,msg) {
            alert(msg);
        }

    });
}

function showModal(showMe) {
    if (showMe) {
        if (currentCard !== null) {
            var crd = $(currentCard);
            var idParts = crd[0].id.split("-");
            var action = idParts[0];
            var studId = idParts[1];
            if (action === "reset") {
                // list has been reset.

            }
        }
        $("#cardHolder").empty().append(currentCard);
        $("#shadowBox").show();
    } else {
        $("#shadowBox").hide();
    }
}

function replaceCard() {
    if (currentCard !== null) {
        var crd = $(currentCard);
        var idParts = crd[0].id.split("-");
        var action = idParts[0];
        var studId = idParts[1];
        if (action === "reset") {
            //reset whole list
        } else {
            // update single card
            var cardId = "student-" + studId;
            var oldCard = $("#" + cardId);
            //oldCard.replaceWith(crd);
            //crd.attr("id", cardId);
            updateStudent(oldCard);
            currentCard = null;
        }
    } else {
        alert("Oops no card to replace.");
    }
}

function getRandomStudent() {
    $.ajax({
        url: "GetRandom",
        method: "post",
        data: {
            code: myCode
        },
        success: function (newCrd) {
            //alert(crd + "\n\n " +newCrd);
            currentCard = newCrd;
            showModal(true);
        },
        error: function (err, msg) {
            alert(msg);
        }

    });

}

// Action buttons.
$("#btnPickRandomStudent").mouseup(getRandomStudent);
$("#btnExport").mouseup(function () {

});
$("#btnCopyCode").mouseup(function () {

});


// Modal buttons
$("#modal a.closer").mouseup(function () {
    showModal(false);
});

$("#btnOK").mouseup(function () {
    replaceCard(currentCard);
    showModal(false);
});

$("#btnOKAnother").mouseup(function () {
    replaceCard(currentCard);
    getRandomStudent();
});

$("#btnAbsentAnother").mouseup(function () {

});
