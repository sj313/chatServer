using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace ChatServer.Server
{
    abstract public class ServerConnections
    {
        // Dictionary of all connections and their IDs
        public static readonly ConcurrentDictionary<Guid, Connection> CONNECTIONS = new ConcurrentDictionary<Guid, Connection>();
        // Dictionary of all connections for a user and their IDs
        public static readonly ConcurrentDictionary<byte[], ConcurrentDictionary<Guid, Connection>> USER_CONNECTIONS = new ConcurrentDictionary<byte[], ConcurrentDictionary<Guid, Connection>>(new ByteArrayComparer());
        // Using ConcurrentDictionary<T, bool> as ConcurrentHashset<T>
        // Dictionary of all chats each user is in
        public static readonly ConcurrentDictionary<byte[], ConcurrentDictionary<long, bool>> USER_CHATS = new ConcurrentDictionary<byte[], ConcurrentDictionary<long, bool>>(new ByteArrayComparer());
        //Dictionary of all users each chat has
        public static readonly ConcurrentDictionary<long, ConcurrentDictionary<byte[], bool>> CHAT_USERS = new ConcurrentDictionary<long, ConcurrentDictionary<byte[], bool>>();

        // Add Connection to appropriate collections
        public static void Add(Connection connection)
        {
            var connectionID = connection.ConnectionID;

            // Add connection to list of all connections
            CONNECTIONS.TryAdd(connectionID, connection);

            // If there is no user associated with the connection it should not be added to any other collections
            if (connection.User != null)
            {
                var userID = connection.User.ID;

                // Check whether this is the first connection for the user
                if (USER_CONNECTIONS.ContainsKey(userID))
                {
                    // If this is not the first connection for the user add connection to existing dictionary of connections in USER_CONNECTIONS
                    USER_CONNECTIONS[userID].TryAdd(connectionID, connection);
                }
                else
                {
                    // If this is the first connection for a user create a dictionary of connections with this connection and add to USER_CONNECTIONS
                    USER_CONNECTIONS.TryAdd(userID, new ConcurrentDictionary<Guid, Connection>(new KeyValuePair<Guid, Connection>[] {new KeyValuePair<Guid, Connection>(connectionID, connection)}));
                }
            }
        }

        // Remove connection from all collections
        public static void Remove(Connection connection)
        {
            var userID = connection.User.ID;
            var connectionID = connection.ConnectionID;
            Connection tempConnection;
            ConcurrentDictionary<Guid, Connection> tempUser;

            // Remove the connection from the list of all connections
            CONNECTIONS.TryRemove(connectionID, out tempConnection);

            // If there is no user associated with the connection it should not be in any other collections
            if (userID != null)
            {
                // Remove the connection from the list of connections for this user in USER_CONNECTIONS
                USER_CONNECTIONS[userID].TryRemove(connectionID, out tempConnection);

                // Check to see if this was the last connection for this user
                if (USER_CONNECTIONS[userID].IsEmpty)
                {
                    // If this was the last connection for this user remove the user from USER_CONNECTIONS
                    USER_CONNECTIONS.TryRemove(connection.User.ID, out tempUser);

                    // Send diconnect message
                    ServerTransmissionHandler.Send(connection.User.ID, $"------------------User: {connection.User.Name} disconnected ------------------");

                    // Log disconnect in console
                    Console.WriteLine($"{connection.User.Name} (User) Disconnected");
                }
            }
        }

        // Add user to CHAT_USERS and add chat to USER_CHATS
        public static void AddToChat(byte[] userID, long chatID)
        {
            // Check whether the user is already in any chats
            if (USER_CHATS.ContainsKey(userID))
            {
                // If the user is already in a chat add this chat to the list of chats for the user
                USER_CHATS[userID].TryAdd(chatID, true);
            }
            else
            {
                // If the user is not in any chats create a new dictionary of chats for the user with the new chat and add this to USER_CHATS
                USER_CHATS.TryAdd(userID, new ConcurrentDictionary<long, bool>(new KeyValuePair<long, bool>[] {new KeyValuePair<long, bool>(chatID, true)}));
            }
            
            // Check whether the chat already has any users
            if (ServerConnections.CHAT_USERS.ContainsKey(chatID))
            {
                // If the chat already had users add this user to the list of users
                CHAT_USERS[chatID].TryAdd(userID, true);
            }
            else
            {
                // If the chat has no uses create a new dictionary of users for the chat with the new user and add this to CHAT_USERS
                CHAT_USERS.TryAdd(chatID, new ConcurrentDictionary<byte[], bool>(new KeyValuePair<byte[], bool>[] {new KeyValuePair<byte[], bool>(userID, true)}, new ByteArrayComparer()));
            }
        }
    }

    // EqualityComparer to use byte arrays as dictionary and hashset keys
    class ByteArrayComparer : EqualityComparer<byte[]>
    {
        public override bool Equals(byte[] first, byte[] second)
        {
            if (first == null || second == null) {
                return first == second;
            }
            if (ReferenceEquals(first, second)) {
                return true;
            }
            if (first.Length != second.Length) {
                return false;
            }
            return first.SequenceEqual(second);
        }
        
        public override int GetHashCode(byte[] obj)
        {
            if (obj == null) {
                throw new ArgumentNullException("obj");
            }
            if (obj.Length < 4) return Convert.ToInt32(obj[0]);
            var hashBytes = new byte[] {
                obj[0],
                obj[1],
                obj[obj.Length-1],
                obj[obj.Length-2]
            };
            return BitConverter.ToInt32(hashBytes);
        }
    }
}