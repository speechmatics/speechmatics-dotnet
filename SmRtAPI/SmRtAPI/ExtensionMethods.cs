using System;
using System.ComponentModel;
using Speechmatics.Realtime.Client.Enumerations;

namespace Speechmatics.Realtime.Client
{
    internal static class ExtensionMethods
    {
        public static string ToApiString(this Enum enumValue)
        {
            var name = enumValue.GetType().Name;

            switch (name)
            {
                case "AudioFormatType":
                    switch ((AudioFormatType)enumValue)
                    {
                        case AudioFormatType.Raw:
                            return "raw";
                        case AudioFormatType.File:
                            return "file";
                        default:
                            throw new InvalidEnumArgumentException(nameof(enumValue));
                    }
                case "AudioFormatEncoding":
                    {
                        switch ((AudioFormatEncoding)enumValue)
                        {
                            case AudioFormatEncoding.File:
                                return "File";
                            case AudioFormatEncoding.PcmF32Le:
                                return "pcm_f32le";
                            case AudioFormatEncoding.PcmS16Le:
                                return "pcm_s16le";
                            case AudioFormatEncoding.Mulaw:
                                return "mulaw";
                            default:
                                throw new InvalidEnumArgumentException(nameof(enumValue));
                        }
                    }
                default:
                    throw new InvalidEnumArgumentException(nameof(enumValue));
            }
        }
    }
}