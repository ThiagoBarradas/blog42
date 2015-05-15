/*
 * previewPost.js
 * Script responsável por executar a pré visualização de uma postagem
 *
 */

// Função que excuta o preview
function previewPost(_postId, _idTitle, _idContent, _idChangeAuthor) {

    // Recebe valores
    var title = $("#"+_idTitle).val();
    var content = CKEDITOR.instances[_idContent].getData();
    var changeAuthor = false;

    // Verifica se recebeu id de checkbox que verifica se admin quer alterar autor
    if (_idChangeAuthor != null && typeof (_idChangeAuthor) != "undefined" && _idChangeAuthor != "" && $("#"+_idChangeAuthor).length)
    {
        changeAuthor = $("#" + _idChangeAuthor).prop("checked");
    }

    // Monta pseudo form para post
    var formString = '<form id="formPreview" target="preview" method="post" name="preview" action="/Admin/Post/Preview">'+
                        '<input type="hidden" name="postId" />'+
                        '<input type="hidden" name="title" />'+
                        '<input type="hidden" name="content" />'+
                        '<input type="hidden" name="changeAuthor" />'+
                    '</form>';
    var form = $(formString);

    // Popula form
    form.children("input[name=postId]").val(_postId);
    form.children("input[name=title]").val(title);
    form.children("input[name=content]").val(content);
    form.children("input[name=changeAuthor]").val(changeAuthor);

    win = window.open(null, "preview"); // Cria nova janela
    win.focus(); // Atribui foco
    form.submit(); // Submete formulário e enfim, abre preview
}