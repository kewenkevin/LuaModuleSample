using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace ND.UI.I18n
{
    public enum LocalizationKeyRule
    {
        Hash,
        CRC
    }
    public class LocalizationUtils
    {
        public static string GenerateLocalizationKey(string s, LocalizationKeyRule rule = LocalizationKeyRule.CRC)
        {
            switch (rule)
            {
                case LocalizationKeyRule.Hash:
                    return GenerateHashKey(s);
                case LocalizationKeyRule.CRC:
                    return GenerateCRCKey(s);
                default:
                    return GenerateHashKey(s);
            }
        }
        private static string GenerateCRCKey(string s)
        {
            return CRC32.GetCRC32(s).ToString();
        }
        private static string GenerateHashKey(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            
            // md5哈希
            byte[] tmpData = Encoding.UTF8.GetBytes(s);
            byte[] tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpData);

            StringBuilder res = new StringBuilder();
            foreach (byte b in tmpHash)
            {
                res.AppendFormat("{0:x2}",b);
            }

            // 最多后两位字符的Unicode编码
            string tmpS = string.Empty;
            if (s.Length == 1)
            {
                tmpS = s;
            }
            else if (s.Length >= 2)
            {
                tmpS = s.Substring(0, 2);
            }
            
            tmpData = Encoding.Unicode.GetBytes(tmpS);
            foreach (byte b in tmpData)
            {
                res.AppendFormat("{0:x2}",b);
            }

            return res.ToString();
        }
        
        public static bool CheckLocalizationKey(string s)
        {
            // 合法性检查
            /*if (string.IsNullOrEmpty(s))
            {
                return false;
            }*/
            Regex regex = new Regex("^[a-z0-9_-]*$");
            if (!regex.IsMatch(s))
            {
                return false;
            }
            return true;
        }
    }
}