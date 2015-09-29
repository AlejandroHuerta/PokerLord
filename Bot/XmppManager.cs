#define DEVELOPMENT

using Bot.Messages;
using LogParser;
using Matrix.Xmpp;
using Matrix.Xmpp.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot {
    class XmppManager {
#if DEVELOPMENT
        string domain = "players.texasholdem.com";
        string gameDomain = "game@players.texasholdem.com";
#else
        string domain = "players.beta.texasholdem.com";
        string gameDomain = "gameserver3@backend.beta.texasholdem.com";
#endif

        public interface MessageHandler {
            void OpenTable(OpenTableMessage message);
            void TableState(TableStateMessage message);
            void BeginGame(BeginGame message);
            void SetPlayers(SetPlayers message);
            void SetActivePlayer(SetActivePlayer message);
            void SetPlayerAction(SetPlayerAction message);
            void Eliminated(EliminatedMessage message);
            void DealCommunityCards(DealCommunityCards message);
            void DealHoleCards(DealHoleCards message);
            void CloseTable(CloseTable message);
        }

        static private XmppManager instance;

        static public XmppManager Instance {
            get {
                if (instance == null) {
                    instance = new XmppManager();
                }
                return instance;
            }//get
        }//Instance

        XmppClient xmppClient;
        MessageHandler messageHandler;

        private XmppManager() {
            xmppClient = new XmppClient();

            xmppClient.OnLogin += OnLogin;
            xmppClient.OnMessage += OnMessage;

            xmppClient.Resource = Guid.NewGuid().ToString();
            xmppClient.SetXmppDomain(domain);
        }

        public void SetMessageHandler(MessageHandler handler) {
            messageHandler = handler;
        }

        public void Login(string username, string password) {
            xmppClient.SetUsername(username);
            xmppClient.Password = password;

            xmppClient.Open();
        }

        void OnLogin(object sender, Matrix.EventArgs e) {
            Console.WriteLine("Login Successful as " + xmppClient.Username);
        }

        void OnMessage(object sender, MessageEventArgs e) {
            var simpleAction = GrabAction(e.Message.Body);

            switch (simpleAction) {
            case "OPEN_TABLE":
                OpenTableMessage openTable = JsonConvert.DeserializeObject<OpenTableMessage>(e.Message.Body);
                messageHandler.OpenTable(openTable);          
                break;
            case "TABLE_STATE":
                TableStateMessage stateMessage = JsonConvert.DeserializeObject<TableStateMessage>(e.Message.Body);
                messageHandler.TableState(stateMessage);
                break;
            case "ELIMINATED":
                EliminatedMessage eliminated = JsonConvert.DeserializeObject<EliminatedMessage>(e.Message.Body);
                messageHandler.Eliminated(eliminated);
                break;
            case "CLOSE_TABLE":
                CloseTable closeTable = JsonConvert.DeserializeObject<CloseTable>(e.Message.Body);
                messageHandler.CloseTable(closeTable);
                break;
            case "BEGIN_GAME":
                BeginGame beginGame = JsonConvert.DeserializeObject<BeginGame>(e.Message.Body);
                messageHandler.BeginGame(beginGame);
                break;
            case "DEAL_COMMUNITY_CARDS":
                DealCommunityCards dealCommunityCards = JsonConvert.DeserializeObject<DealCommunityCards>(e.Message.Body);
                messageHandler.DealCommunityCards(dealCommunityCards);
                break;
            case "SET_ACTIVE_PLAYER":
                SetActivePlayer setActivePlayer = JsonConvert.DeserializeObject<SetActivePlayer>(e.Message.Body);
                messageHandler.SetActivePlayer(setActivePlayer);
                break;
            case "SET_PLAYER_ACTION":
                SetPlayerAction setPlayerAction = JsonConvert.DeserializeObject<SetPlayerAction>(e.Message.Body);
                messageHandler.SetPlayerAction(setPlayerAction);
                break;
            case "SET_PLAYERS":
                SetPlayers setPlayers = JsonConvert.DeserializeObject<SetPlayers>(e.Message.Body);
                messageHandler.SetPlayers(setPlayers);
                break;
            case "CHAT":
                Console.WriteLine(GrabBody(e.Message.Body));
                break;
            case "DEAL_HOLE_CARDS":
                DealHoleCards dealHoleCards = JsonConvert.DeserializeObject<DealHoleCards>(e.Message.Body);
                messageHandler.DealHoleCards(dealHoleCards);
                break;
            default:
                break;
            }//switch
        }//OnMessage

        string GrabBody(string message) {
            JObject xmppBody = JObject.Parse(message);
            return xmppBody["message"]["body"].ToString();
        }

        string GrabAction(string message) {
            JObject xmppMessage = JObject.Parse(message);
            return xmppMessage["message"]["action"].ToString();
        }

        public void SendAction(Player.Action action, string tableId, int amount = 0) {
            ClientMessage clientMessage = new ClientMessage();
            clientMessage.messageType = "GAME";
            clientMessage.tableId = tableId;

            switch (action) {
            case Player.Action.Check:
                clientMessage.message.action = "CHECK";
                break;
            case Player.Action.Fold:
                clientMessage.message.action = "FOLD";
                break;
            case Player.Action.Call:
                clientMessage.message.action = "CALL";
                break;
            case Player.Action.Raise:
                clientMessage.message.action = "RAISE";
                clientMessage.message.amount = amount;
                break;
            case Player.Action.Bet:
                clientMessage.message.action = "BET";
                clientMessage.message.amount = amount;
                break;
            case Player.Action.AllIn:
                clientMessage.message.action = "BET";
                clientMessage.message.amount = amount;
                break;
            default:
                break;
            }//switch

            SendMessage(clientMessage);
        }//sendCommand

        public void SendUnsubscribe(string tableId) {
            ClientMessage clientMessage = new ClientMessage();
            clientMessage.messageType = "GAME";
            clientMessage.tableId = tableId;
            clientMessage.message.action = "UNSUBSCRIBE";

            SendMessage(clientMessage);
        }//SendUnsubscribe

        void SendMessage(ClientMessage clientMessage) {
            Message message = new Message();
            message.From = xmppClient.Username + "@" + domain;
            message.To = gameDomain;
            message.Body = JsonConvert.SerializeObject(clientMessage);

            xmppClient.Send(message);
        }//SendMessage

        public void RequestState(string tableId) {
            var stateRequest = new StateRequest();
            stateRequest.messageType = "STATE";
            stateRequest.tableId = tableId;

            Message message = new Message();
            message.From = xmppClient.Username + "@" + domain;
            message.To = gameDomain;
            message.Body = JsonConvert.SerializeObject(stateRequest);

            xmppClient.Send(message);
        }
    }
}
