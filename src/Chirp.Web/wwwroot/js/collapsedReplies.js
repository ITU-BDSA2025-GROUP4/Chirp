
var buttons = document.getElementsByClassName("collapseButton");

for (var button of buttons) {
    button.addEventListener("click", function() {
        let replies = this.previousElementSibling;

        if (replies.style.display === "block") {
            button.innerHTML = "Show remaining replies"
            replies.style.display = "none";
        } else {
            button.innerHTML = "Hide replies"
            replies.style.display = "block";
        }
    })
}

function sleep(time) {
  return new Promise((resolve) => setTimeout(resolve, time));
}

function toggle_reply_input(button) {
    let replyInputForm = button.nextElementSibling;
    button.style.display = "none";
    replyInputForm.style.display = "block";

    let replyInputField = replyInputForm.getElementsByClassName("ReplyInputField")[0];

    replyInputField.focus();

    console.log(document.activeElement===replyInputField);
}
