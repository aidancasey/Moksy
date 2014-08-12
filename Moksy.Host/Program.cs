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

            try
            {
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
            catch (System.AggregateException aggregate)
            {
                Console.WriteLine("ERROR: Launching Moksy.Host.Exe");
                Console.WriteLine(aggregate.Message);
                if (aggregate.InnerException.GetType().FullName == "System.ServiceModel.AddressAccessDeniedException")
                {
                    Console.WriteLine();
                    Console.WriteLine("Moksy launches a real HTTP Server and opens up a HTTP Endpoint so that your");
                    Console.WriteLine("tests and other services can hit it. ");
                    Console.WriteLine();
                    Console.WriteLine("By default, only applications launched as Administrator are allowed to");
                    Console.WriteLine("launch the HTTP Server and create the end-point. ");
                    Console.WriteLine();
                    Console.WriteLine("You are not running as Administrator. ");
                    Console.WriteLine();
                    Console.WriteLine("There are two ways to proceed:");
                    Console.WriteLine();
                    Console.WriteLine("1. Launch Moksy.Host.Exe as Administrator. ");
                    Console.WriteLine();
                    Console.WriteLine("2. Or perform the following operation once from an Administrator Command Prompt");
                    Console.WriteLine("   to create the reservation that will allow everyone to launch a HTTP Server");
                    Console.WriteLine("   and open up the end-point: ");
                    Console.WriteLine();
                    Console.WriteLine(string.Format(@"      netsh http add urlacl url=http://+:{0}/ sddl=""D:(A;;GX;;;WD)""", port));
                    Console.WriteLine();
                    Console.WriteLine("   Then re-run Moksy.Host.Exe as an ordinary user. ");
                    Console.WriteLine();
                    System.Environment.Exit(123);
                }

                System.Environment.Exit(1);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: Launching Moksy.Host.Exe");
                Console.WriteLine(ex.Message);
                System.Environment.Exit(1);
            }
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
