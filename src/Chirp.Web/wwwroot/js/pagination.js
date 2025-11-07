 
let params = new URL(document.location.toString()).searchParams;

function redirect_to_page(num, authorName) {
    let form = document.createElement("form");
    form.method = "POST";
    form.action = "/?handler=PageHandle";

    let page = document.createElement("input");
    page.name = "Page";
    page.value = num;
    form.appendChild(page);

    let author = document.createElement("input");
    author.name = "Author";

    if(authorName != null) {
        author.value = authorName;
    } else {
        author.value = " ";
    }
    form.appendChild(author);

    document.body.appendChild(form);
    form.submit();
}

function parse_page_number_or_default(def) {
    if(params.get("page") != null) {
        let tmp = parseInt(params.get("page"));
        if(tmp != NaN) return tmp;
    }
    return def;
}

function next_page() {
    let page = 2;
    if(params.get("page") != null) {
        let tmp = parseInt(params.get("page"));
        if(tmp != NaN) page = tmp + 1;
    }

    redirect_to_page(page, params.get("author"));
}

function prev_page() {
    let page = 1;
    if(params.get("page") != null) {
        let tmp = parseInt(params.get("page"));
        if(tmp != NaN) page = tmp - 1;
    }

    redirect_to_page(page, params.get("author"));
}

function goto_page(pageInput) {
    let page = 1;
    if(pageInput) {
        let tmp = parseInt(pageInput);
        if(tmp != NaN) page = tmp;
    }

    redirect_to_page(page, params.get("author"));
}

const node = document.getElementById("PageInputField");
node.addEventListener("keyup", function(event) {
    if(event.key.toLowerCase() == "enter") {
        goto_page(node.value);
    }
})

