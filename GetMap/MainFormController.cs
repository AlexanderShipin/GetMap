using System.ComponentModel;
using System.Drawing.Imaging;
using GetMap.Properties;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net;
using GetMap.Sources;

namespace GetMap
{
	public class MainFormController
	{
		private string FullFileNameFormatString = "{0}s={1}z={4}tileX={2}tileY={3}.png";
		private string tilesDirectory = "C:\\GetMapTmp\\cache\\";
		private static int tileSize = 256;
		private long imageQuality = 80;
		private int maxMapWidthHeight = 7000 / tileSize * tileSize;
		private int tilesAttached;
		private int totalTilesQuantity;

		public delegate bool CheckCancellationHandler();
		public event CheckCancellationHandler CheckCancellation;

		public delegate void ReportProgressHandler(int percent, object userState);
		public event ReportProgressHandler ReportProgress;

		//todo: refactor this method
		public void GetMap(MainFormModel model, BackgroundWorker worker, DoWorkEventArgs e)
		{
			var strategyCreator = new StrategyCreator();
			var strategy = strategyCreator.CreateStrategy(model.MapSourceName);

			var leftTopTile = strategy.Tile(model.LeftTopLat, model.LeftTopLon, model.Zoom);
			var rightBottomTile = strategy.Tile(model.RightBottomLat, model.RightBottomLon, model.Zoom);

			if (!Directory.Exists(tilesDirectory))
				Directory.CreateDirectory(tilesDirectory);

			int mapWidthInTiles;
			int mapHeightInTiles;
			int internationalDateLineTileX = strategy.X(180, model.Zoom) - 1;
			if (model.RightBottomLon > model.LeftTopLon)
			{
				mapWidthInTiles = rightBottomTile.X - leftTopTile.X + 1;
				mapHeightInTiles = rightBottomTile.Y - leftTopTile.Y + 1;
			}
			else
			{
				mapWidthInTiles = (internationalDateLineTileX - leftTopTile.X + 1) + (rightBottomTile.X + 1);
				mapHeightInTiles = rightBottomTile.Y - leftTopTile.Y + 1;
			}
			int mapWidth = tileSize * mapWidthInTiles;
			int mapHeight = tileSize * mapHeightInTiles;

			CheckMapSize(mapWidth, mapHeight);

			//todo: dialog whether a user wants too many big maps here
			int splitByWidthMapsQuantity = (int) Math.Ceiling((decimal) (mapWidth) / maxMapWidthHeight);
			int splitByHeightMapsQuantity = (int) Math.Ceiling((decimal) (mapHeight) / maxMapWidthHeight);

			string fileName = "no_file_name";
			if (splitByWidthMapsQuantity > 1 || splitByHeightMapsQuantity > 1)
			{
				CreateFolderForSplitMap(model, ref fileName);
			}

			tilesAttached = 0;
			totalTilesQuantity = mapWidthInTiles * mapHeightInTiles;
			for (int i = 0; i < splitByHeightMapsQuantity; i++)
			{
				for (int j = 0; j < splitByWidthMapsQuantity; j++)
				{
					int currentMapWidth = j == splitByWidthMapsQuantity - 1 ? mapWidth - maxMapWidthHeight * j : maxMapWidthHeight;
					int currentMapHeight = i == splitByHeightMapsQuantity - 1 ? mapHeight - maxMapWidthHeight * i : maxMapWidthHeight;

					var currentLeftTopTile = new Tile(leftTopTile.X + j * maxMapWidthHeight / tileSize, leftTopTile.Y + i * maxMapWidthHeight / tileSize);
					int currentRightBottomX = j == splitByWidthMapsQuantity - 1 ? rightBottomTile.X : currentLeftTopTile.X - 1 + maxMapWidthHeight / tileSize;
					int currentRightBottomY = i == splitByHeightMapsQuantity - 1 ? rightBottomTile.Y : currentLeftTopTile.Y - 1 + maxMapWidthHeight / tileSize;
					var currentRightBottomTile = new Tile(currentRightBottomX, currentRightBottomY);

					if (currentLeftTopTile.X > internationalDateLineTileX)
						currentLeftTopTile.X = currentLeftTopTile.X - internationalDateLineTileX - 1;
					if (currentRightBottomTile.X > internationalDateLineTileX)
						currentRightBottomTile.X = currentRightBottomTile.X - internationalDateLineTileX - 1;

					if (splitByWidthMapsQuantity > 1 || splitByHeightMapsQuantity > 1)
						model.Path = Path.GetDirectoryName(model.Path) + "\\" + fileName + "-" + (i + 1) + "-" + (j + 1) + ".png";

					LoadTilesAndSaveMap(model, currentMapWidth, currentMapHeight, currentLeftTopTile, currentRightBottomTile, internationalDateLineTileX);
				}
			}
		}

		public void EmptyCache()
		{
			Directory.Delete(tilesDirectory, true);
		}

		public Bitmap GetZoomExampleMap(MainFormModel model)
		{
			var strategyCreator = new StrategyCreator();
			var strategy = strategyCreator.CreateStrategy(model.MapSourceName);
			Tile tile;
			if (model.LeftTopLon < model.RightBottomLon)
			{
				tile = strategy.Tile(model.RightBottomLat + (model.LeftTopLat - model.RightBottomLat) / 2, model.LeftTopLon + (model.RightBottomLon - model.LeftTopLon) / 2, model.Zoom);
			}
			else
			{
				var medianLon = model.LeftTopLon + ((180 - model.LeftTopLon) + Math.Abs(-180 - model.RightBottomLon)) / 2;
				if (medianLon > 180)
					medianLon = medianLon - 360;
				tile = strategy.Tile((model.LeftTopLat - model.RightBottomLat) / 2, medianLon, model.Zoom);
			}

			if (!Directory.Exists(tilesDirectory))
				Directory.CreateDirectory(tilesDirectory);

			var map = new Bitmap(tileSize, tileSize);
			using (Graphics graphics = Graphics.FromImage(map))
			{
				graphics.CompositingQuality = CompositingQuality.HighQuality;
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.DrawImage(map, 0, 0, tileSize, tileSize);

				LoadAndAttachTileToMap(graphics, model, 0, 0, tile);
			}
			return map;
		}

		private static void CreateFolderForSplitMap(MainFormModel model, ref string fileName)
		{
			//to avoid situation when map is saved to existing file but actually will be saved in several files (with old name and suffixes)
			if (File.Exists(model.Path))
				File.Delete(model.Path);

			fileName = Path.GetFileNameWithoutExtension(model.Path);
			Directory.CreateDirectory(Path.GetDirectoryName(model.Path) + "\\" + fileName);

			model.Path = Path.GetDirectoryName(model.Path) + "\\" + fileName + "\\" + Path.GetFileName(model.Path);
		}

		private void LoadTilesAndSaveMap(MainFormModel model, int mapWidth, int mapHeight, Tile leftTopTile, Tile rightBottomTile, int internationalDateLineTileX)
		{
			using (var map = new Bitmap(mapWidth, mapHeight))
			{
				using (Graphics graphics = Graphics.FromImage(map))
				{
					graphics.CompositingQuality = CompositingQuality.HighSpeed;
					graphics.InterpolationMode = InterpolationMode.Low;
					graphics.SmoothingMode = SmoothingMode.HighSpeed;
					graphics.DrawImage(map, 0, 0, mapWidth, mapHeight);

					AttachTilesToMap(graphics, model, leftTopTile, rightBottomTile, internationalDateLineTileX);
				}

				var eps = new EncoderParameters(1);
				eps.Param[0] = new EncoderParameter(Encoder.Quality, imageQuality);

				var jpegCodec = getEncoderInfo("image/jpeg");
				map.Save(model.Path, jpegCodec, eps);
			}
		}

		private ImageCodecInfo getEncoderInfo(string mimeType)
		{
			// Get image codecs for all image formats
			ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

			// Find the correct image codec
			for (int i = 0; i < codecs.Length; i++)
				if (codecs[i].MimeType == mimeType)
					return codecs[i];
			return null;
		}

		private void CheckMapSize(int mapWidth, int mapHeight)
		{
			if(mapHeight <= 0 || mapWidth <= 0)
				throw new Exception(Resources.MainForm_Exception_InvalidMapSize);
		}

		private void AttachTilesToMap(Graphics graphics, MainFormModel model, Tile leftTopTile, Tile rightBottomTile, int internationalDateLineTileX)
		{
			if ((model.LeftTopLon >= model.RightBottomLon) && (leftTopTile.X > rightBottomTile.X))
			{
				AttachTilesIterations(graphics, model, leftTopTile.X, leftTopTile, new Tile(internationalDateLineTileX, rightBottomTile.Y), -1);
				AttachTilesIterations(graphics, model, leftTopTile.X, new Tile(0, leftTopTile.Y), rightBottomTile, internationalDateLineTileX);
			}
			else
				AttachTilesIterations(graphics, model, leftTopTile.X, leftTopTile, rightBottomTile, -1);
		}

		private void AttachTilesIterations(Graphics graphics, MainFormModel model, int tileXToContinueFrom, Tile leftTopTile, Tile rightBottomTile, int internationalDateLineTileX)
		{
			for (int i = leftTopTile.X; i <= rightBottomTile.X; i++)
			{
				if (CheckCancellation())
					break;
				for (int j = leftTopTile.Y; j <= rightBottomTile.Y; j++)
				{
					if (CheckCancellation())
						break;
					LoadAndAttachTileToMap(graphics, model, (internationalDateLineTileX + 1 + i - tileXToContinueFrom) * tileSize, (j - leftTopTile.Y) * tileSize, new Tile(i, j));
					var percent = ++tilesAttached * 100d / totalTilesQuantity;
					ReportProgress((int)percent, String.Empty);
				}
			}
		}

		private void LoadAndAttachTileToMap(Graphics graphics, MainFormModel model, int imageLeftTopX, int imageLeftTopY, Tile tile)
		{
			var fullFilename = String.Format(FullFileNameFormatString, tilesDirectory, model.MapSourceName, tile.X, tile.Y, model.Zoom);
			if (!File.Exists(fullFilename))
				SaveTile(model, tile);

			Bitmap tileFile = null;
			try
			{
				try
				{
					tileFile = new Bitmap(fullFilename, true);
				}
				catch
				{
					tileFile = new Bitmap(tileSize, tileSize);
				}
				graphics.DrawImage(tileFile, imageLeftTopX, imageLeftTopY, tileSize, tileSize);
			}
			finally
			{
				tileFile.Dispose();
			}
		}

		private void SaveTile(MainFormModel model, Tile tile)
		{
			using (var tileImage = new Bitmap(tileSize, tileSize))
			{
				using (Graphics tileGraphics = Graphics.FromImage(tileImage))
				{
					tileGraphics.CompositingQuality = CompositingQuality.HighSpeed;
					tileGraphics.InterpolationMode = InterpolationMode.Low;
					tileGraphics.SmoothingMode = SmoothingMode.HighSpeed;
					tileGraphics.DrawImage(tileImage, 0, 0, tileSize, tileSize);

					try
					{
						foreach (string sourceFormat in model.MapSourceFormat)
						{
							if (!string.IsNullOrEmpty(sourceFormat))
							{
								using (var tileLayerImage = LoadTile(sourceFormat, tile, model.Zoom))
								{
									tileGraphics.DrawImage(tileLayerImage, 0, 0, tileSize, tileSize);
								}
							}
						}
					}
					catch (WebException)
					{
						//tile doesn't exist
					}
				}

				var eps = new EncoderParameters(1);
				eps.Param[0] = new EncoderParameter(Encoder.Quality, imageQuality);

				var pngCodec = getEncoderInfo("image/png");
				tileImage.Save(String.Format(FullFileNameFormatString, tilesDirectory, model.MapSourceName, tile.X, tile.Y, model.Zoom), pngCodec, eps);
			}
		}

		private Image LoadTile(string mapSourceFormat, Tile tile, int zoom)
		{
			var authRequest = (HttpWebRequest)WebRequest.Create(string.Format(mapSourceFormat, tile.X, tile.Y, zoom));
			authRequest.Method = "GET";
			authRequest.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; ru; rv:1.9.1.3)";//for Google
			var authResponse = (HttpWebResponse)authRequest.GetResponse();
			var tileStream = Image.FromStream(authResponse.GetResponseStream());
			authResponse.Close();
			return tileStream;
		}
	}
}
