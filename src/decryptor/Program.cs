using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Decryptor
{
    public class Program
    {
        static void Main(string[] args)
        {
            var TargettedDirectories =
                new string[4] { "Documents", "Pictures", "Videos", "Desktop" };
            string home =
                Environment
                    .GetFolderPath(Environment.SpecialFolder.UserProfile);
            Console.Write("Enter The Key to Decrypt: ");
            string key = Console.ReadLine();

            if (key != null)
            {
                for (var i = 0; i < TargettedDirectories.Length; i++)
                {
                    string path = $"{home}/{TargettedDirectories[i]}";
                    DecryptDirectory (path, key);
                }
            }
        }

        public static void DecryptDirectory(
            string TargetDirectory,
            string DecryptionKey
        )
        {
            List<string> Files = getFilesRecursive(TargetDirectory);

            // string[] Files = Directory.GetFiles(TargetDirectory);
            for (int i = 0; i < Files.Count; i++)
            {
                var file = Files[i];

                var text = System.IO.File.ReadAllText(file);

                try
                {
                    if (file.EndsWith(".encrypted"))
                    {
                        var Decrypted =
                            AesOperation.DecryptString(DecryptionKey, text);

                        File.WriteAllText (file, Decrypted);
                        System
                            .IO
                            .File
                            .Move(file, file.Replace(".encrypted", ""));
                        Console.WriteLine($"Successfully Decrypted {file}");
                    }
                }
                catch
                {
                    Console.WriteLine($"Unable To Decrypt {file}");
                }
            }
        }

        public static List<string> getFilesRecursive(string sDir)
        {
            var files = new List<string>();
            try
            {
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    getFilesRecursive (d);
                }
                foreach (var file in Directory.GetFiles(sDir))
                {
                    files.Add (file);
                    // if (files.Length == 0)
                    // {
                    //     files[0] = file;
                    // }
                    // else
                    // {
                    //     files[files.Length + 1] = file;
                    // }
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            return files;
        }
    }

    public class AesOperation
    {
        public static string DecryptString(string key, string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor =
                    aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (
                        CryptoStream cryptoStream =
                            new CryptoStream((Stream) memoryStream,
                                decryptor,
                                CryptoStreamMode.Read)
                    )
                    {
                        using (
                            StreamReader streamReader =
                                new StreamReader((Stream) cryptoStream)
                        )
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
