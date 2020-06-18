using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
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
using LiveCharts.Wpf;

namespace STM32G4DAQ {

	public static class Opcodes {
		public static byte setCurrentA = 0x01;
		public static byte setCurrentB = 0x02;

		public static byte setAnalogInA = 0x03;
	}
	public partial class MainWindow : Window {
		private SerialPort serialPort = new SerialPort();

		Stopwatch rxUSBStopwatch = new Stopwatch();

		int rxCount = 0;
		float rxUSBDatarate = 0;
		float txUSBDatarate = 0;
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
					Values = new ChartValues<double> { },
					PointGeometry = null,
					LineSmoothness = 0,
					Fill = Brushes.Transparent
				},
				new LineSeries {
					Title = "CH2",
					Values = new ChartValues<double> { },
					PointGeometry = null,
					LineSmoothness = 0,
					Fill = Brushes.Transparent
				},
				new LineSeries {
					Title = "CH3",
					Values = new ChartValues<double> { },
					PointGeometry = null,
					LineSmoothness = 0,
					Fill = Brushes.Transparent
				}
			};
		}

		private delegate void SerialReceiveDelegate(byte[] rxData, int length);

		private void OnSerialReceive(object sender, SerialDataReceivedEventArgs e) {
			byte[] rxData = new byte[1024];
			int rxLength = serialPort.BytesToRead;

			if (rxLength > 1024) {
				serialPort.DiscardInBuffer();
			}
			else {
				serialPort.Read(rxData, 0, rxLength);

				rxCount += rxLength;

				if (rxUSBStopwatch.ElapsedMilliseconds > 1000) {
					rxUSBStopwatch.Reset();

					rxUSBDatarate = (float)rxCount * 0.001f;
					rxCount = 0;

					rxUSBStopwatch.Start();
				}
			}

			Dispatcher.Invoke(DispatcherPriority.Send, new SerialReceiveDelegate(SerialReceiveProcessing), rxData, rxLength);
		}

		private void SerialSendCommand(byte opcode, byte[] data) {
			byte[] txData = new byte[1024];
			int txLength = data.Length;

			//Add Header
			txData[0] = opcode;
			txData[1] = (byte)txLength;

			//Add Payload
			Array.Copy(data, 0, txData, 2, txLength);

			//Add CRC
			txData[2 + txLength] = 0;
			txData[3 + txLength] = 0;

			serialPort.Write(txData, 0, (txLength + 4));
		}

		private void SerialReceiveProcessing(byte[] rxData, int length) {
			rxUSBDatarateLabel.Content = rxUSBDatarate.ToString("0.0");
			txUSBDatarateLabel.Content = txUSBDatarate.ToString("0.0");
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

		private void AnalogInAChannel1Changed(object sender, RoutedEventArgs e) {
			if (enableValueChangedEvents == false) {
				return;
			}

			byte[] data = new byte[6];

			data[0] = 1;
			data[1] = (byte)analogInAChannel1Mode.SelectedIndex;
			data[2] = (byte)analogInAChannel1Samplerate.SelectedIndex;
			data[3] = (byte)analogInAChannel1Resolution.SelectedIndex;
			data[4] = (byte)analogInAChannel1Gain.SelectedIndex;
			data[5] = (byte)analogInAChannel1Scale.SelectedIndex;

			SerialSendCommand(Opcodes.setAnalogInA, data);
		}
	}
}
