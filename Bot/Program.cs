using System.Threading;

namespace Bot {
    class Program {
        const string LIC = @"eJxkkd1ugkAQRl/FeGvqglqqzbqpohIsoIKUn7sVVtgCC4XFIk9fo61e9O6b
c2YykwzUaEBYRTpNlrJq2sXRU5Uf+TcuyWt6U10Et2Ue1gFXQ6RjXtIGggeB
uxozTvkZiRDcM5TriucZKRE0cEbQ+lJS1tFx22IIrgjKeVZgdkam7W06Kgv6
EPwhuMwwTdHndeqNkwZXcZ6GJOsHeQbBzV7a70vsIsScLJuClmRxSWggiJIw
EocQ/FPQohHDvC4J0pUk9HWw/VB6vusNxlSqXV/jLJXaEpwMYavHss1m2iie
2IqnJI5GT5tac4MoMiLLOFDMHMGiibZI5KCJ+fvsoMR0HstsPrbjZ2ecHFnl
6nvVnbSONYhwwunEP7+sic8D33EtYra6KcZDM49yTyrmu3rlg5nhWV/GypMN
QRztjz083DrqFILH3RD8/g39CCA=";



        static void Main(string[] args) {
            System.Console.SetWindowSize(111, 50);
            Matrix.License.LicenseManager.SetLicense(LIC);

            System.Console.Write("Username: ");
            var username = System.Console.ReadLine();
            System.Console.Write("Password: ");
            var password = System.Console.ReadLine();

            BotManager.Instance.SetUsername(username);
            XmppManager.Instance.SetMessageHandler(BotManager.Instance);
            XmppManager.Instance.Login(username, HttpHelper.Instance.GetAccessToken(username, password));

            while (true) {
                Thread.Sleep(500);
            }
        }
    }
}
