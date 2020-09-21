namespace ChatServer.Transmissions
{
  public sealed partial class Message
  {
      public Message(EncryptedMessage encryptedMessage)
      {
          EncryptedMessage = encryptedMessage;
      }

      public Message(ServerMessage serverMessage)
      {
          ServerMessage = serverMessage;
      }

  }

}