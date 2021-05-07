// Cria Container para o modal em todas as páginas
$('.wrapper').prepend('<div id="modalContainer"></div>');

// Cria o modal e appenda no container
function CriaModal(nome, img) {
    var modal = '<div class="modal fade" id="fotoModal" tabindex="-1" role="dialog" aria-labelledby="fotoModal" aria-hidden="true">'
        + '<div class="modal-dialog modal-dialog-centered" role="document">'
        + '<div class="modal-content">'
        + '<div class="modal-header">'
        + '<h5 class="modal-title">' + nome + '</h5>'
        + '<span onclick="DelModal()" style="float: right; top: 10px; position: absolute; right: 10px;" class="close" data-dismiss="modal" aria-label="Close">'
        + '<span aria-hidden="true">&times;</span>'
        + '</span>'
        + '</div>'
        + '<div class="modal-body">'
        + '<img src="' + img + '" style="margin-left: auto; margin-right:auto; width: auto; height: auto; min-width: 500px; max-width: 500px; max-height: auto;">'
        + '</div>'
        + '</div>'
        + '</div>'
        + '</div>';

    $('#modalContainer').html(modal);
}

// Deleta o modal para evitar erros com modais duplicados
function DelModal() {
    $('#fotoModal').on('hidden.bs.modal', function () {
        $('#fotoModal').remove();
    });
}