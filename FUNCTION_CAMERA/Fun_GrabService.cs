using DRIVER_CAMERA;
using DRIVER_CAMERA.Infos;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FUNCTION_CAMERA
{
	public class CFun_GrabService
	{
		#region Define Variable

		bool					_bgrabbing;

		ILogger					_Ilogger;
		IDriver_Camera_Factory	_Idriver_factory_Hik;
		IDriver_Camera_Factory	_Idriver_factory_Basler;
		IDriver_Camera_Control	_Idriver_camera;

		tsInfo_Grab				_tsinfo_grab;

		public Action<tsInfo_Grab>			m_dlImageGrabbed		{ get; set; }
		public Action<CInfo_Camera_Para>	m_dlParameterChanged	{ get; set; }
		#endregion

		public		CFun_GrabService()
		{
			_Idriver_factory_Hik	= Driver_Camera_Factory.Instance.Get_Camera_Factory(EMAKER.eHik);
			_Idriver_factory_Basler = Driver_Camera_Factory.Instance.Get_Camera_Factory(EMAKER.eBasler);

			_Ilogger = LogManager.GetCurrentClassLogger();

			_bgrabbing = false;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////

		public IEnumerable<CInfo_Camera_Factory>		Sub_GetDeviceInfos()
		{
			var Infos = new List<CInfo_Camera_Factory>();
			Infos.AddRange(_Idriver_factory_Hik.Get_Devices());
			Infos.AddRange(_Idriver_factory_Basler.Get_Devices());

			_Ilogger.Info("GetDeviceInfos");

			return Infos;
		}

		public CInfo_Camera_Para						Sub_GetParameterInfo()
		{
			if (_Idriver_camera != null && _Idriver_camera.Get_Connected())
				return _Idriver_camera.GetParameterInfo();

			return null;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////

		public	bool	Sub_Connect(CInfo_Camera_Factory info)
		{
			Sub_Disconnect();

			switch(info.m_temaker)
			{
				case EMAKER.eHik:
					if (_Idriver_factory_Hik.Get_Exist(info) == false)
						return false;

					_Idriver_camera = _Idriver_factory_Hik.Set_Connect(info);

					break;
				case EMAKER.eBasler:
					if (_Idriver_factory_Basler.Get_Exist(info) == false)
						return false;

					_Idriver_camera = _Idriver_factory_Basler.Set_Connect(info);

					break;
			}

			_Idriver_camera.ImageGrabbed = _Sub_Set_Grabbed;
			_Ilogger.Info("Connect");

			return true;
		}

		public	void	Sub_Disconnect()
		{
			if (_Idriver_camera != null)
			{
				_Idriver_camera.Set_Disconnect();
				_Idriver_camera = null;
			}

		}

		public	bool	Sub_IsConnected()
		{
			if (_Idriver_camera != null)
				return _Idriver_camera.Get_Connected();

			return false;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////

		private	void	_Sub_Set_Grabbed(tsInfo_Grab tsInfoGrab)
		{
			if (_bgrabbing)
			{
				_tsinfo_grab = tsInfoGrab;
				_bgrabbing = false;
			}

			if (m_dlImageGrabbed != null)
				m_dlImageGrabbed(tsInfoGrab);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////


		public	async Task<tsInfo_Grab?> Grab()
		{
			try
			{
				if (_bgrabbing)
					return null;

				_bgrabbing = true;
				if (_Idriver_camera.Set_StartGrab(1) == false)
				{
					Sub_Stop();
					return null;
				}

				while (_bgrabbing)
					await Task.Delay(1);

				return _tsinfo_grab;
			}
			catch (Exception e)
			{
				_bgrabbing = false;
				return null;
			}
		}

		public	bool	Sub_StartGrab(int grabCount = -1)
		{
			if (_Idriver_camera != null)
				return _Idriver_camera.Set_StartGrab(grabCount);

			return false;
		}

		public	bool	Sub_Stop()
		{
			_bgrabbing = false;

			if (_Idriver_camera != null)
				return _Idriver_camera.Set_Stop();

			return false;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////

		public bool		Set_Parameter(ECAM_PARA tsECam_Para, double value)
		{
			if (_Idriver_camera != null)
			{
				if (_Idriver_camera.Set_Parameter(tsECam_Para, value))
				{
					if (m_dlParameterChanged != null)
						m_dlParameterChanged(Sub_GetParameterInfo());

					return true;
				}
			}


			return false;
		}

		public bool		Set_TriggerMode(bool isTriggerMode)
		{
			if (_Idriver_camera != null)
				return _Idriver_camera.Set_TriggerMode(isTriggerMode);

			return false;
		}

		public bool		Set_Auto(ECAM_AUTO_TYPE tsECam_Type, ECAM_AUTO_VALUE tsEcam_Value)
		{
			if (_Idriver_camera != null)
				return _Idriver_camera.Set_Auto(tsECam_Type, tsEcam_Value);

			return false;
		}

		public bool		Set_ROI(uint x, uint y, uint width, uint height)
		{
			if (_Idriver_camera != null)
				return _Idriver_camera.Set_ROI(x, y, width, height);

			return false;
		}
		////////////////////////////////////////////////////////////////////////////////////////////////
	}
}
