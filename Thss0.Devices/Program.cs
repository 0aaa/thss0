using Thss0.Devices;

const ushort clientsQty = 6;
var clients = new Device[clientsQty];
for (int i = 0; i < clientsQty; i++)
{
    clients[i] = new Device();
}