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
    public class DevicesController : Controller
    {
        private ushort _dataMax = 0;

        [HttpGet("{order:bool?}/{printBy:int?}/{page:int?}")]
        public async Task<ActionResult<IEnumerable<ProcedureViewModel>>> GetDevices(bool order = true, int printBy = 20, int page = 1)
        {
            _dataMax = 0;
            string[] deviceNames = { "ECG", "X-Ray", "MRI", "CT", "UA", "EEG" };
            var devices = new List<Device>();
            for (int i = 0; i < deviceNames.Length; i++)
            {
                devices.Add(new Device { Name = deviceNames[i] });
                await Client(devices[i]);
            }
            return Json(new
            {
                content = devices
                , total_amount = devices.Count
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Device>> GetDevice(string id)
        {
            _dataMax = 1;
            var device = new Device { Name = id };
            await Client(device);
            return device;
        }

        private async Task Client(Device device)
        {
            const string localhost = "127.0.0.1";
            const ushort port = 25545;
            const string username = "admin";
            const string password = "admin";
            var client = new TcpClient();
            var buffer = new byte[1024];
            try
            {
                await client.ConnectAsync(new IPEndPoint(IPAddress.Parse(localhost), port));
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            string answer = Encoding.Default.GetString(buffer.Take(await client.GetStream().ReadAsync(buffer)).ToArray());
            if (answer != "100")// "Continue".
            {
                device.Content = answer;
            }
            else
            {
                await client.GetStream().WriteAsync(Encoding.UTF8.GetBytes($"{username}\t{password}"));
                SendRequest(client, device);
            }
        }
        private async void SendRequest(TcpClient client, Device device)
        {
            var buffer = new byte[1024];
            if (Encoding.Default.GetString(buffer.Take(await client.GetStream().ReadAsync(buffer)).ToArray()) != "200")// "OK".
            {
                return;
            }
            device.Availability = true;
            await client.GetStream().WriteAsync(Encoding.UTF8.GetBytes(_dataMax.ToString()));
            if (_dataMax > 0)
            {
                string[] exitCodes = { "429", "401", "226", "503" };// "Too Many Requests", "Unauthorized", "IM Used", "Service Unavailable".
                while (!exitCodes.Contains(string.Join("", device.Content.TakeLast(3).ToArray())))
                {
                    try
                    {
                        client.GetStream().Write(Encoding.UTF8.GetBytes("100"));// "Continue" - get data.
                        device.Content += Encoding.Default.GetString(buffer.Take(client.GetStream().Read(buffer)).ToArray());
                    }
                    catch (IOException e)
                    {
                        device.Content = e.Message;
                    }
                }
                device.Content = string.Join("", device.Content.Take(device.Content.Length - 3).ToArray());
            }
        }
    }
}