using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
	public class Client
	{
		protected internal string Id { get; set; }
		public string username;
		public NetworkStream stream;
		private readonly TcpClient tcp;
		private Server server;
		public Client(TcpClient client, Server server, string username)
		{
			Id = Guid.NewGuid().ToString();
			stream = client.GetStream();
			this.username = username;
			tcp = client;
			this.server = server;
		}
		public void Procces()
		{
			try
			{
				stream = tcp.GetStream();
				var people = "";
				if (server.clients.Count >1)
				{
					server.clients.ForEach(s => {
						if (s != server.clients.Last())
						{
							people += s.username + " ";
							people += s.Id + " ";
						}
					}); }
				else
				{
					people = "none";
				}
				server.BroadcastMessage(people, this.Id);

				var message = String.Format("{0} in Chat", username);
				Console.WriteLine(message);
				message = "";
				message += String.Format("new {0} {1}", this.Id, this.username);
				server.BroadcastMessage(message,"not me");
				while (true)
				{
					try
					{
						
						message = GetMessage();
						Console.WriteLine(String.Format("{0}: {1}", username, message.Split(' ')[1]));
						server.BroadcastMessage(this.Id + " "+message.Substring(message.IndexOf(' ')+1), message.Split(' ')[0]);
					}
					catch
					{
						message = string.Format("{0} leave chat", username);
						Console.WriteLine(message);
						message = String.Format("delete {0}", Id);
						server.BroadcastMessage(message,"all");
						server.RemoveConnection(this.Id);
						this.Close();
						break;
					}
				}
			}
			catch
			{
				server.clients.Remove(this);
				this.Close();
			}
		}
		public string GetMessage()
		{

			byte[] data = new byte[64]; // буфер для получаемых данных
			StringBuilder builder = new StringBuilder();
			int bytes = 0;
			do
			{
				bytes = stream.Read(data, 0, data.Length);
				builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
			}
			while (stream.DataAvailable);
			if(bytes == 0)
			{
				throw new Exception();
			}
			return builder.ToString();
		}
		protected internal void Close()
		{
			if (stream != null)
				stream.Close();
			server.RemoveConnection(Id);
		}
	}
}
