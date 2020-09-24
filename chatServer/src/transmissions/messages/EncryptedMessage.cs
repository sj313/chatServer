using ChatServer.Encryption;
using Google.Protobuf;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatServer.Transmissions
{
    public sealed partial class EncryptedMessage
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long ID { get; set; }

        public byte[] _message
        {
            get { return this.Message.ToByteArray(); }
            set { this.Message = ByteString.CopyFrom(value); }
        }
        
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