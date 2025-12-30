document.addEventListener("click", (e) => {
    const btn = e.target.closest(".displayReplyInputButton");
    if (!btn) return;
    toggle_reply_input(btn);
});

function toggle_reply_input(button) {
   
    const otherReplyButtons = document.getElementsByClassName("displayReplyInputButton");
    for (const otherButton of otherReplyButtons) {
        reply_input_hide(otherButton); 
    }

    const replyInputForm = button.nextElementSibling;
    if (!replyInputForm) return; 

    button.style.display = "none";
    replyInputForm.style.display = "block";

    const replyInputField = replyInputForm.getElementsByClassName("ReplyInputField")[0];
    if (replyInputField) replyInputField.focus();
}
