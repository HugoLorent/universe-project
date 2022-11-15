using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace universe_project
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string pathDirectory = "C:/Universe";

            Console.WriteLine("Hello Universe !");
            Console.WriteLine("------------------");

            Question1 q1 = new Question1();
            Question2 q2 = new Question2();

            // Version synchrone
            Universe u1 = q1.DeserializeAll(pathDirectory);
            Console.WriteLine("------------------");

            // Version asynchrone
            Task<Universe> task = q2.DeserializeAll(pathDirectory);
            await task;
            Universe u2 = task.Result;
        }        
    }
}
