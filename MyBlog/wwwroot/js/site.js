// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

let index = 0;

function AddTag() {

    var tagEntry = document.getElementById("TagEntry");

    //lets use the serach funtion

    let searchResult = search(tagEntry.value);
    if (searchResult != null) {

        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: `${searchResult}`,
            /*footer: '<a href="">Why do I have this issue?</a>'*/
        })


    }
    else {

        //create new option
        let newOption = new Option(tagEntry.value, tagEntry.value);


        document.getElementById("TagList").options[index++] = newOption;
    }

    

    tagEntry.value = "";

}


function DeleteTag() {

    let tagCount = 1;

    let tagList = document.getElementById("TagList");
    if (!tagList) return false;

    if (tagList.selectedIndex == -1) {

        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: `CHOOSE A TAG BEFORE DELETING`,
            /*footer: '<a href="">Why do I have this issue?</a>'*/
        })


    }
    while (tagCount > 0) {
        
        let selectedIndex = document.getElementById("TagList").selectedIndex;

        if (selectedIndex >= 0) {
            document.getElementById("TagList").options[selectedIndex] = null;
            --tagCount;
        }
        else {
            tagCount = 0;
        }
        index--;

       
    }

}

$("form").on("submit", function () {
    $("#TagList option ").prop("selected", "selected");
})


if (tagValues != "") {
    let tagArray = tagValues.split(",");

    for (var loop = 0; loop < tagArray.length; loop++) {

        ReplaceTag(tagArray[loop], loop);
        index++;
    }
}


function ReplaceTag(tag, index) {
    let newOption = new Option(tag, tag);
    document.getElementById("TagList").options[index] = newOption;
}



function search(str) {
    if (str == '') {
        return 'Empty tags are not permitted';
    }

    let tagsEl = document.getElementById("TagList");

    if (tagsEl) {
        let options = tagsEl.options;

        for (var i = 0; i < options.length; i++) {
            if (options[i].value == str) {
                return `The tag ${str} was not detected as a duplicate and not permitted`;
            }
        }
    }
}