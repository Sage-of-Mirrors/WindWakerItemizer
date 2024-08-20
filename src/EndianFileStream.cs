using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WindWakerItemizer
{
    internal enum EFileEndianness
    {
        Big,
        Little
    }

    internal class EndianStreamReader : IDisposable
    {
        public string FileName { get; set; }

        private EFileEndianness mEndianness;
        private FileStream mStream;
        private bool disposedValue;

        public EndianStreamReader(string fileName, EFileEndianness endianness = EFileEndianness.Big)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException("Unable to open specified file", fileName);
            }

            FileName = fileName;
            mEndianness = endianness;

            mStream = File.OpenRead(fileName);
            if (mStream.SafeFileHandle.IsInvalid)
            {
                throw new Exception("Failed to open file for reading!");
            }
        }

        public void Seek(long offset, SeekOrigin seekOrigin = SeekOrigin.Begin)
        {
            mStream.Seek(offset, seekOrigin);
        }

        public long Tell()
        {
            return mStream.Position;
        }

        public byte ReadByte()
        {
            return (byte)mStream.ReadByte();
        }

        public char ReadChar()
        {
            return (char)mStream.ReadByte();
        }

        public ushort ReadUShort()
        {
            byte[] buf = new byte[2];

            switch(mEndianness)
            {
                case EFileEndianness.Big:
                    buf[1] = ReadByte();
                    buf[0] = ReadByte();
                    break;
                case EFileEndianness.Little:
                    buf[0] = ReadByte();
                    buf[1] = ReadByte();
                    break;
            }

            return BitConverter.ToUInt16(buf, 0);
        }

        public short ReadShort()
        {
            byte[] buf = new byte[2];

            switch (mEndianness)
            {
                case EFileEndianness.Big:
                    buf[1] = ReadByte();
                    buf[0] = ReadByte();
                    break;
                case EFileEndianness.Little:
                    buf[0] = ReadByte();
                    buf[1] = ReadByte();
                    break;
            }

            return BitConverter.ToInt16(buf, 0);
        }

        public uint ReadUInt()
        {
            byte[] buf = new byte[4];

            switch (mEndianness)
            {
                case EFileEndianness.Big:
                    buf[3] = ReadByte();
                    buf[2] = ReadByte();
                    buf[1] = ReadByte();
                    buf[0] = ReadByte();
                    break;
                case EFileEndianness.Little:
                    buf[0] = ReadByte();
                    buf[1] = ReadByte();
                    buf[2] = ReadByte();
                    buf[3] = ReadByte();
                    break;
            }

            return BitConverter.ToUInt32(buf, 0);
        }

        public int ReadInt()
        {
            byte[] buf = new byte[4];

            switch (mEndianness)
            {
                case EFileEndianness.Big:
                    buf[3] = ReadByte();
                    buf[2] = ReadByte();
                    buf[1] = ReadByte();
                    buf[0] = ReadByte();
                    break;
                case EFileEndianness.Little:
                    buf[0] = ReadByte();
                    buf[1] = ReadByte();
                    buf[2] = ReadByte();
                    buf[3] = ReadByte();
                    break;
            }

            return BitConverter.ToInt32(buf, 0);
        }

        public float ReadSingle()
        {
            byte[] buf = new byte[4];

            switch (mEndianness)
            {
                case EFileEndianness.Big:
                    buf[3] = ReadByte();
                    buf[2] = ReadByte();
                    buf[1] = ReadByte();
                    buf[0] = ReadByte();
                    break;
                case EFileEndianness.Little:
                    buf[0] = ReadByte();
                    buf[1] = ReadByte();
                    buf[2] = ReadByte();
                    buf[3] = ReadByte();
                    break;
            }

            return BitConverter.ToSingle(buf, 0);
        }

        public string ReadString()
        {
            List<byte> buf = new List<byte>();

            byte curByte = ReadByte();
            while (curByte != 0)
            {
                buf.Add(curByte);
                curByte = ReadByte();
            }

            return Encoding.UTF8.GetString(buf.ToArray());
        }

        public void Skip(long amt)
        {
            Seek(amt, SeekOrigin.Current);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    mStream.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
