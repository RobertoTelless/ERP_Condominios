// Não conta textarea com editor de texto
function countChar(elm, total) {
    // Valor Default - Caso necessário obter por Ajax
    if (total == undefined || total == null) {
        total = 10000;
    }

    var len = elm.value.length;
    if (len >= total) {
        elm.value = elm.value.substring(0, total);
    } else {
        $(elm).parent().find('div[name="charNum"]').text(1 + len + '/' + total + ' caracteres');
    }
}

// Conta os caracteres
$('textarea').attr('onkeyup', "countChar(this, $(this).attr('data-val-length-max'))");

// Cria o div que mostra o contador
$('textarea').parent().append('<div name="charNum"></div>');



// NÃO FUNCIONA
function countSummernote() {
    //Contador de Caracteres para summernote
    var limiteCaracteres = 10000;
    var caracteres = $(this).text();
    var totalCaracteres = caracteres.length;

    //Update Count value
    if (!$('#total-caracteres').length) {
        $(".note-editor").parent().append('<div id="#total-caracteres"></div>');
        $("#total-caracteres").text(totalCaracteres + '/' + limiteCaracteres);
    } else {
        $("#total-caracteres").text(totalCaracteres + '/' + limiteCaracteres);
    }

    //Check and Limit Charaters
    if (totalCaracteres >= limiteCaracteres) {
        return false;
    }
}