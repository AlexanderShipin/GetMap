using System.Collections.Generic;

namespace GetMap
{
	class StrategyCreator
	{
		readonly Dictionary<string, ICoordinateStrategy> _strategies = new Dictionary<string, ICoordinateStrategy>();

		public StrategyCreator()
		{
			var googleCoordinateStrategy = new GoogleCoordinates();
			var yandexCoordinateStrategy = new YandexCoordinates();

			_strategies.Add("GoogleMap", googleCoordinateStrategy);
			_strategies.Add("GoogleSatellite", googleCoordinateStrategy);
			_strategies.Add("GoogleHybrid", googleCoordinateStrategy);
			_strategies.Add("YandexMap", yandexCoordinateStrategy);
			_strategies.Add("YandexSatellite", yandexCoordinateStrategy);
			_strategies.Add("YandexHybrid", yandexCoordinateStrategy);
		}

		public ICoordinateStrategy CreateStrategy(string provider)
		{
			return _strategies[provider];
		}
	}
}
