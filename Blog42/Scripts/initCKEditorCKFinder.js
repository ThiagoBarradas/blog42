/*
 * initCKEditorCKFinder.js
 *
 * Script inicializar os componentes para transformar um textarea num editor html (CKEditor) e permitir envio de imagem (CKFinder)
 *
 */

// Inicializa editor passando parametros de id do textarea
function init(id)
{
    
    // inicializa editor
    var editor = CKEDITOR.replace(id);
    // Inicializa Finder (upload de imagem) no editor     
    CKFinder.setupCKEditor(editor, '/Content/ckfinder/');
}

