﻿using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;


namespace NotBlackMagic {
	public enum AnalogInMode {
		Mode_Off,
		Mode_Diff,
		Mode_INP,
		Mode_INN
	}
	static class Opcodes {
		public const byte setCurrentA = 0x01;
		public const byte setCurrentB = 0x02;

		public const byte setAnalogInA = 0x03;
		public const byte setAnalogInACH = 0x04;

		public const byte setAnalogOutACH = 0x05;

		public const byte txAnalogInA = 0x81;
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
			byte[] packet = new byte[payload.Length + 5];

			packet[0] = (byte)opcode;
			packet[1] = (byte)(payload.Length >> 8);
			packet[2] = (byte)(payload.Length);

			Array.Copy(payload, 0, packet, 3, payload.Length);

			int crc = 0;
			packet[payload.Length + 3] = (byte)(crc >> 8);
			packet[payload.Length + 4] = (byte)(crc);

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
		volatile float[] values = new float[1024];

		public AnalogInChannel(int channel, float range, AnalogInMode mode) {
			this.channel = channel;
			this.mode = mode;

			double g = Math.Log2(vRef / range);

			this.gain = (float)Math.Pow(2, Math.Round(g));
		}

		public void AddValues(int[] values) {
			for(int i = 0; i < values.Length; i++) {
				float scaling = (vRef / gain) / (1 << (resolution - 1));
				this.values[index++] = (float)(values[i] - (1 << (resolution - 1))) * scaling;

				if (index >= this.values.Length) {
					index = 0;
				}

				if(valueLength < this.values.Length) {
					valueLength += 1;
				}
			}
		}

		public float[] GetValues(int count) {
			float[] temp = new float[count];

			int tempIndex = this.index;
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
		Thread rxUSBThread;
		SerialPort serialPort = new SerialPort();

		Stopwatch usbRXStopwatch = new Stopwatch();
		int usbRXByteCount = 0;
		int usbRXPacketCount = 0;
		float usbRXDatarate = 0;

		AnalogInChannel[] analogInAChannels = new AnalogInChannel[4];

		public bool Connect(string port) {
			if(serialPort.IsOpen == false) {
				try {
					serialPort.PortName = port;
					serialPort.BaudRate = 921600;
					serialPort.DataBits = 8;
					serialPort.StopBits = StopBits.One;
					serialPort.Parity = Parity.None;
					serialPort.Open();

					usbRXStopwatch.Start();

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
			if (serialPort.IsOpen == false) {
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

		private async void USBRXThread() {
			DAQPacket rxPacket = new DAQPacket();

			int index = 0;
			int rxDataPacketLength = 0;
			byte[] rxDataPacket = new byte[550];

			byte[] rxBuffer = new byte[1024];
			while (true) {
				if (serialPort.IsOpen) {
					int rxLength = await serialPort.BaseStream.ReadAsync(rxBuffer, 0, rxBuffer.Length);

					for (int i = 0; i < rxLength; i++) {
						if (index == 0) {
							rxDataPacket[index++] = rxBuffer[i];
						}
						else if (index == 1) {
							rxDataPacket[index++] = rxBuffer[i];
							rxDataPacketLength = (rxBuffer[i] << 8);
						}
						else if (index == 2) {
							rxDataPacket[index++] = rxBuffer[i];
							rxDataPacketLength += rxBuffer[i];

							if(rxDataPacketLength > 512) {
								index = 0;
							}
						}
						else if (index < (rxDataPacketLength + 3 + 2)) {
							rxDataPacket[index++] = rxBuffer[i];
						}
						else {
							rxPacket.Decode(rxDataPacket);

							USBRXProcessing(rxPacket);

							usbRXPacketCount += 1;
							index = 0;
						}
					}

					usbRXByteCount += rxLength;
					if (usbRXStopwatch.ElapsedMilliseconds > 1000) {
						usbRXDatarate = (float)usbRXByteCount;
						usbRXByteCount = 0;

						usbRXStopwatch.Restart();
					}
				}
			}
		}

		private void USBRXProcessing(DAQPacket packet) {
			switch(packet.opcode) {
				case Opcodes.txAnalogInA:  {
					int channel = packet.payload[0];

					if(channel > 4) {
						return;
					}

					if (analogInAChannels[channel - 1] != null) {
						int sampleNum = (packet.payloadLength - 1) / 2;
						int[] values = new int[sampleNum];
						for (int i = 0; i < sampleNum; i++) {
							values[i] = (packet.payload[1 + (2 * i)] << 8) + packet.payload[1 + (2 * i + 1)];
						}

						analogInAChannels[channel - 1].AddValues(values);
					}
					break;
				}
			}
		}

		private void USBWrite(int opcode, byte[] payload) {
			if(serialPort.IsOpen) {
				byte[] packet = DAQPacket.Encode(opcode, payload);
				serialPort.Write(packet, 0, packet.Length);
			}
		}

		public void AddAnalogIn(int channel, float range, AnalogInMode mode) {
			analogInAChannels[channel - 1] = new AnalogInChannel(channel, range, mode);

			//Set DAQ settings: Set Analog In Block
			//byte[] data = new byte[3];
			//data[0] = (byte)mode;		//Set Mode
			//data[1] = 1;				//Set Sample Rate
			//data[2] = 0;				//Set Scale
			//USBWrite(Opcodes.setAnalogInA, data);

			//Set DAQ settings: Set Analog In Channel
			byte[] data = new byte[5];
			data[0] = (byte)channel;	//To set channel
			data[1] = 1;				//If Enabled or Disabeld
			data[2] = 0;				//Set Sampling Rate/Time division
			data[3] = 4;				//Set Resolution
			data[4] = (byte)(3 + (int)Math.Ceiling(Math.Log2(2.048 / range)));	//Set Gain
			USBWrite(Opcodes.setAnalogInACH, data);
		}

		public void SetAnalogInSampligRate(int samplingRate) {
			int rate = (int)((250000.0 / samplingRate) - 1.0);

			byte[] data = new byte[3];
			data[0] = 1;				//Set Mode
			data[1] = (byte)rate;		//Set Sample Rate
			data[2] = 0;				//Set Scale
			USBWrite(Opcodes.setAnalogInA, data);
		}

		public float[] ReadAnalogIn(int channel, int count) {
			if(analogInAChannels[channel - 1] != null) {
				return analogInAChannels[channel - 1].GetValues(count);
			}
			else {
				return null;
			}
		}

		~STMDAQ() {
			if (serialPort.IsOpen == false) {
				serialPort.Close();

				rxUSBThread.Abort();
			}
		}
	}
}