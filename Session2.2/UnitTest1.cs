using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using Session2._2;
using System.Xml.Linq;

[assembly: Parallelize(Workers = 10, Scope = ExecutionScope.MethodLevel)]
namespace Session2._2
{
    [TestClass]
    public class Session2_2
    {
        private static RestClient restClient;

        private static readonly string BaseURL = "https://petstore.swagger.io/v2/";

        private static readonly string PetEndpoint = "pet";

        private static string GetURL(string enpoint) => $"{BaseURL}{enpoint}";

        private static Uri GetUri(string endpoint) => new Uri(GetURL(endpoint));

        private readonly List<UserModel> cleanUpList = new List<UserModel>();

        [TestInitialize]
        public async Task TestInitialize() 
        {
            restClient = new RestClient();        
        }

        [TestCleanup]
        public async Task TestCleanup()
        {
            foreach (var data in cleanUpList) 
            {
                var restRequest = new RestRequest(GetUri($"{PetEndpoint}/{data.Id}"));
                var restResponse = await restClient.DeleteAsync(restRequest);
            }
        }

        [TestMethod]
        public async Task PostReq()
        {
            UserModel inputData = new UserModel()
            {
                Id = 1,
                Category = new Category()
                {
                    Id = 1,
                    Name = "doggie"
                },
                Name = "doggie",
                PhotoUrls = new List<string>
               {
                   "Bork"
               },
                Tags = new List<Category>()
               {
                   new Category()
                   {
                   Id = 1,
                   Name = "bark"
                   }
               },
                Status = "available"
            };
            // Send Post Request
            var postRestRequest = new RestRequest(GetUri(PetEndpoint)).AddJsonBody(inputData);
            var RestResponse = await restClient.ExecutePostAsync(postRestRequest);

            // Status Code Assertion for Post Request
            Assert.AreEqual(HttpStatusCode.OK, RestResponse.StatusCode, "Status code is not equal to 200");

            // Send Get Request
            var getRestReq = new RestRequest(GetUri($"{PetEndpoint}/{inputData.Id}"));
            var getRestRes = await restClient.ExecuteGetAsync<UserModel>(getRestReq);

            // Assertions
            Assert.AreEqual(HttpStatusCode.OK, getRestRes.StatusCode, "Status code is not equal to 200.");
            Assert.AreEqual(inputData.Name, getRestRes.Data.Name, "Name does not match");
            Assert.AreEqual(inputData.Category.Name, getRestRes.Data.Category.Name, "Category does not match.");
            Assert.AreEqual(inputData.PhotoUrls[0], getRestRes.Data.PhotoUrls[0], "PhotoURl not found");
            Assert.AreEqual(inputData.Tags[0].Name, getRestRes.Data.Tags[0].Name, "Tags not found");
            Assert.AreEqual(inputData.Status, getRestRes.Data.Status, "Status code is not equal to 200");

            // Cleanup
            cleanUpList.Add(inputData);
        }
    }
}