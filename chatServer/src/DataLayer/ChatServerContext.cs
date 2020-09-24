using System.Data.Entity;

namespace ChatServer.DataLayer
{
    //[DbConfigurationType(typeof(MySqlEFConfiguration))]

    public class ChatServerContext : DbContext
    {
        const string SERVER_NAME = "<SERVER>";

        public ChatServerContext() : base(@"Server=localhost;Database=ChatServer;Trusted_Connection=True;")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transmissions.EncryptedMessage>().Ignore(em => em.Message);
            modelBuilder.Entity<Transmissions.EncryptedMessage>().Ignore(em => em.UserID);

            modelBuilder.Entity<Transmissions.ServerMessage>().Ignore(sm => sm.Message);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Chat> Chats { get; set; }

        public DbSet<Transmissions.Message> Messages { get; set; }
        public DbSet<Transmissions.EncryptedMessage> EncryptedMessages { get; set; }
        public DbSet<Transmissions.ServerMessage> ServerMessages { get; set; }

        public User GetUserForEncryptedMessage(Transmissions.EncryptedMessage encryptedMessage)
        {
            return this.Users.Find(encryptedMessage._user_id);
        }

        public string GetSenderNameForMessage(Transmissions.Message message)
        {
            return message.HasEncyptedMessage() ? this.GetUserForEncryptedMessage(message.EncryptedMessage).Name : SERVER_NAME;
        }
    }
}
