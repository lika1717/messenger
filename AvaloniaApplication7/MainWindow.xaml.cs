using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Collections;
using System.Linq;
using System.Threading;

namespace AvaloniaApplication7
{
	public class MainWindow : Window
	{
		View client { get; set; }
		string NameUser { get; set; }
		string ip { get; set; }
		public MainWindow()
		{
			InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
			this.FindControl<Button>("button1").Click += button1_Click;
			this.FindControl<Button>("button2").Click += button2_Click; ;
			client = new View();

			Closed += MainWindow_Closed; ;
			this.FindControl<ListBox>("list1").SelectionChanged += List1_SelectionChanged; 
			
		}

		

		private void List1_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (this.FindControl<ListBox>("list1").SelectedIndex == -1)
			{
				this.FindControl<TextBlock>("block1").Text = "";
			}
			else
			{
				this.FindControl<TextBlock>("block1").Text = client.Members.ElementAt(this.FindControl<ListBox>("list1").SelectedIndex).Chat;
			}
		}

		private void button2_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
		{
			client.Proccess(NameUser,ip);
			this.FindControl<Button>("button2").IsEnabled = false;			
			var objects = new ArrayList();
			objects.Add(this.FindControl<TextBlock>("block1"));
			objects.Add(this.FindControl<ListBox>("list1"));
			Thread receiveThread = new Thread(new ParameterizedThreadStart(client.ReceiveMessage));
			receiveThread.Start(objects);
		}

		private void button1_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
		{
			client.SendMessage(this.FindControl<TextBox>("textBox1"), this.FindControl<ListBox>("list1"), this.FindControl<TextBlock>("block1"));
		}




		private void MainWindow_Closed(object sender, System.EventArgs e)
		{
			client.Disconnect();
			Environment.Exit(0);
		}
		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}

		public void ChangeTitle(string name, string ip)
		{
			this.Title = name;
			NameUser = name;
			this.ip = ip;
		}
	}
}
