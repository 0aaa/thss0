using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Thss0.Devices
{
    internal class Device
    {
        public Device()
            => Server().Wait();

        private static async Task Server()
        {
            const string localhost = "127.0.0.1";
            const ushort port = 25545;
            const ushort clientsMax = 1;
            var server = new TcpListener(new IPEndPoint(IPAddress.Parse(localhost), port));
            ushort clientsCount = 0;
            server.Start();
            Console.WriteLine("the server launch");
            while (true)
            {
                var client = await server.AcceptTcpClientAsync();
                if (clientsCount < clientsMax)
                {
                    clientsCount++;
                    Console.WriteLine($"{DateTime.Now.ToShortTimeString()}\t{client.Client.RemoteEndPoint}");
                    await Task.Run(async () =>
                    {
                        await client.GetStream().WriteAsync(Encoding.UTF8.GetBytes("100"));// "Continue".
                        HandleClient(client);
                        Console.WriteLine($"{DateTime.Now.ToShortTimeString()}\tdisconnected\t{client.Client.RemoteEndPoint}");
                        client.Close();
                        clientsCount--;
                    });
                }
                else
                {
                    await client.GetStream().WriteAsync(Encoding.UTF8.GetBytes("503"));// "Service Unavailable".
                    client.Close();
                }
            }
        }
        private static void HandleClient(TcpClient client)
        {
            const ushort attemptsMax = 1;
            const string EcgData = @"  time    MLII    V1
                (sec)   (mV)    (mV)
                300.000  -0.095  -0.140
                300.003  -0.110  -0.140
                300.006  -0.110  -0.120
                300.008  -0.115  -0.110
                300.011  -0.115  -0.120
                300.014  -0.110  -0.110
                300.017  -0.100  -0.120";
            // const string UaData = @"COLOR YELLOW YELLOW
            //     APPEARANCE HAZY CLEAR
            //     GLUCOSE NEGATIVE NEGATIVE
            //     BILIRUBIN NEGATIVE NEGATIVE
            //     KETONE NEGATIVE NEGATIVE
            //     SPEC GRAV 1.017 1.003 - 1.035
            //     BLOOD NEGATIVE TRACE NEGATIVE
            //     PH 6.0 5.0 - 8.0
            //     PROTEIN NEGATIVE NEGATIVE
            //     UROBILINOGEN 0.3 0.1 - 1.0 mg/dL
            //     NITRITE POSITIVE H NEGATIVE
            //     LEUK ESTERASE 2+ H NEGATIVE";
            var authCredentials = new[] { new { username = "admin", password = "admin" } };
            ushort attempts = 0;
            ushort dataMax = 0;
            ushort dataCount = 0;
            var buffer = new byte[1024];
            var inputAndPassword = new string[2];
            while (attempts < attemptsMax)
            {
                inputAndPassword = Encoding.Default.GetString(buffer.Take(client.GetStream().Read(buffer)).ToArray()).Split('\t');
                if (inputAndPassword[0] == "226" || inputAndPassword[0] == "" || inputAndPassword[1] == "")// "IM Used".
                {
                    break;
                }
                if (authCredentials.Any(a => a.username == inputAndPassword[0] && a.password == inputAndPassword[1]))
                {
                    attempts = attemptsMax;
                    client.GetStream().Write(Encoding.UTF8.GetBytes("200"));// "OK".
                    dataMax = Convert.ToUInt16(Encoding.Default.GetString(buffer.Take(client.GetStream().Read(buffer)).ToArray()));
                    while (dataCount < dataMax)
                    {
                        if (Encoding.Default.GetString(buffer.Take(client.GetStream().Read(buffer)).ToArray()) == "100"/* && dataCount!= _dataMax - 1*/)// "Continue".
                        {
                            dataCount++;
                            client.GetStream().Write(Encoding.UTF8.GetBytes(EcgData));
                            Console.WriteLine($"{EcgData}\t200");// "OK".
                        }
                        else
                        {
                            attempts = attemptsMax;
                            break;
                        }
                    }
                    client.GetStream().Write(Encoding.UTF8.GetBytes("226"));// "IM Used"
                    return;
                }
                else
                {
                    attempts++;
                    if (attempts == attemptsMax)
                    {
                        client.GetStream().Write(Encoding.UTF8.GetBytes("401"));// "Unauthorized".
                        return;
                    }
                }
            }
            client.GetStream().Write(Encoding.UTF8.GetBytes("429"));// "Too Many Requests".
        }
    }
}
