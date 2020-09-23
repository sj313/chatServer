using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Google.Protobuf.WellKnownTypes;

namespace ChatServer.Transmissions
{
    public sealed partial class Message
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long ID { get; set; }

        public DateTime _createdAt
        {
            get { return this.CreatedAt.ToDateTime(); }
            set { this.CreatedAt = Timestamp.FromDateTime(value); }
        }
        public Message(Int64 chatID, EncryptedMessage encryptedMessage)
        {
            ChatID = chatID;
            EncryptedMessage = encryptedMessage;
            OnConstruction();
        }

        public Message(Int64 chatID, ServerMessage serverMessage)
        {
            ChatID = chatID;
            ServerMessage = serverMessage;
            OnConstruction();
        }

        partial void OnConstruction()
        {
            this._createdAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
        }

        public byte[] GetUnderlyingMessageContent()
        {
            return this.HasEncyptedMessage() ? this.EncryptedMessage._message : this.ServerMessage._message;
        }

        public bool HasServerMessage()
        {
            return this.ServerMessage != null;
        }

        public bool HasEncyptedMessage()
        {
            return this.EncryptedMessage != null;
        }

        public System.Type GetUnderlyingMessageType()
        {
            return this.HasEncyptedMessage() ? this.EncryptedMessage.GetType() : this.ServerMessage.GetType();
        }

    }

}