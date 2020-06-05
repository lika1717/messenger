using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaloniaApplication7
{
	public class AutorizationAvalonia : Window
	{
		public AutorizationAvalonia()
		{
			this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
			
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
			this.FindControl<Button>("button1").Click += AutorizationAvalonia_Click;
		}

		private void AutorizationAvalonia_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
		{
			var textbox1 = this.FindControl<TextBox>("textbox1");
			var textbox2 = this.FindControl<TextBox>("textbox2");
			if (textbox1.Text != "" && textbox2.Text !="")
			{
				var main = new MainWindow(/*textbox1.Text.Replace(" ", "_")*/);
				main.ChangeTitle(textbox1.Text.Replace(" ", "_"),textbox2.Text);
				main.Show();
				this.Owner = main;
				this.Hide();
			}
		}
	}
}
