using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Google.Protobuf.WellKnownTypes;
using Google.Protobuf;

namespace ChatServer.Transmissions
{
    public sealed partial class Message
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long ID { get; set; }

        public Guid ChatID {get {return new Guid(ChatIDBytes.ToByteArray());}}

        public DateTime _createdAt
        {
            get { return this.CreatedAt.ToDateTime(); }
            set { this.CreatedAt = Timestamp.FromDateTime(value); }
        }
        public Message(Guid chatID, EncryptedMessage encryptedMessage)
        {
            ChatIDBytes = ByteString.CopyFrom(chatID.ToByteArray());
            EncryptedMessage = encryptedMessage;
            OnConstruction();
        }

        public Message(Guid chatID, ServerMessage serverMessage)
        {
            ChatIDBytes = ByteString.CopyFrom(chatID.ToByteArray());
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

        internal Errors.Error Validate()
        {
            if (EncryptedMessage == null && ServerMessage == null) return Errors.Error.NoContent;
            return Errors.Error.NoError;
        }
    }

}