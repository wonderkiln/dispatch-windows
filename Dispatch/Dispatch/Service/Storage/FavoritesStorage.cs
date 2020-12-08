using Dispatch.Helpers;
using Dispatch.Service.Model;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Dispatch.Service.Storage
{
    public class FavoritesStorage : Observable
    {
        public ObservableCollection<FavoriteItem> Items { get; } = new ObservableCollection<FavoriteItem>();

        private static string CreateMD5(string input)
        {
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.ASCII.GetBytes(input);
                var hashBytes = md5.ComputeHash(inputBytes);

                var sb = new StringBuilder();

                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }

                return sb.ToString();
            }
        }

        private string filePath;

        public FavoritesStorage(string name)
        {
            try
            {
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "Favorites", CreateMD5(name));

                var text = File.ReadAllText(filePath);
                var items = JsonConvert.DeserializeObject<FavoriteItem[]>(text);

                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void Add(FavoriteItem item)
        {
            Items.Add(item);
            Save();
        }

        public void Remove(FavoriteItem item)
        {
            Items.Remove(item);
            Save();
        }

        private void Save()
        {
            try
            {
                // Create directory in case it doesn't exist
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                var json = JsonConvert.SerializeObject(Items.ToArray());
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
