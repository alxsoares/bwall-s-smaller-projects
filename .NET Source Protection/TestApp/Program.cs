using System;
using System.Collections.Generic;
using System.Text;
using NET_Source_Protection;

namespace TestApp
{
    class Testing : IStartUp
    {
        static void Main(string[] args)
        {
            new Testing().Start();
        }

        public void Start()
        {
            Console.WriteLine("Hello world!");
        }
    }
}
