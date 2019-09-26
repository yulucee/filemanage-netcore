using System.Linq;
using System.Text;

namespace DosyaSistemiDAL.Common
{
    public class ExtensionMethod
    {
        public static string[] Parcala(string DosyaId)
        {
            char[] ayrac = { '*' };
            string metin = DosyaId;
            string[] parcalar = metin.Split(ayrac);
            parcalar = parcalar.Skip(1).ToArray();
            for (int i = 1; i < parcalar.Length; i++)
            {
                if (parcalar[i].Contains("undefined"))
                {
                    parcalar = parcalar.Where(w => w != parcalar[i]).ToArray();
                }
            }
            return parcalar;
        }
        public static string sha256(string value)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(value));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }
    }
}
