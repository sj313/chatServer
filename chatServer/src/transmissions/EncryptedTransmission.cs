using System;
using ChatServer.Encryption;
using Google.Protobuf;

namespace ChatServer.Transmissions {

    public sealed partial class EncryptedTransmission
    {
        public EncryptedTransmission(Transmission transmission, byte[] password)
        {
            EncryptedTransmission_ = ByteString.CopyFrom(AES.Encrypt(transmission.ToByteArray(), password));
        }

        public Transmission Decrypt(byte[] password)
        {
            var decryptedBytes =  AES.Decrypt(EncryptedTransmission_.ToByteArray(), password);
            var parser = new MessageParser<Transmission>(() => new Transmission());
            return parser.ParseFrom(decryptedBytes);
        }
    }

}