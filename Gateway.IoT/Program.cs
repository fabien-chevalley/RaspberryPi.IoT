using System;
using Woopsa;

namespace Gateway.IoT
{
    class Program
    {
        static void Main(string[] args)
        {

            var root = new WoopsaObject(null, "Gateway.IoT");
            WoopsaClient client = new WoopsaClient("http://192.168.1.66/woopsa", root);
            client.Username = "admin";
            client.Password = "admin";

            client.CreateBoundRoot("device_garage");

            var woopsaServer = new WoopsaServer(root, 10443);
            woopsaServer.Authenticator = new SimpleAuthenticator("Raspberry", 
                (sender, e) =>  e.IsAuthenticated = e.Username == "admin" && e.Password == "admin");
        }
    }
}
