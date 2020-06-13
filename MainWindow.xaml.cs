using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
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

		byte[] txBuffer = new byte[34];

		public MainWindow() {
			InitializeComponent();
		}

		private delegate void SerialRecevieDelegate(byte[] rxData, int length);

		private void OnSerialReceive(object sender, SerialDataReceivedEventArgs e) {
			SerialPort sp = (SerialPort)sender;

			byte[] rxData = new byte[100];
			int rxLength = serialPort.ReadByte();

			serialPort.Read(rxData, 0, rxLength);
			//int i;
			//for(i = 0; i < rxLength; i++) {
			//	rxData[i] = (byte)serialPort.ReadByte();
			//}
			int crc = serialPort.ReadByte();

			Dispatcher.Invoke(DispatcherPriority.Send, new SerialRecevieDelegate(SerialReceiveProcessing), rxData, rxLength);
		}

		private void SerialReceiveProcessing(byte[] rxData, int length) {

		}

		private void SerialButtonClick(object sender, RoutedEventArgs e) {
			//if (SerialButton.Content.ToString() == "Connect") {
			if (serialPort.IsOpen == false) {
				try {
					serialPort.PortName = portNamesComboBox.Text;
					serialPort.BaudRate = Convert.ToInt32(baudRatesComboBox.Text);
					//serialPort.DataBits = 8;
					//serialPort.StopBits = StopBits.None;
					//serialPort.Handshake = 0;
					//serialPort.Parity = Parity.None;
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
