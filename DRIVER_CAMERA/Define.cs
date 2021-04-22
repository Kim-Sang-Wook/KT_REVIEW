namespace DRIVER_CAMERA
{
	public enum EMAKER
	{
		eHik, eBasler
	}

	public enum EINTERFACE
	{
		eUSB, eGIGE
	}

	public enum ECAM_PARA
	{
		eWidth, eHeight, eOffsetX, eOffsetY, eExposure, eGain, eFrameRate, eTriggerDelay
	}

	public enum ECAM_AUTO_TYPE
	{
		eExposure, eGain, eWhiteBalance
	}

	public enum ECAM_AUTO_VALUE
	{
		Off, Once, Continuous
	}

	public enum EGRAB_RESULT
	{
		eSuccess, eError, eTimeout, eNotConnected
	}

	public enum ESAVE_MODE
	{
		All, None, OK, NG
	}

}
