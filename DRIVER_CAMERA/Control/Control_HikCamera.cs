using DRIVER_CAMERA.Infos;
using MvCamCtrl.NET;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DRIVER_CAMERA.Control
{
	public class Control_HikCamera : IDriver_Camera_Control
	{
		#region Define Variable
		private int _ngrabCount, _ncount;

		private MyCamera							_device;
		private MyCamera.MV_IMAGE_BASIC_INFO		_info;

		private	GCHandle							_handle;
		public GCHandle								Handle
		{ get { return _handle; } }

		private static MyCamera.cbOutputExdelegate	_imageCallback;
		public static MyCamera.cbOutputExdelegate	ImageCallback { get { return _imageCallback; } }

		#endregion

		public Control_HikCamera(MyCamera device, MyCamera.MV_IMAGE_BASIC_INFO info)
		{
			_imageCallback	= new MyCamera.cbOutputExdelegate(Set_ImageCallback);
			_handle			= GCHandle.Alloc(this);

			_device			= device;
			_info			= info;

			_ngrabCount		= -1;
			_ncount			= 0;
		}

		~Control_HikCamera()
		{
			Handle.Free();
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public bool Get_Connected()
		{
			if (_device != null)
				return _device.MV_CC_IsDeviceConnected_NET();

			return false;
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public bool Set_Disconnect()
		{
			Set_Stop();

			if (MyCamera.MV_OK != _device.MV_CC_CloseDevice_NET())
				return false;

			if (MyCamera.MV_OK != _device.MV_CC_DestroyDevice_NET())
				return false;

			return true;
		}

		public bool Set_Stop()
		{
			if (MyCamera.MV_OK != _device.MV_CC_StopGrabbing_NET())
				return false;

			return true;
		}

		public bool Set_StartGrab(int nGrabCount = -1)
		{
			_ncount = 0;
			_ngrabCount = nGrabCount;

			if (MyCamera.MV_OK != _device.MV_CC_StartGrabbing_NET())
				return false;

			return true;
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public bool Set_Parameter(ECAM_PARA parameter, double value)
		{
			int nRet = 0;
			switch (parameter)
			{
				case ECAM_PARA.eOffsetX:
					nRet = _device.MV_CC_SetAOIoffsetX_NET((uint)Math.Round(value));
					break;
				case ECAM_PARA.eOffsetY:
					nRet = _device.MV_CC_SetAOIoffsetY_NET((uint)Math.Round(value));
					break;
				case ECAM_PARA.eWidth:
					nRet = _device.MV_CC_SetWidth_NET((uint)Math.Round(value));
					break;
				case ECAM_PARA.eHeight:
					nRet = _device.MV_CC_SetHeight_NET((uint)Math.Round(value));
					break;
				case ECAM_PARA.eExposure:
					nRet = _device.MV_CC_SetExposureTime_NET((float)value);
					break;
				case ECAM_PARA.eGain:
					nRet = _device.MV_CC_SetGain_NET((float)value);
					break;
				case ECAM_PARA.eFrameRate:
					nRet = _device.MV_CC_SetFrameRate_NET((float)value);
					break;
				case ECAM_PARA.eTriggerDelay:
					nRet = _device.MV_CC_SetTriggerDelay_NET((float)value);
					break;
			}

			return MyCamera.MV_OK == nRet;
		}

		public bool Set_TriggerMode(bool isTriggerMode)
		{
			if (isTriggerMode)
				return MyCamera.MV_OK == _device.MV_CC_SetTriggerMode_NET((uint)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON);
			else
				return MyCamera.MV_OK == _device.MV_CC_SetTriggerMode_NET((uint)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF);
		}

		public bool Set_Auto(ECAM_AUTO_TYPE type, ECAM_AUTO_VALUE value)
		{
			int nRet = 0;
			switch (type)
			{
				case ECAM_AUTO_TYPE.eExposure:
					switch (value)
					{
						case ECAM_AUTO_VALUE.Off:
							nRet = _device.MV_CC_SetExposureAutoMode_NET(
								(uint)MyCamera.MV_CAM_EXPOSURE_AUTO_MODE.MV_EXPOSURE_AUTO_MODE_OFF);
							break;
						case ECAM_AUTO_VALUE.Once:
							nRet = _device.MV_CC_SetExposureAutoMode_NET(
								(uint)MyCamera.MV_CAM_EXPOSURE_AUTO_MODE.MV_EXPOSURE_AUTO_MODE_ONCE);
							break;
						case ECAM_AUTO_VALUE.Continuous:
							nRet = _device.MV_CC_SetExposureAutoMode_NET(
								(uint)MyCamera.MV_CAM_EXPOSURE_AUTO_MODE.MV_EXPOSURE_AUTO_MODE_CONTINUOUS);
							break;
					}
					break;
				case ECAM_AUTO_TYPE.eGain:
					switch (value)
					{
						case ECAM_AUTO_VALUE.Off:
							nRet = _device.MV_CC_SetGainMode_NET(
								(uint)MyCamera.MV_CAM_GAIN_MODE.MV_GAIN_MODE_OFF);
							break;
						case ECAM_AUTO_VALUE.Once:
							nRet = _device.MV_CC_SetGainMode_NET(
								(uint)MyCamera.MV_CAM_GAIN_MODE.MV_GAIN_MODE_ONCE);
							break;
						case ECAM_AUTO_VALUE.Continuous:
							nRet = _device.MV_CC_SetGainMode_NET(
								(uint)MyCamera.MV_CAM_GAIN_MODE.MV_GAIN_MODE_CONTINUOUS);
							break;
					}
					break;
				case ECAM_AUTO_TYPE.eWhiteBalance:
					switch (value)
					{
						case ECAM_AUTO_VALUE.Off:
							nRet = _device.MV_CC_SetBalanceWhiteAuto_NET(
								(uint)MyCamera.MV_CAM_BALANCEWHITE_AUTO.MV_BALANCEWHITE_AUTO_OFF);
							break;
						case ECAM_AUTO_VALUE.Once:
							nRet = _device.MV_CC_SetBalanceWhiteAuto_NET(
								(uint)MyCamera.MV_CAM_BALANCEWHITE_AUTO.MV_BALANCEWHITE_AUTO_ONCE);
							break;
						case ECAM_AUTO_VALUE.Continuous:
							nRet = _device.MV_CC_SetBalanceWhiteAuto_NET(
								(uint)MyCamera.MV_CAM_BALANCEWHITE_AUTO.MV_BALANCEWHITE_AUTO_CONTINUOUS);
							break;
					}

					break;
			}

			return MyCamera.MV_OK == nRet;
		}

		public bool Set_ROI(uint x, uint y, uint width, uint height)
		{
			if (MyCamera.MV_OK != _device.MV_CC_SetAOIoffsetX_NET(x))
				return false;

			if (MyCamera.MV_OK != _device.MV_CC_SetAOIoffsetY_NET(y))
				return false;

			if (MyCamera.MV_OK != _device.MV_CC_SetWidth_NET(width))
				return false;

			if (MyCamera.MV_OK != _device.MV_CC_SetHeight_NET(height))
				return false;

			return true;
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public Action<tsInfo_Grab>	ImageGrabbed { get; set; }

		private static void			Set_ImageCallback(IntPtr pData, ref MyCamera.MV_FRAME_OUT_INFO_EX pFrameInfo, IntPtr pUser)
		{
			var camera = (Control_HikCamera)((GCHandle)pUser).Target;

			if (camera._ngrabCount > 0)
			{
				camera._ncount++;
				if (camera._ngrabCount >= camera._ncount)
					camera.Set_Stop();
			}

			if (camera.ImageGrabbed != null)
				camera.ImageGrabbed(ConvertImage(pFrameInfo, pData, camera._device));
		}

		private static tsInfo_Grab	ConvertImage(MyCamera.MV_FRAME_OUT_INFO_EX frameInfo, IntPtr pData, MyCamera device)
		{
			uint channel = frameInfo.nFrameLen / frameInfo.nWidth / frameInfo.nHeight;

			if (channel == 1)
			{
				var data = new byte[frameInfo.nFrameLen];
				Marshal.Copy(pData, data, 0, (int)frameInfo.nFrameLen);
				return new tsInfo_Grab(EGRAB_RESULT.eSuccess, frameInfo.nWidth, frameInfo.nHeight, 1, data);
			}
			else
			{
				var data = new byte[frameInfo.nWidth * frameInfo.nHeight * 3];

				var handle = GCHandle.Alloc(data, GCHandleType.Pinned);

				var stConverPixelParam = new MyCamera.MV_PIXEL_CONVERT_PARAM();
				stConverPixelParam.nWidth = frameInfo.nWidth;
				stConverPixelParam.nHeight = frameInfo.nHeight;
				stConverPixelParam.pSrcData = pData;
				stConverPixelParam.nSrcDataLen = frameInfo.nFrameLen;
				stConverPixelParam.enSrcPixelType = frameInfo.enPixelType;
				stConverPixelParam.enDstPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed;
				stConverPixelParam.pDstBuffer = handle.AddrOfPinnedObject();
				stConverPixelParam.nDstBufferSize = (uint)(frameInfo.nWidth * frameInfo.nHeight * 3);

				if (MyCamera.MV_OK != device.MV_CC_ConvertPixelType_NET(ref stConverPixelParam))
				{
					handle.Free();
					return new tsInfo_Grab(EGRAB_RESULT.eError);
				}

				handle.Free();

				return new tsInfo_Grab(EGRAB_RESULT.eSuccess, frameInfo.nWidth, frameInfo.nHeight, 3, data);
			}
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public CInfo_Camera_Para							GetParameterInfo()
		{
			var triggerMode = new MyCamera.MVCC_ENUMVALUE();
			_device.MV_CC_GetTriggerMode_NET(ref triggerMode);

			return new CInfo_Camera_Para(triggerMode.nCurValue == (uint)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON,
										GetParameterDictionary(),
										GetAutoValueDictionary());
		}

		private IDictionary<ECAM_PARA, tsCamera_Para_item>	GetParameterDictionary()
		{
			var dictionary		= new Dictionary<ECAM_PARA, tsCamera_Para_item>();
			var width			= new MyCamera.MVCC_INTVALUE();
			var height			= new MyCamera.MVCC_INTVALUE();
			var offsetX			= new MyCamera.MVCC_INTVALUE();
			var offsetY			= new MyCamera.MVCC_INTVALUE();
			var exposure		= new MyCamera.MVCC_FLOATVALUE();
			var gain			= new MyCamera.MVCC_FLOATVALUE();
			var frameRate		= new MyCamera.MVCC_FLOATVALUE();
			var triggerDelay	= new MyCamera.MVCC_FLOATVALUE();

			_device.MV_CC_GetWidth_NET(ref width);
			_device.MV_CC_GetHeight_NET(ref height);
			_device.MV_CC_GetAOIoffsetX_NET(ref offsetX);
			_device.MV_CC_GetAOIoffsetY_NET(ref offsetY);
			_device.MV_CC_GetExposureTime_NET(ref exposure);
			_device.MV_CC_GetGain_NET(ref gain);
			_device.MV_CC_GetFrameRate_NET(ref frameRate);
			_device.MV_CC_GetTriggerDelay_NET(ref triggerDelay);

			dictionary[ECAM_PARA.eWidth]		= new tsCamera_Para_item(width.nCurValue, width.nMin, width.nMax);
			dictionary[ECAM_PARA.eHeight]		= new tsCamera_Para_item(height.nCurValue, height.nMin, height.nMax);
			dictionary[ECAM_PARA.eOffsetX]		= new tsCamera_Para_item(offsetX.nCurValue, offsetX.nMin, offsetX.nMax);
			dictionary[ECAM_PARA.eOffsetY]		= new tsCamera_Para_item(offsetY.nCurValue, offsetY.nMin, offsetY.nMax);
			dictionary[ECAM_PARA.eExposure]		= new tsCamera_Para_item(exposure.fCurValue, exposure.fMin, exposure.fMax);
			dictionary[ECAM_PARA.eGain]			= new tsCamera_Para_item(gain.fCurValue, gain.fMin, gain.fMax);
			dictionary[ECAM_PARA.eFrameRate]	= new tsCamera_Para_item(frameRate.fCurValue, frameRate.fMin, frameRate.fMax);
			dictionary[ECAM_PARA.eTriggerDelay]	= new tsCamera_Para_item(triggerDelay.fCurValue, triggerDelay.fMin, triggerDelay.fMax);

			return dictionary;
		}

		private IDictionary<ECAM_AUTO_TYPE, ECAM_AUTO_VALUE>	GetAutoValueDictionary()
		{
			var dictionary			= new Dictionary<ECAM_AUTO_TYPE, ECAM_AUTO_VALUE>();
			var autoExposure		= new MyCamera.MVCC_ENUMVALUE();
			var autoGain			= new MyCamera.MVCC_ENUMVALUE();
			var autoWhiteBalance	= new MyCamera.MVCC_ENUMVALUE();

			_device.MV_CC_GetExposureAutoMode_NET(ref autoExposure);
			_device.MV_CC_GetGainMode_NET(ref autoGain);
			_device.MV_CC_GetBalanceWhiteAuto_NET(ref autoWhiteBalance);

			switch ((MyCamera.MV_CAM_EXPOSURE_AUTO_MODE)autoExposure.nCurValue)
			{
				case MyCamera.MV_CAM_EXPOSURE_AUTO_MODE.MV_EXPOSURE_AUTO_MODE_OFF:
					dictionary[ECAM_AUTO_TYPE.eExposure] = ECAM_AUTO_VALUE.Off;
					break;
				case MyCamera.MV_CAM_EXPOSURE_AUTO_MODE.MV_EXPOSURE_AUTO_MODE_ONCE:
					dictionary[ECAM_AUTO_TYPE.eExposure] = ECAM_AUTO_VALUE.Once;
					break;
				case MyCamera.MV_CAM_EXPOSURE_AUTO_MODE.MV_EXPOSURE_AUTO_MODE_CONTINUOUS:
					dictionary[ECAM_AUTO_TYPE.eExposure] = ECAM_AUTO_VALUE.Continuous;
					break;
			}

			switch ((MyCamera.MV_CAM_GAIN_MODE)autoGain.nCurValue)
			{
				case MyCamera.MV_CAM_GAIN_MODE.MV_GAIN_MODE_OFF:
					dictionary[ECAM_AUTO_TYPE.eGain] = ECAM_AUTO_VALUE.Off;
					break;
				case MyCamera.MV_CAM_GAIN_MODE.MV_GAIN_MODE_ONCE:
					dictionary[ECAM_AUTO_TYPE.eGain] = ECAM_AUTO_VALUE.Once;
					break;
				case MyCamera.MV_CAM_GAIN_MODE.MV_GAIN_MODE_CONTINUOUS:
					dictionary[ECAM_AUTO_TYPE.eGain] = ECAM_AUTO_VALUE.Continuous;
					break;
			}

			switch ((MyCamera.MV_CAM_BALANCEWHITE_AUTO)autoWhiteBalance.nCurValue)
			{
				case MyCamera.MV_CAM_BALANCEWHITE_AUTO.MV_BALANCEWHITE_AUTO_OFF:
					dictionary[ECAM_AUTO_TYPE.eWhiteBalance] = ECAM_AUTO_VALUE.Off;
					break;
				case MyCamera.MV_CAM_BALANCEWHITE_AUTO.MV_BALANCEWHITE_AUTO_ONCE:
					dictionary[ECAM_AUTO_TYPE.eWhiteBalance] = ECAM_AUTO_VALUE.Once;
					break;
				case MyCamera.MV_CAM_BALANCEWHITE_AUTO.MV_BALANCEWHITE_AUTO_CONTINUOUS:
					dictionary[ECAM_AUTO_TYPE.eWhiteBalance] = ECAM_AUTO_VALUE.Continuous;
					break;
			}

			return dictionary;
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	}
}
