using System;

namespace GetMap
{
	public class MainFormModel
	{
		public static string DefaultMapPath = "C:\\GetMapTmp\\GetMap{0}.png";

		public string MapSourceFormat { get; set; }
		public string MapSourceName { get; set; }

		private float _leftTopLat;
		public float LeftTopLat
		{
			get { return _leftTopLat; }
			set
			{
				_leftTopLat = value;
				if (value > 85)
					_leftTopLat = 85;
				if(value < -85)
					_leftTopLat = -85;
			}
		}

		private float _leftTopLon ;
		public float LeftTopLon {
			get { return _leftTopLon; }
			set
			{
				_leftTopLon = value;
				if (value > 179)
					_leftTopLon = -180 + (value - 179) % 360;
				if(value < -180)
					_leftTopLon = 179 - (value + 180) % 360;
			}
		}

		private float _rightBottomLat;
		public float RightBottomLat
		{
			get { return _rightBottomLat; }
			set
			{
				_rightBottomLat = value;
				if (value > 85)
					_rightBottomLat = 85;
				if(value < -85)
					_rightBottomLat = -85;
			}
		}

		private float _rightBottomLon;
		public float RightBottomLon
		{
			get { return _rightBottomLon; }
			set
			{
				_rightBottomLon = value;
				if (value >= 180)
					_rightBottomLon = -180 + (value - 179) % 360;
				if (value < -180)
					_rightBottomLon = 179 - (value + 180) % 360;
			}
		}

		public int Zoom { get; set; }
		public string Path { get; set; }
	}
}
