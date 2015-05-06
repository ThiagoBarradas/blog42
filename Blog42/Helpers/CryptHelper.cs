using System;
using System.Security.Cryptography;
using System.Text;

namespace Blog42.Helpers
{
    /*
     * Classe utilitária para critografia/hash
     */
    public class CryptHelper
    {
        /*
         * Método que recebe uma string e retorna sua hash MD5. 
         * Utilizado para criptografar a senha do usuário.
         * 
         */
        public static String CryptPassword(String password)
        {

            // Inicializa o objeto responsavél por calcular a hash.
            MD5 md5 = new MD5CryptoServiceProvider();

            // captura os bytes da string (password) recebido 
            Byte[] originalBytes = ASCIIEncoding.Default.GetBytes(password);
            // faz o cálculo da hash md5 de acordo com os bytes originais            
            Byte[] encodedBytes = md5.ComputeHash(originalBytes);

            // transforma os bytes da hash em string. Converte para caixa baixa e remove os '-' adicionados na conversão.
            String hash = BitConverter.ToString(encodedBytes).ToLower().Replace("-", "");

            // retorna o password em md5
            return hash;
        }
    }
}