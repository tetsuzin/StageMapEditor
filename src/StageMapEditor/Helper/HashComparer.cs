using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace StageMapEditor.Helper
{
    public class HashComparer
    {
        public SHA1Managed SHA1Managed;

        public HashComparer()
        {
            SHA1Managed = new SHA1Managed();
        }

        public bool IsUpdated(FileInfo original, FileInfo target)
        {
            var originalHash = GetHash(original);
            var targetHash = GetHash(target);

            return
                ByteArraytoString(originalHash) !=
                ByteArraytoString(targetHash);
        }


        public bool IsUpdated(FileInfo original, string target)
        {
            var originalHash = GetHash(original);
            var targetHash = GetHash(target);

            return
                ByteArraytoString(originalHash) !=
                ByteArraytoString(targetHash);
        }

        public bool IsUpdated(string original, string target)
        {
            var originalHash = GetHash(original);
            var targetHash = GetHash(target);

            return
                ByteArraytoString(originalHash) !=
                ByteArraytoString(targetHash);
        }

        public byte[] GetHash(FileInfo input)
        {
            using (var st = new FileStream(input.FullName, FileMode.Open))
            {
                return SHA1Managed.ComputeHash(st);
            }
        }

        public byte[] GetHash(string input)
        {
            return SHA1Managed.ComputeHash(Encoding.UTF8.GetBytes(input));
        }

        public string ByteArraytoString(byte[] bytes)
        {
            var sb = new StringBuilder(bytes.Length);

            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("X2"));
            }

            return sb.ToString();
        }

    }
}
