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

			var leftTopX = strategy.X(model.LeftTopLon, model.Zoom);
			var leftTopY = strategy.Y(model.LeftTopLat, model.Zoom);
			var rightBottomX = strategy.X(model.RightBottomLon, model.Zoom);
			var rightBottomY = strategy.Y(model.RightBottomLat, model.Zoom);

			if (!Directory.Exists(tilesDirectory))
				Directory.CreateDirectory(tilesDirectory);

			int mapWidth;
			int mapHeight;
			int internationalDateLineTileX = strategy.X(180, model.Zoom) - 1;
			if (model.RightBottomLon > model.LeftTopLon)
			{
				mapWidth = tileSize * (rightBottomX - leftTopX + 1);
				mapHeight = tileSize * (rightBottomY - leftTopY + 1);
			}
			else
			{
				mapWidth = tileSize * ((internationalDateLineTileX - leftTopX + 1) + (rightBottomX + 1));
				mapHeight = tileSize * (rightBottomY - leftTopY + 1);
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
					int currentLeftTopX = leftTopX + j * maxMapWidthHeight / tileSize;
					int currentLeftTopY = leftTopY + i * maxMapWidthHeight / tileSize;
					int currentRightButtomX = j == splitedByWidthMapsQuantity - 1 ? rightBottomX : currentLeftTopX - 1 + (j + 1) * maxMapWidthHeight / tileSize;
					int currentRightButtomY = i == splitedByHeightMapsQuantity - 1 ? rightBottomY : currentLeftTopY - 1 + (i + 1) * maxMapWidthHeight / tileSize;

					if (currentLeftTopX > internationalDateLineTileX)
						currentLeftTopX = currentLeftTopX - internationalDateLineTileX - 1;
					if (currentRightButtomX > internationalDateLineTileX)
						currentRightButtomX = currentRightButtomX - internationalDateLineTileX - 1;

					if (splitedByWidthMapsQuantity > 1 || splitedByHeightMapsQuantity > 1)
						model.Path = Path.GetDirectoryName(model.Path) + "\\" + fileName + "-" + (i + 1) + "-" + (j + 1) + ".png";
					
					LoadTilesAndSaveMap(model, currentMapWidth, currentMapHeight, currentLeftTopX, currentLeftTopY, currentRightButtomX, currentRightButtomY, internationalDateLineTileX);
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

		private void LoadTilesAndSaveMap(MainFormModel model, int mapWidth, int mapHeight, int leftTopX, int leftTopY, int rightBottomX, int rightBottomY, int internationalDateLineTileX)
		{
			using (var map = new Bitmap(mapWidth, mapHeight))
			{
				using (Graphics graphics = Graphics.FromImage(map))
				{
					graphics.CompositingQuality = CompositingQuality.HighSpeed;
					graphics.InterpolationMode = InterpolationMode.Low;
					graphics.SmoothingMode = SmoothingMode.HighSpeed;
					graphics.DrawImage(map, 0, 0, mapWidth, mapHeight);

					AttachTilesToMap(graphics, model, leftTopX, leftTopY, rightBottomX, rightBottomY, internationalDateLineTileX);
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

			var tileX = strategy.X(39.833734F, model.Zoom);
			var tileY = strategy.Y(57.632846F, model.Zoom);

			if (!Directory.Exists(tilesDirectory))
				Directory.CreateDirectory(tilesDirectory);

			var map = new Bitmap(tileSize, tileSize);
			using (Graphics graphics = Graphics.FromImage(map))
			{
				graphics.CompositingQuality = CompositingQuality.HighQuality;
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.DrawImage(map, 0, 0, tileSize, tileSize);

				LoadAndAttachTileToMap(graphics, model, 0, 0, tileX, tileY);
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

		private void AttachTilesToMap(Graphics graphics, MainFormModel model, int tileLeftTopX, int tileLeftTopY, int tileRightBottomX, int tileRightBottomY, int internationalDateLineTileX)
		{
			if (model.LeftTopLon < model.RightBottomLon)
			{
				AttachTilesIterations(graphics, model, tileLeftTopX, tileLeftTopX, tileRightBottomX, tileLeftTopY, tileRightBottomY, -1);
			}
			else
			{
				AttachTilesIterations(graphics, model, tileLeftTopX, tileLeftTopX, internationalDateLineTileX, tileLeftTopY, tileRightBottomY, -1);
				AttachTilesIterations(graphics, model, tileLeftTopX, 0, tileRightBottomX, tileLeftTopY, tileRightBottomY, internationalDateLineTileX);
			}
		}

		private void AttachTilesIterations(Graphics graphics, MainFormModel model, int tileXToContinueFrom, int tileStartX, int tileEndX, int tileStartY, int tileEndY, int internationalDateLineTileX)
		{
			for (int i = tileStartX; i < tileEndX + 1; i++)
			{
				if (CheckCancellation())
					break;
				for (int j = tileStartY; j <= tileEndY; j++)
				{
					if (CheckCancellation())
						break;
					LoadAndAttachTileToMap(graphics, model, (internationalDateLineTileX + 1 + i - tileXToContinueFrom) * tileSize, (j - tileStartY) * tileSize, i, j);
				}
			}
		}

		private void LoadAndAttachTileToMap(Graphics graphics, MainFormModel model, int imageLeftTopX, int imageLeftTopY, int tileX, int tileY)
		{
			var fullFilename = String.Format(FullFileNameFormatString, tilesDirectory, model.MapSourceName, tileX, tileY, model.Zoom);
			if (!File.Exists(fullFilename))
				SaveTile(model, tileX, tileY);

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

		private void SaveTile(MainFormModel model, int tileX, int tileY)
		{
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
								tileGraphics.DrawImage(LoadTile(sourceFormat, tileX, tileY, model.Zoom), 0, 0, tileSize, tileSize);
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
				tile.Save(String.Format(FullFileNameFormatString, tilesDirectory, model.MapSourceName, tileX, tileY, model.Zoom), pngCodec, eps);
			}
		}

		private Image LoadTile(string mapSourceFormat, int tileX, int tileY, int zoom)
		{
			var authRequest = (HttpWebRequest)WebRequest.Create(string.Format(mapSourceFormat, tileX, tileY, zoom));
			authRequest.Method = "GET";
			authRequest.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; ru; rv:1.9.1.3)";//for Google
			var authResponse = (HttpWebResponse)authRequest.GetResponse();
			var tile = Image.FromStream(authResponse.GetResponseStream());
//			tile.Save(String.Format(FullFileNameFormatString, path, mapSourceName, tileX, tileY, zoom));
			authResponse.Close();
			return tile;
		}
	}
}
