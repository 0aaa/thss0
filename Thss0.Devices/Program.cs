using Thss0.Devices;

Console.WriteLine("Hello, World!");
const string localhost = "127.0.0.1";
const ushort clientsQty = 2;
var clients = new Client[clientsQty];
for (int i = 0; i < clientsQty; i++)
{
    clients[i] = new Client(localhost);
}