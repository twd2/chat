using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace ChatClient
{
    class Packet
    {
        public const byte PACKET_LOGIN = 0;
        public const byte PACKET_REGISTER = 1;
        public const byte PACKET_LIST_USER = 2;
        public const byte PACKET_LIST_BUDDY = 3;
        public const byte PACKET_ADD_BUDDY = 4;
        public const byte PACKET_REMOVE_BUDDY = 5;
        public const byte PACKET_MESSAGE = 6;
        public const byte PACKET_RAW = 254;
        public const byte PACKET_RESET = 255;

        public static byte[] Read(Stream stream, out byte type)
        {
            type = 0;
            byte[] buffer = new byte[5];
            if (Util.SafeRead(stream, buffer, 0, buffer.Length) <= 0)
            {
                return null;
            }
            uint size = 0;
            size |= (uint)buffer[0] << 24;
            size |= (uint)buffer[1] << 16;
            size |= (uint)buffer[2] << 8;
            size |= (uint)buffer[3] << 0;
            type = buffer[4];

            if (size == 0)
            {
                return new byte[0];
            }

            buffer = new byte[size];
            if (Util.SafeRead(stream, buffer, 0, buffer.Length) <= 0)
            {
                return null;
            }
            return buffer;
        }

        public static void Write(Stream stream, byte[] buffer, byte type)
        {
            uint len = (uint)buffer.Length;
            byte[] header = new byte[5];
            header[0] = (byte)((len >> 24) & 0xff);
            header[1] = (byte)((len >> 16) & 0xff);
            header[2] = (byte)((len >> 8) & 0xff);
            header[3] = (byte)((len >> 0) & 0xff);
            header[4] = type;
            Util.SafeWrite(stream, header, 0, header.Length);
            Util.SafeWrite(stream, buffer, 0, buffer.Length);
        }

        public static void Write(Stream stream, LoginRequest p)
        {
            Write(stream, p.ToByteArray(), PACKET_LOGIN);
        }

        public static void Write(Stream stream, RegisterRequest p)
        {
            Write(stream, p.ToByteArray(), PACKET_REGISTER);
        }

        public static void Write(Stream stream, ListUserRequest p)
        {
            Write(stream, p.ToByteArray(), PACKET_LIST_USER);
        }

        public static void Write(Stream stream, ListBuddyRequest p)
        {
            Write(stream, p.ToByteArray(), PACKET_LIST_BUDDY);
        }

        public static void Write(Stream stream, AddBuddyRequest p)
        {
            Write(stream, p.ToByteArray(), PACKET_ADD_BUDDY);
        }

        public static void Write(Stream stream, RemoveBuddyRequest p)
        {
            Write(stream, p.ToByteArray(), PACKET_REMOVE_BUDDY);
        }

        public static void Write(Stream stream, Message p)
        {
            Write(stream, p.ToByteArray(), PACKET_MESSAGE);
        }

        public static void Write(Stream stream, Reset p)
        {
            Write(stream, p.ToByteArray(), PACKET_RESET);
        }
    }
}
