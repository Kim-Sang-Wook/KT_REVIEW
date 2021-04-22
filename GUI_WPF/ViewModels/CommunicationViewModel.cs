using FUNCTION_CAMERA;
using GUI_WPF.Infos;
using Prism.Mvvm;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GUI_WPF.ViewModels
{
	class CommunicationViewModel : BindableBase
	{
		private CStateStore _CStateStore;										//_stateStore;
		public	CStateStore m_CStateStore	{ get { return _CStateStore; } }	//StateStore

		public CommunicationViewModel(CFun_GrabService CFun_Grab, CStateStore CStatestore, CancellationToken token)
		{
			_CStateStore = CStatestore;

			Task.Run(async () =>
			{
				while (token.IsCancellationRequested == false)
				{
					_CStateStore.m_tsconnect_cam = GetConnectionInfo(CFun_Grab.Sub_IsConnected());

					await Task.Delay(1000).ConfigureAwait(false);
				}
			});
		}

		private tsInfo_module_Connect GetConnectionInfo(bool isConnected)
		{
			if (isConnected)
			{
				return new tsInfo_module_Connect(isConnected, "ONLINE", Colors.Green);
			}

			return new tsInfo_module_Connect(isConnected, "OFFLINE", Colors.Red);
		}
	}
}
