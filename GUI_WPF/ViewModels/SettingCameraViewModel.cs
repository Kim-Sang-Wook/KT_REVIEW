using CORE;
using DRIVER_CAMERA;
using DRIVER_CAMERA.Infos;
using FUNCTION_CAMERA;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GUI_WPF.ViewModels
{
	class SettingCameraViewModel : BindableBase
	{
		private bool								_bCAM_Set_Enable;	//_isEnableCameraSetting
		public	bool								m_bCAM_Set_Enable	//IsEnableCameraSetting
		{
			get
			{
				return _bCAM_Set_Enable;
			}
			set
			{
				SetProperty(ref _bCAM_Set_Enable, value);
			}
		}

		public	CStateStore							m_CStateStore		{ get; private set; }
		public	CCoreConfig							m_CCoreConfig		{ get; private set; }
		private CInfo_Camera_Para					_CInfo_Camera_Para; //_parameterInfo 
		public	CInfo_Camera_Para					m_CInfo_Camera_Para	//ParameterInfo
		{
			get
			{
				return _CInfo_Camera_Para;
			}
			set
			{
				SetProperty(ref _CInfo_Camera_Para, value);

				m_bCAM_Set_Enable = m_CInfo_Camera_Para != null ? true : false;
			}
		}

		public	DelegateCommand						m_dlRefreshCMD		{ get; set; }	//RefreshCommand
		public	DelegateCommand						m_dlConnectCMD		{ get; set; }	//ConnectCommand
		public	DelegateCommand						m_dlDisconnectCMD	{ get; set; }	//DisconnectCommand
		public	DelegateCommand						m_dlTriggerModeCMD	{ get; set; }	//SetTriggerModeCommand
		public	DelegateCommand<ECAM_AUTO_TYPE?>	m_dlAutoCMD			{ get; set; }	//SetAutoCommand
		public	IEnumerable<ECAM_AUTO_VALUE>		m_IEAutoValue		{ get; set; }   //AutoValues
		private IEnumerable<CInfo_Camera_Factory>	_ICcameraInfos;     //_cameraInfos
		public	IEnumerable<CInfo_Camera_Factory>	m_ICCameraInfos		//CameraInfos
		{
			get
			{
				return _ICcameraInfos;
			}
			set
			{
				SetProperty(ref _ICcameraInfos, value);
			}
		}



		public SettingCameraViewModel(CFun_GrabService CFun_grabService, CCoreConfig CCoreconfig, CStateStore CStatestore)
		{
			m_CStateStore	= CStatestore;
			m_CCoreConfig	= CCoreconfig;
			m_IEAutoValue	= Enum.GetValues(typeof(ECAM_AUTO_VALUE)).Cast<ECAM_AUTO_VALUE>();
			m_ICCameraInfos = CFun_grabService.Sub_GetDeviceInfos();

			if (CFun_grabService.Sub_IsConnected())
			{
				m_CInfo_Camera_Para = CFun_grabService.Sub_GetParameterInfo();
			}

			CFun_grabService.m_dlParameterChanged += (paramterInfo =>
			{
				m_CInfo_Camera_Para = paramterInfo;
			});


			m_dlRefreshCMD = new DelegateCommand(() =>
			{
				m_ICCameraInfos = CFun_grabService.Sub_GetDeviceInfos();
			});

			m_dlConnectCMD = new DelegateCommand(() =>
			{
				if (CCoreconfig.CInfoCamera != null)
				{
					if (CFun_grabService.Sub_Connect(CCoreconfig.CInfoCamera))
						m_CInfo_Camera_Para = CFun_grabService.Sub_GetParameterInfo();
				}
			});

			m_dlDisconnectCMD = new DelegateCommand(() =>
			{
				CFun_grabService.Sub_Disconnect();
				m_CInfo_Camera_Para = null;
			});

			m_dlAutoCMD = new DelegateCommand<ECAM_AUTO_TYPE?>(type =>
			{
				if (type == null)	return;

				CFun_grabService.Set_Auto(type.Value, m_CInfo_Camera_Para.m_Idictautovalues[type.Value]);
				m_CInfo_Camera_Para = CFun_grabService.Sub_GetParameterInfo();
			});

			m_dlTriggerModeCMD = new DelegateCommand(() =>
			{
				CFun_grabService.Set_TriggerMode(m_CInfo_Camera_Para.m_btrigger);
			});
			
		}
	}
}
