using Dropbox_Lister.Services.ListServices;
using System.Net.Http;
using Xunit;
using FakeItEasy;
using System.Collections.Generic;
using Dropbox_Lister.DTOs;

namespace Dropbox_Lister.Test
{
    public class ListServiceTests
    {
        private HttpClient client = A.Fake<HttpClient>();

        [Fact]
        public void Path_Should_Validate()
        {

            ListService service = new ListService(client);
            var validPath = service.IsValidPath("/");
            var validPathLong = service.IsValidPath("/some/valid/path");
            var emptyPath = service.IsValidPath("");
            var falsePath = service.IsValidPath("\\");
            var falsePathLong = service.IsValidPath("path\\wrong");


            Assert.True(validPath);

            Assert.True(validPathLong);

            Assert.False(emptyPath);

            Assert.False(falsePath);

            Assert.False(falsePathLong);
        }


        [Fact]
        public void Should_Find_Element_With_Given_Path()
        {

            ListService service = new ListService(client);

            List<Content> contents = new List<Content>()
            {
                new Content() { Path ="/Test", Tag="folder"},
                new Content() { Path ="/", Tag="folder"}
            };

            Assert.True(service.IsFolderExist("/", contents));
            Assert.True(service.IsFolderExist("/Test", contents));
            Assert.False(service.IsFolderExist("/Test/", contents));
            Assert.False(service.IsFolderExist("\\", contents));
        }

        [Fact]
        public void Should_Validate_Root_Path()
        {
            var service = new ListService(client);

            Assert.True(service.IsRootPath("/"));
            Assert.False(service.IsRootPath(""));
        }

        [Fact]
        public void Should_Validate_Folder_Exist()
        {


            ListService service = new ListService(client);

            List<Content> contents = new List<Content>()
            {
                new Content() { Path ="/Test", Tag="folder"},
                new Content() { Path ="/Test", Tag="file"},
            };

            List<Content> emptyContent = new();

            List<Content> contentsWithFaultyData = new List<Content>()
            {
                new Content() { Path ="/Test", Tag="file"},
                new Content() { Path ="/Test", Tag="Folder"},
            };


            Assert.True(service.IsFolderExist("/Test", contents));
            Assert.False(service.IsFolderExist("/Test", contentsWithFaultyData));
            Assert.False(service.IsFolderExist("/Test", emptyContent));
            Assert.False(service.IsFolderExist("", emptyContent));
        }


        [Fact]
        public void Shoud_Print()
        {
           ListService listService = new ListService(client);

           var list = (List<Content>)A.CollectionOfDummy<Content>(5);


        }

        [Fact]
        public async void Should_Get_Content()
        {

            //ListService listService = new ListService(client);

            //List<Content> contents = new List<Content>()
            //{
            //    new Content() { Path ="/Test", Tag="folder"},
            //    new Content() { Path ="/Test", Tag="file"},
            //};
            ////var obj = new { path = "somePath", include_mounted_folders = true };
            ////A.CallTo(() => client.PostAsJsonAsync("", obj, default))
            ////    .Returns(Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK) 
            ////    {
            ////        Content = new StringContent(JsonConvert.SerializeObject(contents).ToString())
            ////    }));

            //Assert.Equal(contents, await listService.GetContentList("somePath"));
        }
    }
}
