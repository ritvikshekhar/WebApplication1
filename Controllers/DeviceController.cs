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

        // get device serial number by ID
        // useful for users to quickly find a device's serial number

        [HttpGet("{id}/serial")]
        public async Task<IActionResult> GetDeviceSerialNumber(int id)
        {
            var serialNumber = await _context.DeviceData
                .Where(d => d.DeviceID == id)
                .Select(d => d.SerialNumber)
                .FirstOrDefaultAsync();

            if (serialNumber == null)
                return NotFound();

            return Ok(serialNumber);
        }

        // for technician to add the device data
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

            device.DeviceID = 0;
            device.UpdatedAt = DateTime.UtcNow;

            _context.DeviceData.Add(device);
            await _context.SaveChangesAsync();

            return Ok(device);
        }
    }
}