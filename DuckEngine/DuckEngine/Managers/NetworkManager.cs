using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;

namespace DuckEngine.Network
{
    /// <summary>
    /// Class responsible for network communications.
    /// </summary>
    public class NetworkManager
    {
        NetworkSession networkSession;
        public NetworkSession Session { get { return networkSession; } }

        AvailableNetworkSessionCollection gameList;

        bool server = false;
        public bool IsCreated { get { return server && (networkSession != null); } }
        public bool IsJoined { get { return !server && (networkSession != null); } }

        public NetworkManager()
        {
        }

        void Update(GameTime gameTime)
        {
            networkSession.Update();
        }

        #region Server
        public void BeginCreateGame()
        {
            NetworkSession.BeginCreate(NetworkSessionType.Ranked, 4, 30, new AsyncCallback(CreateGame), null);
        }

        void CreateGame(IAsyncResult result)
        {
            networkSession = NetworkSession.EndCreate(result);
            server = true;
        }
        #endregion

        #region Client
        public void BeginFindGames(NetworkSessionProperties properties)
        {
            NetworkSession.BeginFind(NetworkSessionType.Ranked, 4, properties, new AsyncCallback(FindGames), null);
        }

        void FindGames(IAsyncResult result)
        {
            gameList = NetworkSession.EndFind(result);
        }

        AvailableNetworkSessionCollection GetGames()
        {
            return gameList;
        }

        public void BeginJoinGame(AvailableNetworkSession session)
        {
            NetworkSession.BeginJoin(session, new AsyncCallback(JoinGame), null);
        }

        void JoinGame(IAsyncResult result)
        {
            networkSession = NetworkSession.EndJoin(result);
            server = false;
        }
        #endregion
    }
}
