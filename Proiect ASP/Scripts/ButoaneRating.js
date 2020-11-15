// aici e partea de adaugare, e ok, nu trebuie modificata

var ratingNouBtn = document.getElementById("ratingNouBtn");
var ratingNouDiv = document.getElementById("reviewNou");

if (ratingNouBtn != null) {

    var idProdus = ratingNouBtn.value;

    $(ratingNouBtn).on("click", function () {

        $(ratingNouBtn).remove();

        $(ratingNouDiv).load("/ProdusRating/AdaugaRating/" + idProdus);
    });
}

//de aici e partea de editare, are un bug si nu inteleg care

function editeaza(val) {

    var ratingEditareBtns = document.getElementsByClassName("ratingEditareBtn");
    var ratingEditareDivs = document.getElementsByClassName("reviewEditare");

    var ratingEditareDiv;

    for (let i = 0; i < ratingEditareBtns.length; i++) {
        console.log(i);
        if (ratingEditareBtns[i].value == val) {

            ratingEditareDiv = ratingEditareDivs[i];
        }

        $(ratingEditareBtns[i]).remove();
    }

    $(ratingEditareDiv).load("/ProdusRating/EditeazaRating/" + val);
}

// incerc sa fac sa mearga toate butoanele de editare, am inclus vechea versiune doar pt primul buton mai jos
/*
var ratingEditareBtns = document.getElementsByClassName("ratingEditareBtn");
var ratingEditareDivs = document.getElementsByClassName("reviewEditare");

if (ratingEditareBtns.length > 0) {

    for (let i = 0; i < ratingEditareBtns.length; i++) {

        $(ratingEditareBtns[i]).on("click", function () {

            $(ratingEditareBtns[i]).remove();

            $(ratingEditareDivs[i]).load("/ProdusRating/EditeazaRating/" + ratingEditareBtns[i].value);
        });
    }
}
*/
//versiunea veche care functiona doar pentru primul buton de editare, dar functiona bine
//ca sa functioneze, trebuie modificat si Afisare.cshtml si pus id in loc de class la butoanele si div urile respective
/*
var ratingEditareBtn = document.getElementById("ratingEditareBtn");
var ratingEditareDiv = document.getElementById("reviewEditare");

if (ratingEditareBtn != null) {

    var prodRating = ratingEditareBtn.value;

    $(ratingEditareBtn).on("click", function () {

        $(ratingEditareBtn).remove();

        $(ratingEditareDiv).load("/ProdusRating/EditeazaRating/" + prodRating);
    });
}
*/


