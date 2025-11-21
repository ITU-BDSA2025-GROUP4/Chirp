 
let params = new URL(document.location.toString()).searchParams;

function redirect_to_page(num, authorName) {
    if(!authorName) {
        window.location.href =`/?page=${num}`
    } else {
        window.location.href =`/?page=${num}&author=${authorName}`
    }
}

function parse_page_number_or_default(def) {
    if(params.get("page") != null) {
        let tmp = parseInt(params.get("page"));
        if(!isNaN(tmp)) return tmp;
    }
    return def;
}

function next_page() {
    let page = 2;
    if(params.get("page") != null) {
        let tmp = parseInt(params.get("page"));
        if(!isNaN(tmp)) page = tmp + 1;
    }

    redirect_to_page(page, params.get("author"));
}

function prev_page() {
    let page = 1;
    if(params.get("page") != null) {
        let tmp = parseInt(params.get("page"));
        if(!isNaN(tmp)) page = tmp - 1;
    }

    redirect_to_page(page, params.get("author"));
}

function goto_page(pageInput) {
    let page = 1;
    if(pageInput) {
        let tmp = parseInt(pageInput);
        if(!isNaN(tmp)) page = tmp;
    }

    redirect_to_page(page, params.get("author"));
}

const node = document.getElementById("PageInputField");
node.addEventListener("keyup", function(event) {
    if(event.key.toLowerCase() == "enter") {
        goto_page(node.value);
    }
})

