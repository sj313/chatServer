using ChatServer.Encryption;
using Google.Protobuf;

namespace ChatServer.Transmissions
{
  public sealed partial class EncryptedMessage
  {
      public EncryptedMessage(byte[] message, byte[] key)
      {
          Message = ByteString.CopyFrom(AES.Encrypt(message, key));
      }

      public byte[] Decrypt(byte[] key)
      {
          return AES.Decrypt(Message.ToByteArray(), key);
      }
  }

}