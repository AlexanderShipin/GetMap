using System;
using GetMap.Sources;

namespace GetMap
{
	class GoogleCoordinates : ICoordinateStrategy
	{
		public int X(float lon, int zoom)
		{
			return (int)((lon + 180) / (360 / Math.Pow(2, zoom)));
		}

		public int Y(float lat, int zoom)
		{
			//sphericalMercator
			var leftTopLatRad = Math.PI * lat / 180;
			return (int)((Math.PI - (0.5 * Math.Log((1 + Math.Sin(leftTopLatRad)) / (1 - Math.Sin(leftTopLatRad))))) / (2 * Math.PI / Math.Pow(2, zoom)));
		}

		public Tile Tile(float lon, float lat, int zoom)
		{
			return new Tile(X(lon, zoom), Y(lat, zoom));
		}
	}
}
