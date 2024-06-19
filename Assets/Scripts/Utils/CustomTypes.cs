using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.PhotonUnityNetworking.Code.Common.Pool;

namespace Utils
{
    public static class CustomTypes
    {
        private const byte PoolObjectDataVoCode = 0;

        public static void Register()
        {
            PhotonPeer.RegisterType(typeof(PoolObjectDataVo), PoolObjectDataVoCode, SerializePoolObjectDataVo,
                DeserializePoolObjectDataVo);
        }

        private static readonly byte[]
            MemPoolObjectDataVo = new byte[4 + 256];

        private static short SerializePoolObjectDataVo(StreamBuffer outStream, object customObject)
        {
            var vo = (PoolObjectDataVo)customObject;
            var index = 0;
            lock (MemPoolObjectDataVo)
            {
                var bytes = MemPoolObjectDataVo;

                SerializeString(vo.Key, bytes, ref index);

                SerializeBool(vo.Ifs, bytes, ref index);

                outStream.Write(bytes, 0, index);
            }

            return (short)index;
        }

        private static object DeserializePoolObjectDataVo(StreamBuffer inStream, short length)
        {
            var vo = new PoolObjectDataVo();
            var index = 0;
            lock (MemPoolObjectDataVo)
            {
                inStream.Read(MemPoolObjectDataVo, 0, length);

                vo.Key = DeserializeString(MemPoolObjectDataVo, ref index);

                vo.Ifs = DeserializeBool(MemPoolObjectDataVo, ref index);
            }

            return vo;
        }

        private static void SerializeString(string value, byte[] target, ref int targetOffset)
        {
            var stringBytes = System.Text.Encoding.UTF8.GetBytes(value);
            var stringLength = stringBytes.Length;
            Protocol.Serialize(stringLength, target, ref targetOffset);
            System.Buffer.BlockCopy(stringBytes, 0, target, targetOffset, stringLength);
            targetOffset += stringLength;
        }

        private static string DeserializeString(byte[] source, ref int offset)
        {
            Protocol.Deserialize(out int stringLength, source, ref offset);
            var value = System.Text.Encoding.UTF8.GetString(source, offset, stringLength);
            offset += stringLength;
            return value;
        }

        private static void SerializeBool(bool value, IList<byte> target, ref int targetOffset) => 
            target[targetOffset++] = (byte)(value ? 1 : 0);

        private static bool DeserializeBool(IReadOnlyList<byte> source, ref int offset) => 
            source[offset++] == 1;
    }
}