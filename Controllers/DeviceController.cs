using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.data;

namespace project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DevicesController : ControllerBase
    {
        private readonly app_DBcontext _context;

        public DevicesController(app_DBcontext context)
        {
            _context = context;
        }

        // ─────────────────────────────
        //  1. GET ALL DEVICES (optionally filter by updated date)
        // ─────────────────────────────

        /// <summary>
        /// Get all devices that have a support label.
        /// Optionally filter by devices updated since the given date.
        /// </summary>
        /// <param name="changedSince">Optional date filter (only return devices updated after this)</param>
        /// <returns>List of matching devices</returns>
        [HttpGet]
        public async Task<IActionResult> GetDevices([FromQuery] DateTime? changedSince)
        {
            var query = _context.DeviceData
                .Where(d => !string.IsNullOrEmpty(d.SupportLabelIdentifier));

            if (changedSince.HasValue)
            {
                query = query.Where(d => d.UpdatedAt >= changedSince.Value);
            }

            var result = await query.ToListAsync();
            return Ok(result);
        }

        // ─────────────────────────────
        //  2. GET DEVICE BY ID
        // ─────────────────────────────

        /// <summary>
        /// Get a device by its unique ID.
        /// </summary>
        /// <param name="id">DeviceID (int)</param>
        /// <returns>Device if found, else 404</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDeviceById(int id)
        {
            var device = await _context.DeviceData.FindAsync(id);
            if (device == null) return NotFound();
            return Ok(device);
        }

        // ─────────────────────────────
        //  3. GET DEVICE BY SUPPORT LABEL
        // ─────────────────────────────

        /// <summary>
        /// Get a device by its support label identifier.
        /// </summary>
        /// <param name="label">SupportLabelIdentifier</param>
        /// <returns>Device if found, else 404</returns>
        [HttpGet("label/{label}")]
        public async Task<IActionResult> GetDeviceBySupportLabel(string label)
        {
            var device = await _context.DeviceData
                .FirstOrDefaultAsync(d => d.SupportLabelIdentifier == label);

            if (device == null) return NotFound();
            return Ok(device);
        }

        // ─────────────────────────────
        //  4. SEARCH DEVICE BY NAME
        // ─────────────────────────────

        /// <summary>
        /// Search devices by name (case-insensitive contains).
        /// </summary>
        /// <param name="name">Partial or full name to search</param>
        /// <returns>List of matching devices</returns>
        [HttpGet("search")]
        public async Task<IActionResult> SearchByDeviceName([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Search keyword missing.");

            var results = await _context.DeviceData
                .Where(d => d.DeviceName.ToLower().Contains(name.ToLower()))
                .ToListAsync();

            return Ok(results);
        }

        // ─────────────────────────────
        // 5. POST: ADD NEW DEVICE
        // ─────────────────────────────

        /// <summary>
        /// Add a new device to the database.
        /// DeviceID and UpdatedAt are set automatically.
        /// </summary>
        /// <param name="device">Device data (without DeviceID)</param>
        /// <returns>The saved device with ID and UpdatedAt</returns>
        [HttpPost]
        public async Task<IActionResult> AddDevice([FromBody] DeviceData device)
        {
            if (device == null ||
                string.IsNullOrWhiteSpace(device.SerialNumber) ||
                string.IsNullOrWhiteSpace(device.DeviceName) ||
                string.IsNullOrWhiteSpace(device.SupportLabelIdentifier))
            {
                return BadRequest("Missing required fields.");
            }

            device.UpdatedAt = DateTime.UtcNow;

            _context.DeviceData.Add(device);
            await _context.SaveChangesAsync();

            return Ok(device);
        }
    }
}