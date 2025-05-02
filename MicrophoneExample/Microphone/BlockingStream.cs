﻿using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

// https://stackoverflow.com/a/3729877

namespace Speechmatics.Realtime.Microphone
{
    public class BlockingStream : Stream
    {
        private readonly BlockingCollection<byte[]> _blocks;
        private byte[] _currentBlock;
        private int _currentBlockIndex;

        public BlockingStream(int streamWriteCountCache)
        {
            _blocks = new BlockingCollection<byte[]>(streamWriteCountCache);
        }

        public override bool CanTimeout => false;
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => throw new NotSupportedException();

        public override void Flush()
        {
        }

        public long TotalBytesWritten { get; private set; }
        public int WriteCount { get; private set; }

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            ValidateBufferArgs(buffer, offset, count);

            int bytesRead = 0;
            while (true)
            {
                if (_currentBlock != null)
                {
                    int copy = Math.Min(count - bytesRead, _currentBlock.Length - _currentBlockIndex);
                    Buffer.BlockCopy(_currentBlock, _currentBlockIndex, buffer, offset + bytesRead, copy);
                    _currentBlockIndex += copy;
                    bytesRead += copy;

                    if (_currentBlock.Length <= _currentBlockIndex)
                    {
                        _currentBlock = null;
                        _currentBlockIndex = 0;
                    }

                    if (bytesRead == count)
                        return bytesRead;
                }

                if (!_blocks.TryTake(out _currentBlock, Timeout.Infinite))
                    return bytesRead;
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            ValidateBufferArgs(buffer, offset, count);

            var newBuf = new byte[count];
            Buffer.BlockCopy(buffer, offset, newBuf, 0, count);
            _blocks.Add(newBuf);
            TotalBytesWritten += count;
            WriteCount++;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _blocks.Dispose();
            }
        }

        public override void Close()
        {
            CompleteWriting();
            base.Close();
        }

        public void CompleteWriting()
        {
            _blocks.CompleteAdding();
        }

        private static void ValidateBufferArgs(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (buffer.Length - offset < count)
                throw new ArgumentException($"{nameof(buffer)}.Length - {nameof(offset)} < {nameof(count)}");
        }
    }
}