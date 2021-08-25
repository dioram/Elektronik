using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using RosSharp.RosBridgeClient.MessageTypes.Std;
using RosMessage = RosSharp.RosBridgeClient.Message;
using RosString = RosSharp.RosBridgeClient.MessageTypes.Std.String;

namespace Elektronik.RosPlugin.Common.RosMessages
{
    public static partial class MessageParser
    {
        private static readonly Dictionary<string, Func<Stream, bool, RosMessage>> CustomParsers = new()
        {
            {"std_msgs/msg/String", ParseStringMessage},
            {"std_msgs/String", ParseStringMessage},
            {"visualization_msgs/msg/MarkerArray", ParseMarkerArray},
            {"visualization_msgs/MarkerArray", ParseMarkerArray},
            {"sensing_msgs/VideoFrame", ParseVideoFrame},
            {"sensing_msgs/msg/VideoFrame", ParseVideoFrame},
            {"control_msgs/CarState", ParseCarState},
            {"control_msgs/msg/CarState", ParseCarState},
            {"control_msgs/Command", ParseCommand},
            {"control_msgs/msg/Command", ParseCommand},
            {"gps_msgs/Gga", ParseGga},
            {"gps_msgs/msg/Gga", ParseGga},
            {"control_msgs/GSensorAcc", ParseGSensorAcc},
            {"control_msgs/msg/GSensorAcc", ParseGSensorAcc},
            {"control_msgs/WheelAxleVelocity", ParseWheelAxleVelocity},
            {"control_msgs/msg/WheelAxleVelocity", ParseWheelAxleVelocity},
        };

        public static RosMessage? Parse(byte[] data, string topic, bool cdr)
        {
            MemoryStream memoryStream = new(data);
            memoryStream.Position = 0;
            return Parse(memoryStream, topic, cdr);
        }        
        
        public static T? Parse<T>(byte[] data, string topic, bool cdr) where T : RosMessage
        {
            MemoryStream memoryStream = new(data);
            memoryStream.Position = 0;
            return Parse(memoryStream, topic, cdr) as T;
        }

        public static RosMessage? Parse(Stream data, string topic, bool cdr)
        {
            ParseInt32(data, cdr);
            return CustomParsers.ContainsKey(topic) ? CustomParsers[topic](data, cdr) : null;
        }

        private static RosString ParseStringMessage(Stream data, bool cdr)
            => new(ParseString(data, cdr));

        private static MarkerArray ParseMarkerArray(Stream data, bool cdr)
            => new MarkerArray {Markers = ParseArray(data, ParseMarker, cdr)};

        private static VideoFrame ParseVideoFrame(Stream data, bool cdr)
            => new VideoFrame
            {
                Header = ParseHeader(data, cdr),
                dts = ParseUInt64(data, cdr),
                pts = ParseUInt64(data, cdr),
                Duration = ParseUInt64(data, cdr),
                Data = ParseByteArray(data, cdr),
            };

        private static CarState ParseCarState(Stream data, bool cdr)
            => new CarState
            {
                Header = ParseHeader(data, cdr),
                is_brake = ParseBoolean(data, cdr),
                brake_press = ParseUInt16(data, cdr),
                is_acceleration = ParseBoolean(data, cdr),
                acceleration_press = ParseDouble(data, cdr),
                button_state = ParseBoolean(data, cdr),
            };

        private static Command ParseCommand(Stream data, bool cdr)
            => new Command()
            {
                Header = ParseHeader(data, cdr),
                type = ParseByte(data, cdr),
                acc = ParseDouble(data, cdr),
                dec = ParseDouble(data, cdr),
                angle = ParseDouble(data, cdr),
                current_long_ref = ParseDouble(data, cdr),
                current_lat_ref = ParseDouble(data, cdr),
                current_lat_ref_yaw = ParseDouble(data, cdr),
            };

        private static Gga ParseGga(Stream data, bool cdr)
            => new Gga()
            {
                Header = ParseHeader(data, cdr),
                latitude = ParseDouble(data, cdr),
                longitude = ParseDouble(data, cdr),
                fix_quality = ParseByte(data, cdr),
                sats_online = ParseUInt16(data, cdr),
                hdop = ParseSingle(data, cdr),
                altitude = ParseDouble(data, cdr),
                height_of_geoid = ParseDouble(data, cdr),
                time_from_last_update = ParseSingle(data, cdr),
            };
        
        private static GSensorAcc ParseGSensorAcc(Stream data, bool cdr)
            => new GSensorAcc()
            {
                Header = ParseHeader(data, cdr),
                valid = ParseBoolean(data, cdr),
                lat_accel = ParseDouble(data, cdr),
                long_accel = ParseDouble(data, cdr),
                yaw_rate = ParseDouble(data, cdr),
                speed = ParseDouble(data, cdr),
            };
        
        private static WheelAxleVelocity ParseWheelAxleVelocity(Stream data, bool cdr)
            => new WheelAxleVelocity()
            {
                Header = ParseHeader(data, cdr),
                axle = ParseByte(data, cdr),
                left_wheel = ParseDouble(data, cdr),
                right_wheel = ParseDouble(data, cdr),
            };

        private static Marker ParseMarker(Stream data, bool cdr) =>
                new Marker
                {
                    Header = ParseHeader(data, cdr),
                    Ns = ParseString(data, cdr),
                    Id = ParseInt32(data, cdr),
                    Form = (Marker.MarkerForm) ParseUInt32(data, cdr),
                    Action = (Marker.MarkerAction) ParseUInt32(data, cdr),
                    Pose = ParsePose(data, cdr),
                    Scale = ParseVector3(data, cdr),
                    Color = ParseColorRGBA(data, cdr),
                    Lifetime = ParseTime(data, cdr),
                    FrameLocked = ParseBoolean(data, cdr),
                    Points = ParseArray(data, ParsePoint, cdr),
                    Colors = ParseArray(data, ParseColorRGBA, cdr),
                    Text = ParseString(data, cdr),
                    MeshResource = ParseString(data, cdr),
                    MeshUseEmbeddedMaterials = ParseBoolean(data, cdr),
                };
        
        private static byte[] ParseByteArray(Stream data, bool cdr) => ReadBytes(ParseInt32(data, cdr), data, false);

        private static T[] ParseArray<T>(Stream data, Func<Stream, bool, T> parser, bool cdr)
        {
            var amount = ParseInt32(data, cdr);
            var objs = new T[amount];
            for (int i = 0; i < amount; i++)
            {
                objs[i] = parser(data, cdr);
            }

            return objs;
        }

        private static T[] ParseArray<T>(Stream data, int count, Func<Stream, bool, T> parser, bool cdr)
        {
            var objs = new T[count];
            for (int i = 0; i < count; i++)
            {
                objs[i] = parser(data, cdr);
            }

            return objs;
        }

        private static string ParseString(Stream data, bool cdr)
        {
            return Encoding.UTF8.GetString(ReadBytes(ParseInt32(data, cdr), data, false)).TrimEnd('\0');
        }

        private static Header ParseHeader(Stream data, bool cdr)
        {
            return new(0, ParseTime(data, cdr), ParseString(data, cdr));
        }

        private static short ParseInt16(Stream data, bool cdr) => BitConverter.ToInt16(ReadBytes(2, data, cdr), 0);

        private static ushort ParseUInt16(Stream data, bool cdr) => BitConverter.ToUInt16(ReadBytes(2, data, cdr), 0);

        private static int ParseInt32(Stream data, bool cdr) => BitConverter.ToInt32(ReadBytes(4, data, cdr), 0);

        private static uint ParseUInt32(Stream data, bool cdr) => BitConverter.ToUInt32(ReadBytes(4, data, cdr), 0);

        private static long ParseInt64(Stream data, bool cdr) => BitConverter.ToInt64(ReadBytes(8, data, cdr), 0);

        private static ulong ParseUInt64(Stream data, bool cdr) => BitConverter.ToUInt64(ReadBytes(8, data, cdr), 0);

        private static float ParseSingle(Stream data, bool cdr) => BitConverter.ToSingle(ReadBytes(4, data, cdr), 0);

        private static double ParseDouble(Stream data, bool cdr) => BitConverter.ToDouble(ReadBytes(8, data, cdr), 0);

        // ReSharper disable once UnusedParameter.Local
        private static bool ParseBoolean(Stream data, bool cdr) => data.ReadByte() != 0;

        // ReSharper disable once UnusedParameter.Local
        private static sbyte ParseSByte(Stream data, bool cdr) => (sbyte) data.ReadByte();

        // ReSharper disable once UnusedParameter.Local
        private static byte ParseByte(Stream data, bool cdr) => (byte) data.ReadByte();

        private static byte[] ReadBytes(int count, Stream data, bool cdr)
        {
            if (cdr) Align(data, count);
            var buffer = new byte[count];
            data.Read(buffer, 0, count);
            return buffer;
        }

        private static void Align(Stream data, int len)
        {
            if (len != 8)
            {
                while ((data.Position % len) != 0 && data.Position < data.Length)
                {
                    data.ReadByte();
                }
            }
            else
            {
                while ((data.Position % len) != 4 && data.Position < data.Length)
                {
                    data.ReadByte();
                }
            }
        }
    }
}