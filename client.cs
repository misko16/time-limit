using System;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    class Program
    {
        private const string ServerIP = "127.0.0.1"; // IP-адреса сервера
        private const int ServerPort = 8888; // Порт сервера

        static void Main()
        {
            TcpClient client = new TcpClient();

            try
            {
                // Підключення до сервера
                client.Connect(ServerIP, ServerPort);
                Console.WriteLine("Підключено до сервера.");

                // Отримання мережевого потоку для читання/запису даних
                NetworkStream stream = client.GetStream();

                // Виконання команд клієнта
                GetTimeLimit(stream);

                // Закриття з'єднання з сервером
                stream.Close();
                client.Close();
                Console.WriteLine("З'єднання з сервером завершено.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Помилка клієнта: " + ex.Message);
            }
        }

        static void GetTimeLimit(NetworkStream stream)
        {
            // Відправка команди GET_LIMIT на сервер
            string request = "GET_LIMIT";
            byte[] requestData = Encoding.UTF8.GetBytes(request);
            stream.Write(requestData, 0, requestData.Length);

            // Отримання відповіді від сервера
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine("Обмеження часу: " + response);
        }
    }
}
