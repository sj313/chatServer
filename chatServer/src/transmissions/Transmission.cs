using System;
using ChatServer.Encryption;
using Google.Protobuf;

namespace ChatServer.Transmisisons {

    public sealed partial class Transmission
    {
        public Transmission(ReadOnlySpan<byte> privKey, byte[] data)
        {
            Type = this.GetType().FullName;
            Data = ByteString.CopyFrom(RSA.Sign(privKey, data));
            OnConstruction();
        }

        public void AppendToDataFront(byte[] dataToAppend)
        {
            var appendSize = dataToAppend.Length - 1;
            var data = Data.ToByteArray();
            var appendedData = new byte[data.Length + appendSize];
            dataToAppend.CopyTo(appendedData, 0);
            data.CopyTo(appendedData, appendSize);
            Data = ByteString.CopyFrom(appendedData);
        }

        public byte[] GetData()
        {
            return RSA.RemoveSignature(Data.ToByteArray());
        }

        public bool Verify(ReadOnlySpan<byte> pubKey)
        {
            return RSA.Verify(pubKey, Data.ToByteArray());
        }

    }

}