

namespace DRIVER_CAMERA.Infos
{
	public class CInfo_Camera_Factory
	{
		private EMAKER		_temaker;
		public	EMAKER		m_temaker		{ get { return _temaker; } }

		private EINTERFACE	_teinterface;
		public	EINTERFACE	m_teinterface	{ get { return _teinterface; } }

		private string		_strmodel;
		public	string		m_strmodel		{ get { return _strmodel; } }

		private string		_strserial;
		public	string		m_strserial		{ get { return _strserial; } }

		public CInfo_Camera_Factory(EMAKER teMaker, EINTERFACE teInterface, string strModel, string strSerial)
		{
			_temaker		= teMaker;
			_teinterface	= teInterface;
			_strmodel		= strModel;
			_strserial		= strSerial;
		}
	}
}
