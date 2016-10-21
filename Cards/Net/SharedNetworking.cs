using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace JoePitt.Cards.Net
{
    internal static class SharedNetworking
    {
        public static byte[] Receive(TcpClient Client)
        {
            try
            {
                int bytesRead = 0;
                int length = 0;
                byte[] buffer = new byte[4];
                NetworkStream stream = Client.GetStream();
                while (bytesRead < 4)
                {
                    buffer[bytesRead] = (byte)stream.ReadByte();
                    bytesRead++;
                }
                length = BitConverter.ToInt32(buffer, 0);
                buffer = new byte[length];

                bytesRead = 0;
                while (bytesRead < length)
                {
                    buffer[bytesRead] = (byte)stream.ReadByte();
                    bytesRead++;
                }
                return buffer;
            }
            catch (IOException)
            {
                return new byte[0];
            }
        }

        public static string ReceiveString(TcpClient Client)
        {
            string message = Encoding.Default.GetString(Receive(Client));
            return message;
        }

        public static bool Send(TcpClient Client, string Message)
        {
            byte[] messageBytes = Encoding.Default.GetBytes(Message);
            return Send(Client, messageBytes);
        }

        public static bool Send(TcpClient Client, byte[] Message)
        {
            int bytesSend = 0;
            int length = Message.Length;
            byte[] buffer = BitConverter.GetBytes(length);
            try
            {
                if (Client.Connected)
                {
                    NetworkStream stream = Client.GetStream();
                    while (bytesSend < 4)
                    {
                        stream.WriteByte(buffer[bytesSend]);
                        bytesSend++;
                    }

                    bytesSend = 0;
                    while (bytesSend < length)
                    {
                        stream.WriteByte(Message[bytesSend]);
                        bytesSend++;
                    }
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
