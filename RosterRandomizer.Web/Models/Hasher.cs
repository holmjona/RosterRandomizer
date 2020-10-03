using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Web.Helpers;

namespace RosterRandomizer.Web.Models {
    public static class Hasher {
        //static String pepper = "BsbKf8prL7V2OtM8NcjcMg==";
        //https://www.codeproject.com/Articles/844722/Hashing-Passwords-using-ASP-NETs-Crypto-Class
        public static string HashPass(string pass, string salt) {
            string hash = Crypto.SHA256(pass + salt);
            //bool verify;
            // if(Crypto.VerifyHashedPassword(hash,pass)) {
            //    return hash;
            //}

            // this vvv was for .Net Core. Does nto work in .Net Framework
            //https://monkelite.com/how-to-hash-password-in-asp-net-core/
            //byte[] hash;//= KeyDerivation.Pbkdf2(pass, Encoding.UTF8.GetBytes(salt),// + pepper),
            //     //KeyDerivationPrf.HMACSHA256, 1000, 256 / 8);
            //return Convert.ToBase64String(hash);
            return hash;
        }



        public static string GenerateSalt() {
            byte[] rndBytes = new byte[128 / 8];
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetBytes(rndBytes);
            return Convert.ToBase64String(rndBytes);
        }

        public static string getRandomKey() {
            string newKey;
            do {
                newKey = GenerateSalt().Substring(0, 8);
            } while (Contains(newKey, @"+/:\%10oOiILl"));
            //return String.Format("{0}-{1}",
            //    newKey.Substring(0,4).ToUpper(), 
            //    newKey.Substring(4).ToUpper()) ;
            return newKey.ToUpper();
        }

        private static bool Contains(string hayStack, string needles) {
            bool foundIt = false;
            foreach (char ch in needles) {
                if (hayStack.Contains(ch)) {
                    foundIt = true;
                }
            }
            return foundIt;
        }

    }
}