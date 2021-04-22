using DRIVER_CAMERA.Infos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRIVER_CAMERA
{
	public interface	IDriver_Camera_Control
	{
		bool Get_Connected();

		bool Set_Disconnect();
		bool Set_Stop();
		bool Set_StartGrab(int grabCount = -1);

		bool Set_Parameter(ECAM_PARA parameter, double value);
		bool Set_TriggerMode(bool isTriggerMode);
		bool Set_Auto(ECAM_AUTO_TYPE type, ECAM_AUTO_VALUE value);
		bool Set_ROI(uint x, uint y, uint width, uint height);

		Action<tsInfo_Grab>	ImageGrabbed { get; set; }
		CInfo_Camera_Para	GetParameterInfo();
	}
}
