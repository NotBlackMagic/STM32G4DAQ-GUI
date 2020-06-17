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
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		private SerialPort serialPort = new SerialPort();

		Stopwatch rxUSBStopwatch = new Stopwatch();

		int rxCount = 0;
		float rxUSBDatarate = 0;
		float txUSBDatarate = 0;
		byte[] txBuffer = new byte[34];

		public SeriesCollection SeriesAnalogIn { get; set; }
		public MainWindow() {
			InitializeComponent();

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

			if(rxLength > 1024) {
				serialPort.DiscardInBuffer();
			}
			else {
				serialPort.Read(rxData, 0, rxLength);

				rxCount += rxLength;

				if(rxUSBStopwatch.ElapsedMilliseconds > 1000) {
					rxUSBStopwatch.Reset();

					rxUSBDatarate = (float)rxCount * 0.001f;
					rxCount = 0;

					rxUSBStopwatch.Start();
				}
			}

			Dispatcher.Invoke(DispatcherPriority.Send, new SerialReceiveDelegate(SerialReceiveProcessing), rxData, rxLength);
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
	}
}
