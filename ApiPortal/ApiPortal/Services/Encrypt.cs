using System.Text;

namespace ApiPortal.Services
{
    public class Encrypt
    {
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string QueryString(IDictionary<string, string> dict)
        {
            var list = new List<string>();
            foreach (var item in dict)
            {
                list.Add(item.Key + "=" + item.Value);
            }
            return string.Join("&", list);
        }

        public static string EnsureBase64Length(string base64String)
        {
            int padding = 4 - (base64String.Length % 4);

            if (padding < 4)
            {
                for (int i = 0; i < padding; i++)
                {
                    base64String += "=";
                }
            }

            return base64String;
        }
    }
}
