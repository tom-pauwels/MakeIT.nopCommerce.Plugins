using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MakeIT.Nop.Plugin.Shipping.Bpost.ShippingManager
{
    public static class CheckSumCalculator
    {
        public static string CalculateChecksum(Dictionary<string, string> pairs, string passPhrase)
        {
            var string2Hash = string.Empty;
            foreach (KeyValuePair<string, string> item in pairs.OrderBy(key => key.Key))
            {
                string2Hash += string.Format("{0}={1}&", item.Key, item.Value);
            }
            string2Hash += passPhrase;

            return CalculateSHA(string2Hash);
        }

        private static string CalculateSHA(string hashText)
        {
            var sha = new SHA256CryptoServiceProvider();
            var utf8Bytes = sha.ComputeHash(new UTF8Encoding().GetBytes(hashText));

            return BitConverter.ToString(utf8Bytes).Replace("-", string.Empty).ToLower();
        }
    }
}