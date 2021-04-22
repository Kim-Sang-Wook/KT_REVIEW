using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRIVER_CAMERA.Infos
{
	public struct tsCamera_Para_item
	{
		public double Current { get; set; }
		public double Min { get; set; }
		public double Max { get; set; }

		public tsCamera_Para_item(double current, double min, double max)
		{
			Current = current;
			Min = min;
			Max = max;
		}
	}

	public class CInfo_Camera_Para
	{
		public bool m_btrigger { get; set; }

		private IDictionary<ECAM_PARA, tsCamera_Para_item> _Idictparas;
		public	IDictionary<ECAM_PARA, tsCamera_Para_item> m_Idictparas			{ get { return _Idictparas; } }

		private IDictionary<ECAM_AUTO_TYPE, ECAM_AUTO_VALUE> _Idictautovalues;
		public	IDictionary<ECAM_AUTO_TYPE, ECAM_AUTO_VALUE> m_Idictautovalues	{ get { return _Idictautovalues; } }

		public	CInfo_Camera_Para(	bool bTrigger,
									IDictionary<ECAM_PARA, tsCamera_Para_item> IdictParas,
									IDictionary<ECAM_AUTO_TYPE, ECAM_AUTO_VALUE> IdictAutoValues)
		{
			m_btrigger			= bTrigger;
			_Idictparas			= IdictParas;
			_Idictautovalues	= IdictAutoValues;
		}
	}
}
