using DRIVER_CAMERA.Control;
using DRIVER_CAMERA.Infos;
using System.Collections.Generic;
using System.Linq;

namespace DRIVER_CAMERA
{
	public class Factory_BaslerCamera : IDriver_Camera_Factory
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public bool						Get_Exist(CInfo_Camera_Factory CameraInfo)
		{
			if (CameraInfo.m_temaker != EMAKER.eBasler)
				return false;

			var infos = Get_Devices();
			return infos.Any(i
				=> i.m_temaker		== CameraInfo.m_temaker
				&& i.m_teinterface	== CameraInfo.m_teinterface
				&& i.m_strmodel		== CameraInfo.m_strmodel
				&& i.m_strserial	== CameraInfo.m_strserial);
		}

		public IEnumerable<CInfo_Camera_Factory> Get_Devices()
		{
			List<Basler.Pylon.ICameraInfo> baslerCameraInfos = Basler.Pylon.CameraFinder.Enumerate();
			List<CInfo_Camera_Factory> cameraInfos = new List<CInfo_Camera_Factory>();
			foreach (var info in baslerCameraInfos)
			{
				var type = info[Basler.Pylon.CameraInfoKey.DeviceType];
				if (type == "BaslerUsb")
				{
					cameraInfos.Add(new CInfo_Camera_Factory(EMAKER.eBasler,EINTERFACE.eUSB,
															 info[Basler.Pylon.CameraInfoKey.ModelName],
															 info[Basler.Pylon.CameraInfoKey.SerialNumber]));
				}
				else if (type == "BaslerGigE")
				{
					cameraInfos.Add(new CInfo_Camera_Factory(EMAKER.eBasler,EINTERFACE.eGIGE,
															 info[Basler.Pylon.CameraInfoKey.ModelName],
															 info[Basler.Pylon.CameraInfoKey.SerialNumber]));
				}
			}

			return cameraInfos;
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public IDriver_Camera_Control	Set_Connect(CInfo_Camera_Factory cameraInfo)
		{
			var baslerInfo = GetBaslerInfo(cameraInfo);

			if (baslerInfo == null)
				return null;
			
			return new Control_BaslerCamera(baslerInfo);
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		private Basler.Pylon.ICameraInfo GetBaslerInfo(CInfo_Camera_Factory info)
		{
			if (info.m_temaker != EMAKER.eBasler)
				return null;

			List<Basler.Pylon.ICameraInfo> cameraInfos = Basler.Pylon.CameraFinder.Enumerate();
			foreach (var baslerInfo in cameraInfos)
			{
				var type = baslerInfo[Basler.Pylon.CameraInfoKey.DeviceType];
				switch (info.m_teinterface)
				{
					case EINTERFACE.eUSB:
						if (type == "BaslerUsb"
							&& baslerInfo[Basler.Pylon.CameraInfoKey.ModelName] == info.m_strmodel
							&& baslerInfo[Basler.Pylon.CameraInfoKey.SerialNumber] == info.m_strserial)
							return baslerInfo;

						break;
					case EINTERFACE.eGIGE:
						if (type == "BaslerGigE"
							&& baslerInfo[Basler.Pylon.CameraInfoKey.ModelName] == info.m_strmodel
							&& baslerInfo[Basler.Pylon.CameraInfoKey.SerialNumber] == info.m_strserial)
							return baslerInfo;

						break;
				}
			}

			return null;
		}
		
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	}
}
