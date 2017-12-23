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

        public static int SafeRead(Stream stream, byte[] buffer, int offset, int length)
        {
            try
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
            catch (Exception)
            {
                return -1;
            }
        }

        public static void SafeWrite(Stream stream, byte[] buffer, int offset, int length)
        {
            stream.Write(buffer, offset, length);
        }
    }
}
