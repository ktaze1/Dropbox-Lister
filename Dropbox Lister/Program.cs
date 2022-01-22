using Dropbox_Lister.Services.AuthServices;
using Dropbox_Lister.Services.ListServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Dropbox_Lister
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);

            IConfiguration config = builder.Build();
            string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(config["appkey"] + ":" + config["appsecret"]));
            var serviceProvider = new ServiceCollection()
                .AddScoped(sp =>
                {
                    var client = new HttpClient()
                    {
                        BaseAddress = new Uri("https://api.dropboxapi.com/")
                    };
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", svcCredentials);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    return client;
                })
                .AddSingleton(_ => builder.Build())
                .AddScoped<IAuthService, AuthService>()
                .AddSingleton<IListService, ListService>()
                .BuildServiceProvider();


            var authorization = serviceProvider.GetService<IAuthService>();
            authorization.GetCode(config["appkey"]);

            var fetchList = serviceProvider.GetService<IListService>();
            fetchList.ListContent();

        }
    }

}