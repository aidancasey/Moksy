using Moksy.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length != 1)
            {
                Usage();
                System.Environment.Exit(1);
            }

            int port = 0;
            var success = Int32.TryParse(args[0], out port);
            if (!success)
            {
                Usage();
                System.Environment.Exit(1);
            }

            Application app = new Application(port);
            app.Start();

            Console.WriteLine("----------------------------------------------");
            Console.WriteLine("MOKSY: REST API / JSON Endpoint Faking Toolkit");
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine("by Grey Ham");
            Console.WriteLine();
            Console.WriteLine("See www.brekit.com for more information. ");
            Console.WriteLine("Source at https://github.com/greyham/Moksy");
            Console.WriteLine();
            Console.WriteLine(string.Format("Running Moksy on Port {0}. ", port));
            Console.WriteLine(string.Format("Navigate to: http://localhost:{0} for a sanity test.", port));

            Console.WriteLine("Press a key to exit...");
            Console.ReadKey();
            app.Stop();
        }

        static void Usage()
        {
            System.Console.WriteLine("Usage: Moksy.Host <PortNumber>");
            System.Console.WriteLine();
            System.Console.WriteLine("   eg: Moksy.Host 10011");
            System.Console.WriteLine();
            System.Console.WriteLine("NOTE: To simplify matters, run the host as an Administrator so that no reservation needs to be created manually for the port. ");
        }
    }
}
