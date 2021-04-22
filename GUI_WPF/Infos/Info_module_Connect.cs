
using System.Windows.Media;

namespace GUI_WPF.Infos
{
	struct tsInfo_module_Connect
	{
		private bool _bisConnected;
		public	bool m_bIsConnected	{ get { return _bisConnected; } }

		private string _strmsg;
		public	string m_strMsg	{ get { return _strmsg; } }

		private Brush _brush;
		public	Brush Brush			{ get { return _brush; } }

		public tsInfo_module_Connect(bool bIsConnected, string strmsg, Color color)
		{
			_bisConnected	= bIsConnected;
			_strmsg			= strmsg;
			_brush			= new SolidColorBrush(color);
			_brush.Freeze();
		}
	}
}

