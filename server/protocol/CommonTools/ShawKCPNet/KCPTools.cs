using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ShawnFramework.ShawLog;

namespace ShawnFramework.ShawKCPNet
{
    public class KCPTools
    {
        public static byte[] Serialize<T>(T msg) where T : KCPMsg
        {
            using (MemoryStream ms = new MemoryStream())
            {
                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(ms, msg);
                    ms.Seek(0, SeekOrigin.Begin);
                    return ms.ToArray();
                }
                catch (SerializationException e)
                {
                    LogCore.Error($"Client Udp Recive Data Exception:{e.Message}");
                    throw;
                }
            }
        }
        public static T DeSerialize<T>(byte[] bytes) where T : KCPMsg
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    T msg = (T)bf.Deserialize(ms);
                    return msg;
                }
                catch (SerializationException e)
                {
                    LogCore.Error($"Failed to Deserialize.Reason:{e.Message} bytesLen:{bytes.Length}");
                    throw;
                }
            }
        }

        public static byte[] Compress(byte[] input)
        {
            using (MemoryStream outMS = new MemoryStream())
            {
                using (GZipStream gzs = new GZipStream(outMS, CompressionMode.Compress, true))
                {
                    gzs.Write(input, 0, input.Length);
                    gzs.Close();
                    return outMS.ToArray();
                }
            }
        }
        public static byte[] DeCompress(byte[] input)
        {
            using (MemoryStream inputMS = new MemoryStream(input))
            {
                using (MemoryStream outMs = new MemoryStream())
                {
                    using (GZipStream gzs = new GZipStream(inputMS, CompressionMode.Decompress))
                    {
                        byte[] bytes = new byte[1024];
                        int len = 0;
                        while ((len = gzs.Read(bytes, 0, bytes.Length)) > 0)
                        {
                            outMs.Write(bytes, 0, len);
                        }
                        gzs.Close();
                        return outMs.ToArray();
                    }
                }
            }
        }

        static readonly DateTime utcStart = new DateTime(1970, 1, 1);
        public static ulong GetUTCStartMilliseconds()
        {
            TimeSpan ts = DateTime.UtcNow - utcStart;
            return (ulong)ts.TotalMilliseconds;
        }
    }
}
