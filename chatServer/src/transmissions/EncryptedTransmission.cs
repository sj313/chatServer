using System;
using ChatServer.Encryption;
using Google.Protobuf;

namespace ChatServer.Transmisisons {

    public sealed partial class EncryptedTransmission
    {
        public EncryptedTransmission(Transmission transmission, byte[] password)
        {
            EncryptedTransmission_ = ByteString.CopyFrom(AES.Encrypt(transmission.ToByteArray(), password));
        }

        public byte[] Decrypt(byte[] password)
        {
            return AES.Decrypt(EncryptedTransmission_.ToByteArray(), password);
        }
    }

}