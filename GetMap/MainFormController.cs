using System.ComponentModel;
using System.Drawing.Imaging;
using GetMap.Properties;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net;

namespace GetMap
{
	public class MainFormController
	{
		private string FileFormatString = "{0}s={1}z={4}x={2}y={3}.png";
		private string tilesDirectory = "C:\\GetMapTmp\\";
		private static int tileSize = 256;
		private long imageQuality = 80;
		private int maxMapWidthHeight = 10000 / tileSize * tileSize;

		public delegate bool CheckCancellationHandler();
		public event CheckCancellationHandler CheckCancellation;

		public void GetMap(MainFormModel model, BackgroundWorker worker, DoWorkEventArgs e)
		{
			var strategyCreator = new StrategyCreator();
			var strategy = strategyCreator.CreateStrategy(model.MapSourceName);

			var leftTopX = strategy.X(model.LeftTopLon, model.Zoom);
			var leftTopY = strategy.Y(model.LeftTopLat, model.Zoom);
			var rightBottomX = strategy.X(model.RightBottomLon, model.Zoom);
			var rightBottomY = strategy.Y(model.RightBottomLat, model.Zoom);

			if (!Directory.Exists(tilesDirectory))
				Directory.CreateDirectory(tilesDirectory);

			int mapWidth;
			int mapHeight;
			int internationalDateLineTileNumber = strategy.X(180, model.Zoom) - 1;
			if(rightBottomX >= leftTopX)
			{
				mapWidth = tileSize * (rightBottomX - leftTopX + 1);
				mapHeight = tileSize * (rightBottomY - leftTopY + 1);
			}
			else
			{
				mapWidth = tileSize * ((internationalDateLineTileNumber - leftTopX + 1) + (rightBottomX + 1));
				mapHeight = tileSize * (rightBottomY - leftTopY + 1);
			}

			CheckMapSize(mapWidth, mapHeight);

			//todo: dialog whether the user wants too many big maps here
			int splitedByWidthMapsQuantity = (int) Math.Ceiling((decimal) (mapWidth) / maxMapWidthHeight);
			int splitedByHeightMapsQuantity = (int) Math.Ceiling((decimal) (mapHeight) / maxMapWidthHeight);
			for (int i = 0; i < splitedByHeightMapsQuantity; i++)
			{
				for (int j = 0; j < splitedByWidthMapsQuantity; j++)
				{
					int currentMapWidth = j == splitedByWidthMapsQuantity - 1 ? mapWidth - maxMapWidthHeight * j : maxMapWidthHeight;
					int currentMapHeight = i == splitedByHeightMapsQuantity - 1 ? mapHeight - maxMapWidthHeight * i : maxMapWidthHeight;
					int currentLeftTopX = leftTopX + j * maxMapWidthHeight / tileSize;
					int currentLeftTopY = leftTopY + i * maxMapWidthHeight / tileSize;
					int currentRightButtomX = j == splitedByWidthMapsQuantity - 1 ? rightBottomX : currentLeftTopX - 1 + (j + 1) * maxMapWidthHeight / tileSize;
					int currentRightButtomY = i == splitedByHeightMapsQuantity - 1 ? rightBottomY : currentLeftTopY - 1 + (i + 1) * maxMapWidthHeight / tileSize;

					if (currentLeftTopX > internationalDateLineTileNumber)
						currentLeftTopX = currentLeftTopX - internationalDateLineTileNumber - 1;
					if (currentRightButtomX > internationalDateLineTileNumber)
						currentRightButtomX = currentRightButtomX - internationalDateLineTileNumber - 1;

					model.Path = String.Format(MainFormModel.DefaultMapPath, (i + 1) + "-" + (j + 1));
					LoadTilesAndSaveMap(model, currentMapWidth, currentMapHeight, currentLeftTopX, currentLeftTopY, currentRightButtomX, currentRightButtomY, internationalDateLineTileNumber);
				}
			}
		}

		private void LoadTilesAndSaveMap(MainFormModel model, int mapWidth, int mapHeight, int leftTopX, int leftTopY, int rightBottomX, int rightBottomY, int internationalDateLineTileNumber)
		{
			using (var map = new Bitmap(mapWidth, mapHeight))
			{
				using (Graphics graphics = Graphics.FromImage(map))
				{
					graphics.CompositingQuality = CompositingQuality.HighSpeed;
					graphics.InterpolationMode = InterpolationMode.Low;
					graphics.SmoothingMode = SmoothingMode.HighSpeed;
					graphics.DrawImage(map, 0, 0, mapWidth, mapHeight);

					AttachTilesToMap(graphics, model, leftTopX, leftTopY, rightBottomX, rightBottomY, internationalDateLineTileNumber);
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

			var leftTopX = strategy.X(39.833734F, model.Zoom);
			var leftTopY = strategy.Y(57.632846F, model.Zoom);

			if (!Directory.Exists(tilesDirectory))
				Directory.CreateDirectory(tilesDirectory);

			var map = new Bitmap(tileSize, tileSize);
			using (Graphics graphics = Graphics.FromImage(map))
			{
				graphics.CompositingQuality = CompositingQuality.HighQuality;
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.DrawImage(map, 0, 0, tileSize, tileSize);

				LoadAndAttachTileToMap(graphics, model, leftTopX, leftTopY, leftTopX, leftTopY, -1);
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
			//todo: bitmap limit size
		}

		private void AttachTilesToMap(Graphics graphics, MainFormModel model, int leftTopX, int leftTopY, int rightBottomX, int rightBottomY, int idlTileNumber)
		{
			if(leftTopX <= rightBottomX)
			{
				for(int i = leftTopX; i <= rightBottomX; i++)
				{
					if (CheckCancellation())
						break;
					for(int j = leftTopY; j <= rightBottomY; j++)
					{
						if (CheckCancellation())
							break;
						LoadAndAttachTileToMap(graphics, model, leftTopX, leftTopY, i, j, -1);
					}
				}
			}
			else
			{
				for (int i = leftTopX; i < idlTileNumber + 1; i++)
				{
					if (CheckCancellation())
						break;
					for (int j = leftTopY; j <= rightBottomY; j++)
					{
						if (CheckCancellation())
							break;
						LoadAndAttachTileToMap(graphics, model, leftTopX, leftTopY, i, j, -1);
					}
				}

				for (int i = 0; i <= rightBottomX; i++)
				{
					if (CheckCancellation())
						break;
					for (int j = leftTopY; j <= rightBottomY; j++)
					{
						if (CheckCancellation())
							break;
						LoadAndAttachTileToMap(graphics, model, leftTopX, leftTopY, i, j, idlTileNumber);
					}
				}
			}
		}

		private void LoadAndAttachTileToMap(Graphics graphics, MainFormModel model, int leftTopX, int leftTopY, int i, int j, int idlTileNumber)
		{
			if (!File.Exists(String.Format(FileFormatString, tilesDirectory, model.MapSourceName, i, j, model.Zoom)))
				using (var tile = new Bitmap(tileSize, tileSize))
				{
					using (Graphics tileGraphics = Graphics.FromImage(tile))
					{
						tileGraphics.CompositingQuality = CompositingQuality.HighSpeed;
						tileGraphics.InterpolationMode = InterpolationMode.Low;
						tileGraphics.SmoothingMode = SmoothingMode.HighSpeed;
						tileGraphics.DrawImage(tile, 0, 0, tileSize, tileSize);

						try
						{
							foreach (string sourceFormat in model.MapSourceFormat)
							{
								if (!string.IsNullOrEmpty(sourceFormat))
									tileGraphics.DrawImage(LoadTile(sourceFormat, i, j, model.Zoom), 0, 0, tileSize, tileSize);
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
					tile.Save(String.Format(FileFormatString, tilesDirectory, model.MapSourceName, i, j, model.Zoom), pngCodec, eps);
				}

			Bitmap tileFile = null;
			try
			{
				try
				{
					tileFile = new Bitmap(String.Format(FileFormatString, tilesDirectory, model.MapSourceName, i, j, model.Zoom), true);
				}
				catch
				{
					tileFile = new Bitmap(tileSize, tileSize);
				}
				graphics.DrawImage(tileFile, (idlTileNumber + 1 + i - leftTopX) * tileSize, (j - leftTopY) * tileSize, tileSize, tileSize);
			}
			finally
			{
				tileFile.Dispose();
			}
		}

		private Image LoadTile(string mapSourceFormat, int x, int y, int zoom)
		{
			var authRequest = (HttpWebRequest)WebRequest.Create(string.Format(mapSourceFormat, x, y, zoom));
			authRequest.Method = "GET";
			authRequest.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; ru; rv:1.9.1.3)";//for Google
			var authResponse = (HttpWebResponse)authRequest.GetResponse();
			var tile = Image.FromStream(authResponse.GetResponseStream());
//			tile.Save(String.Format(FileFormatString, path, mapSourceName, x, y, zoom));
			authResponse.Close();
			return tile;
		}
	}
}
