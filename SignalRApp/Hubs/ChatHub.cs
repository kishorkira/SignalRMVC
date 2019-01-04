using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using SignalRApp.Models;

namespace SignalRApp.Hubs
{
    public class ChatHub : Hub
    {
        

        public void Hello()
        {
            Clients.All.hello();
        }
        public void Send(string name, string message)
        {
            Clients.All.broadcastMessage(name, message);
        }

        public void Send(string from,string to ,string message)
        {
           var connectionIds =ConnectionMapping<string>.GetConnections(to).ToList();
             connectionIds.AddRange(ConnectionMapping<string>.GetConnections(from));

            foreach (var connectionId in connectionIds)
            {
                Clients.Client(connectionId).newMessage(from, message);
            }

        }

        public override Task OnConnected()
        {

            string name = Context.QueryString["username"];
            ConnectionMapping<string>.Add(name, Context.ConnectionId);
            return base.OnConnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {

            string name = Context.QueryString["username"];
            ConnectionMapping<string>.Remove(name, Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }
        public override Task OnReconnected()
        {
            string name = Context.QueryString["username"];

            if (!ConnectionMapping<string>.GetConnections(name).Contains(Context.ConnectionId))
            {
                ConnectionMapping<string>.Add(name, Context.ConnectionId);
            }

            return base.OnReconnected();
        }

    }
}