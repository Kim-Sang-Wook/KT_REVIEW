using CORE;
using FUNCTION_CAMERA;
using Newtonsoft.Json;
using Prism.Ioc;
using Prism.Unity;
using System;
using System.IO;
using System.Windows;
using System.Windows.Data;
using GUI_WPF.Views;
using System.Threading;

namespace GUI_WPF
{
	[ValueConversion(typeof(bool), typeof(bool))]
	public class InverseBooleanConverter : IValueConverter
	{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (targetType != typeof(bool))
				throw new InvalidOperationException("The target must be a boolean");

			return !(bool)value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		#endregion
	}

	public partial class App : PrismApplication
	{
		private CancellationTokenSource _cts;

		protected override Window CreateShell()
		{
			return new ShellView();
			//return Container.Resolve<ShellView>();
		}

		protected override void RegisterTypes(IContainerRegistry containerRegistry)
		{
			_cts = new CancellationTokenSource();

			containerRegistry.RegisterInstance(File.Exists("CoreConfig.json") ? JsonConvert.DeserializeObject<CCoreConfig>(File.ReadAllText("CoreConfig.json")) : new CCoreConfig())
							 .RegisterSingleton<CAppState>()
							 .RegisterSingleton<CStateStore>()
							 .RegisterSingleton<CFun_GrabService>()
							 .RegisterInstance(_cts)
							 .Register<CancellationToken>(i => _cts.Token);

			var config = Container.Resolve<CCoreConfig>();
			if (config.CInfoCamera != null)
			{
				Container.Resolve<CFun_GrabService>().Sub_Connect(config.CInfoCamera);
			}
		}

		protected override void OnExit(ExitEventArgs e)
		{
			File.WriteAllText("CoreConfig.json", JsonConvert.SerializeObject(Container.Resolve<CCoreConfig>()));
			base.OnExit(e);
		}
	}
}
	