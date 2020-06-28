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

		byte[] txBuffer = new byte[34];

		int[] analogInADataIndex = new int[4];
		double[][] analogInAData = new double[4][];

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
			analogInAData[0] = new double[1024];
			analogInAData[1] = new double[1024];
			analogInAData[2] = new double[1024];
			analogInAData[3] = new double[1024];

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
					Fill = Brushes.Transparent
				},
				new LineSeries {
					Title = "CH2",
					Values = new ChartValues<float>(),
					PointGeometry = null,
					LineSmoothness = 0,
					Fill = Brushes.Transparent
				},
				new LineSeries {
					Title = "CH3",
					Values = new ChartValues<float>(),
					PointGeometry = null,
					LineSmoothness = 0,
					Fill = Brushes.Transparent
				},
				new LineSeries {
					Title = "CH4",
					Values = new ChartValues<float>(),
					PointGeometry = null,
					LineSmoothness = 0,
					Fill = Brushes.Transparent
				}
			};

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

			float[] channel1 = daq.AnalogIn(1, 1000);
			if(channel1 != null) {
				SeriesAnalogIn[0].Values = new ChartValues<float>(channel1);
			}

			//SeriesAnalogIn[1].Values = new ChartValues<float>(analogInAData[1]);
			//SeriesAnalogIn[2].Values = new ChartValues<float>(analogInAData[2]);
			//SeriesAnalogIn[3].Values = new ChartValues<float>(analogInAData[3]);
		}

		private void SerialSendCommand(byte opcode, byte[] data) {
			byte[] txData = new byte[1024];
			int txLength = data.Length;

			//Add Header
			txData[0] = opcode;
			txData[1] = (byte)(txLength >> 8);
			txData[2] = (byte)txLength;

			//Add Payload
			Array.Copy(data, 0, txData, 3, txLength);

			//Add CRC
			txData[3 + txLength] = 0;
			txData[4 + txLength] = 0;

			//if(serialPort.IsOpen) {
			//	serialPort.Write(txData, 0, (txLength + 5));
			//}
		}

		private void SerialReceiveProcessing(byte[] rxData, int length) {
			return;

			int index = 0;
			while (index < length) {
				int opcode = rxData[index++];
				int packetLength = (rxData[index++] << 8) + rxData[index++];
				int crc = (rxData[index + packetLength] << 8) + rxData[index + packetLength];

				switch (opcode) {
					case Opcodes.txAnalogInA: {
						int channel = rxData[index++];

						for(int i = 1; i < packetLength; i += 2) {
							double value = (rxData[index++] << 8) + rxData[index++];
							value = value * 2.048 / 4095 - 1.024;
							value = value * 8 * 2;

							analogInAData[channel - 1][analogInADataIndex[channel - 1]++] = value;
							if (analogInADataIndex[channel - 1] >= 1024) {
								analogInADataIndex[channel - 1] = 0;
							}

							//SeriesAnalogIn[channel - 1].Values.Add(value);
							//if (SeriesAnalogIn[channel - 1].Values.Count > 1024) {
							//	SeriesAnalogIn[channel - 1].Values.RemoveAt(0);
							//}
						}
						break;
					}
					default: {
						index += packetLength;
						break;
					}
				}

				//Add CRC length, read already
				index += 2;
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

			byte[] data = new byte[2];

			data[0] = (byte)currentSourceAComboBox.SelectedIndex;
			data[1] = (byte)currentOutAComboBox.SelectedIndex;

			SerialSendCommand(Opcodes.setCurrentA, data);
		}
		private void CurrentOutputBChanged(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			byte[] data = new byte[2];

			data[0] = (byte)currentSourceBComboBox.SelectedIndex;
			data[1] = (byte)currentOutBComboBox.SelectedIndex;

			SerialSendCommand(Opcodes.setCurrentB, data);
		}

		private void AnalogInAChanged(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			byte[] data = new byte[6];

			data[0] = (byte)analogInAMode.SelectedIndex;
			data[1] = (byte)analogInASamplerate.SelectedIndex;
			data[2] = (byte)analogInAScale.SelectedIndex;

			//SerialSendCommand(Opcodes.setAnalogInA, data);
		}

		private void AnalogInAChannel1Changed(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			byte[] data = new byte[6];

			data[0] = 1;
			data[1] = (byte)analogInAChannel1Enabled.SelectedIndex;
			data[2] = (byte)analogInAChannel1RateDiv.SelectedIndex;
			data[3] = (byte)analogInAChannel1Resolution.SelectedIndex;
			data[4] = (byte)analogInAChannel1Gain.SelectedIndex;

			//SerialSendCommand(Opcodes.setAnalogInACH, data);
			daq.AddAnalogIn(1, 15, (AnalogInMode)analogInAMode.SelectedIndex);
		}

		private void AnalogInAChannel2Changed(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			byte[] data = new byte[6];

			data[0] = 2;
			data[1] = (byte)analogInAChannel2Enabled.SelectedIndex;
			data[2] = (byte)analogInAChannel2RateDiv.SelectedIndex;
			data[3] = (byte)analogInAChannel2Resolution.SelectedIndex;
			data[4] = (byte)analogInAChannel2Gain.SelectedIndex;

			SerialSendCommand(Opcodes.setAnalogInACH, data);
		}

		private void AnalogInAChannel3Changed(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			byte[] data = new byte[6];

			data[0] = 3;
			data[1] = (byte)analogInAChannel3Enabled.SelectedIndex;
			data[2] = (byte)analogInAChannel3RateDiv.SelectedIndex;
			data[3] = (byte)analogInAChannel3Resolution.SelectedIndex;
			data[4] = (byte)analogInAChannel3Gain.SelectedIndex;

			SerialSendCommand(Opcodes.setAnalogInACH, data);
		}

		private void AnalogInAChannel4Changed(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			byte[] data = new byte[6];

			data[0] = 4;
			data[1] = (byte)analogInAChannel4Enabled.SelectedIndex;
			data[2] = (byte)analogInAChannel4RateDiv.SelectedIndex;
			data[3] = (byte)analogInAChannel4Resolution.SelectedIndex;
			data[4] = (byte)analogInAChannel4Gain.SelectedIndex;

			SerialSendCommand(Opcodes.setAnalogInACH, data);
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
				if(offset > 15000) {
					offset = 15000;
				}
				else if(offset < -15000) {
					offset = -15000;
				}

				//Convert to valid values, DAC steps
				float step = 30000 / 4096;
				offset = offset + 15000;
				int nSteps = Convert.ToInt32( offset / step);
				offset = Convert.ToInt32(nSteps * step) - 15000;

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
			offset += 15000;
			float step = 30000 / 4096;
			int nSteps = Convert.ToInt32(offset / step);

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

			SerialSendCommand(Opcodes.setAnalogOutACH, data);
		}
	}
}
