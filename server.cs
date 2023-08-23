using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class Program
    {
        private const int Port = 8888; // Порт для підключення клієнта
        private static Dictionary<IPAddress, int> clientLimits = new Dictionary<IPAddress, int>(); // Словник для зберігання обмежень часу для клієнтів

        static void Main()
        {
            Console.WriteLine("Сервер запущений. Очікування підключень...");

            // Створення TCP/IP сокету сервера
            TcpListener server = new TcpListener(IPAddress.Any, Port);

            try
            {
                // Запуск прослуховування підключень
                server.Start();

                while (true)
                {
                    // Приймання підключення клієнта
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Клієнт підключений.");

                    // Обробка клієнта в окремому потоці
                    HandleClient(client);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Помилка сервера: " + ex.Message);
            }
            finally
            {
                // Зупинка сервера
                server.Stop();
            }
        }

        static void HandleClient(TcpClient client)
        {
            // Отримання IP-адреси клієнта
            IPAddress clientIP = ((IPEndPoint)client.Client.RemoteEndPoint).Address;
            Console.WriteLine("IP-адреса клієнта: " + clientIP);

            // Отримання мережевого потоку для читання/запису даних
            NetworkStream stream = client.GetStream();

            try
            {
                byte[] buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    // Отримання рядка з даними від клієнта
                    string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine("Від клієнта: " + data);

                    // Обробка отриманого запиту
                    string response = ProcessRequest(data, clientIP);

                    // Відправка відповіді клієнту
                    byte[] responseData = Encoding.UTF8.GetBytes(response);
                    stream.Write(responseData, 0, responseData.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Помилка обробки клієнта: " + ex.Message);
            }
            finally
            {
                // Закриття з'єднання з клієнтом
                stream.Close();
                client.Close();
                Console.WriteLine("Клієнт відключений.");
            }
        }

        static string ProcessRequest(string request, IPAddress clientIP)
        {
            string[] requestData = request.Split(':');
            string command = requestData[0];

            if (command == "SET_LIMIT")
            {
                if (requestData.Length == 2)
                {
                    int limit;
                    if (int.TryParse(requestData[1], out limit))
                    {
                        clientLimits[clientIP] = limit;
                        return "SUCCESS: Time limit set for the client";
                    }
                    else
                    {
                        return "ERROR: Invalid limit value";
                    }
                }
                else
                {
                    return "ERROR: Invalid command format";
                }
            }
            else if (command == "GET_LIMIT")
            {
                if (clientLimits.ContainsKey(clientIP))
                {
                    int limit = clientLimits[clientIP];
                    return limit.ToString();
                }
                else
                {
                    return "0";
                }
            }
            else
            {
                return "ERROR: Invalid command";
            }
        }
    }
}
