using Dropbox_Lister.Services.AuthServices;
using FakeItEasy;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net.Http;
using Xunit;

namespace Dropbox_Lister.Test
{
    public class AuthServiceTests
    {
        private HttpClient client = A.Fake<HttpClient>();
        private IConfigurationRoot config = A.Fake<IConfigurationRoot>();


        [Fact]
        public async void GetCode_Should_Throw_Exception_On_Empty_String()
        {
            var authService = new AuthService(client, config);
            await Assert.ThrowsAsync<ArgumentNullException>(() => authService.GetCode(""));
        }

        [Fact]
        public async void Config_Parameters_Should_Not_Be_Empty()
        {

            var authService = new AuthService(client, config);
            A.CallTo(() => config["openBrowserLink"]).Returns("");

            await Assert.ThrowsAsync<ArgumentNullException>(() => authService.GetCode("someAppKey"));
        }

        [Fact]
        public async void Config_Parameters_Should_Start_With_Link()
        {

            var authService = new AuthService(client, config);
            A.CallTo(() => config["openBrowserLink"]).Returns("http://someothersite.com");

            await Assert.ThrowsAsync<ArgumentNullException>(() => authService.GetCode("someAppKey"));
        }

        [Fact]
        public void EnterAppCode_Should_Throw_On_Empty_String()
        {

            AuthService authService = new AuthService(client, config);
            var code = "";

            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            var stringReader = new StringReader(code);
            Console.SetIn(stringReader);

            
            Assert.Throws<ArgumentNullException>(() => authService.EnterAppCode());
        }
    }
}
