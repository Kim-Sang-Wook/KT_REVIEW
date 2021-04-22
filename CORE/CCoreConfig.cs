using DRIVER_CAMERA;
using DRIVER_CAMERA.Infos;
using System.Drawing.Imaging;

namespace CORE
{
	public class CCoreConfig
	{
		public bool m_bUseHost		{ get; set; }
		public bool m_bUseInspect	{ get; set; }

		public int m_nHostPort		{ get; set; }
		public int m_nInspectPort	{ get; set; }
		public int m_nStoreDay		{ get; set; }
		public int m_nLightNum		{ get; set; }

		public string m_strPath_Result	{ get; set; }
		public string m_strPath_Temp	{ get; set; }

		public byte[] m_byLightValues	{ get; set; }


		public ESAVE_MODE teSaveMode	{ get; set; }

		public CInfo_Camera_Factory CInfoCamera	{ get; set; }
		//public SerialInfo LightSerialInfo { get; set; }

		public ImageFormat m_ImageFormat { get; set; }

		public CCoreConfig()
		{
			//LightSerialInfo = new SerialInfo();

			m_nHostPort		= 5555;
			m_nInspectPort	= 4444;
			m_nStoreDay		= 90;
			m_nLightNum		= 1;
			m_strPath_Temp	= "Temp";
			m_ImageFormat	= ImageFormat.Bmp;
		}
	}

	public class CAppState
	{
		public bool m_bManual_Mode	{ get; set; }
		public bool m_bAutoEnable	{ get; set; }
		public bool m_bManualEnable	{ get; set; }
		public bool m_bGrabEnable	 { get; set; }
	}
}
