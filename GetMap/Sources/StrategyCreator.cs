using System.Collections.Generic;

namespace GetMap
{
	class StrategyCreator
	{
		readonly Dictionary<string, ICoordinateStrategy> _strategies = new Dictionary<string, ICoordinateStrategy>();

		public StrategyCreator()
		{
			_strategies.Add("GoogleMap", new GoogleCoordinate());
			_strategies.Add("GoogleSatellite", new GoogleCoordinate());
			_strategies.Add("YandexMap", new YandexCoordinate());
			_strategies.Add("YandexSatellite", new YandexCoordinate());
		}

		public ICoordinateStrategy CreateStrategy(string provider)
		{
			return _strategies[provider];
		}
	}
}
