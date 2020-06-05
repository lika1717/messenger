using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Windows;
using System.Collections;
using Avalonia.Threading;
using Avalonia.Controls;

namespace AvaloniaApplication7
{
	class View
	{
		int counter = 0;
		string userName;
		private string host { get; set; }
		private const int port = 8888;
		public List<EntityForList> Members { get; private set; }
		TcpClient client;
		NetworkStream stream;
		public void Proccess(string name, string host)
		{

			userName = name;
			this.host = host;
			client = new TcpClient();
			Members = new List<EntityForList>();
			
			try
			{
				client.Connect(host, port); 
				stream = client.GetStream();
				string message = userName;
				byte[] data1 = Encoding.Unicode.GetBytes(message);
				stream.Write(data1, 0, data1.Length);				
				
				
			}


			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Disconnect();
			}

		}
		// отправка сообщений
		public void SendMessage(TextBox box,ListBox list,TextBlock text)
		{
			if (box.Text != "")
			{
				try
				{
					Members.ElementAt(list.SelectedIndex).AddChat(String.Format("You: {0}", box.Text));
					
					Dispatcher.UIThread.InvokeAsync(() => text.Text = Members.ElementAt(list.SelectedIndex).Chat);
					string message = Members.ElementAt(list.SelectedIndex).Id + " " + box.Text;
					byte[] data = Encoding.Unicode.GetBytes(message);
					stream.Write(data, 0, data.Length);
					
					box.Text = "";
				}
				catch
				{
					return;
				}
			}

		}
		
		public void ReceiveMessage(object list)
		{
			var lists = (ArrayList)list;
			var block1 = (TextBlock)lists[0];
			var list1 = (ListBox)lists[1];			
			while (true)
			{
				try
				{
					byte[] data = new byte[64]; 
					StringBuilder builder = new StringBuilder();
					int bytes = 0;
					do
					{
						bytes = stream.Read(data, 0, data.Length);
						builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
					}
					while (stream.DataAvailable);
					if (counter == 0)
					{
						if (builder.ToString() != "none")
						{
							List<string> members = builder.ToString().Split(' ').ToList();
							for (int i = 0; i < members.Count-1; i+=2)
							{
								Members.Add(new EntityForList(members[i+1], members[i], ""));
							}
							
							Dispatcher.UIThread.InvokeAsync(() => list1.Items = Members, DispatcherPriority.MaxValue);
							counter++;
						}
					}					
					else
					{
						string message = builder.ToString();
						if (message.Split(' ')[0]=="new")
						{
							Members.Add(new EntityForList(message.Split(' ')[1], message.Split(' ')[2], ""));
							
							
							Dispatcher.UIThread.InvokeAsync(() => list1.Items = null, DispatcherPriority.MaxValue);
							Dispatcher.UIThread.InvokeAsync(() => list1.Items = Members, DispatcherPriority.MaxValue);
						}
						else if (message.Split(' ')[0] == "delete")
						{
							var index = -1;
							index = list1.SelectedIndex;
							if(Members.ElementAt(index).Id == message.Split(' ')[1])
							{
								Dispatcher.UIThread.InvokeAsync(() => list1.SelectedIndex = -1, DispatcherPriority.MaxValue);
							}
							Members.Remove(Members.FindLast(s => s.Id == message.Split(' ')[1]));
							Dispatcher.UIThread.InvokeAsync(() => list1.Items = null, DispatcherPriority.MaxValue);
							Dispatcher.UIThread.InvokeAsync(() => list1.Items = Members, DispatcherPriority.MaxValue);
						}
						else
						{
							foreach (var s in Members)
							{
								if(s.Id == message.Split(' ')[0])
								{
									s.AddChat(s.Name+ ": "+ message.Substring(message.IndexOf(' ')+1));

									try
									{
										int index = -1;									
										index = list1.SelectedIndex;
										if (s.Id == Members.ElementAt(index).Id)
										{
											
											Dispatcher.UIThread.InvokeAsync(() => block1.Text = null, DispatcherPriority.MaxValue);
											Dispatcher.UIThread.InvokeAsync(() => block1.Text = s.Chat, DispatcherPriority.MaxValue);
										}
									}
									catch
									{
										
									}									
								}
							}							
						}
					}
					counter++;
				}
				catch(Exception ex)
				{
					Console.WriteLine(ex.Message);
					Console.ReadLine();
					Disconnect();
				}
			}
		}
		public void Disconnect()
		{
			if (stream != null)
				stream.Close();
			if (client != null)
				client.Close();
			
			Environment.Exit(0);
		}
	}
}
