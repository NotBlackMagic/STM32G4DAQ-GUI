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
		private SerialPort serialPort = new SerialPort();

		Stopwatch rxUSBStopwatch = new Stopwatch();

		int rxCount = 0;
		float rxUSBDatarate = 0;
		float txUSBDatarate = 0;
		int rxDroppedCount = 0;
		float rxUSBDroppedRate = 0;
		byte[] txBuffer = new byte[34];

		bool enableValueChangedEvents = false;

		public SeriesCollection SeriesAnalogIn { get; set; }
		public MainWindow() {
			enableValueChangedEvents = false;
			InitializeComponent();
			enableValueChangedEvents = true;

			rxUSBStopwatch.Start();

			//Init Graph Series
			SeriesAnalogIn = new SeriesCollection {
				new LineSeries {
					Title = "CH1",
					Values = new ChartValues<double>(),
					PointGeometry = null,
					LineSmoothness = 0,
					Fill = Brushes.Transparent
				},
				new LineSeries {
					Title = "CH2",
					Values = new ChartValues<double>(),
					PointGeometry = null,
					LineSmoothness = 0,
					Fill = Brushes.Transparent
				},
				new LineSeries {
					Title = "CH3",
					Values = new ChartValues<double>(),
					PointGeometry = null,
					LineSmoothness = 0,
					Fill = Brushes.Transparent
				},
				new LineSeries {
					Title = "CH4",
					Values = new ChartValues<double>(),
					PointGeometry = null,
					LineSmoothness = 0,
					Fill = Brushes.Transparent
				}
			};

			DataContext = this;
		}

		private delegate void SerialReceiveDelegate(int opcode, byte[] rxData, int length);

		private void OnSerialReceive(object sender, SerialDataReceivedEventArgs e) {
			byte[] rxData = new byte[1024];
			int opcode = serialPort.ReadByte();
			int rxLength = (serialPort.ReadByte() << 8) + serialPort.ReadByte();
			rxLength += 2;  //Add bytes CRC to length

			if (rxLength > 1024 || serialPort.BytesToRead > (serialPort.ReadBufferSize * 0.9)) {
				rxDroppedCount += serialPort.BytesToRead;
				serialPort.ReadExisting();
			}
			else {
				serialPort.Read(rxData, 0, rxLength);

				if(rxLength > 3) {
					Dispatcher.Invoke(DispatcherPriority.Send, new SerialReceiveDelegate(SerialReceiveProcessing), opcode, rxData, rxLength);
				}

				rxCount += rxLength + 3;

				if (rxUSBStopwatch.ElapsedMilliseconds > 1000) {
					rxUSBStopwatch.Reset();

					rxUSBDatarate = (float)rxCount * 0.001f;
					rxCount = 0;

					rxUSBDroppedRate = (float)rxDroppedCount * 0.001f;
					rxDroppedCount = 0;

					rxUSBStopwatch.Start();
				}
			}
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

			if(serialPort.IsOpen) {
				serialPort.Write(txData, 0, (txLength + 5));
			}
		}

		private void SerialReceiveProcessing(int opcode, byte[] rxData, int length) {
			rxUSBDatarateLabel.Content = rxUSBDatarate.ToString("0.0");
			txUSBDatarateLabel.Content = txUSBDatarate.ToString("0.0");
			rxUSBDroppedLabel.Content = rxUSBDroppedRate.ToString("0.0");

			int crc = (rxData[length - 2] << 8) + rxData[length - 1];

			length -= 2;    //Remove CRC length
			switch (opcode) {
				case Opcodes.txAnalogInA: {
					int channel = rxData[0];

					for (int i = 0; i < (length / 2); i++) {
						double value = ((rxData[1 + (2 * i)] << 8) + rxData[1 + (2 * i + 1)]) * 2.048 / 4095 - 1.024;
						value = value * 16 * 2;
						SeriesAnalogIn[channel - 1].Values.Add(value);
						if (SeriesAnalogIn[channel - 1].Values.Count > 1000) {
							SeriesAnalogIn[channel - 1].Values.RemoveAt(0);
						}
					}
					break;
				}
			}
		}

		private void SerialButtonClick(object sender, RoutedEventArgs e) {
			//if (SerialButton.Content.ToString() == "Connect") {
			if (serialPort.IsOpen == false) {
				try {
					serialPort.PortName = portNamesComboBox.Text;
					serialPort.BaudRate = Convert.ToInt32(baudRatesComboBox.Text);
					serialPort.DataBits = 8;
					serialPort.StopBits = StopBits.One;
					serialPort.Parity = Parity.None;
					//serialPort.DtrEnable = true;
					serialPort.Open();

					serialPort.DataReceived += new SerialDataReceivedEventHandler(OnSerialReceive);

					//LogTextBox.Text += "INFO: Serial Connected: " + serialPort.PortName.ToString() + " : " + serialPort.BaudRate.ToString() + Environment.NewLine;

					serialButton.Content = "Disconnect";
					serialButton.Background = Brushes.LimeGreen;
				}
				catch (Exception ex) {
					//LogTextBox.Text += "ERROR: " + ex.ToString() + Environment.NewLine;
				}
			}
			else {
				serialPort.Close();

				//LogTextBox.Text += "INFO: Serial Disconnected" + Environment.NewLine;

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

			SerialSendCommand(Opcodes.setAnalogInA, data);
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

			SerialSendCommand(Opcodes.setAnalogInACH, data);
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
			float step = 30000 / 4096;
			int nSteps = Convert.ToInt32(offset / step);

			int freq = 0;
			int.TryParse(analogOutAChannel1Freq.Text, out freq);

			int amp = 0;
			int.TryParse(analogOutAChannel1Amp.Text, out amp);

			int dc = 0;
			int.TryParse(analogOutAChannel1DC.Text, out dc);

			byte[] data = new byte[10];

			data[0] = 1;
			data[1] = (byte)analogOutAChannel1Mode.SelectedIndex;
			data[2] = (byte)(nSteps >> 8);
			data[3] = (byte)nSteps;
			data[4] = (byte)(freq >> 16);
			data[5] = (byte)(freq >> 8);
			data[6] = (byte)freq;
			data[7] = (byte)(amp >> 8);
			data[8] = (byte)amp;
			data[9] = (byte)dc;

			//SerialSendCommand(Opcodes.setAnalogOutACH, data);
		}
	}
}
