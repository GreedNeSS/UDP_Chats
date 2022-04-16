using MultiBroadcast;

Console.WriteLine("***** Multi Broadcast *****");

Console.WriteLine("Введите своё имя: ");
string userName = Console.ReadLine();

using (UDPClientApp appClient = new UDPClientApp(userName))
{
    appClient.ReceiveMessage();
    Console.WriteLine("Введите сообщение для отправки:");

    while (true)
    {
        string message = Console.ReadLine();
        appClient.SendMessage(message);
    }
}