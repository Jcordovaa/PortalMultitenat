using System.Security.Cryptography;
using System.Text;

namespace ApiPortal.Services
{
    public class HashPassword
    {
        public string HashCode(string str)
        {
            try
            {
                string rethash = string.Empty;
                try
                {
                    SHA1 hash = SHA1.Create();
                    ASCIIEncoding encoder = new ASCIIEncoding();
                    byte[] combined = encoder.GetBytes(str);
                    hash.ComputeHash(combined);
                    rethash = Convert.ToBase64String(hash.Hash);
                }
                catch (Exception ex)
                {
                    string strerr = "Error in HashCode : " + ex.Message;
                }
                return rethash;
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
