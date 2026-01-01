
var buttons = document.getElementsByClassName("collapseButton");

const hiddenRepliesText = "Show remaining replies";
const shownRepliesText = "Hide replies";

for (var button of buttons) {
    button.addEventListener("click", function() {
        let replies = this.previousElementSibling;

        if (replies.style.display === "block") {
            button.innerHTML = hiddenRepliesText;
            replies.style.display = "none";
        } else {
            button.innerHTML = shownRepliesText;
            replies.style.display = "block";
        }
    })
}

function sleep(time) {
  return new Promise((resolve) => setTimeout(resolve, time));
}
window.reply_input_hide = function(button) {
    let replyInputForm = button.nextElementSibling;
    button.style.display = "block";
    replyInputForm.style.display = "none";
};

window.toggle_reply_input = function(button) {
    let otherReplyButtons = document.getElementsByClassName("displayReplyInputButton");
    for (const otherButton of otherReplyButtons) {
        window.reply_input_hide(otherButton);
    }

    let replyInputForm = button.nextElementSibling;
    button.style.display = "none";
    replyInputForm.style.display = "block";

    let replyInputField = replyInputForm.getElementsByClassName("ReplyInputField")[0];
    replyInputField.focus();
};
