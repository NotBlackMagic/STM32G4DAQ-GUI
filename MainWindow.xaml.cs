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

using NotBlackMagic;

namespace STM32G4DAQ {
	public partial class MainWindow : Window {
		STMDAQ daq = new STMDAQ();

		const int graphPoints = 512;

		float[] analogInARanges = new float[8];
		double[][] analogInAData = new double[8][];
		ScottPlot.PlottableSignal[] plottableSignalsA = new ScottPlot.PlottableSignal[8];

		float[] analogInBRanges = new float[8];
		double[][] analogInBData = new double[8][];
		ScottPlot.PlottableSignal[] plottableSignalsB = new ScottPlot.PlottableSignal[8];

		bool enableValueChangedEvents = false;

		public MainWindow() {
			enableValueChangedEvents = false;
			InitializeComponent();
			enableValueChangedEvents = true;

			//Init Variables
			analogInARanges[0] = 0;
			analogInARanges[1] = 0;
			analogInARanges[2] = 0;
			analogInARanges[3] = 0;
			analogInARanges[4] = 0;
			analogInARanges[5] = 0;
			analogInARanges[6] = 0;
			analogInARanges[7] = 0;
			analogInAData[0] = new double[graphPoints];
			analogInAData[1] = new double[graphPoints];
			analogInAData[2] = new double[graphPoints];
			analogInAData[3] = new double[graphPoints];
			analogInAData[4] = new double[graphPoints];
			analogInAData[5] = new double[graphPoints];
			analogInAData[6] = new double[graphPoints];
			analogInAData[7] = new double[graphPoints];

			analogInBRanges[0] = 0;
			analogInBRanges[1] = 0;
			analogInBRanges[2] = 0;
			analogInBRanges[3] = 0;
			analogInBRanges[4] = 0;
			analogInBRanges[5] = 0;
			analogInBRanges[6] = 0;
			analogInBRanges[7] = 0;
			analogInBData[0] = new double[graphPoints];
			analogInBData[1] = new double[graphPoints];
			analogInBData[2] = new double[graphPoints];
			analogInBData[3] = new double[graphPoints];
			analogInBData[4] = new double[graphPoints];
			analogInBData[5] = new double[graphPoints];
			analogInBData[6] = new double[graphPoints];
			analogInBData[7] = new double[graphPoints];

			//Init GUI Update dispatcher
			DispatcherTimer dispatcherTimer = new DispatcherTimer();
			dispatcherTimer.Interval = TimeSpan.FromMilliseconds(100);
			dispatcherTimer.Tick += GUIUpdateHandler;
			dispatcherTimer.Start();

			//Init Graph
			//plottableSignalsA[0] = CartesianChart.plt.PlotSignal(analogInAData[0], label: "CH1A", color: System.Drawing.Color.Red, lineWidth: 2);
			//plottableSignalsA[1] = CartesianChart.plt.PlotSignal(analogInAData[1], label: "CH2A", color: System.Drawing.Color.Orange, lineWidth: 2);
			//plottableSignalsA[2] = CartesianChart.plt.PlotSignal(analogInAData[2], label: "CH3A", color: System.Drawing.Color.Green, lineWidth: 2);
			//plottableSignalsA[3] = CartesianChart.plt.PlotSignal(analogInAData[3], label: "CH4A", color: System.Drawing.Color.Lime, lineWidth: 2);
			//plottableSignalsA[4] = CartesianChart.plt.PlotSignal(analogInAData[4], label: "CH5A", color: System.Drawing.Color.Blue, lineWidth: 2);
			//plottableSignalsA[5] = CartesianChart.plt.PlotSignal(analogInAData[5], label: "CH6A", color: System.Drawing.Color.Aqua, lineWidth: 2);
			//plottableSignalsA[6] = CartesianChart.plt.PlotSignal(analogInAData[6], label: "CH7A", color: System.Drawing.Color.Purple, lineWidth: 2);
			//plottableSignalsA[7] = CartesianChart.plt.PlotSignal(analogInAData[7], label: "CH8A", color: System.Drawing.Color.Violet, lineWidth: 2);
			plottableSignalsA[0] = null;
			plottableSignalsA[1] = null;
			plottableSignalsA[2] = null;
			plottableSignalsA[3] = null;
			plottableSignalsA[4] = null;
			plottableSignalsA[5] = null;
			plottableSignalsA[6] = null;
			plottableSignalsA[7] = null;

			//plottableSignalsB[0] = CartesianChart.plt.PlotSignal(analogInBData[0], label: "CH1B", color: System.Drawing.Color.Red, lineWidth: 2);
			//plottableSignalsB[1] = CartesianChart.plt.PlotSignal(analogInBData[1], label: "CH2B", color: System.Drawing.Color.Orange, lineWidth: 2);
			//plottableSignalsB[2] = CartesianChart.plt.PlotSignal(analogInBData[2], label: "CH3B", color: System.Drawing.Color.Green, lineWidth: 2);
			//plottableSignalsB[3] = CartesianChart.plt.PlotSignal(analogInBData[3], label: "CH4B", color: System.Drawing.Color.Lime, lineWidth: 2);
			//plottableSignalsB[4] = CartesianChart.plt.PlotSignal(analogInBData[4], label: "CH5B", color: System.Drawing.Color.Blue, lineWidth: 2);
			//plottableSignalsB[5] = CartesianChart.plt.PlotSignal(analogInBData[5], label: "CH6B", color: System.Drawing.Color.Aqua, lineWidth: 2);
			//plottableSignalsB[6] = CartesianChart.plt.PlotSignal(analogInBData[6], label: "CH7B", color: System.Drawing.Color.Purple, lineWidth: 2);
			//plottableSignalsB[7] = CartesianChart.plt.PlotSignal(analogInBData[7], label: "CH8B", color: System.Drawing.Color.Violet, lineWidth: 2);
			plottableSignalsB[0] = null;
			plottableSignalsB[1] = null;
			plottableSignalsB[2] = null;
			plottableSignalsB[3] = null;
			plottableSignalsB[4] = null;
			plottableSignalsB[5] = null;
			plottableSignalsB[6] = null;
			plottableSignalsB[7] = null;

			//Set Axis Scale
			CartesianChart.plt.Axis(0, graphPoints, -16, 16);

			//Set Labels
			//CartesianChart.plt.Legend(
			//	enableLegend: true,
			//	location: ScottPlot.legendLocation.upperCenter
			//);

			//Other Configs
			CartesianChart.Configure(
				enablePanning: false,
				enableRightClickZoom: false,
				enableScrollWheelZoom: false
			);

			CartesianChart.Render();
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
			for (int ch = 1; ch < 9; ch++) {
				double[] temp = daq.ReadAnalogInVolt(ch, graphPoints);
				if (temp != null) {
					Array.Copy(temp, analogInAData[ch - 1], graphPoints);

					//Calculate Standart Deviation in ADC counts, raw mode
					int[] rawValues = daq.ReadAnalogIn(ch, graphPoints);

					double avg = rawValues.Average();
					double sumSquares = 0;
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

			for (int ch = 1; ch < 9; ch++) {
				double[] temp = daq.ReadAnalogInVolt((8+ch), graphPoints);
				if (temp != null) {
					Array.Copy(temp, analogInBData[ch - 1], graphPoints);
				}
			}

			CartesianChart.Render(skipIfCurrentlyRendering: true);
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

		private void NumberValidationTextBox(object sender, TextCompositionEventArgs e) {
			Regex regex = new Regex("[^0-9-]+");
			e.Handled = regex.IsMatch(e.Text);
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
			daq.SetAnalogInSampligRate(1, sampligRate);
		}

		private void AnalogInBChanged(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			int sampligRate = (int)(250000.0 / (1 + analogInBSamplerate.SelectedIndex));
			daq.SetAnalogInSampligRate(9, sampligRate);
		}

		private void AnalogInAChannel1Changed(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			float range = (float)Math.Pow(2, (5 - analogInAChannel1Range.SelectedIndex));

			analogInARanges[0] = range;

			float maxRange = 0;
			for(int i = 0; i < analogInARanges.Length; i++) {
				if(analogInARanges[i] > maxRange) {
					maxRange = analogInARanges[i];
				}
			}

			System.Drawing.Color drawColor = System.Drawing.Color.FromArgb(analogInAChannel1Color.SelectedColor.Value.A, analogInAChannel1Color.SelectedColor.Value.R, analogInAChannel1Color.SelectedColor.Value.G, analogInAChannel1Color.SelectedColor.Value.B);
			//Func<double, string> formatFunc = (x) => string.Format("{0:0.000}", x);
			CartesianChart.plt.Remove(plottableSignalsA[0]);
			if ((AnalogInMode)analogInAChannel1Mode.SelectedIndex == AnalogInMode.Mode_Single || (AnalogInMode)analogInAChannel1Mode.SelectedIndex == AnalogInMode.Mode_Diff) {
				plottableSignalsA[0] = CartesianChart.plt.PlotSignal(analogInAData[0], label: "CH1A", color: drawColor, lineWidth: 2);

				CartesianChart.plt.Axis(y1: -maxRange, y2: maxRange);
			}

			daq.AddAnalogIn(1, range, (AnalogInMode)analogInAChannel1Mode.SelectedIndex);
		}

		private void AnalogInAChannel2Changed(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			float range = (float)Math.Pow(2, (5 - analogInAChannel2Range.SelectedIndex));

			analogInARanges[1] = range;

			float maxRange = 0;
			for (int i = 0; i < analogInARanges.Length; i++) {
				if (analogInARanges[i] > maxRange) {
					maxRange = analogInARanges[i];
				}
			}

			System.Drawing.Color drawColor = System.Drawing.Color.FromArgb(analogInAChannel2Color.SelectedColor.Value.A, analogInAChannel2Color.SelectedColor.Value.R, analogInAChannel2Color.SelectedColor.Value.G, analogInAChannel2Color.SelectedColor.Value.B);
			//Func<double, string> formatFunc = (x) => string.Format("{0:0.000}", x);
			CartesianChart.plt.Remove(plottableSignalsA[1]);
			if ((AnalogInMode)analogInAChannel2Mode.SelectedIndex == AnalogInMode.Mode_Single) {
				plottableSignalsA[1] = CartesianChart.plt.PlotSignal(analogInAData[1], label: "CH2A", color: drawColor, lineWidth: 2);

				CartesianChart.plt.Axis(y1: -maxRange, y2: maxRange);
			}

			daq.AddAnalogIn(2, range, (AnalogInMode)analogInAChannel1Mode.SelectedIndex);
		}

		private void AnalogInAChannel3Changed(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			float range = (float)Math.Pow(2, (5 - analogInAChannel3Range.SelectedIndex));

			analogInARanges[2] = range;

			float maxRange = 0;
			for (int i = 0; i < analogInARanges.Length; i++) {
				if (analogInARanges[i] > maxRange) {
					maxRange = analogInARanges[i];
				}
			}

			System.Drawing.Color drawColor = System.Drawing.Color.FromArgb(analogInAChannel3Color.SelectedColor.Value.A, analogInAChannel3Color.SelectedColor.Value.R, analogInAChannel3Color.SelectedColor.Value.G, analogInAChannel3Color.SelectedColor.Value.B);
			//Func<double, string> formatFunc = (x) => string.Format("{0:0.000}", x);
			CartesianChart.plt.Remove(plottableSignalsA[2]);
			if ((AnalogInMode)analogInAChannel3Mode.SelectedIndex == AnalogInMode.Mode_Single || (AnalogInMode)analogInAChannel3Mode.SelectedIndex == AnalogInMode.Mode_Diff) {
				plottableSignalsA[2] = CartesianChart.plt.PlotSignal(analogInAData[2], label: "CH3A", color: drawColor, lineWidth: 2);

				CartesianChart.plt.Axis(y1: -maxRange, y2: maxRange);
			}

			daq.AddAnalogIn(3, range, (AnalogInMode)analogInAChannel3Mode.SelectedIndex);
		}

		private void AnalogInAChannel4Changed(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			float range = (float)Math.Pow(2, (5 - analogInAChannel4Range.SelectedIndex));

			analogInARanges[3] = range;

			float maxRange = 0;
			for (int i = 0; i < analogInARanges.Length; i++) {
				if (analogInARanges[i] > maxRange) {
					maxRange = analogInARanges[i];
				}
			}

			System.Drawing.Color drawColor = System.Drawing.Color.FromArgb(analogInAChannel4Color.SelectedColor.Value.A, analogInAChannel4Color.SelectedColor.Value.R, analogInAChannel4Color.SelectedColor.Value.G, analogInAChannel4Color.SelectedColor.Value.B);
			//Func<double, string> formatFunc = (x) => string.Format("{0:0.000}", x);
			CartesianChart.plt.Remove(plottableSignalsA[3]);
			if ((AnalogInMode)analogInAChannel4Mode.SelectedIndex == AnalogInMode.Mode_Single) {
				plottableSignalsA[3] = CartesianChart.plt.PlotSignal(analogInAData[3], label: "CH4A", color: drawColor, lineWidth: 2);

				CartesianChart.plt.Axis(y1: -maxRange, y2: maxRange);
			}

			daq.AddAnalogIn(4, range, (AnalogInMode)analogInAChannel3Mode.SelectedIndex);
		}

		private void AnalogInAChannel5Changed(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			float range = (float)Math.Pow(2, (5 - analogInAChannel5Range.SelectedIndex));

			analogInARanges[4] = range;

			float maxRange = 0;
			for (int i = 0; i < analogInARanges.Length; i++) {
				if (analogInARanges[i] > maxRange) {
					maxRange = analogInARanges[i];
				}
			}

			System.Drawing.Color drawColor = System.Drawing.Color.FromArgb(analogInAChannel5Color.SelectedColor.Value.A, analogInAChannel5Color.SelectedColor.Value.R, analogInAChannel5Color.SelectedColor.Value.G, analogInAChannel5Color.SelectedColor.Value.B);
			//Func<double, string> formatFunc = (x) => string.Format("{0:0.000}", x);
			CartesianChart.plt.Remove(plottableSignalsA[4]);
			if ((AnalogInMode)analogInAChannel5Mode.SelectedIndex == AnalogInMode.Mode_Single || (AnalogInMode)analogInAChannel5Mode.SelectedIndex == AnalogInMode.Mode_Diff) {
				plottableSignalsA[4] = CartesianChart.plt.PlotSignal(analogInAData[4], label: "CH5A", color: drawColor, lineWidth: 2);

				CartesianChart.plt.Axis(y1: -maxRange, y2: maxRange);
			}

			daq.AddAnalogIn(5, range, (AnalogInMode)analogInAChannel5Mode.SelectedIndex);
		}

		private void AnalogInAChannel6Changed(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			float range = (float)Math.Pow(2, (5 - analogInAChannel6Range.SelectedIndex));

			analogInARanges[5] = range;

			float maxRange = 0;
			for (int i = 0; i < analogInARanges.Length; i++) {
				if (analogInARanges[i] > maxRange) {
					maxRange = analogInARanges[i];
				}
			}

			System.Drawing.Color drawColor = System.Drawing.Color.FromArgb(analogInAChannel6Color.SelectedColor.Value.A, analogInAChannel6Color.SelectedColor.Value.R, analogInAChannel6Color.SelectedColor.Value.G, analogInAChannel6Color.SelectedColor.Value.B);
			//Func<double, string> formatFunc = (x) => string.Format("{0:0.000}", x);
			CartesianChart.plt.Remove(plottableSignalsA[5]);
			if ((AnalogInMode)analogInAChannel6Mode.SelectedIndex == AnalogInMode.Mode_Single) {
				plottableSignalsA[5] = CartesianChart.plt.PlotSignal(analogInAData[5], label: "CH6A", color: drawColor, lineWidth: 2);

				CartesianChart.plt.Axis(y1: -maxRange, y2: maxRange);
			}

			daq.AddAnalogIn(6, range, (AnalogInMode)analogInAChannel5Mode.SelectedIndex);
		}

		private void AnalogInAChannel7Changed(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			float range = (float)Math.Pow(2, (5 - analogInAChannel7Range.SelectedIndex));

			analogInARanges[6] = range;

			float maxRange = 0;
			for (int i = 0; i < analogInARanges.Length; i++) {
				if (analogInARanges[i] > maxRange) {
					maxRange = analogInARanges[i];
				}
			}

			System.Drawing.Color drawColor = System.Drawing.Color.FromArgb(analogInAChannel7Color.SelectedColor.Value.A, analogInAChannel7Color.SelectedColor.Value.R, analogInAChannel7Color.SelectedColor.Value.G, analogInAChannel7Color.SelectedColor.Value.B);
			//Func<double, string> formatFunc = (x) => string.Format("{0:0.000}", x);
			CartesianChart.plt.Remove(plottableSignalsA[6]);
			if ((AnalogInMode)analogInAChannel7Mode.SelectedIndex == AnalogInMode.Mode_Single || (AnalogInMode)analogInAChannel7Mode.SelectedIndex == AnalogInMode.Mode_Diff) {
				plottableSignalsA[6] = CartesianChart.plt.PlotSignal(analogInAData[6], label: "CH7A", color: drawColor, lineWidth: 2);

				CartesianChart.plt.Axis(y1: -maxRange, y2: maxRange);
			}

			daq.AddAnalogIn(7, range, (AnalogInMode)analogInAChannel7Mode.SelectedIndex);
		}

		private void AnalogInAChannel8Changed(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			float range = (float)Math.Pow(2, (5 - analogInAChannel8Range.SelectedIndex));

			analogInARanges[7] = range;

			float maxRange = 0;
			for (int i = 0; i < analogInARanges.Length; i++) {
				if (analogInARanges[i] > maxRange) {
					maxRange = analogInARanges[i];
				}
			}

			System.Drawing.Color drawColor = System.Drawing.Color.FromArgb(analogInAChannel8Color.SelectedColor.Value.A, analogInAChannel8Color.SelectedColor.Value.R, analogInAChannel8Color.SelectedColor.Value.G, analogInAChannel8Color.SelectedColor.Value.B);
			//Func<double, string> formatFunc = (x) => string.Format("{0:0.000}", x);
			CartesianChart.plt.Remove(plottableSignalsA[7]);
			if ((AnalogInMode)analogInAChannel8Mode.SelectedIndex == AnalogInMode.Mode_Single) {
				plottableSignalsA[7] = CartesianChart.plt.PlotSignal(analogInAData[7], label: "CH8A", color: drawColor, lineWidth: 2);

				CartesianChart.plt.Axis(y1: -maxRange, y2: maxRange);
			}

			daq.AddAnalogIn(8, range, (AnalogInMode)analogInAChannel7Mode.SelectedIndex);
		}

		private void AnalogInBChannel1Changed(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			float range = (float)Math.Pow(2, (5 - analogInBChannel1Range.SelectedIndex));

			analogInBRanges[0] = range;

			float maxRange = 0;
			for (int i = 0; i < analogInBRanges.Length; i++) {
				if (analogInBRanges[i] > maxRange) {
					maxRange = analogInBRanges[i];
				}
			}

			System.Drawing.Color drawColor = System.Drawing.Color.FromArgb(analogInBChannel1Color.SelectedColor.Value.A, analogInBChannel1Color.SelectedColor.Value.R, analogInBChannel1Color.SelectedColor.Value.G, analogInBChannel1Color.SelectedColor.Value.B);
			//Func<double, string> formatFunc = (x) => string.Format("{0:0.000}", x);
			CartesianChart.plt.Remove(plottableSignalsB[0]);
			if ((AnalogInMode)analogInBChannel1Mode.SelectedIndex == AnalogInMode.Mode_Single || (AnalogInMode)analogInBChannel1Mode.SelectedIndex == AnalogInMode.Mode_Diff) {
				plottableSignalsB[0] = CartesianChart.plt.PlotSignal(analogInBData[0], label: "CH1B", color: drawColor, lineWidth: 2);

				CartesianChart.plt.Axis(y1: -maxRange, y2: maxRange);
			}

			daq.AddAnalogIn(8+1, range, (AnalogInMode)analogInBChannel1Mode.SelectedIndex);
		}

		private void AnalogInBChannel2Changed(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			float range = (float)Math.Pow(2, (5 - analogInBChannel2Range.SelectedIndex));

			analogInBRanges[1] = range;

			float maxRange = 0;
			for (int i = 0; i < analogInBRanges.Length; i++) {
				if (analogInBRanges[i] > maxRange) {
					maxRange = analogInBRanges[i];
				}
			}

			System.Drawing.Color drawColor = System.Drawing.Color.FromArgb(analogInBChannel2Color.SelectedColor.Value.A, analogInBChannel2Color.SelectedColor.Value.R, analogInBChannel2Color.SelectedColor.Value.G, analogInBChannel2Color.SelectedColor.Value.B);
			//Func<double, string> formatFunc = (x) => string.Format("{0:0.000}", x);
			CartesianChart.plt.Remove(plottableSignalsB[1]);
			if ((AnalogInMode)analogInBChannel2Mode.SelectedIndex == AnalogInMode.Mode_Single) {
				plottableSignalsB[1] = CartesianChart.plt.PlotSignal(analogInBData[1], label: "CH2B", color: drawColor, lineWidth: 2);

				CartesianChart.plt.Axis(y1: -maxRange, y2: maxRange);
			}

			daq.AddAnalogIn(8+2, range, (AnalogInMode)analogInBChannel1Mode.SelectedIndex);
		}

		private void AnalogInBChannel3Changed(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			float range = (float)Math.Pow(2, (5 - analogInBChannel3Range.SelectedIndex));

			analogInBRanges[2] = range;

			float maxRange = 0;
			for (int i = 0; i < analogInBRanges.Length; i++) {
				if (analogInBRanges[i] > maxRange) {
					maxRange = analogInBRanges[i];
				}
			}

			System.Drawing.Color drawColor = System.Drawing.Color.FromArgb(analogInBChannel3Color.SelectedColor.Value.A, analogInBChannel3Color.SelectedColor.Value.R, analogInBChannel3Color.SelectedColor.Value.G, analogInBChannel3Color.SelectedColor.Value.B);
			//Func<double, string> formatFunc = (x) => string.Format("{0:0.000}", x);
			CartesianChart.plt.Remove(plottableSignalsB[2]);
			if ((AnalogInMode)analogInBChannel3Mode.SelectedIndex == AnalogInMode.Mode_Single || (AnalogInMode)analogInBChannel3Mode.SelectedIndex == AnalogInMode.Mode_Diff) {
				plottableSignalsB[2] = CartesianChart.plt.PlotSignal(analogInBData[2], label: "CH3B", color: drawColor, lineWidth: 2);

				CartesianChart.plt.Axis(y1: -maxRange, y2: maxRange);
			}

			daq.AddAnalogIn(8+3, range, (AnalogInMode)analogInBChannel3Mode.SelectedIndex);
		}

		private void AnalogInBChannel4Changed(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			float range = (float)Math.Pow(2, (5 - analogInBChannel4Range.SelectedIndex));

			analogInBRanges[3] = range;

			float maxRange = 0;
			for (int i = 0; i < analogInBRanges.Length; i++) {
				if (analogInBRanges[i] > maxRange) {
					maxRange = analogInBRanges[i];
				}
			}

			System.Drawing.Color drawColor = System.Drawing.Color.FromArgb(analogInBChannel4Color.SelectedColor.Value.A, analogInBChannel4Color.SelectedColor.Value.R, analogInBChannel4Color.SelectedColor.Value.G, analogInBChannel4Color.SelectedColor.Value.B);
			//Func<double, string> formatFunc = (x) => string.Format("{0:0.000}", x);
			CartesianChart.plt.Remove(plottableSignalsB[3]);
			if ((AnalogInMode)analogInBChannel4Mode.SelectedIndex == AnalogInMode.Mode_Single) { 
				plottableSignalsB[3] = CartesianChart.plt.PlotSignal(analogInBData[3], label: "CH4B", color: drawColor, lineWidth: 2);

				CartesianChart.plt.Axis(y1: -maxRange, y2: maxRange);
			}

			daq.AddAnalogIn(8+4, range, (AnalogInMode)analogInBChannel3Mode.SelectedIndex);
		}

		private void AnalogInBChannel5Changed(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			float range = (float)Math.Pow(2, (5 - analogInBChannel5Range.SelectedIndex));

			analogInBRanges[4] = range;

			float maxRange = 0;
			for (int i = 0; i < analogInBRanges.Length; i++) {
				if (analogInBRanges[i] > maxRange) {
					maxRange = analogInBRanges[i];
				}
			}

			System.Drawing.Color drawColor = System.Drawing.Color.FromArgb(analogInBChannel5Color.SelectedColor.Value.A, analogInBChannel5Color.SelectedColor.Value.R, analogInBChannel5Color.SelectedColor.Value.G, analogInBChannel5Color.SelectedColor.Value.B);
			//Func<double, string> formatFunc = (x) => string.Format("{0:0.000}", x);
			CartesianChart.plt.Remove(plottableSignalsB[4]);
			if ((AnalogInMode)analogInBChannel5Mode.SelectedIndex == AnalogInMode.Mode_Single || (AnalogInMode)analogInBChannel5Mode.SelectedIndex == AnalogInMode.Mode_Diff) {
				plottableSignalsB[4] = CartesianChart.plt.PlotSignal(analogInBData[4], label: "CH5B", color: drawColor, lineWidth: 2);

				CartesianChart.plt.Axis(y1: -maxRange, y2: maxRange);
			}

			daq.AddAnalogIn(8+5, range, (AnalogInMode)analogInBChannel5Mode.SelectedIndex);
		}

		private void AnalogInBChannel6Changed(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			float range = (float)Math.Pow(2, (5 - analogInBChannel6Range.SelectedIndex));

			analogInBRanges[5] = range;

			float maxRange = 0;
			for (int i = 0; i < analogInBRanges.Length; i++) {
				if (analogInBRanges[i] > maxRange) {
					maxRange = analogInBRanges[i];
				}
			}

			System.Drawing.Color drawColor = System.Drawing.Color.FromArgb(analogInBChannel6Color.SelectedColor.Value.A, analogInBChannel6Color.SelectedColor.Value.R, analogInBChannel6Color.SelectedColor.Value.G, analogInBChannel6Color.SelectedColor.Value.B);
			//Func<double, string> formatFunc = (x) => string.Format("{0:0.000}", x);
			CartesianChart.plt.Remove(plottableSignalsB[5]);
			if((AnalogInMode)analogInBChannel6Mode.SelectedIndex == AnalogInMode.Mode_Single) {
				plottableSignalsB[5] = CartesianChart.plt.PlotSignal(analogInBData[5], label: "CH6B", color: drawColor, lineWidth: 2);

				CartesianChart.plt.Axis(y1: -maxRange, y2: maxRange);
			}

			daq.AddAnalogIn(8+6, range, (AnalogInMode)analogInBChannel5Mode.SelectedIndex);
		}

		private void AnalogInBChannel7Changed(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			float range = (float)Math.Pow(2, (5 - analogInBChannel7Range.SelectedIndex));

			analogInBRanges[6] = range;

			float maxRange = 0;
			for (int i = 0; i < analogInBRanges.Length; i++) {
				if (analogInBRanges[i] > maxRange) {
					maxRange = analogInBRanges[i];
				}
			}

			System.Drawing.Color drawColor = System.Drawing.Color.FromArgb(analogInBChannel7Color.SelectedColor.Value.A, analogInBChannel7Color.SelectedColor.Value.R, analogInBChannel7Color.SelectedColor.Value.G, analogInBChannel7Color.SelectedColor.Value.B);
			//Func<double, string> formatFunc = (x) => string.Format("{0:0.000}", x);
			CartesianChart.plt.Remove(plottableSignalsB[6]);
			if ((AnalogInMode)analogInBChannel7Mode.SelectedIndex == AnalogInMode.Mode_Single || (AnalogInMode)analogInBChannel7Mode.SelectedIndex == AnalogInMode.Mode_Diff) {
				plottableSignalsB[6] = CartesianChart.plt.PlotSignal(analogInBData[6], label: "CH7B", color: drawColor, lineWidth: 2);

				CartesianChart.plt.Axis(y1: -maxRange, y2: maxRange);
			}

			daq.AddAnalogIn(8+7, range, (AnalogInMode)analogInBChannel7Mode.SelectedIndex);
		}

		private void AnalogInBChannel8Changed(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			float range = (float)Math.Pow(2, (5 - analogInBChannel8Range.SelectedIndex));

			analogInBRanges[7] = range;

			float maxRange = 0;
			for (int i = 0; i < analogInBRanges.Length; i++) {
				if (analogInBRanges[i] > maxRange) {
					maxRange = analogInBRanges[i];
				}
			}

			System.Drawing.Color drawColor = System.Drawing.Color.FromArgb(analogInBChannel8Color.SelectedColor.Value.A, analogInBChannel8Color.SelectedColor.Value.R, analogInBChannel8Color.SelectedColor.Value.G, analogInBChannel8Color.SelectedColor.Value.B);
			//Func<double, string> formatFunc = (x) => string.Format("{0:0.000}", x);
			CartesianChart.plt.Remove(plottableSignalsB[7]);
			if ((AnalogInMode)analogInBChannel8Mode.SelectedIndex == AnalogInMode.Mode_Single) {
				plottableSignalsB[7] = CartesianChart.plt.PlotSignal(analogInBData[7], label: "CH8B", color: drawColor, lineWidth: 2);

				CartesianChart.plt.Axis(y1: -maxRange, y2: maxRange);
			}

			daq.AddAnalogIn(8+8, range, (AnalogInMode)analogInBChannel7Mode.SelectedIndex);
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
			analogOutAChannel1Offset.Text = offset.ToString();

			int freq = 0;
			int.TryParse(analogOutAChannel1Freq.Text, out freq);
			if (freq > 100000) {
				freq = 100000;
			}
			else if(freq < 0) {
				freq = 0;
			}
			analogOutAChannel1Freq.Text = freq.ToString();

			int amplitude = 0;
			int.TryParse(analogOutAChannel1Amp.Text, out amplitude);
			if (amplitude > 25000) {
				amplitude = 25000;
			}
			else if(amplitude < 0) {
				amplitude = 0;
			}

			//Convert to valid values, DAC steps
			//float step = 25000 / 4096;
			//int nSteps = Convert.ToInt32(amplitude / step);
			//amplitude = Convert.ToInt32(nSteps * step);

			analogOutAChannel1Amp.Text = amplitude.ToString();

			int dutyCycle = 0;
			int.TryParse(analogOutAChannel1DC.Text, out dutyCycle);
			if (dutyCycle > 100) {
				dutyCycle = 100;
			}
			if(dutyCycle < 0) {
				dutyCycle = 0;
			}
			analogOutAChannel1DC.Text = dutyCycle.ToString();

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
			analogOutAChannel2Offset.Text = offset.ToString();

			int freq = 0;
			int.TryParse(analogOutAChannel2Freq.Text, out freq);
			if (freq > 100000) {
				freq = 100000;
			}
			else if (freq < 0) {
				freq = 0;
			}
			analogOutAChannel2Freq.Text = freq.ToString();

			int amplitude = 0;
			int.TryParse(analogOutAChannel2Amp.Text, out amplitude);
			if (amplitude > 25000) {
				amplitude = 25000;
			}
			else if (amplitude < 0) {
				amplitude = 0;
			}

			//Convert to valid values, DAC steps
			//float step = 25000 / 4096;
			//int nSteps = Convert.ToInt32(amplitude / step);
			//amplitude = Convert.ToInt32(nSteps * step);

			analogOutAChannel2Amp.Text = amplitude.ToString();

			int dutyCycle = 0;
			int.TryParse(analogOutAChannel2DC.Text, out dutyCycle);
			if (dutyCycle > 100) {
				dutyCycle = 100;
			}
			if (dutyCycle < 0) {
				dutyCycle = 0;
			}
			analogOutAChannel2DC.Text = dutyCycle.ToString();


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
			analogOutBChannel1Offset.Text = offset.ToString();

			int freq = 0;
			int.TryParse(analogOutBChannel1Freq.Text, out freq);
			if (freq > 100000) {
				freq = 100000;
			}
			else if (freq < 0) {
				freq = 0;
			}
			analogOutBChannel1Freq.Text = freq.ToString();

			int amplitude = 0;
			int.TryParse(analogOutBChannel1Amp.Text, out amplitude);
			if (amplitude > 25000) {
				amplitude = 25000;
			}
			else if (amplitude < 0) {
				amplitude = 0;
			}

			//Convert to valid values, DAC steps
			//float step = 25000 / 4096;
			//int nSteps = Convert.ToInt32(amplitude / step);
			//amplitude = Convert.ToInt32(nSteps * step);

			analogOutBChannel1Amp.Text = amplitude.ToString();

			int dutyCycle = 0;
			int.TryParse(analogOutBChannel1DC.Text, out dutyCycle);
			if (dutyCycle > 100) {
				dutyCycle = 100;
			}
			if (dutyCycle < 0) {
				dutyCycle = 0;
			}
			analogOutBChannel1DC.Text = dutyCycle.ToString();

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
			analogOutBChannel2Offset.Text = offset.ToString();

			int freq = 0;
			int.TryParse(analogOutBChannel2Freq.Text, out freq);
			if (freq > 100000) {
				freq = 100000;
			}
			else if (freq < 0) {
				freq = 0;
			}
			analogOutBChannel2Freq.Text = freq.ToString();

			int amplitude = 0;
			int.TryParse(analogOutBChannel2Amp.Text, out amplitude);
			if (amplitude > 25000) {
				amplitude = 25000;
			}
			else if (amplitude < 0) {
				amplitude = 0;
			}

			//Convert to valid values, DAC steps
			//float step = 25000 / 4096;
			//int nSteps = Convert.ToInt32(amplitude / step);
			//amplitude = Convert.ToInt32(nSteps * step);

			analogOutBChannel2Amp.Text = amplitude.ToString();

			int dutyCycle = 0;
			int.TryParse(analogOutBChannel2DC.Text, out dutyCycle);
			if (dutyCycle > 100) {
				dutyCycle = 100;
			}
			if (dutyCycle < 0) {
				dutyCycle = 0;
			}
			analogOutBChannel2DC.Text = dutyCycle.ToString();


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
