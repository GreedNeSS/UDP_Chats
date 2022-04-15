using ConsoleChat;

Console.WriteLine("***** UDP Client App *****");

Console.WriteLine("Введите порт для получения данных: ");
int localPort = int.Parse(Console.ReadLine());
Console.WriteLine("Введите IP адрес: ");
string address = Console.ReadLine();
Console.WriteLine("Введите порт для отправки сообщений: ");
int remotePort = int.Parse(Console.ReadLine());

using (UDPClientApp appClient = new UDPClientApp(localPort, remotePort, address))
{
    appClient.ReceiveMessage();
    Console.WriteLine("Введите сообщение для отправки:");

    while (true)
    {
        string message = Console.ReadLine();
        appClient.SendMessage(message);
    }
}