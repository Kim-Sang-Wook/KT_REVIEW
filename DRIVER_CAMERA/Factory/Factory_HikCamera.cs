using System.Collections.Generic;
using MvCamCtrl.NET;
using System;
using System.Runtime.InteropServices;
using DRIVER_CAMERA.Control;
using DRIVER_CAMERA.Infos;

namespace DRIVER_CAMERA
{
	public class Factory_HikCamera: IDriver_Camera_Factory
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public bool						Get_Exist(CInfo_Camera_Factory CameraInfo)
		{
			if (CameraInfo.m_temaker != EMAKER.eHik)
				return false;

			MyCamera.MV_CC_DEVICE_INFO stDevInfo;
			return GetHikDeviceInfo(CameraInfo, out stDevInfo);
		}

		public IEnumerable<CInfo_Camera_Factory> Get_Devices()
		{
			var cameraInfos = new List<CInfo_Camera_Factory>();

			var stDevList = new MyCamera.MV_CC_DEVICE_INFO_LIST();

			if (MyCamera.MV_OK != MyCamera.MV_CC_EnumDevices_NET(MyCamera.MV_GIGE_DEVICE | MyCamera.MV_USB_DEVICE, ref stDevList))
				return cameraInfos;

			for (Int32 i = 0; i < stDevList.nDeviceNum; i++)
			{
				var stDevInfo = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(stDevList.pDeviceInfo[i], typeof(MyCamera.MV_CC_DEVICE_INFO));

				if (MyCamera.MV_GIGE_DEVICE == stDevInfo.nTLayerType)
				{
					var stGigEDeviceInfo = (MyCamera.MV_GIGE_DEVICE_INFO)MyCamera.ByteToStruct(stDevInfo.SpecialInfo.stGigEInfo, typeof(MyCamera.MV_GIGE_DEVICE_INFO));
					if (stGigEDeviceInfo.chManufacturerName == "Hikvision")
						cameraInfos.Add(new CInfo_Camera_Factory(EMAKER.eHik, EINTERFACE.eGIGE, stGigEDeviceInfo.chModelName, stGigEDeviceInfo.chSerialNumber));
				}
				else if (MyCamera.MV_USB_DEVICE == stDevInfo.nTLayerType)
				{
					var stUsb3DeviceInfo = (MyCamera.MV_USB3_DEVICE_INFO)MyCamera.ByteToStruct(stDevInfo.SpecialInfo.stUsb3VInfo, typeof(MyCamera.MV_USB3_DEVICE_INFO));
					if (stUsb3DeviceInfo.chManufacturerName == "Hikvision")
						cameraInfos.Add(new CInfo_Camera_Factory(EMAKER.eHik, EINTERFACE.eUSB, stUsb3DeviceInfo.chModelName, stUsb3DeviceInfo.chSerialNumber));
				}
			}

			return cameraInfos;
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public IDriver_Camera_Control	Set_Connect(CInfo_Camera_Factory CameraInfo)
		{
			if (Get_Exist(CameraInfo) == false)
				return null;

			MyCamera.MV_CC_DEVICE_INFO stDevInfo;
			if (GetHikDeviceInfo(CameraInfo, out stDevInfo) == false)
				return null;

			var device = new MyCamera();

			if (MyCamera.MV_OK != device.MV_CC_CreateDevice_NET(ref stDevInfo))
				return null;


			if (MyCamera.MV_OK != device.MV_CC_OpenDevice_NET())
				return null;

			if (CameraInfo.m_teinterface == EINTERFACE.eGIGE)
			{
				int nPacketSize = device.MV_CC_GetOptimalPacketSize_NET();
				if (nPacketSize > 0)
				{
					if (MyCamera.MV_OK != device.MV_CC_SetIntValue_NET("GevSCPSPacketSize", (uint)nPacketSize))
						return null;
				}
				else
				{
					return null;
				}
			}

			var info = new MyCamera.MV_IMAGE_BASIC_INFO();

			if (MyCamera.MV_OK != device.MV_CC_GetImageInfo_NET(ref info))
				return null;

			var camera = new Control_HikCamera(device, info);

			if (MyCamera.MV_OK != device.MV_CC_RegisterImageCallBackEx_NET(Control_HikCamera.ImageCallback, GCHandle.ToIntPtr(camera.Handle)))
				return null;

			return camera;
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static bool				GetHikDeviceInfo(CInfo_Camera_Factory CameraInfo, out MyCamera.MV_CC_DEVICE_INFO stDevInfo)
		{
			stDevInfo = new MyCamera.MV_CC_DEVICE_INFO();

			var stDevList = new MyCamera.MV_CC_DEVICE_INFO_LIST();
			var nRet = MyCamera.MV_CC_EnumDevices_NET(MyCamera.MV_GIGE_DEVICE | MyCamera.MV_USB_DEVICE, ref stDevList);
			if (MyCamera.MV_OK != nRet)
				return false;

			for (Int32 i = 0; i < stDevList.nDeviceNum; i++)
			{
				stDevInfo = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(stDevList.pDeviceInfo[i], typeof(MyCamera.MV_CC_DEVICE_INFO));

				switch (CameraInfo.m_teinterface)
				{
					case EINTERFACE.eUSB:
						if (MyCamera.MV_USB_DEVICE == stDevInfo.nTLayerType)
						{
							var stUsb3DeviceInfo = (MyCamera.MV_USB3_DEVICE_INFO)MyCamera.ByteToStruct(stDevInfo.SpecialInfo.stUsb3VInfo, typeof(MyCamera.MV_USB3_DEVICE_INFO));

							if (CameraInfo.m_strmodel == stUsb3DeviceInfo.chModelName
								&& CameraInfo.m_strserial == stUsb3DeviceInfo.chSerialNumber)
							{
								return true;
							}
						}

						break;
					case EINTERFACE.eGIGE:
						if (MyCamera.MV_GIGE_DEVICE == stDevInfo.nTLayerType)
						{
							var stGigEDeviceInfo = (MyCamera.MV_GIGE_DEVICE_INFO)MyCamera.ByteToStruct(stDevInfo.SpecialInfo.stGigEInfo, typeof(MyCamera.MV_GIGE_DEVICE_INFO));

							if (CameraInfo.m_strmodel == stGigEDeviceInfo.chModelName
								&& CameraInfo.m_strserial == stGigEDeviceInfo.chSerialNumber)
							{
								return true;
							}
						}

						break;
				}
			}

			return false;
		}
		
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	}
}
