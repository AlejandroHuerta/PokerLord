#define DEVELOPMENT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot {
    class Config {
#if DEVELOPMENT
        public static string DOMAIN = "https://devplay.texasholdem.com/";
        public static string LOGIN_URI = "https://devlogin.texasholdem.com/oauth/token";
        public static string XMPP_DOMAIN = "players.texasholdem.com";
        public static string XMPP_GAME_DOMAIN = "game@players.texasholdem.com";
#else
        public static string DOMAIN = "https://play.texasholdem.com/";
        public static string LOGIN_URI = "https://login.texasholdem.com/oauth/token";
        public static string XMPP_DOMAIN = "players.beta.texasholdem.com";
        public static string XMPP_GAME_DOMAIN = "gameserver3@backend.beta.texasholdem.com";
#endif
    }
}
