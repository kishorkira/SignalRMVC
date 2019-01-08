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
        static List<Message> _messages { get; set; } = new List<Message>();

        public void Hello()
        {
            Clients.All.hello();
        }
        public void Send(string from, string message, string datetime)
        {
            _messages.Add(new Message { Body = message, From = from, To = "all", DateTimeStr = datetime });
            Clients.All.broadcastMessage(from, message, datetime);
        }

        public void Send(string from,string to ,string message, string datetime)
        {
            _messages.Add(new Message {Body=message,From=from,To=to, DateTimeStr = datetime });
           var connectionIds =ConnectionMapping<string>.GetConnections(to).ToList();
             connectionIds.AddRange(ConnectionMapping<string>.GetConnections(from));

            foreach (var connectionId in connectionIds)
            {
                Clients.Client(connectionId).newMessage(from, message, datetime);
            }

        }

        public override Task OnConnected()
        {

            string name = Context.QueryString["username"];
            ConnectionMapping<string>.Add(name, Context.ConnectionId);
            Clients.All.userJoinOrLeave(ConnectionMapping<string>.GetConnections());
            return base.OnConnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {

            string name = Context.QueryString["username"];
            ConnectionMapping<string>.Remove(name, Context.ConnectionId);
            Clients.All.userJoinOrLeave(ConnectionMapping<string>.GetConnections());
            return base.OnDisconnected(stopCalled);
        }
        public override Task OnReconnected()
        {
            string name = Context.QueryString["username"];

            if (!ConnectionMapping<string>.GetConnections(name).Contains(Context.ConnectionId))
            {
                ConnectionMapping<string>.Add(name, Context.ConnectionId);
            }
            Clients.All.userJoinOrLeave(ConnectionMapping<string>.GetConnections());

            return base.OnReconnected();
        }

        public async Task JoinGroup(string userName, string groupName)
        {
            var connectionId = Context.ConnectionId;
            await Groups.Add(Context.ConnectionId, groupName);
            Clients.Group(groupName).newMemberJoinedGroupMessage($"{userName} joined the group");

        }

        public Task LeaveGroup(string userName, string groupName)
        {
            var connectionId = Context.ConnectionId;
            Clients.Group(groupName).newMemberLeftGroupMessage($"{userName} Left the group");
            return Groups.Remove(Context.ConnectionId, groupName);

        }

        public void GetMessages(string from, string to)
        {
            var messages = new List<Message>();
            if (to.ToLower() == "all")
            {
                messages = _messages.Where(m => m.To == "all").ToList();
            }
            else
            {
                messages = _messages.Where(m => m.From == from && m.To == to || m.From==to && m.To == from).ToList();
            }
            Clients.Client(Context.ConnectionId).updateMessages(messages);
        }
    }
}