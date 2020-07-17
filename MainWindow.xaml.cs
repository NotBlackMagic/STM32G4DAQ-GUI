using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

using LiveCharts;
using LiveCharts.Helpers;
using LiveCharts.Wpf;

using NotBlackMagic;

namespace STM32G4DAQ {

	public static class Opcodes {
		public const byte setCurrentA = 0x01;
		public const byte setCurrentB = 0x02;

		public const byte setAnalogInA = 0x03;
		public const byte setAnalogInACH = 0x04;

		public const byte setAnalogOutACH = 0x05;

		public const byte txAnalogInA = 0x81;
	}
	public partial class MainWindow : Window {
		STMDAQ daq = new STMDAQ();

		const int graphPoints = 512;

		byte[] txBuffer = new byte[34];

		int[] analogInADataIndex = new int[8];
		double[][] analogInAData = new double[8][];

		bool enableValueChangedEvents = false;

		public SeriesCollection SeriesAnalogIn { get; set; }
		public MainWindow() {
			enableValueChangedEvents = false;
			InitializeComponent();
			enableValueChangedEvents = true;

			//Init Variables
			analogInADataIndex[0] = 0;
			analogInADataIndex[1] = 0;
			analogInADataIndex[2] = 0;
			analogInADataIndex[3] = 0;
			analogInADataIndex[4] = 0;
			analogInADataIndex[5] = 0;
			analogInADataIndex[6] = 0;
			analogInADataIndex[7] = 0;
			analogInAData[0] = new double[graphPoints];
			analogInAData[1] = new double[graphPoints];
			analogInAData[2] = new double[graphPoints];
			analogInAData[3] = new double[graphPoints];
			analogInAData[4] = new double[graphPoints];
			analogInAData[5] = new double[graphPoints];
			analogInAData[6] = new double[graphPoints];
			analogInAData[7] = new double[graphPoints];

			//Init GUI Update dispatcher
			DispatcherTimer dispatcherTimer = new DispatcherTimer();
			dispatcherTimer.Interval = TimeSpan.FromMilliseconds(1000);
			dispatcherTimer.Tick += GUIUpdateHandler;
			dispatcherTimer.Start();

			//Init Graph Series
			SeriesAnalogIn = new SeriesCollection {
				new LineSeries {
					Title = "CH1",
					Values = new ChartValues<float>(),
					PointGeometry = null,
					LineSmoothness = 0,
					Stroke = Brushes.Red,
					Fill = Brushes.Transparent
				},
				new LineSeries {
					Title = "CH2",
					Values = new ChartValues<float>(),
					PointGeometry = null,
					LineSmoothness = 0,
					Stroke = Brushes.Orange,
					Fill = Brushes.Transparent
				},
				new LineSeries {
					Title = "CH3",
					Values = new ChartValues<float>(),
					PointGeometry = null,
					LineSmoothness = 0,
					Stroke = Brushes.Green,
					Fill = Brushes.Transparent
				},
				new LineSeries {
					Title = "CH4",
					Values = new ChartValues<float>(),
					PointGeometry = null,
					LineSmoothness = 0,
					Stroke = Brushes.Lime,
					Fill = Brushes.Transparent
				},
				new LineSeries {
					Title = "CH5",
					Values = new ChartValues<float>(),
					PointGeometry = null,
					LineSmoothness = 0,
					Stroke = Brushes.Blue,
					Fill = Brushes.Transparent
				},
				new LineSeries {
					Title = "CH6",
					Values = new ChartValues<float>(),
					PointGeometry = null,
					LineSmoothness = 0,
					Stroke = Brushes.Aqua,
					Fill = Brushes.Transparent
				},
				new LineSeries {
					Title = "CH7",
					Values = new ChartValues<float>(),
					PointGeometry = null,
					LineSmoothness = 0,
					Stroke = Brushes.Purple,
					Fill = Brushes.Transparent
				},
				new LineSeries {
					Title = "CH8",
					Values = new ChartValues<float>(),
					PointGeometry = null,
					LineSmoothness = 0,
					Stroke = Brushes.Violet,
					Fill = Brushes.Transparent
				}
			};

			analogInChart.AxisX.Clear();
			Axis axisX = new Axis { MinValue = 0, MaxValue = graphPoints};
			axisX.Separator = new LiveCharts.Wpf.Separator { Step = (graphPoints / 16.0) };
			analogInChart.AxisX.Add(axisX);

			DataContext = this;
		}

		private void GUIUpdateHandler(object sender, EventArgs e) {
			float rxRate = daq.USBRXDatarate() * 0.001f;
			rxUSBDatarateLabel.Content = rxRate.ToString("0.0");
			//txUSBDatarateLabel.Content = txUSBDatarate.ToString("0.0");
			//rxUSBDroppedLabel.Content = rxUSBDroppedRate.ToString("0.0");

			//analogInACH1kspsLabel.Content = analogInAksps[0].ToString("0.0");
			//analogInACH2kspsLabel.Content = analogInAksps[1].ToString("0.0");
			//analogInACH3kspsLabel.Content = analogInAksps[2].ToString("0.0");
			//analogInACH4kspsLabel.Content = analogInAksps[3].ToString("0.0");

			for(int ch = 1; ch < 9; ch++) {
				float[] chValues = daq.ReadAnalogIn(ch, graphPoints);
				if (chValues != null) {
					SeriesAnalogIn[ch-1].Values = new ChartValues<float>(chValues);
				}
			}

			//float[] channel1 = daq.ReadAnalogIn(1, 1000);
			//if (channel1 != null) {
			//	SeriesAnalogIn[0].Values = new ChartValues<float>(channel1);
			//}
			//float[] channel3 = daq.ReadAnalogIn(3, 1000);
			//if (channel3 != null) {
			//	SeriesAnalogIn[2].Values = new ChartValues<float>(channel3);
			//}
			//float[] channel5 = daq.ReadAnalogIn(5, 1000);
			//if (channel5 != null) {
			//	SeriesAnalogIn[4].Values = new ChartValues<float>(channel5);
			//}
			//float[] channel7 = daq.ReadAnalogIn(7, 1000);
			//if (channel7 != null) {
			//	SeriesAnalogIn[6].Values = new ChartValues<float>(channel7);
			//}

			//SeriesAnalogIn[1].Values = new ChartValues<float>(analogInAData[1]);
			//SeriesAnalogIn[2].Values = new ChartValues<float>(analogInAData[2]);
			//SeriesAnalogIn[3].Values = new ChartValues<float>(analogInAData[3]);
		}

		private void SerialButtonClick(object sender, RoutedEventArgs e) {
			if (daq.IsConnected() == false) {
				if(daq.Connect(portNamesComboBox.Text)) {
					serialButton.Content = "Disconnect";
					serialButton.Background = Brushes.LimeGreen;
				}
			}
			else {
				daq.Disconnect();

				serialButton.Content = "Connect";
				serialButton.Background = Brushes.Red;
			}
		}

		private void CurrentOutputAChanged(object sender, RoutedEventArgs e) {
			if(enableValueChangedEvents == false) {
				return;
			}

			int current = currentOutAComboBox.SelectedIndex * 100;

			daq.AddCurrentOut(1, current);
		}
		private void CurrentOutputBChanged(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			int current = currentOutBComboBox.SelectedIndex * 100;

			daq.AddCurrentOut(2, current);
		}

		private void AnalogInAChanged(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			int sampligRate = (int)(250000.0 / (1 + analogInASamplerate.SelectedIndex));
			daq.SetAnalogInSampligRate(sampligRate);

			byte[] data = new byte[6];

			//data[0] = (byte)analogInAMode.SelectedIndex;
			data[1] = (byte)analogInASamplerate.SelectedIndex;
			data[2] = (byte)analogInAScale.SelectedIndex;

			//SerialSendCommand(Opcodes.setAnalogInA, data);
		}

		private void AnalogInAChannel1Changed(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			float range = (float)Math.Pow(2, (5 - analogInAChannel1Range.SelectedIndex));

			Func<double, string> formatFunc = (x) => string.Format("{0:0.000}", x);

			analogInChart.AxisY.Clear();
			Axis axisY = new Axis { MinValue = -range, MaxValue = range, LabelFormatter = formatFunc };
			axisY.Separator = new LiveCharts.Wpf.Separator { Step = (range/16.0) };
			analogInChart.AxisY.Add(axisY);

			//SerialSendCommand(Opcodes.setAnalogInACH, data);
			daq.AddAnalogIn(1, range, (AnalogInMode)analogInAChannel1Mode.SelectedIndex);
		}

		private void AnalogInAChannel2Changed(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			float range = (float)Math.Pow(2, (5 - analogInAChannel2Range.SelectedIndex));

			//analogInChart.AxisY.Clear();
			//Axis axisY = new Axis { MinValue = -range, MaxValue = range };
			//axisY.Separator = new LiveCharts.Wpf.Separator { Step = (range / 16.0) };
			//analogInChart.AxisY.Add(axisY);

			//SerialSendCommand(Opcodes.setAnalogInACH, data);
			daq.AddAnalogIn(2, range, (AnalogInMode)analogInAChannel1Mode.SelectedIndex);
		}

		private void AnalogInAChannel3Changed(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			float range = (float)Math.Pow(2, (5 - analogInAChannel3Range.SelectedIndex));

			//analogInChart.AxisY.Clear();
			//Axis axisY = new Axis { MinValue = -range, MaxValue = range };
			//axisY.Separator = new LiveCharts.Wpf.Separator { Step = (range / 16.0) };
			//analogInChart.AxisY.Add(axisY);

			//SerialSendCommand(Opcodes.setAnalogInACH, data);
			daq.AddAnalogIn(3, range, (AnalogInMode)analogInAChannel3Mode.SelectedIndex);
		}

		private void AnalogInAChannel4Changed(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			float range = (float)Math.Pow(2, (5 - analogInAChannel4Range.SelectedIndex));

			//analogInChart.AxisY.Clear();
			//Axis axisY = new Axis { MinValue = -range, MaxValue = range };
			//axisY.Separator = new LiveCharts.Wpf.Separator { Step = (range / 16.0) };
			//analogInChart.AxisY.Add(axisY);

			//SerialSendCommand(Opcodes.setAnalogInACH, data);
			daq.AddAnalogIn(4, range, (AnalogInMode)analogInAChannel3Mode.SelectedIndex);
		}

		private void AnalogInAChannel5Changed(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			float range = (float)Math.Pow(2, (5 - analogInAChannel5Range.SelectedIndex));

			//analogInChart.AxisY.Clear();
			//Axis axisY = new Axis { MinValue = -range, MaxValue = range };
			//axisY.Separator = new LiveCharts.Wpf.Separator { Step = (range / 16.0) };
			//analogInChart.AxisY.Add(axisY);

			//SerialSendCommand(Opcodes.setAnalogInACH, data);
			daq.AddAnalogIn(5, range, (AnalogInMode)analogInAChannel5Mode.SelectedIndex);
		}

		private void AnalogInAChannel6Changed(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			float range = (float)Math.Pow(2, (5 - analogInAChannel6Range.SelectedIndex));

			//analogInChart.AxisY.Clear();
			//Axis axisY = new Axis { MinValue = -range, MaxValue = range };
			//axisY.Separator = new LiveCharts.Wpf.Separator { Step = (range / 16.0) };
			//analogInChart.AxisY.Add(axisY);

			//SerialSendCommand(Opcodes.setAnalogInACH, data);
			daq.AddAnalogIn(6, range, (AnalogInMode)analogInAChannel5Mode.SelectedIndex);
		}

		private void AnalogInAChannel7Changed(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			float range = (float)Math.Pow(2, (5 - analogInAChannel7Range.SelectedIndex));

			//analogInChart.AxisY.Clear();
			//Axis axisY = new Axis { MinValue = -range, MaxValue = range };
			//axisY.Separator = new LiveCharts.Wpf.Separator { Step = (range / 16.0) };
			//analogInChart.AxisY.Add(axisY);

			//SerialSendCommand(Opcodes.setAnalogInACH, data);
			daq.AddAnalogIn(7, range, (AnalogInMode)analogInAChannel7Mode.SelectedIndex);
		}

		private void AnalogInAChannel8Changed(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			float range = (float)Math.Pow(2, (5 - analogInAChannel8Range.SelectedIndex));

			//analogInChart.AxisY.Clear();
			//Axis axisY = new Axis { MinValue = -range, MaxValue = range };
			//axisY.Separator = new LiveCharts.Wpf.Separator { Step = (range / 16.0) };
			//analogInChart.AxisY.Add(axisY);

			//SerialSendCommand(Opcodes.setAnalogInACH, data);
			daq.AddAnalogIn(8, range, (AnalogInMode)analogInAChannel7Mode.SelectedIndex);
		}

		private void AnalogOutOffsetValidation(object sender, TextCompositionEventArgs e) {
			Regex regex = new Regex("[^0-9-]+");

			bool isNumber = regex.IsMatch(e.Text);

			if(isNumber) {
				//Get Whole text in textbox
				string offsetStr = ((TextBox)sender).Text + e.Text;

				//Convert to int
				int offset = 0;
				int.TryParse(offsetStr, out offset);

				//Check input range
				if(offset > 12500) {
					offset = 12500;
				}
				else if(offset < -12500) {
					offset = -12500;
				}

				((TextBox)sender).Text = offset.ToString();

				e.Handled = true;
			}
			else {
				e.Handled = false;
			}
		}

		private void AnalogOutFrequencyValidation(object sender, TextCompositionEventArgs e) {
			Regex regex = new Regex("[^0-9]+");

			bool isNumber = regex.IsMatch(e.Text);
			if (isNumber) {
				e.Handled = true;
			}
			else {
				e.Handled = false;
			}
		}

		private void AnalogOutAmplitudeValidation(object sender, TextCompositionEventArgs e) {
			Regex regex = new Regex("[^0-9]+");

			bool isNumber = regex.IsMatch(e.Text);
			if (isNumber) {
				//Get Whole text in textbox
				string ampStr = ((TextBox)sender).Text + e.Text;

				//Convert to int
				int amplitude = 0;
				int.TryParse(ampStr, out amplitude);

				//Check input range
				if (amplitude > 30000) {
					amplitude = 30000;
				}

				//Convert to valid values, DAC steps
				float step = 30000 / 4096;
				int nSteps = Convert.ToInt32(amplitude / step);
				amplitude = Convert.ToInt32(nSteps * step);

				((TextBox)sender).Text = amplitude.ToString();

				e.Handled = true;
			}
			else {
				e.Handled = false;
			}
		}

		private void AnalogOutDCValidation(object sender, TextCompositionEventArgs e) {
			Regex regex = new Regex("[^0-9]+");

			bool isNumber = regex.IsMatch(e.Text);
			if (isNumber) {
				//Get Whole text in textbox
				string dcStr = ((TextBox)sender).Text + e.Text;

				//Convert to int
				int dc = 0;
				int.TryParse(dcStr, out dc);

				//Check input range
				if (dc > 100) {
					dc = 100;
				}

				((TextBox)sender).Text = dc.ToString();

				e.Handled = true;
			}
			else {
				e.Handled = false;
			}
		}
		private void AnalogOutAChannel1Changed(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			int offset = 0;
			int.TryParse(analogOutAChannel1Offset.Text, out offset);

			int freq = 0;
			int.TryParse(analogOutAChannel1Freq.Text, out freq);

			int amp = 0;
			int.TryParse(analogOutAChannel1Amp.Text, out amp);

			int dc = 0;
			int.TryParse(analogOutAChannel1DC.Text, out dc);

			float baseFreq = 10000000;
			float freqDiv = baseFreq / freq;

			//Generate the signal pointds
			UInt16[] buffer = new UInt16[20];
			buffer[0] = 2048;
			buffer[1] = 2680;
			buffer[2] = 3252;
			buffer[3] = 3705;
			buffer[4] = 3996;
			buffer[5] = 4095;
			buffer[6] = 3996;
			buffer[7] = 3705;
			buffer[8] = 3252;
			buffer[9] = 2680;
			buffer[10] = 2048;
			buffer[11] = 1415;
			buffer[12] = 844;
			buffer[13] = 391;
			buffer[14] = 100;
			buffer[15] = 0;
			buffer[16] = 100;
			buffer[17] = 391;
			buffer[18] = 844;
			buffer[19] = 1415;

			int bufferLength = buffer.Length;

			//Generate the packet
			byte[] data = new byte[6 + (bufferLength*2)];

			data[0] = 1;
			data[1] = (byte)(freq >> 16);
			data[2] = (byte)(freq >> 8);
			data[3] = (byte)freq;
			data[4] = (byte)(bufferLength >> 8);
			data[5] = (byte)(bufferLength);

			for(int i = 0; i < bufferLength; i++) {
				data[6 + (2 * i)] = (byte)(buffer[i] >> 8);
				data[6 + (2 * i + 1)] = (byte)(buffer[i]);
			}

			daq.AddAnalogOutput(1, (offset / 1000.0));

			//SerialSendCommand(Opcodes.setAnalogOutACH, data);
		}

		private void AnalogOutAChannel2Changed(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			int offset = 0;
			int.TryParse(analogOutAChannel2Offset.Text, out offset);

			int freq = 0;
			int.TryParse(analogOutAChannel2Freq.Text, out freq);

			int amp = 0;
			int.TryParse(analogOutAChannel2Amp.Text, out amp);

			int dc = 0;
			int.TryParse(analogOutAChannel2DC.Text, out dc);

			float baseFreq = 10000000;
			float freqDiv = baseFreq / freq;

			daq.AddAnalogOutput(2, (offset / 1000.0));

			//SerialSendCommand(Opcodes.setAnalogOutACH, data);
		}
	}
}
