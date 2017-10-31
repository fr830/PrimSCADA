using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace SCADA
{
    class PModbus
    {
        int Retries = 3;

        private void GetCRC(byte[] message, ref byte[] CRC)
        {
            //Function expects a modbus message of any length as well as a 2 byte CRC array in which to 
            //return the CRC values:

            ushort CRCFull = 0xFFFF;
            byte CRCHigh = 0xFF, CRCLow = 0xFF;
            char CRCLSB;

            for (int i = 0; i < (message.Length) - 2; i++)
            {
                CRCFull = (ushort)(CRCFull ^ message[i]);

                for (int j = 0; j < 8; j++)
                {
                    CRCLSB = (char)(CRCFull & 0x0001);
                    CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);

                    if (CRCLSB == 1)
                        CRCFull = (ushort)(CRCFull ^ 0xA001);
                }
            }
            CRC[1] = CRCHigh = (byte)((CRCFull >> 8) & 0xFF);
            CRC[0] = CRCLow = (byte)(CRCFull & 0xFF);
        }

        private bool CheckResponse(byte[] response)
        {
            //Perform a basic CRC check: 
            byte[] CRC = new byte[2];
            GetCRC(response, ref CRC);
            if (CRC[0] == response[response.Length - 2] && CRC[1] == response[response.Length - 1])
                return true;
            else
                return false;
        }

        public float SendFc3(SerialPort serialPort, byte address, ushort start, ushort registers, ref ushort[] values, ref byte[] buffer)
        {
            if (serialPort.IsOpen)
            {
                //Очищаем входной и выходной буфера:
                serialPort.DiscardOutBuffer();
                serialPort.DiscardInBuffer();
                //Function 3 request is always 8 bytes:
                byte[] message = new byte[8];
                //Function 3 response buffer:
                byte[] response = new byte[5 + 2 * registers];
                //Build outgoing modbus message:
                BuildMessage(address, (byte)3, start, registers, ref message);
                //Send modbus message to Serial Port:
                
                serialPort.Write(message, 0, message.Length);
                Thread.Sleep(105);
                serialPort.Read(response, 0, response.Length);
                Thread.Sleep(105);
                
                //Evaluate message:
                if (CheckResponse(response))
                {
                    //Return requested register values

                    for (int i = 0; i < (response.Length - 5) / 2; i++)
                    {
                        values[i] = response[2 * i + 3];
                        values[i] <<= 8;
                        values[i] += response[2 * i + 4];
                    }

                    buffer[0] = BitConverter.GetBytes(values[0])[1];
                    buffer[1] = BitConverter.GetBytes(values[0])[0];
                    buffer[2] = BitConverter.GetBytes(values[1])[1];
                    buffer[3] = BitConverter.GetBytes(values[1])[0];

                    Retries = 0;

                    return BitConverter.ToSingle(buffer, 0);
                }
                else
                {
                    Retries++;

                    if (Retries == 3)
                    {
                        new Exception("Ошибка CRC");
                    }

                    return float.NaN;
                }
            }
            else
            {
                return float.NaN;
            }
        }

        public float SendFc4(SerialPort serialPort, byte address, ushort start, ushort registers, ref ushort[] values, ref byte[] buffer)
        {
            if (serialPort.IsOpen)
            {
                //Очищаем входной и выходной буфера:
                serialPort.DiscardOutBuffer();
                serialPort.DiscardInBuffer();
                //Function 3 request is always 8 bytes:
                byte[] message = new byte[8];
                //Function 3 response buffer:
                byte[] response = new byte[5 + 2 * registers];
                //Build outgoing modbus message:
                BuildMessage(address, (byte)4, start, registers, ref message);
                //Send modbus message to Serial Port:

                serialPort.Write(message, 0, message.Length);
                Thread.Sleep(105);
                serialPort.Read(response, 0, response.Length);
                Thread.Sleep(105);

                //Evaluate message:
                if (CheckResponse(response))
                {
                    //Return requested register values

                    for (int i = 0; i < (response.Length - 5) / 2; i++)
                    {
                        values[i] = response[2 * i + 3];
                        values[i] <<= 8;
                        values[i] += response[2 * i + 4];
                    }

                    buffer[0] = BitConverter.GetBytes(values[0])[1];
                    buffer[1] = BitConverter.GetBytes(values[0])[0];
                    buffer[2] = BitConverter.GetBytes(values[1])[1];
                    buffer[3] = BitConverter.GetBytes(values[1])[0];

                    Retries = 0;

                    return BitConverter.ToSingle(buffer, 0);
                }
                else
                {
                    Retries++;

                    if (Retries == 3)
                    {
                        new Exception("Ошибка CRC");
                    }

                    return float.NaN;
                }
            }
            else
            {
                return float.NaN;
            }
        }

        private void BuildMessage(byte address, byte type, ushort start, ushort registers, ref byte[] message)
        {
            byte[] CRC = new byte[2];
            message[0] = address;
            message[1] = type;
            message[2] = (byte)(start >> 8);
            message[3] = (byte)start;
            message[4] = (byte)(registers >> 8);
            message[5] = (byte)registers;
            GetCRC(message, ref CRC);
            message[message.Length - 2] = CRC[0];
            message[message.Length - 1] = CRC[1];
        }
    }
}
