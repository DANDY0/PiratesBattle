using ScriptsPhotonCommon.Pool;
using ExitGames.Client.Photon;

namespace Utils
{
    public static class CustomTypes
    {
        public static readonly byte PoolObjectDataVoCode = 0; // Код типа, должен быть уникальным

        public static void Register()
        {
            PhotonPeer.RegisterType(typeof(PoolObjectDataVo), PoolObjectDataVoCode, SerializePoolObjectDataVo,
                DeserializePoolObjectDataVo);
        }

        private static readonly byte[]
            memPoolObjectDataVo = new byte[4 + 256]; // Убедитесь, что размер буфера достаточен для хранения строки

        private static short SerializePoolObjectDataVo(StreamBuffer outStream, object customObject)
        {
            PoolObjectDataVo vo = (PoolObjectDataVo)customObject;
            int index = 0; // Переместили объявление переменной index сюда
            lock (memPoolObjectDataVo)
            {
                byte[] bytes = memPoolObjectDataVo;

                // Сериализация строки
                SerializeString(vo.Key, bytes, ref index);

                // Сериализация булевого значения
                SerializeBool(vo.Ifs, bytes, ref index);

                outStream.Write(bytes, 0, index);
            }

            return (short)index;
        }

        private static object DeserializePoolObjectDataVo(StreamBuffer inStream, short length)
        {
            PoolObjectDataVo vo = new PoolObjectDataVo();
            int index = 0; // Переместили объявление переменной index сюда
            lock (memPoolObjectDataVo)
            {
                inStream.Read(memPoolObjectDataVo, 0, length);

                // Десериализация строки
                vo.Key = DeserializeString(memPoolObjectDataVo, ref index);

                // Десериализация булевого значения
                vo.Ifs = DeserializeBool(memPoolObjectDataVo, ref index);
            }

            return vo;
        }

        private static void SerializeString(string value, byte[] target, ref int targetOffset)
        {
            byte[] stringBytes = System.Text.Encoding.UTF8.GetBytes(value);
            int stringLength = stringBytes.Length;
            Protocol.Serialize(stringLength, target, ref targetOffset);
            System.Buffer.BlockCopy(stringBytes, 0, target, targetOffset, stringLength);
            targetOffset += stringLength;
        }

        private static string DeserializeString(byte[] source, ref int offset)
        {
            int stringLength;
            Protocol.Deserialize(out stringLength, source, ref offset);
            string value = System.Text.Encoding.UTF8.GetString(source, offset, stringLength);
            offset += stringLength;
            return value;
        }

        private static void SerializeBool(bool value, byte[] target, ref int targetOffset)
        {
            target[targetOffset++] = (byte)(value ? 1 : 0);
        }

        private static bool DeserializeBool(byte[] source, ref int offset)
        {
            return source[offset++] == 1;
        }
    }
}