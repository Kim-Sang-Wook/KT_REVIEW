using System;
using System.Collections.Generic;
using Basler.Pylon;
using DRIVER_CAMERA.Infos;

namespace DRIVER_CAMERA
{
	public class Control_BaslerCamera : IDriver_Camera_Control
	{
		#region Define Variable
		private int					_ngrabCount, _ncount;
		private ICamera				_camera;
		private PixelDataConverter	_converter;
		#endregion

		public Control_BaslerCamera(ICameraInfo cameraInfo)
		{
			_ngrabCount = -1;
			_ncount = 0;

			_camera = new Camera(cameraInfo);
			_camera.StreamGrabber.ImageGrabbed += StreamGrabber_ImageGrabbed;

			_camera.Open();

			_converter = new Basler.Pylon.PixelDataConverter();
			_converter.OutputPixelFormat = Basler.Pylon.PixelType.RGB8packed;
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public bool Get_Connected()
		{
			if (_camera != null)
				return _camera.IsConnected;

			return false;
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public bool Set_Disconnect()
		{
			Set_Stop();

			if (_camera.IsOpen == false)
				return false;

			_camera.Close();
			_camera.Dispose(); 

			return true;
		}

		public bool Set_Stop()
		{
			if (_camera.StreamGrabber.IsGrabbing)
			{
				_camera.StreamGrabber.Stop();
				return true;
			}

			return false;
		}

		public bool Set_StartGrab(int nGrabCount = -1)
		{
			_ngrabCount = nGrabCount;
			_ncount = 0;

			if (_camera.IsOpen == false
				|| _camera.IsConnected == false
				|| _camera.StreamGrabber.IsGrabbing == true)
				return false;

			_camera.StreamGrabber.Start(GrabStrategy.OneByOne, GrabLoop.ProvidedByStreamGrabber);

			return true;
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public bool Set_Auto(ECAM_AUTO_TYPE type, ECAM_AUTO_VALUE value)
		{
			switch (type)
			{
				case ECAM_AUTO_TYPE.eExposure:
					switch (value)
					{
						case ECAM_AUTO_VALUE.Off:
							return _camera.Parameters[PLCamera.ExposureAuto].TrySetValue(PLCamera.ExposureAuto.Off);
						case ECAM_AUTO_VALUE.Once:
							return _camera.Parameters[PLCamera.ExposureAuto].TrySetValue(PLCamera.ExposureAuto.Once);
						case ECAM_AUTO_VALUE.Continuous:
							return _camera.Parameters[PLCamera.ExposureAuto].TrySetValue(PLCamera.ExposureAuto.Continuous);
					}

					break;
				case ECAM_AUTO_TYPE.eGain:
					switch (value)
					{
						case ECAM_AUTO_VALUE.Off:
							return _camera.Parameters[PLCamera.GainAuto].TrySetValue(PLCamera.GainAuto.Off);
						case ECAM_AUTO_VALUE.Once:
							return _camera.Parameters[PLCamera.GainAuto].TrySetValue(PLCamera.GainAuto.Once);
						case ECAM_AUTO_VALUE.Continuous:
							return _camera.Parameters[PLCamera.GainAuto].TrySetValue(PLCamera.GainAuto.Continuous);
					}
					break;
				case ECAM_AUTO_TYPE.eWhiteBalance:
					switch (value)
					{
						case ECAM_AUTO_VALUE.Off:
							return _camera.Parameters[PLCamera.BalanceWhiteAuto].TrySetValue(PLCamera.BalanceWhiteAuto.Off);
						case ECAM_AUTO_VALUE.Once:
							return _camera.Parameters[PLCamera.BalanceWhiteAuto].TrySetValue(PLCamera.BalanceWhiteAuto.Once);
						case ECAM_AUTO_VALUE.Continuous:
							return _camera.Parameters[PLCamera.BalanceWhiteAuto].TrySetValue(PLCamera.BalanceWhiteAuto.Continuous);
					}
					break;
			}

			return false;
		}

		public bool Set_Parameter(ECAM_PARA parameter, double value)
		{
			switch (parameter)
			{
				case ECAM_PARA.eOffsetX:
					return _camera.Parameters[PLCamera.OffsetX].TrySetValue((long)Math.Round(value));
				case ECAM_PARA.eOffsetY:
					return _camera.Parameters[PLCamera.OffsetY].TrySetValue((long)Math.Round(value));
				case ECAM_PARA.eWidth:
					return _camera.Parameters[PLCamera.Width].TrySetValue((long)Math.Round(value));
				case ECAM_PARA.eHeight:
					return _camera.Parameters[PLCamera.Height].TrySetValue((long)Math.Round(value));
				case ECAM_PARA.eExposure:
					return _camera.Parameters[PLCamera.ExposureTime].TrySetValue(value);
				case ECAM_PARA.eGain:
					return _camera.Parameters[PLCamera.Gain].TrySetValue(value);
				case ECAM_PARA.eFrameRate:
					return _camera.Parameters[PLCamera.AcquisitionFrameRate].TrySetValue(value);
				case ECAM_PARA.eTriggerDelay:
					return _camera.Parameters[PLCamera.TriggerDelay].TrySetValue(value);
			}

			return false;
		}

		public bool Set_ROI(uint x, uint y, uint width, uint height)
		{
			if (_camera.Parameters[PLCamera.OffsetX].TrySetValue(x) == false)
				return false;

			if (_camera.Parameters[PLCamera.OffsetY].TrySetValue(y) == false)
				return false;

			if (_camera.Parameters[PLCamera.Width].TrySetValue(width) == false)
				return false;

			if (_camera.Parameters[PLCamera.Height].TrySetValue(height) == false)
				return false;

			return true;
		}

		public bool Set_TriggerMode(bool isTriggerMode)
		{
			if (isTriggerMode)
				return _camera.Parameters[PLCamera.TriggerMode].TrySetValue(PLCamera.TriggerMode.On);
			else
				return _camera.Parameters[PLCamera.TriggerMode].TrySetValue(PLCamera.TriggerMode.Off);
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public Action<tsInfo_Grab>	ImageGrabbed { get; set; }

		private void				StreamGrabber_ImageGrabbed(object sender, ImageGrabbedEventArgs e)
		{
			if (_ngrabCount > 0)
			{
				_ncount++;

				if (_ncount >= _ngrabCount)
					Set_Stop();
			}

			IGrabResult result = e.GrabResult;

			if (result.GrabSucceeded)
			{
				if (result.PixelTypeValue.IsMonoImage())
				{
					var src = result.PixelData as byte[];
					var data = new byte[src.Length];
					Array.Copy(src, data, src.Length);
					if (ImageGrabbed != null)
					{
						ImageGrabbed(new tsInfo_Grab(EGRAB_RESULT.eSuccess, result.Width, result.Height, 1, data));
					}

					return;
				}
				else
				{
					var data = new byte[result.Width * result.Height * 3];
					_converter.Convert(data, result);

					if (ImageGrabbed != null)
					{
						ImageGrabbed(new tsInfo_Grab(EGRAB_RESULT.eSuccess, result.Width, result.Height, 3, data));
					}

					return;
				}
			}

			if (ImageGrabbed != null)
				ImageGrabbed(new tsInfo_Grab(EGRAB_RESULT.eError));
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		public CInfo_Camera_Para							GetParameterInfo()
		{
			return new CInfo_Camera_Para(	_camera.Parameters[PLCamera.TriggerMode].GetValue() == PLCamera.TriggerMode.On,
											GetParameterDictionary(),
											GetAutoValueDictionary());
		}

		private IDictionary<ECAM_PARA, tsCamera_Para_item>	GetParameterDictionary()
		{
			var dictionary = new Dictionary<ECAM_PARA, tsCamera_Para_item>();

			dictionary[ECAM_PARA.eWidth] = new tsCamera_Para_item(	_camera.Parameters[PLCamera.Width].GetValue(),
																	_camera.Parameters[PLCamera.Width].GetMinimum(),
																	_camera.Parameters[PLCamera.Width].GetMaximum());

			dictionary[ECAM_PARA.eHeight] = new tsCamera_Para_item(	_camera.Parameters[PLCamera.Height].GetValue(),
																	_camera.Parameters[PLCamera.Height].GetMinimum(),
																	_camera.Parameters[PLCamera.Height].GetMaximum());

			dictionary[ECAM_PARA.eOffsetX] = new tsCamera_Para_item(_camera.Parameters[PLCamera.OffsetX].GetValue(),
																	_camera.Parameters[PLCamera.OffsetX].GetMinimum(),
																	_camera.Parameters[PLCamera.OffsetX].GetMaximum());

			dictionary[ECAM_PARA.eOffsetY] = new tsCamera_Para_item(_camera.Parameters[PLCamera.OffsetY].GetValue(),
																	_camera.Parameters[PLCamera.OffsetY].GetMinimum(),
																	_camera.Parameters[PLCamera.OffsetY].GetMaximum());

			dictionary[ECAM_PARA.eExposure] = new tsCamera_Para_item(_camera.Parameters[PLCamera.ExposureTime].GetValue(),
																	_camera.Parameters[PLCamera.ExposureTime].GetMinimum(),
																	_camera.Parameters[PLCamera.ExposureTime].GetMaximum());

			dictionary[ECAM_PARA.eGain] = new tsCamera_Para_item(	_camera.Parameters[PLCamera.Gain].GetValue(),
																	_camera.Parameters[PLCamera.Gain].GetMinimum(),
																	_camera.Parameters[PLCamera.Gain].GetMaximum());

			dictionary[ECAM_PARA.eFrameRate] = new tsCamera_Para_item(_camera.Parameters[PLCamera.AcquisitionFrameRate].GetValue(),
																	_camera.Parameters[PLCamera.AcquisitionFrameRate].GetMinimum(),
																	_camera.Parameters[PLCamera.AcquisitionFrameRate].GetMaximum());

			dictionary[ECAM_PARA.eTriggerDelay] = new tsCamera_Para_item(_camera.Parameters[PLCamera.TriggerDelay].GetValue(),
																		_camera.Parameters[PLCamera.TriggerDelay].GetMinimum(),
																		_camera.Parameters[PLCamera.TriggerDelay].GetMaximum());

			return dictionary;
		}

		private IDictionary<ECAM_AUTO_TYPE, ECAM_AUTO_VALUE>	GetAutoValueDictionary()
		{
			var dictionary = new Dictionary<ECAM_AUTO_TYPE, ECAM_AUTO_VALUE>();

			var exposureAuto = _camera.Parameters[PLCamera.ExposureAuto].GetValue();

			if		(exposureAuto == PLCamera.ExposureAuto.Once)
				dictionary[ECAM_AUTO_TYPE.eExposure] = ECAM_AUTO_VALUE.Once;
			else if (exposureAuto == PLCamera.ExposureAuto.Continuous)
				dictionary[ECAM_AUTO_TYPE.eExposure] = ECAM_AUTO_VALUE.Continuous;
			else
				dictionary[ECAM_AUTO_TYPE.eExposure] = ECAM_AUTO_VALUE.Off;


			var gainAuto = _camera.Parameters[PLCamera.GainAuto].GetValue();

			if		(gainAuto == PLCamera.GainAuto.Once)
				dictionary[ECAM_AUTO_TYPE.eGain] = ECAM_AUTO_VALUE.Once;
			else if (gainAuto == PLCamera.GainAuto.Continuous)
				dictionary[ECAM_AUTO_TYPE.eGain] = ECAM_AUTO_VALUE.Continuous;
			else
				dictionary[ECAM_AUTO_TYPE.eGain] = ECAM_AUTO_VALUE.Off;


			var whiteBalanceAuto = _camera.Parameters[PLCamera.BalanceWhiteAuto].GetValue();

			if		(gainAuto == PLCamera.BalanceWhiteAuto.Once)
				dictionary[ECAM_AUTO_TYPE.eWhiteBalance] = ECAM_AUTO_VALUE.Once;
			else if (gainAuto == PLCamera.BalanceWhiteAuto.Continuous)
				dictionary[ECAM_AUTO_TYPE.eWhiteBalance] = ECAM_AUTO_VALUE.Continuous;
			else
				dictionary[ECAM_AUTO_TYPE.eWhiteBalance] = ECAM_AUTO_VALUE.Off;

			return dictionary;
		}
	}
}