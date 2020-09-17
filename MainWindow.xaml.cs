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

		int[] analogInADataIndex = new int[8];
		float[] analogInRanges = new float[8];
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
			analogInRanges[0] = 0;
			analogInRanges[1] = 0;
			analogInRanges[2] = 0;
			analogInRanges[3] = 0;
			analogInRanges[4] = 0;
			analogInRanges[5] = 0;
			analogInRanges[6] = 0;
			analogInRanges[7] = 0;
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

		private void OnWindowClosing(object sender, EventArgs e) {
			daq.Disconnect();
		}

		private void GUIUpdateHandler(object sender, EventArgs e) {
			float rxRate = daq.USBRXDatarate() * 0.001f;
			rxUSBDatarateLabel.Content = rxRate.ToString("0.0");

			float rxErrorRate = daq.USBRXErrorRate();
			rxUSBDroppedLabel.Content = rxErrorRate.ToString("0.0");

			//analogInACH1kspsLabel.Content = analogInAksps[0].ToString("0.0");
			//analogInACH2kspsLabel.Content = analogInAksps[1].ToString("0.0");
			//analogInACH3kspsLabel.Content = analogInAksps[2].ToString("0.0");
			//analogInACH4kspsLabel.Content = analogInAksps[3].ToString("0.0");
			double avg = 0;
			double sumSquares = 0;
			for (int ch = 1; ch < 9; ch++) {
				float[] chValues = daq.ReadAnalogInVolt(ch, graphPoints);
				if (chValues != null) {
					SeriesAnalogIn[ch - 1].Values = new ChartValues<float>(chValues);

					//Calculate Standart Deviation in ADC counts, raw mode
					int[] rawValues = daq.ReadAnalogIn(ch, graphPoints);

					avg = rawValues.Average();
					sumSquares = 0;
					for (int i = 0; i < rawValues.Length; i++) {
						sumSquares += (rawValues[i] - avg) * (rawValues[i] - avg);
					}
					double stdDev = Math.Sqrt(sumSquares / rawValues.Length);

					if (ch == 1) {
						stdCH1Label.Content = "CH1 Std: " + stdDev.ToString("F2");
					}
					else if (ch == 3) {
						stdCH3Label.Content = "CH3 Std: " + stdDev.ToString("F2");
					}
					else if (ch == 5) {
						stdCH5Label.Content = "CH5 Std: " + stdDev.ToString("F2");
					}
					else if (ch == 7) {
						stdCH7Label.Content = "CH7 Std: " + stdDev.ToString("F2");
					}
				}
			}
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

			analogInRanges[0] = range;

			float maxRange = 0;
			for(int i = 0; i < analogInRanges.Length; i++) {
				if(analogInRanges[i] > maxRange) {
					maxRange = analogInRanges[i];
				}
			}

			Func<double, string> formatFunc = (x) => string.Format("{0:0.000}", x);

			analogInChart.AxisY.Clear();
			Axis axisY = new Axis { MinValue = -maxRange, MaxValue = maxRange, LabelFormatter = formatFunc };
			axisY.Separator = new LiveCharts.Wpf.Separator { Step = (maxRange / 16.0) };
			analogInChart.AxisY.Add(axisY);

			daq.AddAnalogIn(1, range, (AnalogInMode)analogInAChannel1Mode.SelectedIndex);
		}

		private void AnalogInAChannel2Changed(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			float range = (float)Math.Pow(2, (5 - analogInAChannel2Range.SelectedIndex));

			analogInRanges[1] = range;

			float maxRange = 0;
			for (int i = 0; i < analogInRanges.Length; i++) {
				if (analogInRanges[i] > maxRange) {
					maxRange = analogInRanges[i];
				}
			}

			Func<double, string> formatFunc = (x) => string.Format("{0:0.000}", x);

			analogInChart.AxisY.Clear();
			Axis axisY = new Axis { MinValue = -maxRange, MaxValue = maxRange, LabelFormatter = formatFunc };
			axisY.Separator = new LiveCharts.Wpf.Separator { Step = (maxRange / 16.0) };
			analogInChart.AxisY.Add(axisY);

			daq.AddAnalogIn(2, range, (AnalogInMode)analogInAChannel1Mode.SelectedIndex);
		}

		private void AnalogInAChannel3Changed(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			float range = (float)Math.Pow(2, (5 - analogInAChannel3Range.SelectedIndex));

			analogInRanges[2] = range;

			float maxRange = 0;
			for (int i = 0; i < analogInRanges.Length; i++) {
				if (analogInRanges[i] > maxRange) {
					maxRange = analogInRanges[i];
				}
			}

			Func<double, string> formatFunc = (x) => string.Format("{0:0.000}", x);

			analogInChart.AxisY.Clear();
			Axis axisY = new Axis { MinValue = -maxRange, MaxValue = maxRange, LabelFormatter = formatFunc };
			axisY.Separator = new LiveCharts.Wpf.Separator { Step = (maxRange / 16.0) };
			analogInChart.AxisY.Add(axisY);

			daq.AddAnalogIn(3, range, (AnalogInMode)analogInAChannel3Mode.SelectedIndex);
		}

		private void AnalogInAChannel4Changed(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			float range = (float)Math.Pow(2, (5 - analogInAChannel4Range.SelectedIndex));

			analogInRanges[3] = range;

			float maxRange = 0;
			for (int i = 0; i < analogInRanges.Length; i++) {
				if (analogInRanges[i] > maxRange) {
					maxRange = analogInRanges[i];
				}
			}

			Func<double, string> formatFunc = (x) => string.Format("{0:0.000}", x);

			analogInChart.AxisY.Clear();
			Axis axisY = new Axis { MinValue = -maxRange, MaxValue = maxRange, LabelFormatter = formatFunc };
			axisY.Separator = new LiveCharts.Wpf.Separator { Step = (maxRange / 16.0) };
			analogInChart.AxisY.Add(axisY);

			daq.AddAnalogIn(4, range, (AnalogInMode)analogInAChannel3Mode.SelectedIndex);
		}

		private void AnalogInAChannel5Changed(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			float range = (float)Math.Pow(2, (5 - analogInAChannel5Range.SelectedIndex));

			analogInRanges[4] = range;

			float maxRange = 0;
			for (int i = 0; i < analogInRanges.Length; i++) {
				if (analogInRanges[i] > maxRange) {
					maxRange = analogInRanges[i];
				}
			}

			Func<double, string> formatFunc = (x) => string.Format("{0:0.000}", x);

			analogInChart.AxisY.Clear();
			Axis axisY = new Axis { MinValue = -maxRange, MaxValue = maxRange, LabelFormatter = formatFunc };
			axisY.Separator = new LiveCharts.Wpf.Separator { Step = (maxRange / 16.0) };
			analogInChart.AxisY.Add(axisY);

			daq.AddAnalogIn(5, range, (AnalogInMode)analogInAChannel5Mode.SelectedIndex);
		}

		private void AnalogInAChannel6Changed(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			float range = (float)Math.Pow(2, (5 - analogInAChannel6Range.SelectedIndex));

			analogInRanges[5] = range;

			float maxRange = 0;
			for (int i = 0; i < analogInRanges.Length; i++) {
				if (analogInRanges[i] > maxRange) {
					maxRange = analogInRanges[i];
				}
			}

			Func<double, string> formatFunc = (x) => string.Format("{0:0.000}", x);

			analogInChart.AxisY.Clear();
			Axis axisY = new Axis { MinValue = -maxRange, MaxValue = maxRange, LabelFormatter = formatFunc };
			axisY.Separator = new LiveCharts.Wpf.Separator { Step = (maxRange / 16.0) };
			analogInChart.AxisY.Add(axisY);

			daq.AddAnalogIn(6, range, (AnalogInMode)analogInAChannel5Mode.SelectedIndex);
		}

		private void AnalogInAChannel7Changed(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			float range = (float)Math.Pow(2, (5 - analogInAChannel7Range.SelectedIndex));

			analogInRanges[6] = range;

			float maxRange = 0;
			for (int i = 0; i < analogInRanges.Length; i++) {
				if (analogInRanges[i] > maxRange) {
					maxRange = analogInRanges[i];
				}
			}

			Func<double, string> formatFunc = (x) => string.Format("{0:0.000}", x);

			analogInChart.AxisY.Clear();
			Axis axisY = new Axis { MinValue = -maxRange, MaxValue = maxRange, LabelFormatter = formatFunc };
			axisY.Separator = new LiveCharts.Wpf.Separator { Step = (maxRange / 16.0) };
			analogInChart.AxisY.Add(axisY);

			daq.AddAnalogIn(7, range, (AnalogInMode)analogInAChannel7Mode.SelectedIndex);
		}

		private void AnalogInAChannel8Changed(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			float range = (float)Math.Pow(2, (5 - analogInAChannel8Range.SelectedIndex));

			analogInRanges[7] = range;

			float maxRange = 0;
			for (int i = 0; i < analogInRanges.Length; i++) {
				if (analogInRanges[i] > maxRange) {
					maxRange = analogInRanges[i];
				}
			}

			Func<double, string> formatFunc = (x) => string.Format("{0:0.000}", x);

			analogInChart.AxisY.Clear();
			Axis axisY = new Axis { MinValue = -maxRange, MaxValue = maxRange, LabelFormatter = formatFunc };
			axisY.Separator = new LiveCharts.Wpf.Separator { Step = (maxRange / 16.0) };
			analogInChart.AxisY.Add(axisY);

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
				//Get Whole text in textbox
				string freqStr = ((TextBox)sender).Text + e.Text;

				//Convert to int
				int frequency = 0;
				int.TryParse(freqStr, out frequency);

				//Check input range
				if (frequency > 100000) {
					frequency = 100000;
				}

				((TextBox)sender).Text = frequency.ToString();

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
				if (amplitude > 25000) {
					amplitude = 25000;
				}

				//Convert to valid values, DAC steps
				float step = 25000 / 4096;
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
			if (offset > 12500) {
				offset = 12500;
			}
			else if (offset < -12500) {
				offset = -12500;
			}

			int freq = 0;
			int.TryParse(analogOutAChannel1Freq.Text, out freq);
			if (freq > 100000) {
				freq = 100000;
			}

			int amplitude = 0;
			int.TryParse(analogOutAChannel1Amp.Text, out amplitude);
			if (amplitude > 25000) {
				amplitude = 25000;
			}

			int dutyCycle = 0;
			int.TryParse(analogOutAChannel1DC.Text, out dutyCycle);
			if (dutyCycle > 100) {
				dutyCycle = 100;
			}

			if (analogOutAChannel1Mode.SelectedIndex == 0) {
				//OFF Mode: DC output with 0V
				double[] values = new double[1];

				values[0] = 0;

				daq.AddAnalogOutput(1, values, 0);
			}
			else if (analogOutAChannel1Mode.SelectedIndex == 1) {
				//DC Mode
				double[] values = new double[1];

				values[0] = (offset / 1000.0);

				daq.AddAnalogOutput(1, values, 0);
			}
			else if (analogOutAChannel1Mode.SelectedIndex == 2) {
				//Sine Mode
				const int maxSampleFreq = 2000000;  //For Sine 2 MHz seems to work fine

				if(freq == 0) {
					return;
				}

				int bufferLen = maxSampleFreq / freq;
				if(bufferLen > 100) {
					bufferLen = 100;
				}

				//Sine Generation
				double[] values = new double[bufferLen];
				for (int i = 0; i < values.Length; i++) {
					values[i] = (offset / 1000.0) + (amplitude / (2 * 1000.0)) * Math.Sin(2 * Math.PI * (i / (double)values.Length));
				}

				freq = freq * values.Length;

				daq.AddAnalogOutput(1, values, freq);
			}
			else if (analogOutAChannel1Mode.SelectedIndex == 3) {
				//Square Mode
				const int maxSampleFreq = 1000000;  //Normal max sample rate of 1MHz, from datasheet

				if (freq == 0) {
					return;
				}

				int bufferLen = maxSampleFreq / freq;
				if (bufferLen > 100) {
					bufferLen = 100;
				}

				//Sqaure Wave generation
				int transitionIndex = ((bufferLen * (100 - dutyCycle)) / 100);
				double[] values = new double[bufferLen];
				for (int i = 0; i < values.Length; i++) {
					if(i < transitionIndex) {
						//PWM/Square Wave Low time
						values[i] = (offset / 1000.0) - (amplitude / (2 * 1000.0));
					}
					else {
						//PWM/Square Wave High time
						values[i] = (offset / 1000.0) + (amplitude / (2 * 1000.0));
					}
				}

				freq = freq * values.Length;

				daq.AddAnalogOutput(1, values, freq);
			}
			else if (analogOutAChannel1Mode.SelectedIndex == 4) {
				//Ramp Mode
				const int maxSampleFreq = 2000000;  //For Ramps 2 MHz seems to work fine

				if (freq == 0) {
					return;
				}

				int bufferLen = maxSampleFreq / freq;
				if (bufferLen > 100) {
					bufferLen = 100;
				}

				//Ramp Wave generation
				int transitionIndex = ((bufferLen * (100 - dutyCycle)) / 100);
				double[] values = new double[bufferLen];
				for (int i = 0; i < values.Length; i++) {
					if (i < transitionIndex) {
						//Ramp Rising Edge time
						double step = ((amplitude / 1000.0) * (i / (double)transitionIndex));
						values[i] = (offset / 1000.0) - (amplitude / (2 * 1000.0)) + step;
					}
					else {
						//Ramp Falling Edge time
						double step = ((amplitude / 1000.0) * ((i - transitionIndex) / (double)(values.Length - transitionIndex)));
						values[i] = (offset / 1000.0) + (amplitude / (2 * 1000.0)) - step;
					}
				}

				freq = freq * values.Length;

				daq.AddAnalogOutput(1, values, freq);
			}
			else if (analogOutAChannel1Mode.SelectedIndex == 5) {
				//Noise Mode
			}
		}

		private void AnalogOutAChannel2Changed(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			int offset = 0;
			int.TryParse(analogOutAChannel2Offset.Text, out offset);
			if (offset > 12500) {
				offset = 12500;
			}
			else if (offset < -12500) {
				offset = -12500;
			}

			int freq = 0;
			int.TryParse(analogOutAChannel2Freq.Text, out freq);
			if (freq > 100000) {
				freq = 100000;
			}

			int amplitude = 0;
			int.TryParse(analogOutAChannel2Amp.Text, out amplitude);
			if (amplitude > 25000) {
				amplitude = 25000;
			}

			int dutyCycle = 0;
			int.TryParse(analogOutAChannel2DC.Text, out dutyCycle);
			if (dutyCycle > 100) {
				dutyCycle = 100;
			}

			if (analogOutAChannel2Mode.SelectedIndex == 0) {
				//OFF Mode: DC output with 0V
				double[] values = new double[1];

				values[0] = 0;

				daq.AddAnalogOutput(2, values, 0);
			}
			else if (analogOutAChannel2Mode.SelectedIndex == 1) {
				//DC Mode
				double[] values = new double[1];

				values[0] = (offset / 1000.0);

				daq.AddAnalogOutput(2, values, 0);
			}
			else if (analogOutAChannel2Mode.SelectedIndex == 2) {
				//Sine Mode
				const int maxSampleFreq = 2000000;  //For Sine 2 MHz seem to work fine

				if (freq == 0) {
					return;
				}

				int bufferLen = maxSampleFreq / freq;
				if (bufferLen > 100) {
					bufferLen = 100;
				}

				//Sine Generation
				double[] values = new double[bufferLen];
				for (int i = 0; i < values.Length; i++) {
					values[i] = (offset / 1000.0) + (amplitude / (2 * 1000.0)) * Math.Sin(2 * Math.PI * (i / (double)values.Length));
				}

				freq = freq * values.Length;

				daq.AddAnalogOutput(2, values, freq);
			}
			else if (analogOutAChannel2Mode.SelectedIndex == 3) {
				//Square Mode
				const int maxSampleFreq = 1000000;  //Normal max sample rate of 1MHz, from datasheet

				if (freq == 0) {
					return;
				}

				int bufferLen = maxSampleFreq / freq;
				if (bufferLen > 100) {
					bufferLen = 100;
				}

				//Sqaure Wave generation
				int transitionIndex = ((bufferLen * (100 - dutyCycle)) / 100);
				double[] values = new double[bufferLen];
				for (int i = 0; i < values.Length; i++) {
					if (i < transitionIndex) {
						//PWM/Square Wave Low time
						values[i] = (offset / 1000.0) - (amplitude / (2 * 1000.0));
					}
					else {
						//PWM/Square Wave High time
						values[i] = (offset / 1000.0) + (amplitude / (2 * 1000.0));
					}
				}

				freq = freq * values.Length;

				daq.AddAnalogOutput(2, values, freq);
			}
			else if (analogOutAChannel2Mode.SelectedIndex == 4) {
				//Ramp Mode
				const int maxSampleFreq = 2000000;  //For Ramps 2 MHz seems to work fine

				if (freq == 0) {
					return;
				}

				int bufferLen = maxSampleFreq / freq;
				if (bufferLen > 100) {
					bufferLen = 100;
				}

				//Ramp Wave generation
				int transitionIndex = ((bufferLen * (100 - dutyCycle)) / 100);
				double[] values = new double[bufferLen];
				for (int i = 0; i < values.Length; i++) {
					if (i < transitionIndex) {
						//Ramp Rising Edge time
						double step = ((amplitude / 1000.0) * (i / (double)transitionIndex));
						values[i] = (offset / 1000.0) - (amplitude / (2 * 1000.0)) + step;
					}
					else {
						//Ramp Falling Edge time
						double step = ((amplitude / 1000.0) * ((i - transitionIndex) / (double)(values.Length - transitionIndex)));
						values[i] = (offset / 1000.0) + (amplitude / (2 * 1000.0)) - step;
					}
				}

				freq = freq * values.Length;

				daq.AddAnalogOutput(2, values, freq);
			}
			else if (analogOutAChannel2Mode.SelectedIndex == 5) {
				//Noise Mode
			}
		}

		private void AnalogOutBChannel1Changed(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			int offset = 0;
			int.TryParse(analogOutBChannel1Offset.Text, out offset);
			if (offset > 12500) {
				offset = 12500;
			}
			else if (offset < -12500) {
				offset = -12500;
			}

			int freq = 0;
			int.TryParse(analogOutBChannel1Freq.Text, out freq);
			if (freq > 100000) {
				freq = 100000;
			}

			int amplitude = 0;
			int.TryParse(analogOutBChannel1Amp.Text, out amplitude);
			if (amplitude > 25000) {
				amplitude = 25000;
			}

			int dutyCycle = 0;
			int.TryParse(analogOutBChannel1DC.Text, out dutyCycle);
			if (dutyCycle > 100) {
				dutyCycle = 100;
			}

			if (analogOutBChannel1Mode.SelectedIndex == 0) {
				//OFF Mode: DC output with 0V
				double[] values = new double[1];

				values[0] = 0;

				daq.AddAnalogOutput(3, values, 0);
			}
			else if (analogOutBChannel1Mode.SelectedIndex == 1) {
				//DC Mode
				double[] values = new double[1];

				values[0] = (offset / 1000.0);

				daq.AddAnalogOutput(3, values, 0);
			}
			else if (analogOutBChannel1Mode.SelectedIndex == 2) {
				//Sine Mode
				const int maxSampleFreq = 2000000;  //For Sine 2 MHz seem to work fine

				if (freq == 0) {
					return;
				}

				int bufferLen = maxSampleFreq / freq;
				if (bufferLen > 100) {
					bufferLen = 100;
				}

				//Sine Generation
				double[] values = new double[bufferLen];
				for (int i = 0; i < values.Length; i++) {
					values[i] = (offset / 1000.0) + (amplitude / (2 * 1000.0)) * Math.Sin(2 * Math.PI * (i / (double)values.Length));
				}

				freq = freq * values.Length;

				daq.AddAnalogOutput(3, values, freq);
			}
			else if (analogOutBChannel1Mode.SelectedIndex == 3) {
				//Square Mode
				const int maxSampleFreq = 1000000;  //Normal max sample rate of 1MHz, from datasheet

				if (freq == 0) {
					return;
				}

				int bufferLen = maxSampleFreq / freq;
				if (bufferLen > 100) {
					bufferLen = 100;
				}

				//Sqaure Wave generation
				int transitionIndex = ((bufferLen * (100 - dutyCycle)) / 100);
				double[] values = new double[bufferLen];
				for (int i = 0; i < values.Length; i++) {
					if (i < transitionIndex) {
						//PWM/Square Wave Low time
						values[i] = (offset / 1000.0) - (amplitude / (2 * 1000.0));
					}
					else {
						//PWM/Square Wave High time
						values[i] = (offset / 1000.0) + (amplitude / (2 * 1000.0));
					}
				}

				freq = freq * values.Length;

				daq.AddAnalogOutput(3, values, freq);
			}
			else if (analogOutBChannel1Mode.SelectedIndex == 4) {
				//Ramp Mode
				const int maxSampleFreq = 2000000;  //For Ramps 2 MHz seems to work fine

				if (freq == 0) {
					return;
				}

				int bufferLen = maxSampleFreq / freq;
				if (bufferLen > 100) {
					bufferLen = 100;
				}

				//Ramp Wave generation
				int transitionIndex = ((bufferLen * (100 - dutyCycle)) / 100);
				double[] values = new double[bufferLen];
				for (int i = 0; i < values.Length; i++) {
					if (i < transitionIndex) {
						//Ramp Rising Edge time
						double step = ((amplitude / 1000.0) * (i / (double)transitionIndex));
						values[i] = (offset / 1000.0) - (amplitude / (2 * 1000.0)) + step;
					}
					else {
						//Ramp Falling Edge time
						double step = ((amplitude / 1000.0) * ((i - transitionIndex) / (double)(values.Length - transitionIndex)));
						values[i] = (offset / 1000.0) + (amplitude / (2 * 1000.0)) - step;
					}
				}

				freq = freq * values.Length;

				daq.AddAnalogOutput(3, values, freq);
			}
			else if (analogOutBChannel1Mode.SelectedIndex == 5) {
				//Noise Mode
			}
		}

		private void AnalogOutBChannel2Changed(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			int offset = 0;
			int.TryParse(analogOutBChannel2Offset.Text, out offset);
			if (offset > 12500) {
				offset = 12500;
			}
			else if (offset < -12500) {
				offset = -12500;
			}

			int freq = 0;
			int.TryParse(analogOutBChannel2Freq.Text, out freq);
			if (freq > 100000) {
				freq = 100000;
			}

			int amplitude = 0;
			int.TryParse(analogOutBChannel2Amp.Text, out amplitude);
			if (amplitude > 25000) {
				amplitude = 25000;
			}

			int dutyCycle = 0;
			int.TryParse(analogOutBChannel2DC.Text, out dutyCycle);
			if (dutyCycle > 100) {
				dutyCycle = 100;
			}

			if (analogOutBChannel2Mode.SelectedIndex == 0) {
				//OFF Mode: DC output with 0V
				double[] values = new double[1];

				values[0] = 0;

				daq.AddAnalogOutput(4, values, 0);
			}
			else if (analogOutBChannel2Mode.SelectedIndex == 1) {
				//DC Mode
				double[] values = new double[1];

				values[0] = (offset / 1000.0);

				daq.AddAnalogOutput(4, values, 0);
			}
			else if (analogOutBChannel2Mode.SelectedIndex == 2) {
				//Sine Mode
				const int maxSampleFreq = 2000000;  //For Sine 2 MHz seem to work fine

				if (freq == 0) {
					return;
				}

				int bufferLen = maxSampleFreq / freq;
				if (bufferLen > 100) {
					bufferLen = 100;
				}

				//Sine Generation
				double[] values = new double[bufferLen];
				for (int i = 0; i < values.Length; i++) {
					values[i] = (offset / 1000.0) + (amplitude / (2 * 1000.0)) * Math.Sin(2 * Math.PI * (i / (double)values.Length));
				}

				freq = freq * values.Length;

				daq.AddAnalogOutput(4, values, freq);
			}
			else if (analogOutBChannel2Mode.SelectedIndex == 3) {
				//Square Mode
				const int maxSampleFreq = 1000000;  //Normal max sample rate of 1MHz, from datasheet

				if (freq == 0) {
					return;
				}

				int bufferLen = maxSampleFreq / freq;
				if (bufferLen > 100) {
					bufferLen = 100;
				}

				//Sqaure Wave generation
				int transitionIndex = ((bufferLen * (100 - dutyCycle)) / 100);
				double[] values = new double[bufferLen];
				for (int i = 0; i < values.Length; i++) {
					if (i < transitionIndex) {
						//PWM/Square Wave Low time
						values[i] = (offset / 1000.0) - (amplitude / (2 * 1000.0));
					}
					else {
						//PWM/Square Wave High time
						values[i] = (offset / 1000.0) + (amplitude / (2 * 1000.0));
					}
				}

				freq = freq * values.Length;

				daq.AddAnalogOutput(4, values, freq);
			}
			else if (analogOutBChannel2Mode.SelectedIndex == 4) {
				//Ramp Mode
				const int maxSampleFreq = 2000000;  //For Ramps 2 MHz seems to work fine

				if (freq == 0) {
					return;
				}

				int bufferLen = maxSampleFreq / freq;
				if (bufferLen > 100) {
					bufferLen = 100;
				}

				//Ramp Wave generation
				int transitionIndex = ((bufferLen * (100 - dutyCycle)) / 100);
				double[] values = new double[bufferLen];
				for (int i = 0; i < values.Length; i++) {
					if (i < transitionIndex) {
						//Ramp Rising Edge time
						double step = ((amplitude / 1000.0) * (i / (double)transitionIndex));
						values[i] = (offset / 1000.0) - (amplitude / (2 * 1000.0)) + step;
					}
					else {
						//Ramp Falling Edge time
						double step = ((amplitude / 1000.0) * ((i - transitionIndex) / (double)(values.Length - transitionIndex)));
						values[i] = (offset / 1000.0) + (amplitude / (2 * 1000.0)) - step;
					}
				}

				freq = freq * values.Length;

				daq.AddAnalogOutput(4, values, freq);
			}
			else if (analogOutBChannel2Mode.SelectedIndex == 5) {
				//Noise Mode
			}
		}
	}
}
