using System;

namespace GetMap
{
	class YandexCoordinate : ICoordinateStrategy
	{
		public int X(float lon, int zoom)
		{
			return (int)((lon + 180) / (360 / Math.Pow(2, zoom)));
		}

		public int Y(float lat, int zoom)
		{
			//wgs84Mercator
			var leftTopLatRad = Math.PI * lat / 180;
			var e = Math.Sqrt(1 - Math.Pow(6356752.3142 / 6378137, 2));//two radiuses of the Earth
			return (int)((Math.PI - Math.Log(Math.Tan(Math.PI / 4 + leftTopLatRad / 2) * Math.Pow(((1 - e * Math.Sin(leftTopLatRad)) / (1 + e * Math.Sin(leftTopLatRad))), e / 2))) / (2 * Math.PI / Math.Pow(2, zoom)));
		}
	}
}
