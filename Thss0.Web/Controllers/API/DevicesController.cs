using System.Net;
using System.Net.Sockets;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;
using Thss0.Web.Models.ViewModels.CRUD;

namespace Thss0.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin, professional")]
    public class DevicesController : Controller
    {
        private static readonly string[] _deviceNames = { "ECG", "EEG" };
        private static ushort _dataMax = 0;
        private const string LOCALHOST = "127.0.0.1";
        private const string USERNAME = "admin";
        private const string PASSWORD = "admin";
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Device>>> GetDevices()// Return the data on the devices names and theirs readinesses.
        {
            _dataMax = 0;
            const ushort port = 25545;
            // const ushort dataMax = 1;
            var server = new TcpListener(new IPEndPoint(IPAddress.Parse(LOCALHOST), port));
            // var closeSerializer = new XmlSerializer(typeof(object));
            server.Start();
            Console.WriteLine("the server launch");
            // var clients = new TcpClient[_devices.Length];
            var devices = new List<Device>();
            for (int i = 0; i < _deviceNames.Length; i++)
            {
                devices.Add(new Device { Name = _deviceNames[i] });
                await Server(server, devices[i]);// Some "for" for the check of the devices availabilities.

                // clients[i] = server.AcceptTcpClient();
                // Console.WriteLine($"{DateTime.Now.ToShortTimeString()}\t{clients[i].Client.RemoteEndPoint}");
                // Task.Run(() =>
                // {
                //     HandleClient(dataMax, clients[i]);
                // });
                // fullServerSerializer.Serialize(clients[i].GetStream(), "503");// Or wait for the data reception.
                // clients[i].Close();
            }
            return Json(new
            {
                content = devices
                ,
                total_amount = devices.Count
            });
        }
        [HttpGet("id")]
        public async Task<ActionResult<Device>> GetDevice(string id)// Return the content data on the ready and selected device.
        {
            // Some "AcceptTcpClient" on the ready and selected device.
            _dataMax = 1;
            const ushort port = 25545;
            var server = new TcpListener(new IPEndPoint(IPAddress.Parse(LOCALHOST), port));
            var device = new Device { Name = id };
            server.Start();
            Console.WriteLine("the server launch");
            await Server(server, device);
            return device;
        }
        private static async Task Server(TcpListener server, Device device)
        {
            // const string localhost = "127.0.0.1";
            // const ushort port = 25545;
            // const ushort dataMax = 1;
            // var server = new TcpListener(new IPEndPoint(IPAddress.Parse(localhost), port));
            // var fullServerSerializer = new XmlSerializer(typeof(object));
            // ushort clientsCount = 0;
            // server.Start();
            // Console.WriteLine("the server launch");
            // while (true)
            // {
            try
            {
                var serializer = new XmlSerializer(typeof(object));
                var client = server.AcceptTcpClient();
                // if (clientsCount < _devices.Length)
                // {
                //     clientsCount++;
                Console.WriteLine($"{DateTime.Now.ToShortTimeString()}\t{client.Client.RemoteEndPoint}");
                await Task.Run(() =>
                {
                    serializer.Serialize(client.GetStream(), "100");// "Continue".

                    HandleClient(client, serializer, device);
                    // clientsCount--;
                    serializer.Serialize(client.GetStream(), "226");// "IM Used".
                    Console.WriteLine($"{DateTime.Now.ToShortTimeString()}\tdisconnected\t{client.Client.RemoteEndPoint}");
                    client.Close();
                });
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
            //     }
            //     else
            //     {
            //         closeSerializer.Serialize(client.GetStream(), "503");
            //         client.Close();
            //     }
            // }
        }
        private static void HandleClient(TcpClient client, XmlSerializer serializer, Device device)
        {
            // const ushort attemptsMax = 3;
            // var serializer = new XmlSerializer(typeof(object));
            // var receivedData = "";
            try
            {
                string dataBuffer;
                var authCredentials = new[] { new { username = USERNAME, password = PASSWORD } };
                string? input, password;
                // ushort attempts = 0;
                ushort dataCount = 0;
                // serializer.Serialize(client.GetStream(), "200");
                // while (attempts < attemptsMax)
                // {
                input = serializer.Deserialize(client.GetStream())?.ToString();
                password = serializer.Deserialize(client.GetStream())?.ToString();
                if (input == "226"/* || input == null || password == null*/)// "IM Used".
                {
                    // break;
                    return;
                }
                if (authCredentials.Any(a => a.username == input && a.password == password))
                {
                    serializer.Serialize(client.GetStream(), "200");// "OK".
                    device.Availability = true;
                    while (dataCount < _dataMax)
                    {
                        if (serializer.Deserialize(client.GetStream())?.ToString() == "data"/* && dataCount!= _dataMax - 1*/)
                        {
                            dataCount++;
                            dataBuffer = serializer.Deserialize(client.GetStream())?.ToString() ?? "";
                            // receivedData = dataBuffer;
                            device.Content += dataBuffer;
                            Console.WriteLine($"{dataBuffer}\t200");// "OK".
                        }
                        else
                        {
                            // attempts = attemptsMax;
                            break;
                        }
                    }
                }
                else
                {
                    // attempts++;
                    // if (attempts < attemptsMax)
                    // {
                    //     serializer.Serialize(client.GetStream(), "401");// "Unauthorized".
                    // }
                    serializer.Serialize(client.GetStream(), "401");// "Unauthorized".
                }
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
            // }
            // serializer.Serialize(client.GetStream(), "429");
            // Console.WriteLine($"{DateTime.Now.ToShortTimeString()}\tdisconnected\t{client.Client.RemoteEndPoint}");
            // client.Close();
        }
    }
}