/*
 * commentNew.js
 *
 * Script responsável por submeter um novo comentário, 
 * Apresenta alerts com bootbox.js de erro ou sucesso 
 * Atualiza div dos comentários
 *
 */

// Variaveis para o id da postagem e url de consumo dos comentários
var postId, urlComments;

// Função que inicializa as variáveis postId e urlComments
function initCommentNew(_postId, _urlComments) {
    postId = _postId;
    urlComments = _urlComments;
}

// Quando elementos da página estiverem prontos para serem manipulados
$(document).ready(function () {

    // Referencia objetos da página
    var submit = $("#submit-comment");
    var inputAuthor = $("#input-author");
    var inputEmail = $("#input-email");
    var inputComment = $("#input-comment");
    var comments = $('#div-comments');

    // Função que habilita ou desabilita o formulário de comentário
    function disabledFormComment(disabled) {
        
        // Altera valor do botão
        if(disabled)
            submit.text("Comentando...");
        else
            submit.text("Publicar Comentário");
        
        // Desabilita itens do formulário 
        submit.prop( "disabled", disabled);
        inputAuthor.prop("disabled", disabled);
        inputEmail.prop("disabled", disabled);
        inputComment.prop("disabled", disabled);
    }

    // Função que rola tela até a div de comentários
    function goToComments() {
        // Rola a tela com animação
        $('html,body').animate({
            scrollTop: comments.offset().top - 67 // Abaixa mais 67 pixel para menu fixo não ocultar conteudo
        }, 300);
    }

    // Função que anima primeiro comentário (pisca)
    function animateFirstComment() {
        // Pega o primeiro comentário
        var element = comments.children("div.box-comment").first();
        // Salva a cor original do background
        var originalBG = element.css("background-color");
        // Pisca e retorna ao background original
        element.css("background-color", "#FFFFAD")
            .animate({ backgroundColor: originalBG }, 2500);
    }

    // Função que exibe alerta usando bootbox. Recebe titulo, mensagem e se operação teve sucesso ou não
    function showAlert(_title, _message, _success) {

        //Define a classe do botão, se indicará sucesso ou falha
        var classButton = (_success) ? "btn-success" : "btn-danger";
        
        // Exibe alerta
        bootbox.dialog({
            message: _message,
            title: _title,
            buttons: {
                success: {
                    label: "Ok",
                    className: classButton,
                    callback: function () {
                        if (_success) {
                            goToComments();  // Rola para os comentários
                            animateFirstComment(); // Anima (pisca) primeiro comentário
                        }
                    }

                }
            }
        });
    }

    // Indica o que fazer ao clicar no botão para enviar comentário
    submit.click(function () {

        // Desabilita form
        disabledFormComment(true);
        
        // Cria objecto com valores que serão submetidos
        var commentNew = { Author: inputAuthor.val(),
                           Email: inputEmail.val(),
                           Comment: inputComment.val(),
                           IsSuccess: "false",
                           Message: "",
                           PostId: postId };

        // Converte objeto para json
        var json = JSON.stringify(commentNew);

        // Inicializa variaveis a serem utilizadas dentro da requisição ajax
        var title, message, success;

        // Tenta fazer a requisição via ajax
        $.ajax({
            url: '/NewComment',     
            type: 'POST',
            dataType: 'json', 
            data: json,
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                // Recebe mensagem que será dada no alert e sinaliza o sucesso
                message = data.Message;
                success = data.IsSuccess;

                // Verifica se comentário foi postado ou não
                if (success)
                {
                    title = "Obrigado amigo terráquio!!!"; // Titulo do alert bootbox para sucesso
                    inputAuthor.val("");   //
                    inputEmail.val("");    // Limpa formulário
                    inputComment.val("");  //
                    comments.load(urlComments); // Recarrega comentários
                }
                else
                {
                    title = "Ops, ocorreu um erro!"; // Titulo do alert bootbox para falha 
                }
            },
            error: function(){
                success = false; // Sinializa erro
                title = "Ops, ocorreu um erro!"; // Titulo do alert bootbox para falha 
                message = "Não foi possível submeter o comentário pois algum erro voador não identificado aconteceu no meio do caminho."; // Mensagem de erro
            },
            complete: function () {
                showAlert(title, message, success); // Exibe alerta
                disabledFormComment(false); // Habilita formulário
            } 
        });
    });
});