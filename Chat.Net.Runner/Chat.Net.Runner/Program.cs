﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Chat.Net.Server;

namespace Chat.Net.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Server!");
            var server = new BaseServer();

            Console.Read();
        }
    }
}
