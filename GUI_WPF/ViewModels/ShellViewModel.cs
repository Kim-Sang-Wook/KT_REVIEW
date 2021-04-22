using Prism.Mvvm;

namespace GUI_WPF.ViewModels
{
	class ShellViewModel : BindableBase
	{
		private string _title = "Prsim Application";

		public string Title
		{
			get { return _title; }
			set { SetProperty(ref _title, value); }
		}

		public ShellViewModel()
		{

		}
	}
}
