using manBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace manBackend.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ExerciseController : ControllerBase
    {
        private readonly BackendDbContext backendDbContext;
        public ExerciseController(BackendDbContext backendDbContext)
        {
            this.backendDbContext = backendDbContext;
        }
        [HttpPost]
        public async Task<IActionResult> AddExercise([FromForm]string roomId, [FromForm]Exercise exercise) 
        {
            string userLogin = User.Claims.FirstOrDefault(c => c.ValueType == ClaimTypes.NameIdentifier).Value;

            var room = backendDbContext.Classrooms.Include(i => i.Teacher).Include(i => i.Exercises)
                .FirstOrDefault(room => room.HashId == roomId && room.Teacher.Login == userLogin);

            if (room == null) 
            {
                return NotFound("Either not a teacher or incorrect room ID");
            }
            room.Exercises.Add(exercise);
            backendDbContext.Update(room);

            await backendDbContext.SaveChangesAsync();

            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> RemoveExercise([FromForm]string roomId, [FromForm]Exercise exercise)
        {
            string userLogin = User.Claims.FirstOrDefault(c => c.ValueType == ClaimTypes.NameIdentifier).Value;

            var room = backendDbContext.Classrooms.Include(i => i.Teacher).Include(i => i.Exercises)
                .FirstOrDefault(room => room.HashId == roomId && room.Teacher.Login == userLogin);

            if (room == null)
            {
                return NotFound("Either not a teacher or incorrect room ID");
            }
            room.Exercises.Remove(exercise);
            backendDbContext.Update(room);

            await backendDbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
