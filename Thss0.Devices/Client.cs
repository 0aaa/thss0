using System.Net;
using System.Net.Sockets;
using System.Xml.Serialization;

namespace Thss0.Devices
{
    internal class Client
    {
        private const string USERNAME = "admin";
        private const string PASSWORD = "admin";
        private const string _testData = "testData";
        public Client(string localhost)
            => Core(localhost);

        private async void Core(string localhost)
        {
            // const string localhost = "127.0.0.1";
                const ushort port = 25545;
                // string username = "";
                // string password = "";
                var client = new TcpClient();
                var serializer = new XmlSerializer(typeof(object));
                // var response = new List<string>();
                // while (true)
                // {
            try
            {
                    await client.ConnectAsync(new IPEndPoint(IPAddress.Parse(localhost), port));
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
                    if (client.Connected)
                    {
                //         break;
                    }
                // }
                // response.Add("200");
                var serverReadiness = serializer.Deserialize(client.GetStream())?.ToString();
                // if (serverReadiness == "503")
                if (serverReadiness != "100")// "Continue".
                {
                    Close(serializer, client);
                }
                else
                {
                    serializer.Serialize(client.GetStream(), USERNAME);
                    serializer.Serialize(client.GetStream(), PASSWORD);
                    ReceiveData(serializer, client/*, response*/);
                }
        }
        private void ReceiveData(XmlSerializer serializer, TcpClient client/*, List<string> response*/)
        {
            string? buffer = serializer.Deserialize(client.GetStream())?.ToString();
            if (buffer != "200")// "OK".
            {
                Close(serializer, client);
            }
            SendData(serializer, client/*, response*/);
            // response.Add($"server:\t{buffer}");
        }
        private void SendData(XmlSerializer serializer, TcpClient client/*, List<string> response*/)
        {
            serializer.Serialize(client.GetStream(), "data");
            serializer.Serialize(client.GetStream(), _testData);
            ReceiveData(serializer, client/*, response*/);
        }
        private void Close(XmlSerializer serializer, TcpClient client)
        {
            serializer.Serialize(client.GetStream(), "226");// "IM Used".
            client.Close();
        }
    }
}
