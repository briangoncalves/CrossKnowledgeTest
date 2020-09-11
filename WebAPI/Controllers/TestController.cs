using CrossKnowledgeSolution.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        SimpleJsonRequest requestService;
        public TestController(IDistributedCache cacheService)
        {
            requestService = new SimpleJsonRequest(cacheService);
        }
        // GET: api/Test
        [HttpGet]
        public async Task<string> Get()
        {
            return await requestService.GetAsync("http://www.google.com");
        }

        // POST: api/Test
        [HttpPost]
        public async Task<string> Post([FromBody] string value)
        {
            var values = new Dictionary<string, string>();
            values.Add("id", "1");
            values.Add("value", value);
            return await requestService.PostAsync("http://www.google.com", values);
        }

        // PUT: api/Test/5
        [HttpPut("{id}")]
        public async Task<string> Put(int id, [FromBody] string value)
        {
            var values = new Dictionary<string, string>();  
            values.Add("id", id.ToString());
            values.Add("value", value);
            return await requestService.PutAsync("http://www.google.com", values);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<string> Delete(int id)
        {
            var values = new Dictionary<string, string>();
            values.Add("id", id.ToString());
            return await requestService.DeleteAsync("http://www.google.com", values);
        }
    }
}
