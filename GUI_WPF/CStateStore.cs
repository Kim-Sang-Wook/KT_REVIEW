using CORE;
using GUI_WPF.Infos;
using Prism.Mvvm;

namespace GUI_WPF
{
	class CStateStore : BindableBase
	{
		private bool	_bLiveMode;							// _isLiveMode;
		public	bool	m_bLiveMode							// IsLiveMode
		{
			get
			{
				return _bLiveMode;
			}
			set
			{
				SetProperty(ref _bLiveMode, value);
				_Fun_CheckState();
			}
		}

		private bool	_bManual_Mode;						// _onManual
		public	bool	m_bManual_Mode                     // OnManual
		{
			get
			{
				return _bManual_Mode;
			}
			set
			{
				SetProperty(ref _bManual_Mode, value);
				_Fun_CheckState();
				m_strmode = _bManual_Mode ? "Manual" : "Auto";
			}
		}

		private bool	_bAutoEnable;						//_isAutoEnabled;
		public	bool	m_bAutoEnable						// IsAutoEnabled
		{
			get
			{
				return _bAutoEnable;
			}
			private set
			{
				SetProperty(ref _bAutoEnable, value);
			}
		}

		private bool	_bManualEnable;						//_isManualEnabled;
		public	bool	m_bManualEnable						//IsManualEnabled
		{
			get
			{
				return _bManualEnable;
			}
			private set
			{
				SetProperty(ref _bManualEnable, value);
			}
		}

		private bool	_bGrabEnable;						//_isGrabEnabled;
		public	bool	m_bGrabEnable						//IsGrabEnabled
		{
			get
			{
				return _bGrabEnable;
			}
			private set
			{
				SetProperty(ref _bGrabEnable, value);
			}
		}

		private bool	_bSet_Mode;							//_isSettingMode;
		public	bool	m__bSet_Mode						//IsSettingMode
		{
			get
			{
				return _bSet_Mode;
			}
			set
			{
				SetProperty(ref _bSet_Mode, value);
			}
		}

		private string	_strmode = "Auto";					// _mode = "Auto";
		public	string	m_strmode							// Mode
		{
			get
			{
				return _strmode;
			}
			set
			{
				SetProperty(ref _strmode, value);
			}
		}


		private CAppState				_appState;

		private tsInfo_module_Connect	_tsconnect_cam;		// ConnectionInfo _cameraInfo;
		public	tsInfo_module_Connect	m_tsconnect_cam		// CameraInfo
		{
			get
			{
				return _tsconnect_cam;
			}
			set
			{
				SetProperty(ref _tsconnect_cam, value);
				_Fun_CheckState();
			}
		}

		private tsInfo_module_Connect	_tsconnect_light;	//_lightInfo;
		public	tsInfo_module_Connect	m_tsconnect_light	//LightInfo
		{
			get
			{
				return _tsconnect_light;
			}
			set
			{
				SetProperty(ref _tsconnect_light, value);
				_Fun_CheckState();
			}
		}

		private tsInfo_module_Connect	_tsconnect_host;    //_hostInfo;
		public	tsInfo_module_Connect	m_tsconnect_host	//HostInfo
		{
			get
			{
				return _tsconnect_host;
			}
			set
			{
				SetProperty(ref _tsconnect_host, value);
				_Fun_CheckState();
			}
		}

		private tsInfo_module_Connect	_tsconnect_inspect; //_inspectorInfo;
		public	tsInfo_module_Connect	m_tsconnect_inspect	//InspectorInfo
		{
			get
			{
				return _tsconnect_inspect;
			}
			set
			{
				SetProperty(ref _tsconnect_inspect, value);
				_Fun_CheckState();
			}
		}

		public CStateStore(CAppState CAppstate)
		{
			_appState = CAppstate;
		}

		private void _Fun_CheckState()
		{
			_appState.m_bManual_Mode =	m_bManual_Mode;

			_appState.m_bAutoEnable =	m_bAutoEnable =		_tsconnect_cam.m_bIsConnected		&&
															_tsconnect_light.m_bIsConnected		&&
															_tsconnect_host.m_bIsConnected		&&
															_tsconnect_inspect.m_bIsConnected	&&
															_bLiveMode == false;

			_appState.m_bManualEnable =	m_bManualEnable =	_tsconnect_cam.m_bIsConnected	|| //&&
															_tsconnect_light.m_bIsConnected || //&&
															_tsconnect_inspect.m_bIsConnected;

			_appState.m_bGrabEnable =	m_bGrabEnable =		_tsconnect_cam.m_bIsConnected; // &&
															//_tsconnect_light.m_bIsConnected;
		}

	}
}
