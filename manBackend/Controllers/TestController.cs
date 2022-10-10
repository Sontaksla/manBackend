using manBackend.Models;
using manBackend.Models.Attributes;
using manBackend.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

namespace manBackend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TestController : ControllerBase
    {
        private readonly BackendDbContext backendDbContext;
        private readonly IConfiguration configuration;
        public TestController(BackendDbContext backendDbContext, IConfiguration configuration)
        {
            this.backendDbContext = backendDbContext;
            this.configuration = configuration;
        }
        [HttpGet]
        public unsafe void Test()
        {
        }
    }
}
