using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ChatClient
{
    class Util
    {
        public const uint CHUNK_SIZE = 4096;

        public static T ByteArrayToStructure<T>(byte[] bytes)
        {
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                return (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            }
            finally
            {
                handle.Free();
            }
        }

        public static byte[] StructureToByteArray<T>(T obj)
        {
            byte[] bytes = new byte[Marshal.SizeOf(typeof(T))];
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                Marshal.StructureToPtr(obj, handle.AddrOfPinnedObject(), false);
                return bytes;
            }
            finally
            {
                handle.Free();
            }
        }

        public static int SafeRead(Stream stream, byte[] buffer, int offset, int length)
        {
            int read = 0;
            while (read < length)
            {
                int result = stream.Read(buffer, offset + read, (int)Math.Min(CHUNK_SIZE, length - (uint)read));
                if (result <= 0)
                {
                    return result;
                }
                read += result;
            }
            return read;
        }

        public static void SafeWrite(Stream stream, byte[] buffer, int offset, int length)
        {
            stream.Write(buffer, offset, length);
        }
    }
}
