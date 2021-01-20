
var incremento = document.getElementById("incremento");
incremento.addEventListener("click", function () {
        var actual = document.getElementById("cant").value;
        actual++;
        document.getElementById("cant").value = "" + actual;
}, {passive: true})

var decremento = document.getElementById("decremento");
decremento.addEventListener("click", function () {
    var actual = document.getElementById("cant").value;

    if (actual > 1) {
        actual--;
        document.getElementById("cant").value = "" + actual;
    }
}, { passive: true })
