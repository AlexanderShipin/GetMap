using System.Drawing;
using GetMap.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace GetMap
{
	public partial class MainForm : Form
	{
		private MainFormModel model;
		private Dictionary<string, KeyValuePair<string, string>> providers = new Dictionary<string, KeyValuePair<string, string>>//new Dictionary<string, string>
			{
				{"Google map", new KeyValuePair<string, string>("GoogleMap", "http://mts.google.com/vt/x={0}&y={1}&z={2}&hl=en")},
				{"Google satellite", new KeyValuePair<string, string>("GoogleSatellite", "http://khms.google.com/kh/v=159&x={0}&y={1}&z={2}")},
				{"Google hybrid", new KeyValuePair<string, string>("GoogleHybrid", "http://mts.google.com/vt/lyrs=h&x={0}&y={1}&z={2}&hl=en")},
				{"Yandex map", new KeyValuePair<string, string>("YandexMap", "http://vec.maps.yandex.net/tiles?l=map&x={0}&y={1}&z={2}&lang=ru_RU")},
				{"Yandex satellite", new KeyValuePair<string, string>("YandexSatellite", "http://sat.maps.yandex.net/tiles?l=sat&x={0}&y={1}&z={2}&lang=ru_RU")},
				{"Yandex hybrid", new KeyValuePair<string, string>("YandexHybrid", "http://vec.maps.yandex.net/tiles?l=skl&x={0}&y={1}&z={2}&lang=ru_RU")},
			};
		private Regex regExCoordinate = new Regex(@"-?\d+(\.\d+)?");
		private Regex regExTwoCoordinates = new Regex(@"(-?\d+(\.\d+)?)+,\s?(-?\d+(\.\d+)?)+");
		private Regex regExFourCoordinates = new Regex(@"(-?\d+(\.\d+)?)+,\s?(-?\d+(\.\d+)?)+;\s?(-?\d+(\.\d+)?)+,\s?(-?\d+(\.\d+)?)+");
		private MainFormController controller = new MainFormController();

		private Stopwatch timeRecorder;
		private string errorMsg;

		public MainForm()
		{
			InitializeComponent();

			sourceComboBox.DataSource = new BindingSource(providers, null);
			sourceComboBox.DisplayMember = "Key";
			sourceComboBox.ValueMember = "Value";
			sourceComboBox.SelectedIndex = 0;
		}

		#region Helpers

		private void GetZoomExampleMap()
		{
			var model = new MainFormModel();
			FillModel(model, false);
			zoomExamplePictureBox.Image = controller.GetZoomExampleMap(model);
		}

		private bool IsValidMainForm()
		{
			bool isValid = true;

			if (!IsValidCoordinateText(leftTopLatTextBox.Text))
				isValid = false;
			if (!IsValidCoordinateText(leftTopLonTextBox.Text))
				isValid = false;
			if (!IsValidCoordinateText(rightBottomLatTextBox.Text))
				isValid = false;
			if (!IsValidCoordinateText(rightBottomLonTextBox.Text))
				isValid = false;

			return isValid;
		}

		private bool IsValidCoordinateText(string text)
		{
			return regExCoordinate.Match(text).Groups[0].Value == text;
		}

		private bool IsValidTwoCoordinatesText(string text, out float lat, out float lng)
		{
			lng = 0;
			return float.TryParse(regExTwoCoordinates.Match(text).Groups[1].Value, NumberStyles.Number, CultureInfo.InvariantCulture, out lat) &&
				   float.TryParse(regExTwoCoordinates.Match(text).Groups[3].Value, NumberStyles.Number, CultureInfo.InvariantCulture, out lng);
		}

		private bool IsValidFourCoordinatesText(string text, out float leftTopLat, out float leftTopLng, out float rightBottomLat, out float rightBottomLng)
		{
			leftTopLng = 0;
			rightBottomLat = 0;
			rightBottomLng = 0;
			return float.TryParse(regExFourCoordinates.Match(text).Groups[1].Value, NumberStyles.Number, CultureInfo.InvariantCulture, out leftTopLat) &&
				   float.TryParse(regExFourCoordinates.Match(text).Groups[3].Value, NumberStyles.Number, CultureInfo.InvariantCulture, out leftTopLng) &&
				   float.TryParse(regExFourCoordinates.Match(text).Groups[5].Value, NumberStyles.Number, CultureInfo.InvariantCulture, out rightBottomLat) &&
				   float.TryParse(regExFourCoordinates.Match(text).Groups[7].Value, NumberStyles.Number, CultureInfo.InvariantCulture, out rightBottomLng);
		}

		private bool IsHybridLayer(string layerName)
		{
			return layerName.Contains("Hybrid");
		}

		#endregion

		private void FillModel(MainFormModel mainFormModel, bool showMessage = true)
		{
			var selectedValue = ((KeyValuePair<string, KeyValuePair<string, string>>) sourceComboBox.SelectedItem).Value;
			if (IsHybridLayer(selectedValue.Key))
			{
				var satelliteFormatPair = providers.Values.FirstOrDefault(v => v.Key == selectedValue.Key.Replace("Hybrid", "Satellite"));
				mainFormModel.MapSourceFormat.Add(satelliteFormatPair.Value);
				mainFormModel.MapSourceFormat.Add(selectedValue.Value);
			}
			else
			{
				mainFormModel.MapSourceFormat.Add(selectedValue.Value);
			}
			mainFormModel.MapSourceName = ((KeyValuePair<string, KeyValuePair<string, string>>)sourceComboBox.SelectedItem).Value.Key;
			mainFormModel.LeftTopLat = float.Parse(leftTopLatTextBox.Text, CultureInfo.InvariantCulture);
			mainFormModel.LeftTopLon = float.Parse(leftTopLonTextBox.Text, CultureInfo.InvariantCulture);
			mainFormModel.RightBottomLat = float.Parse(rightBottomLatTextBox.Text, CultureInfo.InvariantCulture);
			mainFormModel.RightBottomLon = float.Parse(rightBottomLonTextBox.Text, CultureInfo.InvariantCulture);
			mainFormModel.Zoom = zoomTrackBar.Value;
			if (pathTextBox.Text == MainFormModel.DefaultMapPath)
			{
				mainFormModel.Path = MainFormModel.DefaultMapPath + String.Format(MainFormModel.DefaultFileName, selectedValue.Key + "_" +
																												 mainFormModel.LeftTopLat.ToString(CultureInfo.GetCultureInfo("en-us")) + "," +
																												 mainFormModel.LeftTopLon.ToString(CultureInfo.GetCultureInfo("en-us")) + ";" +
																												 mainFormModel.RightBottomLat.ToString(CultureInfo.GetCultureInfo("en-us")) + "," +
																												 mainFormModel.RightBottomLon.ToString(CultureInfo.GetCultureInfo("en-us")) + "_" +
																												 mainFormModel.Zoom);
				if (showMessage)
					MessageBox.Show(String.Format(Resources.MainForm_MapPathNotEntered, mainFormModel.Path), Resources.MainForm_Warning, MessageBoxButtons.OK);
			}
			else
			{
				mainFormModel.Path = pathTextBox.Text;
			}
		}

		#region FormEvents

		private void BuildMapButton_Click(object sender, EventArgs e)
		{
			buildMapButton.Enabled = false;
			cancelButton.Enabled = true;
			toolStripBuildingStatusLabel.Text = String.Empty;
			try
			{
				model = new MainFormModel();
				if (!IsValidMainForm())
				{
					throw new Exception(Resources.MainForm_CoordinatesNotValid);
				}
				FillModel(model);
				if (backgroundWorker.IsBusy != true)
				{
					backgroundWorker.RunWorkerAsync();
				}
			}
			catch (Exception ex)
			{
				toolStripBuildingStatusLabel.Text = Resources.MainForm_ErrorOccuredWithMessage + ": " + ex.Message;
			}
		}

		private void PathButton_Click(object sender, EventArgs e)
		{
			saveMapDialog.ShowDialog();
			pathTextBox.Text = saveMapDialog.FileName;
		}

		private void textBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			char decimalSeparator = Convert.ToChar(CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator);

			if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '-')
			{
				e.Handled = true;
			}

			// only allow one decimal point (not at the beginning)
			var textBox = sender as TextBox;
			if (e.KeyChar == decimalSeparator && textBox != null && (textBox.Text.IndexOf(decimalSeparator) > -1 || textBox.SelectionStart == 0))
			{
				e.Handled = true;
			}

			// only allow one minus at the beginnig of the string
			if (e.KeyChar == '-' && textBox != null && (textBox.Text.IndexOf('-') > -1 || textBox.SelectionStart != 0))
			{
				e.Handled = true;
			}
		}

		private void textBox_KeyDown(object sender, KeyEventArgs e)
		{
			float leftTopLat;
			float leftTopLng;
			float rightBottomLat;
			float rightBottomLng;
			if (ModifierKeys == Keys.Control && e.KeyCode == Keys.V)
			{
				if (!IsValidCoordinateText(Clipboard.GetText()))
				{
					e.SuppressKeyPress = true;
					if (!IsValidFourCoordinatesText(Clipboard.GetText(), out leftTopLat, out leftTopLng, out rightBottomLat, out rightBottomLng))
					{
						if (IsValidTwoCoordinatesText(Clipboard.GetText(), out leftTopLat, out leftTopLng))
						{
							if (sender == leftTopLatTextBox || sender == leftTopLonTextBox)
							{
								leftTopLatTextBox.Text = leftTopLat.ToString(CultureInfo.InvariantCulture);
								leftTopLonTextBox.Text = leftTopLng.ToString(CultureInfo.InvariantCulture);
							}
							if (sender == rightBottomLatTextBox || sender == rightBottomLonTextBox)
							{
								rightBottomLatTextBox.Text = leftTopLat.ToString(CultureInfo.InvariantCulture);
								rightBottomLonTextBox.Text = leftTopLng.ToString(CultureInfo.InvariantCulture);
							}
						}
					}
					else
					{
						leftTopLatTextBox.Text = leftTopLat.ToString(CultureInfo.InvariantCulture);
						leftTopLonTextBox.Text = leftTopLng.ToString(CultureInfo.InvariantCulture);
						rightBottomLatTextBox.Text = rightBottomLat.ToString(CultureInfo.InvariantCulture);
						rightBottomLonTextBox.Text = rightBottomLng.ToString(CultureInfo.InvariantCulture);
					}
				}
			}
		}

		private void zoomTrackBar_ValueChanged(object sender, EventArgs e)
		{
			zoomTextBox.Text = zoomTrackBar.Value.ToString();
			GetZoomExampleMap();
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			if (backgroundWorker.WorkerSupportsCancellation)
			{
				backgroundWorker.CancelAsync();
			}
		}

		private void sourceComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			GetZoomExampleMap();
		}

		private void coordinateTextBox_TextChanged(object sender, EventArgs e)
		{
			GetZoomExampleMap();
		}

		private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			buildMapButton.Enabled = true;
			cancelButton.Enabled = false;
			toolStripProgressBar.Value = 0;

			if (e.Cancelled)
			{
				toolStripBuildingStatusLabel.Text = Resources.MainForm_BuildCancelled;
				return;
			}

			if (string.IsNullOrEmpty(errorMsg))
				toolStripBuildingStatusLabel.Text = Resources.MainForm_BuildingTime + ": " + timeRecorder.Elapsed.ToString(@"mm\:ss\:ff");
			else
				toolStripBuildingStatusLabel.Text = Resources.MainForm_ErrorOccuredWithMessage + ": " + errorMsg;
		}

		private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			var worker = sender as BackgroundWorker;

			controller.ReportProgress += backgroundWorker.ReportProgress;
			controller.CheckCancellation += () =>
			{
				if (worker.CancellationPending)
				{
					e.Cancel = true;
					return true;
				}
				return false;
			};

			errorMsg = string.Empty;
			timeRecorder = Stopwatch.StartNew();
			try
			{
				controller.GetMap(model, worker, e);
			}
			catch (Exception ex)
			{
				errorMsg = ex.Message;
			}
			timeRecorder.Stop();
		}

		private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			toolStripProgressBar.Value = e.ProgressPercentage;
			toolStripBuildingStatusLabel.Text = Resources.MainForm_BuildingTime + ": " + timeRecorder.Elapsed.ToString(@"mm\:ss\:ff");

			//Show percentage
			statusStrip.Refresh();
			using (Graphics gr = toolStripProgressBar.ProgressBar.CreateGraphics())
			{
				gr.DrawString(toolStripProgressBar.Value.ToString() + "%",
					SystemFonts.DefaultFont,
					Brushes.Black,
					new PointF(toolStripProgressBar.Width / 2 - (gr.MeasureString(toolStripProgressBar.Value.ToString() + "%", SystemFonts.DefaultFont).Width / 2.0F),
						toolStripProgressBar.Height / 2 - (gr.MeasureString(toolStripProgressBar.Value.ToString() + "%", SystemFonts.DefaultFont).Height / 2.0F)));
			}
		}

		#endregion
	}
}
