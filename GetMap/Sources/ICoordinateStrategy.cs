namespace GetMap
{
	interface ICoordinateStrategy
	{
		int X(float lon, int zoom);
		int Y(float lat, int zoom);
	}
}
