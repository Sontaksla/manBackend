using manBackend.Models;
using manBackend.Models.Externsions;
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
    public class ClassroomController : ControllerBase
    {
        private readonly BackendDbContext db;
        public ClassroomController(BackendDbContext db)
        {
            this.db = db;
        }

        [HttpPut]
        public async Task<IActionResult> CreateRoom([FromQuery]string roomTitle) 
        {
            var teacherLogin = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier);

            User teacher = db.Users.Include(i => i.Rooms).FirstOrDefault(u => u.Login == teacherLogin.Value);

            if (!teacher)
            {
                return BadRequest("Authorization error. (invalid token)");
            }

            Classroom room = new Classroom(roomTitle, teacher);

            teacher.Rooms.Add(room);
            db.Users.Update(teacher);

            db.Classrooms.Add(room);

            await db.SaveChangesAsync();

            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> JoinRoom([FromQuery]string id) 
        {
            var userLogin = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier);

            User user = db.Users.Include(i => i.Rooms).FirstOrDefault(u => u.Login == userLogin.Value);
            Classroom room = db.Classrooms.Include(i => i.Students).FirstOrDefault(r => r.HashId == id);

            if (!user || room == null) 
            {
                return BadRequest();
            }
            user.Rooms.Add(room);
            db.Update(user);

            room.Students.Add(user);
            db.Update(room);

            await db.SaveChangesAsync();

            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> LeaveRoom([FromQuery]string id)
        {
            var userLogin = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier);

            User user = db.Users.Include(i => i.Rooms).Include(i => i.Mail).FirstOrDefault(u => u.Login == userLogin.Value);
            Classroom room = db.Classrooms.Include(i => i.Students).FirstOrDefault(r => r.HashId == id);
            // teacher can't leave room (only delete)
            if (!user || room == null || room.Teacher == user)
            {
                return BadRequest();
            }

            user.Rooms.Remove(room);
            db.Update(user);

            room.Students.Remove(user);
            db.Update(room);

            await db.SaveChangesAsync();

            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteRoom([FromQuery]string id) 
        {
            var room = db.Classrooms.Include(i => i.Teacher).Include(i => i.Students)
                .FirstOrDefault(r => r.HashId == id);

            if (room == null)
            {
                return NoContent();
            }

            room.Teacher.Rooms.Remove(room);
            db.Users.Update(room.Teacher);

            foreach (var student in room.Students)
            {
                student.Rooms.Remove(room);

                db.Users.Update(student);
            }

            db.Classrooms.Remove(room);

            await db.SaveChangesAsync();

            return Ok();
        }
    }
}
