using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessCenter.Web.Data;
using FitnessCenter.Web.Models;
using FitnessCenter.Web.Services;

namespace FitnessCenter.Web.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrainersApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly AppointmentService _appointmentService;

        public TrainersApiController(ApplicationDbContext context, AppointmentService appointmentService)
        {
            _context = context;
            _appointmentService = appointmentService;
        }

        // GET: api/TrainersApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetTrainers()
        {
            var trainers = await _context.Trainers
                .Include(t => t.Gym)
                .Include(t => t.TrainerServices)
                    .ThenInclude(ts => ts.Service)
                .Select(t => new
                {
                    t.Id,
                    t.FirstName,
                    t.LastName,
                    t.FullName,
                    t.Specializations,
                    t.AvailableHours,
                    GymName = t.Gym != null ? t.Gym.Name : null,
                    Services = t.TrainerServices.Select(ts => new { ts.Service.Id, ts.Service.Name }).ToList()
                })
                .ToListAsync();

            return Ok(trainers);
        }

        // GET: api/TrainersApi/available?date=2024-01-01
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<object>>> GetAvailableTrainers([FromQuery] DateTime? date)
        {
            if (!date.HasValue)
            {
                return BadRequest("Date parameter is required (format: YYYY-MM-DD)");
            }

            var availableTrainers = await _context.Trainers
                .Include(t => t.Gym)
                .Include(t => t.TrainerServices)
                    .ThenInclude(ts => ts.Service)
                .Where(t => t.AvailableHours != null && t.AvailableHours.Contains(date.Value.DayOfWeek.ToString()))
                .ToListAsync();

            var result = availableTrainers
                .Select(t => new
                {
                    t.Id,
                    t.FirstName,
                    t.LastName,
                    t.FullName,
                    t.Specializations,
                    t.AvailableHours,
                    GymName = t.Gym != null ? t.Gym.Name : null,
                    Services = t.TrainerServices.Select(ts => new { ts.Service.Id, ts.Service.Name }).ToList()
                })
                .ToList();

            return Ok(result);
        }

        // GET: api/TrainersApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetTrainer(int id)
        {
            var trainer = await _context.Trainers
                .Include(t => t.Gym)
                .Include(t => t.TrainerServices)
                    .ThenInclude(ts => ts.Service)
                .Where(t => t.Id == id)
                .Select(t => new
                {
                    t.Id,
                    t.FirstName,
                    t.LastName,
                    t.FullName,
                    t.Specializations,
                    t.AvailableHours,
                    GymName = t.Gym != null ? t.Gym.Name : null,
                    Services = t.TrainerServices.Select(ts => new { ts.Service.Id, ts.Service.Name }).ToList()
                })
                .FirstOrDefaultAsync();

            if (trainer == null)
            {
                return NotFound();
            }

            return Ok(trainer);
        }
    }
}

