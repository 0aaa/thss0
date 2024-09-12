using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Thss0.Devices
{
    internal class Device
    {
        public Device()
        {
            Server().Wait();
        }

        private static async Task Server()
        {
            const string ip = "127.0.0.1";
            const int port = 25545;
            const int clientsMax = 2;
            var s = new TcpListener(new IPEndPoint(IPAddress.Parse(ip), port));
            var clientsCnt = 0;
            s.Start();
            Console.WriteLine("the server launch");
            while (true)
            {
                var c = await s.AcceptTcpClientAsync();
                if (clientsCnt < clientsMax)
                {
                    clientsCnt++;
                    Console.WriteLine($"{DateTime.Now.ToShortTimeString()}\t{c.Client.RemoteEndPoint}");
                    await Task.Run(async () =>
                    {
                        await c.GetStream().WriteAsync(Encoding.UTF8.GetBytes("100"));// "Continue".
                        HandleClient(c);
                        Console.WriteLine($"{DateTime.Now.ToShortTimeString()}\tdisconnected\t{c.Client.RemoteEndPoint}");
                        c.Close();
                        clientsCnt--;
                    });
                }
                else
                {
                    await c.GetStream().WriteAsync(Encoding.UTF8.GetBytes("503"));// "Service Unavailable".
                    c.Close();
                }
            }
        }

        private static void HandleClient(TcpClient c)
        {
            const int attemptsMax = 2;
            const string ecgData = @"  time    MLII    V1
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
            var authCredentials = new[] { new { username = "admin", pwd = "admin" } };
            var attempts = 0;
            var dataMax = 0;
            var dataCnt = 0;
            var buff = new byte[1024];
            var cmd = new string[2];
            while (attempts < attemptsMax)
            {
                cmd = Encoding.Default.GetString(buff.Take(c.GetStream().Read(buff)).ToArray()).Split('\t');
                if (cmd[0] == "226" || cmd[0] == "" || cmd[1] == "")// "IM Used".
                {
                    break;
                }
                if (authCredentials.Any(a => a.username == cmd[0] && a.pwd == cmd[1]))
                {
                    attempts = attemptsMax;
                    c.GetStream().Write(Encoding.UTF8.GetBytes("200"));// "OK".
                    dataMax = Convert.ToInt32(Encoding.Default.GetString(buff.Take(c.GetStream().Read(buff)).ToArray()));
                    while (dataCnt < dataMax)
                    {
                        if (Encoding.Default.GetString(buff.Take(c.GetStream().Read(buff)).ToArray()) == "100")// "Continue".
                        {
                            dataCnt++;
                            c.GetStream().Write(Encoding.UTF8.GetBytes(ecgData));
                            Console.WriteLine($"{ecgData}\t200");// "OK".
                        }
                        else
                        {
                            attempts = attemptsMax;
                            break;
                        }
                    }
                    c.GetStream().Write(Encoding.UTF8.GetBytes("226"));// "IM Used".
                    return;
                }
                else
                {
                    attempts++;
                    if (attempts == attemptsMax)
                    {
                        c.GetStream().Write(Encoding.UTF8.GetBytes("401"));// "Unauthorized".
                        return;
                    }
                }
            }
            c.GetStream().Write(Encoding.UTF8.GetBytes("429"));// "Too Many Requests".
        }
    }
}