using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace WDAdmin.WebUI.Infrastructure
{
    /// <summary>
    /// Password generation and hashing
    /// </summary>
    public sealed class PassGenHash
    {
        /// <summary>
        /// The _pass length
        /// </summary>
        private readonly int _passLength = int.Parse(ConfigurationManager.AppSettings["RequiredPasswordLength"]);  //Length for auto-generated pass
        /// <summary>
        /// The _salt length
        /// </summary>
        private readonly int _saltLength = int.Parse(ConfigurationManager.AppSettings["RequiredSaltLength"]); //Length for salt attached to password
        /// <summary>
        /// The _instance
        /// </summary>
        private static PassGenHash _instance;

        /// <summary>
        /// Prevents a default instance of the <see cref="PassGenHash"/> class from being created.
        /// </summary>
        private PassGenHash() { }

        /// <summary>
        /// GetInstance method for Singleton use
        /// </summary>
        /// <value>The get instance.</value>
        public static PassGenHash GetInstance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }
                _instance = new PassGenHash();

                return _instance;
            }
        }

        /// <summary>
        /// Generation of random password for users
        /// </summary>
        /// <returns>New password</returns>
        public string CreateRandomPassword()
        {
            const string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!?_";
            var randNum = new Random();
            var chars = new char[_passLength];

            for (int i = 0; i < _passLength; i++)
            {
                chars[i] = allowedChars[(int)((allowedChars.Length) * randNum.NextDouble())];
            }

            var pass = new string(chars);

            //Check if password contains only allowed chars
            bool match = pass.IndexOfAny(allowedChars.ToCharArray()) != -1;

            //Check if password has the required length and contains only allowed characters, if not - regenerate
            if (pass.Length != _passLength || !match)
            {
                CreateRandomPassword();
            }

            return pass;
        }

        /// <summary>
        /// Generate salted password from input
        /// </summary>
        /// <param name="input">Password</param>
        /// <returns>Returns Tuple with hashed salted input and salt value</returns>
        public Tuple<string, string> CreateSaltedSHA512Hash(string input)
        {
            var rng = new RNGCryptoServiceProvider();
            var saltBytes = new byte[_saltLength];
            rng.GetNonZeroBytes(saltBytes);
            var saltValue = ToHex(saltBytes);
            var inputWithSalt = input + saltValue;
            return new Tuple<string, string>(CreateSHA512Hash(inputWithSalt), saltValue);
        }

        /// <summary>
        /// Generate hash from entered password and saved salt
        /// </summary>
        /// <param name="inputPass">Password</param>
        /// <param name="salt">Salt</param>
        /// <returns>Resturens hashed and salted password</returns>
        public string CheckSaltedPass(string inputPass, string salt)
        {
            return CreateSHA512Hash(inputPass + salt);
        }

        /// <summary>
        /// Generate SHA512 hash for input
        /// </summary>
        /// <param name="input">Input to be hashed</param>
        /// <returns>Returns hashed input string</returns>
        public string CreateSHA512Hash(string input)
        {
            // Convert password into a byte array.
            var inputBytes = Encoding.UTF8.GetBytes(input);

            // Initialize SHA512
            var key = SHA512.Create();

            //Compute hash for input
            var hashBytes = key.ComputeHash(inputBytes);

            // Convert the hashed byte array to hexadecimal string
            var hashValue = ToHex(hashBytes);

            return hashValue;
        }

        /// <summary>
        /// Changes byte array to HEX
        /// </summary>
        /// <param name="bytes">Byte array to be converted</param>
        /// <returns>Hexadecimal version of the given array</returns>
        private string ToHex(byte[] bytes)
        {
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString();
        }
    }
}