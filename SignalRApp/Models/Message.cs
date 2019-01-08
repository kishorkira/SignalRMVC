using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRApp.Models
{
    public class Message
    {        
        public string Body { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string DateTimeStr { get; set; }
    }
}