// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var currentCard = null;

$("#roster-container").mouseup(function (e) {
    e = e || window.event;
    var tar = $(e.target);
    var card = getParentCard(tar);
    if (tar.is("button")) {
        // reset above all else
        //card.removeClass("absent").removeClass("picked");
        changeCard(card, true, true);
    } else {
        //toggleCard(card);
        handleCardClick(card);
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

function handleCardClick(crd, attending = true) {
    var isAbsent = crd.hasClass("absent");
    if (isAbsent) {
        // do nothing
        // ignore clicks on absent students.
    } else {
        changeCard(crd, false, !isAbsent && attending);
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
        url: "StudentRoster/UpdateStudent",
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
        error: function (err, msg) {
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
                showMessage("You have used all of the students in the list. The list was reset.");
                fillStudentList();
            }
        }
        $("#cardHolder").empty().append(currentCard);
        $("#shadowBox").show();
    } else {
        $("#shadowBox").hide();
    }
}

function replaceCardWithCurrent(attending = true) {
    if (currentCard !== null) {
        var crd = $(currentCard);
        var idParts = crd[0].id.split("-");
        var action = idParts[0];
        var studId = idParts[1];
        // update single card
        var cardId = "student-" + studId;
        var oldCard = $("#" + cardId);
        //oldCard.replaceWith(crd);
        //crd.attr("id", cardId);
        // keep consistent with click behavior
        handleCardClick(oldCard, attending);
        currentCard = null;
    } else {
        alert("Oops no card to replace.");
    }
}

function getRandomStudent() {
    $.ajax({
        url: "StudentRoster/GetRandom",
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
    // https://www.w3schools.com/howto/howto_js_copy_clipboard.asp
    var tBox = document.getElementById("txtClassCode");
    // not selectable if disabled.
    tBox.removeAttribute("disabled");
    tBox.style.display = "inline";
    tBox.select();
    //tBox.setSelectionRange(0, 99999); // for mobile devices
    var success = document.execCommand("copy");
    if (success) {
        showMessage("Your class code has been copied to your clipboard.");
    } else {
        showMessage("Not working.");
    }
    // return to disable to stop users from modifying
    tBox.setAttribute("disabled", "disabled");
    tBox.style.display = "none";

});



// Modal buttons
$("#modal a.closer").mouseup(function () {
    showModal(false);
});

$("#btnOK").mouseup(function () {
    replaceCardWithCurrent();
    showModal(false);
});

$("#btnOKAnother").mouseup(function () {
    replaceCardWithCurrent();
    //HACK: delay get random to eliminate file locks on the server.
    delay(getRandomStudent, 1000);
});

$("#btnAbsentAnother").mouseup(function () {
    showMessage("Student will be unavailable for random selection. You can reset the student to return them to the list.");
    replaceCardWithCurrent(false); // not attending.
    //HACK: delay get random to eliminate file locks on the server.
    delay(getRandomStudent, 1000);
});


function delay(action, time) {
    $("#loader").show();
    setTimeout(action, time);
    setTimeout(delayStop, time * 1.1);
}

function delayStop() {
    $("#loader").hide();
}

function fillStudentList() {
    $.ajax({
        url: "StudentRoster/GetList",
        method: "post",
        data: {
            code: myCode
        },
        success: function (newList) {
            $("#roster-container").empty().append(newList);
        },
        error: function (err, msg) {
            alert(msg);
        }

    });
}

function showMessage(txt) {
    var mess = $("#page-message");
    mess.text(txt);
    mess.slideDown();
    setTimeout(function () {
        mess.slideUp(1000);
    }, 5000);
}