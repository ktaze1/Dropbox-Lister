using Dropbox_Lister.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dropbox_Lister.Services.ListServices
{
    public class ListService : IListService
    {
        private readonly HttpClient _client;
        private readonly Regex _regex = new Regex("/(/(.|[\r\n])*)?");

        public ListService(HttpClient client)
        {
            _client = client;
        }

        public async void ListContent()
        {
            Print(await GetContentList("/"));
        }

        private async void Print(List<Content> entries)
        {
            if (entries.Count == 0)
            {
                Console.WriteLine("List is empty, enter new path");
                Print(await GetContentList(Console.ReadLine()));
            }
            else
            {
                foreach (var item in entries)
                {
                    Console.WriteLine("{0}: {1}, [path='{2}']", item.Tag, item.Name, item.Path);
                }
                Console.WriteLine("Type path of the subfolder to see its content:");
                Print(await SearchSubFolder(Console.ReadLine(), entries));
            }
        }

        public async Task<List<Content>> Continue(string cursor)
        {
            var result = new List<Content>();
            var cursorResponse = _client.PostAsJsonAsync("2/files/list_folder/continue", new { cursor = cursor }).Result;
            var cursorResult = JsonConvert.DeserializeObject<ListResult>(await cursorResponse.Content.ReadAsStringAsync());
            if (cursorResult != null)
                result.AddRange(cursorResult.Entries);
            if (cursorResult.HasMore)
                result.AddRange(await Continue(cursorResult.Cursor));
            return result;
        }

        public async Task<List<Content>> GetContentList(string path)
        {
            if (IsValidPath(path))
            {
                List<Content> contentList = new();

                if (path == "/")
                    path = string.Empty;
                var contentResponse = _client.PostAsJsonAsync("2/files/list_folder", new { path = path, include_mounted_folders = true }).Result;
                if (contentResponse.IsSuccessStatusCode)
                {
                    var contentResult = JsonConvert.DeserializeObject<ListResult>(await contentResponse.Content.ReadAsStringAsync());
                    if (contentResult != null)
                    {
                        contentList.AddRange(contentResult.Entries);
                        if (contentResult.HasMore)
                            contentList.AddRange(await Continue(contentResult.Cursor));

                        return contentList;
                    }
                }
                else
                {
                    Console.Error.WriteLine("An error has occurred");
                }
            }
            Console.WriteLine("Invalid path!, Please Try Again");
            return await GetContentList(Console.ReadLine());
        }

        public async Task<List<Content>> SearchSubFolder(string subPath, List<Content> entries)
        {
            if (IsRootPath(subPath))
            {
                return await GetContentList(subPath);
            }
            else if (IsFolderExist(subPath, entries))
            {
                return await GetContentList(subPath);
            }
            else
            {
                Console.WriteLine("No Such Folder Exist, Try Again");
                return await SearchSubFolder(Console.ReadLine(), entries);
            }
        }

        #region Utils
        public bool IsValidPath(string path)
        {
            return _regex.IsMatch(path) ? true : false;
        }


        public bool IsFolderExist(string path, List<Content> contentList)
        {
            return contentList.Any(c => c.Path == path && c.Tag == "folder");
        }

        public bool IsRootPath(string path)
        {
            return path == "/";
        }

        #endregion
    }

}
