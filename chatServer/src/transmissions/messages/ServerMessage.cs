using Google.Protobuf;

namespace ChatServer.Transmissions
{
  public sealed partial class ServerMessage
  {
      public ServerMessage(byte[] message)
      {
          Message = ByteString.CopyFrom(message);
      }

  }

}