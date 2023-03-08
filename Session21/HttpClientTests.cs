
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Session21
{
    [TestClass]
    public class HttpClientTest
    {
        //assigning httpClient variable to access HttpClient methods
            private static HttpClient httpClient;

            //declare base url (found in swagger for this training)
            private static readonly string BaseURL = "https://petstore.swagger.io/v2/";

            //declaring the endpoint to be used
            private static readonly string UsersEndpoint = "pet";

            //declaring the url
            private static string GetURL(string enpoint) => $"{BaseURL}{enpoint}";

            //declaring the URI
            private static Uri GetURI(string endpoint) => new Uri(GetURL(endpoint));

            private readonly List<UserModel> cleanUpList = new List<UserModel>();

            /*we start with test initialize that way this will always be executed first without
            having to be called repeatedly on every method*/

            [TestInitialize]
            public void TestInitialize()
            {
                /*we instantiated the HttpClient here to be able to use the methods
                within HttpClient*/
                httpClient = new HttpClient();
            }

            /*Test cleanup method or can be delete request is for deleting data created 
             * for testing to avoid overloading database of testdata during testing*/
            [TestCleanup]
            public async Task TestCleanUp()
            {
                foreach (var data in cleanUpList)
                {
                    var httpResponse = await httpClient.DeleteAsync(GetURL($"{UsersEndpoint}/{data.Id}"));
                }
            }

            [TestMethod]
            public async Task PutMethod()
            {


            #region create data

            // Create Json Object
            UserModel userData = new UserModel()
            {
                Id = 1032,
                Name = "Milo",
                Status = "Available",
                Tags = new Category[] {new Category { Id=1031, Name="Milo Tag"} },
                PhotoUrls = new string[] { "http://www.Milo.com" },
                Category = new Category()
                {
                    Id = 1031,
                    Name = "Cat"
                },

                };
                //Serialize the  content 
                var request = JsonConvert.SerializeObject(userData);
                var postRequest = new StringContent(request, Encoding.UTF8, "application/json");
                
                //Send post request
                await httpClient.PostAsync(GetURL(UsersEndpoint), postRequest);

            #endregion

            #region get id of the created data

            // Get Request
            var getResponse = await httpClient.GetAsync(GetURI($"{UsersEndpoint}/{userData.Id}"));

            // Deserialize Content
            var listUserData = JsonConvert.DeserializeObject<UserModel>(getResponse.Content.ReadAsStringAsync().Result);

            // filter created data
            var createdUserData = listUserData.Id;
            var createdName = listUserData.Name;
            var createdStatus = listUserData.Status;
            var createdPhotourl = listUserData.PhotoUrls;
            var createdCategory = listUserData.Category;
            #endregion

            //Send put request
            #region send put request to update the data

            userData = new UserModel()
            {
                Id = listUserData.Id,
                Name ="Mochi",
                Status = "Unavailable",
                Tags = new Category[] { new Category { Id = 1032, Name = "Mochi Tag" } },
                PhotoUrls = new string[] { "http://www.Mochi.com" },
                Category = new Category()
                {
                    Id = 1032,
                    Name = "Dog"
                },
            };

             //serialization
             request = JsonConvert.SerializeObject(userData);
             postRequest = new StringContent(request, Encoding.UTF8, "application/json");

            // Send Put Request
            var httpResponse = await httpClient.PutAsync(GetURL($"{UsersEndpoint}"), postRequest);

            // Get Status Code
            var statusCode = httpResponse.StatusCode;

            #endregion


            #region get updated data

            // Get Request
            getResponse = await httpClient.GetAsync(GetURI($"{UsersEndpoint}/{userData.Id}"));

            // Deserialize Content
            listUserData = JsonConvert.DeserializeObject<UserModel>(getResponse.Content.ReadAsStringAsync().Result);

            // filter created data
            createdUserData = listUserData.Id;
            var UpdatedUserData = listUserData.Name;
            var Updatedphotourls = listUserData.PhotoUrls;
            var upadtedCategory = listUserData.Category;
            var updatedStatus = listUserData.Status;

            #endregion

              #region cleanupdata

              // Add data to cleanup list
              cleanUpList.Add(listUserData);

              #endregion

            #region assertion

            // Assertion
            Assert.AreEqual(HttpStatusCode.OK, statusCode, "Status code is not equal to 200");
            Assert.AreEqual(userData.Id, createdUserData, "id are not matching");
            Assert.AreEqual(userData.Name, UpdatedUserData, "Name are not matching");
            Assert.AreNotEqual(createdStatus, updatedStatus, "Status are not matching");
           
            #endregion


        }

            
    }
}
