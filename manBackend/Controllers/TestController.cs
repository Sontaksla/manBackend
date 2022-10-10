using manBackend.Models;
using manBackend.Models.Attributes;
using manBackend.Models.Auth;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace manBackend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TestController : ControllerBase
    {
        private readonly BackendDbContext backendDbContext;
        public TestController(BackendDbContext backendDbContext)
        {
            this.backendDbContext = backendDbContext;
        }
        [HttpGet]
        public unsafe void test() 
        {
            for (int i = 0; i < 3; i++)
            {
                fixed (char* ptr = "myPass" + i.ToString())
                {
                    User user = new User("slawa", "sontaksla", "neget@gmail.com", ptr, 7);

                    backendDbContext.Users.Add(user);
                }
            }
            backendDbContext.SaveChanges();
        }
    }
}
