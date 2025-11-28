
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

