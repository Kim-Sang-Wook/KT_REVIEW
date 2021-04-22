using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRIVER_CAMERA.Infos
{
	public struct tsInfo_Grab
	{
		private int		_nwidth;
		public	int		m_nwidth		{ get { return _nwidth; } }

		private int		_nheight;
		public	int		m_nheight		{ get { return _nheight; } }

		private int		_nchannels;
		public	int		m_nchannels		{ get { return _nchannels; } }

		private byte[]	_bydata;
		public	byte[]	m_bydata		{ get { return _bydata; } }
		

		private EGRAB_RESULT _teresult;
		public	EGRAB_RESULT m_teresult	{ get { return _teresult; } }

		public tsInfo_Grab(EGRAB_RESULT teResult)
		{
			_teresult	= teResult;
			_nwidth		= -1;
			_nheight	= -1;
			_nchannels	= -1;
			_bydata		= null;
		}

		public tsInfo_Grab(EGRAB_RESULT teResult, int nWidth, int nHeight, int nCannels, byte[] byData)
		{
			_teresult	= teResult;
			_nwidth		= nWidth;
			_nheight	= nHeight;
			_nchannels	= nCannels;
			_bydata		= byData;
		}
	}
}
