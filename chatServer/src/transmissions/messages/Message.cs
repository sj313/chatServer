using System;

namespace ChatServer.Transmissions
{
  public sealed partial class Message
  {
      public Message(Int64 chatID, EncryptedMessage encryptedMessage)
      {
          ChatID = chatID;
          EncryptedMessage = encryptedMessage;
      }

      public Message(Int64 chatID, ServerMessage serverMessage)
      {
          ChatID = chatID;
          ServerMessage = serverMessage;
      }

  }

}