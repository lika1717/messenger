using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


namespace ServerCore
{
	public class Server
	{
		static TcpListener tcpListener;
		public List<Client> clients = new List<Client>();

		protected internal void AddConnection(Client client)
		{
			clients.Add(client);
		}

		protected internal void BroadcastMessage(string message, string id)
		{
			byte[] data; 
			for (int i = 0; i < clients.Count; i++)
			{
				if (id == "all")
				{
					data = Encoding.Unicode.GetBytes(message);
					clients[i].stream.Write(data, 0, data.Length);
				}
				
				else if(id == "not me")
				{
					if(clients[i].Id != message.Split(' ')[1])
					{
						data = Encoding.Unicode.GetBytes(message);
						clients[i].stream.Write(data, 0, data.Length);
					}
				} 
				else if(message == "none")
				{
					data = Encoding.Unicode.GetBytes(message);
					clients[i].stream.Write(data, 0, data.Length);
				}
				else if (clients[i].Id == id)
				{
					data = Encoding.Unicode.GetBytes(message);
					clients[i].stream.Write(data, 0, data.Length); //передача данных
				}
			}
		}

		protected internal void RemoveConnection(string id)
		{
			// получаем по id закрытое подключение
			Client client = clients.FirstOrDefault(c => c.Id == id);
			// и удаляем его из списка подключений
			if (client != null)
				clients.Remove(client);
		}
		protected internal void Listen()
		{
			try
			{
				tcpListener = new TcpListener(IPAddress.Any, 8888);
				tcpListener.Start();
				Console.WriteLine("Сервер запущен. Ождание подключений...");

				while (true)
				{
					byte[] data = new byte[64]; // буфер для получаемых данных
					StringBuilder builder = new StringBuilder();
					int bytes = 0;

					TcpClient tcpClient = tcpListener.AcceptTcpClient();
					var stream = tcpClient.GetStream();
					do
					{
						bytes = stream.Read(data, 0, data.Length);
						builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
					}
					while (stream.DataAvailable);

					string message = builder.ToString();
					var client = new Client(tcpClient, this, message);
					AddConnection(client);
					var clientThread = new Thread(new ThreadStart(client.Procces));
					clientThread.Start();


				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Disconnect();
			}
		}
		protected internal void Disconnect()
		{
			tcpListener.Stop(); //остановка сервера

			for (int i = 0; i < clients.Count; i++)
			{
				clients[i].Close(); //отключение клиента
			}
			Environment.Exit(0); //завершение процесса
		}
	}
}
