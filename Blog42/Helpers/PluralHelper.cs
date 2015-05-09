using System;
using System.Text;

namespace Blog42.Helpers
{
    /*
     * Classe utilitária para plurais em palavras
     */
    public class PluralHelper
    {
        /*
         * Método que recebe uma string e pluraliza ou não de acordo com quantidade de itens
         * Recebe string a ser pluralizada e quantidade de itens e string pluralizada, 
         * String pluralizada é iniciada null, que indica que palavra pluralizada é apenas adicionada um 's'
         */
        public static string SimplePluralize(string word, int total, string wordPluralized = null)
        {
            // Verifica se recebeu palavra a se informado em caso de plural, se não recebeu apenas concatena com 's'
            if (wordPluralized == null)
                wordPluralized = word + "s";

            // Se tiver apenas 1 item, retorna palavra singular
            if (total == 1)
                return word;
            else
                return wordPluralized;
        }
    }
}