using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace WindowsFormsApplication1
{
    class HashLayer
    {
        public static byte[] ByteDonustur(string deger)
        {
            UnicodeEncoding ByteConverter = new UnicodeEncoding();
            return ByteConverter.GetBytes(deger);
        }

        public static byte[] Byte8(string deger)
        {
            char[] arrayChar = deger.ToCharArray();
            byte[] arrayByte = new byte[arrayChar.Length];
            for (int i = 0; i < arrayByte.Length; i++)
            {
                arrayByte[i] = Convert.ToByte(arrayChar[i]);
            }
            return arrayByte;
        }

        public static string SHA512(string strGiris)
        {
            SHA512Managed sifre = new SHA512Managed();
            byte[] arySifre = ByteDonustur(strGiris);
            byte[] aryHash = sifre.ComputeHash(arySifre);
            return BitConverter.ToString(aryHash);
        }
    }
}
