using System.Data.Entity;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Security.Cryptography;


namespace ChatServer.DataLayer
{
    public class ChatServerContext : DbContext
    {
        const string SERVER_NAME = "<SERVER>";
        const string CONNECTION_NAME = "ChatServer";
        public static string CONNECTION_STRING = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("./DataLayer/DbConfig.json").Build().GetConnectionString(CONNECTION_NAME);

        public ChatServerContext() : base(CONNECTION_STRING)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transmissions.EncryptedMessage>().Ignore(em => em.Message);
            // modelBuilder.Entity<Transmissions.EncryptedMessage>().Ignore(em => em.UserID);

            modelBuilder.Entity<Transmissions.ServerMessage>().Ignore(sm => sm.Message);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Chat> Chats { get; set; }

        public DbSet<Transmissions.Message> Messages { get; set; }
        public DbSet<Transmissions.EncryptedMessage> EncryptedMessages { get; set; }
        public DbSet<Transmissions.ServerMessage> ServerMessages { get; set; }

        public User GetUserForEncryptedMessage(Transmissions.EncryptedMessage encryptedMessage)
        {
            // Temp change to stop error, definitely will not work as intended
            // return this.Users.Find(encryptedMessage._user_id);
            return new User(null, null);
        }

        public string GetSenderNameForMessage(Transmissions.Message message)
        {
            return message.HasEncyptedMessage() ? this.GetUserForEncryptedMessage(message.EncryptedMessage).Name : SERVER_NAME;
        }
    }
}
