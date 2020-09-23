using Google.Protobuf;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatServer.Transmissions
{
    public sealed partial class ServerMessage
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long ID { get; set; }

        public byte[] _message
        {
            get { return this.Message.ToByteArray(); }
            set { this.Message = ByteString.CopyFrom(value); }
        }

        public ServerMessage(byte[] message)
        {
            Message = ByteString.CopyFrom(message);
        }

    }

}