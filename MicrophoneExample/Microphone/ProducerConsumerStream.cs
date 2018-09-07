using System;
using System.IO;

namespace Speechmatics.Realtime.Microphone
{
    public class ProducerConsumerStream : Stream
    {
        private readonly MemoryStream _innerStream;
        private long _readPosition;
        private long _writePosition;

        public ProducerConsumerStream()
        {
            _innerStream = new MemoryStream();
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;

        public override void Flush()
        {
            lock (_innerStream)
            {
                _innerStream.Flush();
            }
        }

        public override long Length
        {
            get
            {
                lock (_innerStream)
                {
                    return _innerStream.Length;
                }
            }
        }

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            lock (_innerStream)
            {
                _innerStream.Position = _readPosition;
                var red = _innerStream.Read(buffer, offset, count);
                _readPosition = _innerStream.Position;

                if (red == 0)
                {
                    // The buffer is empty but let's pretend it's not and send silence
                    for (var i = 0; i < count; i++)
                    {
                        buffer[offset + i] = 0;
                    }
                    return count;
                }

                return red;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            lock (_innerStream)
            {
                _innerStream.Position = _writePosition;
                _innerStream.Write(buffer, offset, count);
                _writePosition = _innerStream.Position;
            }
        }
    }
}