using GetMap.Sources;

namespace GetMap
{
	interface ICoordinateStrategy
	{
		int X(float lon, int zoom);
		int Y(float lat, int zoom);
		Tile Tile(float lat, float lon, int zoom);
	}
}
