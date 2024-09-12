using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Mvc;
using Thss0.Web.Models.ViewModels;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Thss0.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin, professional")]
    public class DeviceController : Controller
    {
        private int _dataMax = 0;

        [HttpGet("{printBy:int?}/{page:int?}/{order:bool?}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin, professional")]
        public async Task<ActionResult<IEnumerable<ProcedureViewModel>>> Get()
        {
            _dataMax = 0;
            var deviceNames = new string[] { "ECG", "X-Ray", "MRI", "CT", "UA", "EEG" };
            var devices = new List<Device>();
            for (int i = 0; i < deviceNames.Length; i++)
            {
                devices.Add(await Client(deviceNames[i]));
            }
            return Json(new
            {
                content = devices
                , total_amount = devices.Count
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Device>> Get(string id)
        {
            _dataMax = 1;
            return await Client(id);
        }

        private async Task<Device> Client(string id)
        {
            const string ip = "127.0.0.1";
            const int port = 25545;
            const string username = "admin";
            const string pwd = "admin";
            var c = new TcpClient();
            var buff = new byte[1024];
            try
            {
                await c.ConnectAsync(new IPEndPoint(IPAddress.Parse(ip), port));
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.Message);
                return new Device() { Name = id };
            }
            var ans = Encoding.Default.GetString(buff.Take(await c.GetStream().ReadAsync(buff)).ToArray());
            if (ans != "100")// "Continue".
            {
                return new Device { Name = id, Content = ans };
            }
            await c.GetStream().WriteAsync(Encoding.UTF8.GetBytes($"{username}\t{pwd}"));
            return await SendRequest(c, id);
        }

        private async Task<Device> SendRequest(TcpClient c, string id)
        {
            var buff = new byte[1024];
            var d = new Device() { Name = id };
            if (Encoding.Default.GetString(buff.Take(await c.GetStream().ReadAsync(buff)).ToArray()) != "200")// "OK".
            {
                return d;
            }
            d.Availability = true;
            await c.GetStream().WriteAsync(Encoding.UTF8.GetBytes(_dataMax.ToString()));
            if (_dataMax > 0)
            {
                var exitCodes = new string[] { "429", "401", "226", "503" };// "Too Many Requests", "Unauthorized", "IM Used", "Service Unavailable".
                while (!exitCodes.Contains(string.Join("", d.Content.TakeLast(3).ToArray())))
                {
                    try
                    {
                        c.GetStream().Write(Encoding.UTF8.GetBytes("100"));// "Continue" - get data.
                        d.Content += Encoding.Default.GetString(buff.Take(c.GetStream().Read(buff)).ToArray());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                d.Content = string.Join("", d.Content.Take(d.Content.Length - 3).ToArray());
            }
            return d;
        }
    }
}