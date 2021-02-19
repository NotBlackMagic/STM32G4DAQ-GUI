using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Reflection.Emit;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;

namespace NotBlackMagic {
	public enum AnalogInMode {
		Mode_Off,
		Mode_Diff,
		Mode_Single,
	}

	public enum AnalogInSampleRate {
		SampleRate_250k,
		SampleRate_125k,
		SampleRate_62k50,
		SampleRate_31k25,
		SampleRate_15k63,
		SampleRate_7k81,
		SampleRate_3k91,
		SampleRate_1k95,
		SampleRate_0k98,
		SampleRate_0k49,
		SampleRate_0k24,
		SampleRate_0k12
	}

	public enum AnalogInRange {
		InputRange_32,          //Analog Input Amplifer Gain of: 1/16
		InputRange_16,           //Analog Input Amplifer Gain of: 1/8
		InputRange_8,            //Analog Input Amplifer Gain of: 1/4
		InputRange_4,            //Analog Input Amplifer Gain of: 1/2
		InputRange_2,            //Analog Input Amplifer Gain of: 1
		InputRange_1,            //Analog Input Amplifer Gain of: 2
		InputRange_500m,         //Analog Input Amplifer Gain of: 4
		InputRange_250m,         //Analog Input Amplifer Gain of: 8
		InputRange_125m,         //Analog Input Amplifer Gain of: 16
		InputRange_62m5,         //Analog Input Amplifer Gain of: 32
		InputRange_31m25,        //Analog Input Amplifer Gain of: 64
		InputRange_15m625        //Analog Input Amplifer Gain of: 128
	}

	static class Opcodes {
		public const byte reset = 0x01;
		public const byte connect = 0x02;
		public const byte disconnect = 0x03;

		public const byte setCurrentA = 0x11;
		public const byte setCurrentB = 0x12;

		public const byte setAnalogInA = 0x13;
		public const byte setAnalogInB = 0x14;

		public const byte setAnalogInACH = 0x15;
		public const byte setAnalogInBCH = 0x16;

		public const byte setAnalogOutACH = 0x17;
		public const byte setAnalogOutBCH = 0x18;

		public const byte txAnalogInA = 0x81;
		public const byte txAnalogInB = 0x82;
	}
	class DAQPacket {
		public int opcode;
		public byte[] payload = new byte[512];
		public int payloadLength;
		public int crc;

		public void Decode(byte[] packet) {
			opcode = packet[0];
			payloadLength = (packet[1] << 8) + packet[2];
			Array.Copy(packet, 3, payload, 0, payloadLength);
			crc = (packet[payloadLength + 3] << 8) + packet[payloadLength + 4];
		}

		public static byte[] Encode(int opcode, byte[] payload) {
			int payloadLength = 0;
			
			if(payload != null) {
				payloadLength = payload.Length;
			}

			byte[] packet = new byte[payloadLength + 5];

			packet[0] = (byte)opcode;
			packet[1] = (byte)(payloadLength >> 8);
			packet[2] = (byte)(payloadLength);

			if(payloadLength > 0) {
				Array.Copy(payload, 0, packet, 3, payloadLength);
			}

			int crc = 0;
			packet[payloadLength + 3] = (byte)(crc >> 8);
			packet[payloadLength + 4] = (byte)(crc);

			return packet;
		}
	}

	class AnalogInChannel {
		int channel = 1;
		float gain = 0.125f;
		AnalogInMode mode;

		int resolution = 12;
		float vRef = 2.048f;

		volatile int valueLength = 0;
		volatile int index = 0;
		volatile int[] values = new int[1024];

		public AnalogInChannel(int channel, AnalogInRange range, AnalogInMode mode) {
			this.channel = channel;
			this.mode = mode;

			this.gain = (float)Math.Pow(2, ((int)range - 4));
		}

		public void AddValues(int[] values) {
			for(int i = 0; i < values.Length; i++) {
				//double scaling = (vRef / gain) / (1 << (resolution - 1));
				//double voltValue = (values[i] - (1 << (resolution - 1))) * scaling;
				this.values[index++] = values[i];

				if (index >= this.values.Length) {
					index = 0;
				}

				if(valueLength < this.values.Length) {
					valueLength += 1;
				}
			}
		}

		public double GetVoltageScaling() {
			return (vRef / gain) / (1 << (resolution - 1));
		}

		public int[] GetValues(int count) {
			int[] temp = new int[count];

			int tempIndex = this.index - count;

			if(tempIndex < 0) {
				tempIndex += this.values.Length;
			}

			for(int i = 0; i < count; i++) {
				temp[i] = this.values[tempIndex++];
				if(tempIndex >= this.values.Length) {
					tempIndex = 0;
				}
			}

			return temp;
		}
	}
	public class STMDAQ {
		bool rxUSBThreadRun = true;
		Thread rxUSBThread;
		SerialPort serialPort = new SerialPort();

		Stopwatch usbRXStopwatch = new Stopwatch();
		int usbRXByteCount = 0;
		int usbRXPacketCount = 0;
		int usbRXErrorCount = 0;
		float usbRXDatarate = 0;
		float usbRXErrorRate = 0;

		AnalogInChannel[] analogInChannels = new AnalogInChannel[16];

		public bool Connect(string port) {
			if(serialPort.IsOpen == false) {
				try {
					serialPort.PortName = port;
					serialPort.BaudRate = 921600;
					serialPort.DataBits = 8;
					serialPort.StopBits = StopBits.One;
					serialPort.Parity = Parity.None;
					serialPort.Open();

					USBWrite(Opcodes.connect, null);

					usbRXStopwatch.Start();

					rxUSBThreadRun = true;
					rxUSBThread = new Thread(USBRXThread);
					rxUSBThread.Start();
				}
				catch (Exception ex) {
					return false;
				}
			}
			else {
				return false;
			}
			return true;
		}

		public bool Disconnect() {
			if (serialPort.IsOpen == true) {
				USBWrite(Opcodes.disconnect, null);

				rxUSBThreadRun = false;

				//This delay gives time for the USBWrite to complete the write before destroying/closing the connection
				Thread.Sleep(100);

				serialPort.Close();
			}
			else {
				return false;
			}
			return true;
		}

		public bool IsConnected() {
			return serialPort.IsOpen;
		}

		public float USBRXDatarate() {
			return usbRXDatarate;
		}

		public float USBRXErrorRate() {
			return usbRXErrorRate;
		}

		private void USBRXThread() {
			DAQPacket rxPacket = new DAQPacket();

			int index = 0;
			int rxDataPacketLength = 0;
			byte[] rxDataPacket = new byte[550];

			byte[] rxBuffer = new byte[1024];
			while (rxUSBThreadRun) {
				if (serialPort.IsOpen) {
					int rxLength = 0;
					try {
						//rxLength = await serialPort.BaseStream.ReadAsync(rxBuffer, 0, rxBuffer.Length);
						rxLength = serialPort.BaseStream.Read(rxBuffer, 0, rxBuffer.Length);
					} catch {
						return;
					}

					for (int i = 0; i < rxLength; i++) {
						if (index == 0) {
							//First Byte of package is the Opcode
							rxDataPacket[index++] = rxBuffer[i];
						}
						else if (index == 1) {
							//Second and Third byte of package is the length
							rxDataPacket[index++] = rxBuffer[i];
							rxDataPacketLength = (rxBuffer[i] << 8);
						}
						else if (index == 2) {
							//Second and Third byte of package is the length
							rxDataPacket[index++] = rxBuffer[i];
							rxDataPacketLength += rxBuffer[i];

							if(rxDataPacketLength > 512) {
								//RX Buffer overflow
								index = 0;

								usbRXErrorCount += 1;
							}
						}
						else if (index < (rxDataPacketLength + 3 + 2)) {
							//Rest bytes are payload
							rxDataPacket[index++] = rxBuffer[i];
						}
						else {
							//Full packet received
							rxPacket.Decode(rxDataPacket);

							if(USBRXProcessing(rxPacket) != 0) {
								//Packet Processing Error
								usbRXErrorCount += 1;
							}

							usbRXPacketCount += 1;
							index = 0;
						}
					}

					usbRXByteCount += rxLength;
					if (usbRXStopwatch.ElapsedMilliseconds > 1000) {
						usbRXDatarate = (float)usbRXByteCount;
						usbRXByteCount = 0;

						usbRXErrorRate = (float)usbRXErrorCount;
						usbRXErrorCount = 0;

						usbRXStopwatch.Restart();
					}
				}
			}
		}

		private int USBRXProcessing(DAQPacket packet) {
			switch(packet.opcode) {
				case Opcodes.txAnalogInA:  {
					int channel = packet.payload[0];

					if(channel < 1 || channel > (analogInChannels.Length / 2)) {
						return 1;
					}

					if (analogInChannels[channel - 1] != null) {
						int sampleNum = (packet.payloadLength - 1) / 2;
						int[] values = new int[sampleNum];
						for (int i = 0; i < sampleNum; i++) {
							values[i] = (packet.payload[1 + (2 * i)] << 8) + packet.payload[1 + (2 * i + 1)];
						}

						analogInChannels[channel - 1].AddValues(values);
					}
					break;
				}
				case Opcodes.txAnalogInB: {
					int channel = packet.payload[0];

					if (channel < 1 || channel > (analogInChannels.Length / 2)) {
						return 1;
					}

					channel += 8;
					if (analogInChannels[channel - 1] != null) {
						int sampleNum = (packet.payloadLength - 1) / 2;
						int[] values = new int[sampleNum];
						for (int i = 0; i < sampleNum; i++) {
							values[i] = (packet.payload[1 + (2 * i)] << 8) + packet.payload[1 + (2 * i + 1)];
						}

						analogInChannels[channel - 1].AddValues(values);
					}
					break;
				}
			}

			return 0;
		}

		private void USBWrite(int opcode, byte[] payload) {
			if(serialPort.IsOpen) {
				byte[] packet = DAQPacket.Encode(opcode, payload);
				serialPort.Write(packet, 0, packet.Length);
			}
		}

		public void AddAnalogIn(int channel, AnalogInRange range, AnalogInMode mode) {
			analogInChannels[channel - 1] = new AnalogInChannel(channel, range, mode);

			//Set DAQ settings: Set Analog In Block
			//byte[] data = new byte[3];
			//data[0] = (byte)mode;		//Set Mode
			//data[1] = 1;				//Set Sample Rate
			//data[2] = 0;				//Set Scale
			//USBWrite(Opcodes.setAnalogInA, data);

			int nChannels = 0;
			for(int i = 0; i < analogInChannels.Length; i++) {
				if(analogInChannels[i] != null) {
					nChannels += 1;
				}
			}

			//Set DAQ settings: Set Analog In Channel
			byte[] data = new byte[5];
			data[1] = (byte)mode;               //Channel Mode, Differential or Single Ended
			data[2] = (byte)nChannels;          //Set Sampling Rate/Time division
			data[3] = 4;                        //Set Resolution
			data[4] = (byte)(range);			//Set Gain

			if (channel <= (analogInChannels.Length / 2)) {
				data[0] = (byte)channel;            //To set channel
				USBWrite(Opcodes.setAnalogInACH, data);
			}
			else {
				data[0] = (byte)(channel - 8);		//To set channel
				USBWrite(Opcodes.setAnalogInBCH, data);
			} 
		}

		public void SetAnalogInSampligRate(int channel, AnalogInSampleRate samplingRate) {
			byte[] data = new byte[2];
			data[0] = (byte)samplingRate;		//Set Sample Rate
			data[1] = 0;                //Set Scale
			if (channel <= (analogInChannels.Length / 2)) {
				USBWrite(Opcodes.setAnalogInA, data);
			}
			else {
				USBWrite(Opcodes.setAnalogInB, data);
			}
		}

		public int[] ReadAnalogIn(int channel, int count) {
			if(analogInChannels[channel - 1] != null) {
				return analogInChannels[channel - 1].GetValues(count);
			}
			else {
				return null;
			}
		}

		public double[] ReadAnalogInVolt(int channel, int count) {
			if (analogInChannels[channel - 1] != null) {
				double scaling = analogInChannels[channel - 1].GetVoltageScaling();

				int[] values = new int[count];
				values = analogInChannels[channel - 1].GetValues(count);

				//double scaling = (vRef / gain) / (1 << (resolution - 1));
				//double voltValue = (values[i] - (1 << (resolution - 1))) * scaling;

				double[] voltValue = new double[count];
				for(int i = 0; i < count; i++) {
					voltValue[i] = ((values[i] - 2048) * scaling);
				}

				return voltValue;
			}
			else {
				return null;
			}
		}

		public void AddCurrentOut(int channel, int value) {
			byte[] data = new byte[2];

			data[0] = 0x01;
			data[1] = (byte)(value / 100);

			if (channel == 1) {
				USBWrite(Opcodes.setCurrentA, data);
			}
			else if(channel == 2) {
				USBWrite(Opcodes.setCurrentB, data);
			}
		}

		const int rg1 = 12000;
		const int rg2 = 10000;
		const int rf = 62000;
		public void AddAnalogOutput(int channel, double[] value, int frequency) {
			int length = value.Length;

			byte[] data = new byte[6 + 2*length];

			for(int i = 0; i < length; i++) {
				double tempValue = value[i] + ((double)rf / rg2 * 2.048);
				tempValue = tempValue / (1.0 + (double)rf / rg2 + (double)rf / rg1);

				int dacCount = Convert.ToInt32(tempValue * (4096 / 2.048));
				if(dacCount > 4095) {
					dacCount = 4095;
				}
				else if(dacCount < 0) {
					dacCount = 0;
				}

				data[6 + 2 * i] = (byte)(dacCount >> 8);
				data[7 + 2 * i] = (byte)(dacCount);
			}

			data[1] = (byte)(frequency >> 16);				//Output Sampling Frequncy (3 bytes)
			data[2] = (byte)(frequency >> 8);				
			data[3] = (byte)(frequency);
			data[4] = (byte)(length >> 8);					//Output Sampling Buffer Length (uint16_t)
			data[5] = (byte)(length);

			if (channel == 1 || channel == 2) {
				data[0] = (byte)channel;    //Output Channel
				USBWrite(Opcodes.setAnalogOutACH, data);
			}
			else if(channel == 3 || channel == 4) {
				data[0] = (byte)(channel - 2);    //Output Channel
				USBWrite(Opcodes.setAnalogOutBCH, data);
			}
		}

		~STMDAQ() {
			if (serialPort.IsOpen == true) {
				USBWrite(Opcodes.disconnect, null);

				rxUSBThreadRun = false;

				//This delay gives time for the USBWrite to complete the write before destroying/closing the connection
				Thread.Sleep(100);

				serialPort.Close();
			}
		}
	}
}