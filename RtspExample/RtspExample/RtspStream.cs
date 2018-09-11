using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Speechmatics.Client.RtspExample
{
    internal class RtspStream : Stream
    {
        private StreamReader _reader;
        private readonly Process _ffmpegProcess;

        public RtspStream(string rtspEndpoint)
        {
            var ffmpegPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ffmpeg.exe");
            var processStartInfo =
                new ProcessStartInfo(ffmpegPath, $"-nostats -loglevel 1 -i {rtspEndpoint} -vn -f mp3 -")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };
            _ffmpegProcess = new Process { StartInfo = processStartInfo };
        }

        public void Go()
        {
            _ffmpegProcess.Start();
            _reader = _ffmpegProcess.StandardOutput;
        }

        protected override void Dispose(bool disposing)
        {
            _ffmpegProcess?.Dispose();
            base.Dispose();
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _reader.BaseStream.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length { get; }

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }
    }
}
