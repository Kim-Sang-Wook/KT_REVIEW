using DRIVER_CAMERA.Infos;
using System.Collections.Generic;


namespace DRIVER_CAMERA
{
	public class Driver_Camera_Factory
	{
		private static Driver_Camera_Factory _instance;
		public static Driver_Camera_Factory Instance
		{
			get
			{
				if (_instance == null) _instance = new Driver_Camera_Factory();
				return _instance;
			}
		}

		public IDriver_Camera_Factory		Get_Camera_Factory(EMAKER maker)
		{
			switch(maker)
			{
				case EMAKER.eHik:
					return new Factory_HikCamera();
				case EMAKER.eBasler:
					return new Factory_BaslerCamera();
			}

			return null;
		}
	}

	public interface IDriver_Camera_Factory
	{
		bool								Get_Exist(CInfo_Camera_Factory info_camera);
		IEnumerable<CInfo_Camera_Factory>	Get_Devices();

		IDriver_Camera_Control				Set_Connect(CInfo_Camera_Factory cameraInfo);
	}
}
