using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace WebApplication22.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StackOverflowController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public StackOverflowController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IEnumerable<StackOverflowModels>> Get()
        {
            NameValueCollection parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["order"] = "desc";
            parameters["sort"] = "reputation";
            parameters["filter"] = "default";
            parameters["site"] = "stackoverflow";
            UriBuilder ub = new UriBuilder()
            {
                Path = "users",
                Query = HttpUtility.UrlDecode(parameters.ToString())
            };
            HttpClient client = _httpClientFactory.CreateClient("StackOverflowClient");
            using (HttpResponseMessage response = await client.GetAsync(ub.Uri.PathAndQuery))
            {
                response.EnsureSuccessStatusCode();
                string res = await response.Content.ReadAsStringAsync();
                Rootobject rootobject = JsonConvert.DeserializeObject<Rootobject>(res);
                List<StackOverflowModels> models = new List<StackOverflowModels>();
                foreach (Item i in rootobject.items)
                {
                    models.Add(new StackOverflowModels()
                    {
                        UserID = i.user_id,
                        Name = i.display_name,
                        Location = i.location
                    });
                }
                return models;
            }
        }
    }
}
