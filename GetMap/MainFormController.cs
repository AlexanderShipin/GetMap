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
		private string tilesDirectory = "C:\\GetMapTmp\\";
		private static int tileSize = 256;
		private long imageQuality = 80;
		private int maxMapWidthHeight = 7000 / tileSize * tileSize;

		public delegate bool CheckCancellationHandler();
		public event CheckCancellationHandler CheckCancellation;

		//todo: refactor this method
		public void GetMap(MainFormModel model, BackgroundWorker worker, DoWorkEventArgs e)
		{
			var strategyCreator = new StrategyCreator();
			var strategy = strategyCreator.CreateStrategy(model.MapSourceName);

			var leftTopTile = strategy.Tile(model.LeftTopLat, model.LeftTopLon, model.Zoom);
			var rightBottomTile = strategy.Tile(model.RightBottomLat, model.RightBottomLon, model.Zoom);

			if (!Directory.Exists(tilesDirectory))
				Directory.CreateDirectory(tilesDirectory);

			int mapWidth;
			int mapHeight;
			int internationalDateLineTileX = strategy.X(180, model.Zoom) - 1;
			if (model.RightBottomLon > model.LeftTopLon)
			{
				mapWidth = tileSize * (rightBottomTile.X - leftTopTile.X + 1);
				mapHeight = tileSize * (rightBottomTile.Y - leftTopTile.Y + 1);
			}
			else
			{
				mapWidth = tileSize * ((internationalDateLineTileX - leftTopTile.X + 1) + (rightBottomTile.X + 1));
				mapHeight = tileSize * (rightBottomTile.Y - leftTopTile.X + 1);
			}

			CheckMapSize(mapWidth, mapHeight);

			//todo: dialog whether a user wants too many big maps here
			int splitedByWidthMapsQuantity = (int) Math.Ceiling((decimal) (mapWidth) / maxMapWidthHeight);
			int splitedByHeightMapsQuantity = (int)Math.Ceiling((decimal)(mapHeight) / maxMapWidthHeight);

			string fileName = "no_file_name";
			if (splitedByWidthMapsQuantity > 1 || splitedByHeightMapsQuantity > 1)
			{
				CreateFolderForSplittedMap(model, ref fileName);
			}

			for (int i = 0; i < splitedByHeightMapsQuantity; i++)
			{
				for (int j = 0; j < splitedByWidthMapsQuantity; j++)
				{
					int currentMapWidth = j == splitedByWidthMapsQuantity - 1 ? mapWidth - maxMapWidthHeight * j : maxMapWidthHeight;
					int currentMapHeight = i == splitedByHeightMapsQuantity - 1 ? mapHeight - maxMapWidthHeight * i : maxMapWidthHeight;

					var currentLeftTopTile = new Tile(leftTopTile.X + j * maxMapWidthHeight / tileSize, leftTopTile.Y + i * maxMapWidthHeight / tileSize);
					int currentRightBottomX = j == splitedByWidthMapsQuantity - 1 ? rightBottomTile.X : currentLeftTopTile.X - 1 + (j + 1) * maxMapWidthHeight / tileSize;
					int currentRightBottomY = i == splitedByHeightMapsQuantity - 1 ? rightBottomTile.Y : currentLeftTopTile.Y - 1 + (i + 1) * maxMapWidthHeight / tileSize;
					var currentRightBottomTile = new Tile(currentRightBottomX, currentRightBottomY);

					if (currentLeftTopTile.X > internationalDateLineTileX)
						currentLeftTopTile.X = currentLeftTopTile.X - internationalDateLineTileX - 1;
					if (currentRightBottomX > internationalDateLineTileX)
						currentRightBottomTile.X = currentRightBottomX - internationalDateLineTileX - 1;

					if (splitedByWidthMapsQuantity > 1 || splitedByHeightMapsQuantity > 1)
						model.Path = Path.GetDirectoryName(model.Path) + "\\" + fileName + "-" + (i + 1) + "-" + (j + 1) + ".png";

					LoadTilesAndSaveMap(model, currentMapWidth, currentMapHeight, currentLeftTopTile, currentRightBottomTile, internationalDateLineTileX);
				}
			}
		}

		private static void CreateFolderForSplittedMap(MainFormModel model, ref string fileName)
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

		public Bitmap GetZoomExampleMap(MainFormModel model)
		{
			var strategyCreator = new StrategyCreator();
			var strategy = strategyCreator.CreateStrategy(model.MapSourceName);
			Tile tile;
			if (model.LeftTopLon < model.RightBottomLon)
			{
				tile = strategy.Tile(model.LeftTopLat + (model.LeftTopLat - model.RightBottomLat) / 2, model.LeftTopLon + (model.RightBottomLon - model.LeftTopLon) / 2, model.Zoom);
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
			if (model.LeftTopLon < model.RightBottomLon)
			{
				AttachTilesIterations(graphics, model, leftTopTile.X, leftTopTile, rightBottomTile, -1);
			}
			else
			{
				AttachTilesIterations(graphics, model, leftTopTile.X, leftTopTile, new Tile(internationalDateLineTileX, rightBottomTile.Y), -1);
				AttachTilesIterations(graphics, model, leftTopTile.X, new Tile(0, leftTopTile.Y), rightBottomTile, internationalDateLineTileX);
			}
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
								tileGraphics.DrawImage(LoadTile(sourceFormat, tile, model.Zoom), 0, 0, tileSize, tileSize);
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
