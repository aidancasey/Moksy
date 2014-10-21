using Moksy.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Host
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length < 1)
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

            bool simulationsSpecified = false;
            string simulationsPath = null;
            Moksy.Common.SimulationCollection simulations = new Common.SimulationCollection();
            simulationsPath = args.FirstOrDefault(f => f.StartsWith("/File:", StringComparison.InvariantCultureIgnoreCase));
            if (simulationsPath != null)
            {
                simulationsPath = simulationsPath.Substring(6);
                simulationsSpecified = true;
            }
            var log = args.FirstOrDefault(f => f.StartsWith("/Log:true", StringComparison.InvariantCultureIgnoreCase) || string.Compare(f, "/log", true) == 0);

            System.Reflection.Assembly thisExe;
            thisExe = System.Reflection.Assembly.GetExecutingAssembly();
            var list = thisExe.GetManifestResourceNames();

            var header = ReadResource("Moksy.Host.Resources.Header.txt");
            var simulationsText = ReadResource("Moksy.Host.Resources.Simulations.txt");

            ApplicationDirectives parameters = new ApplicationDirectives() { Log = (log != null) };

            try
            {
                Console.WriteLine("----------------------------------------------");
                Console.WriteLine("MOKSY: REST API / JSON Endpoint Faking Toolkit");
                Console.WriteLine("----------------------------------------------");
                Console.WriteLine("by Grey Ham");
                Console.WriteLine();
                Console.WriteLine("See www.brekit.com for more information. ");
                Console.WriteLine("Source at https://github.com/greyham/Moksy");
                Console.WriteLine(); 
                
                if (simulationsSpecified)
                {
                    Console.Write(string.Format("Loading simulations from {0}...", simulationsPath));

                    var contents = System.IO.File.ReadAllText(simulationsPath);
                    simulations = Newtonsoft.Json.JsonConvert.DeserializeObject<Moksy.Common.SimulationCollection>(contents);

                    Console.WriteLine(string.Format("{0} simulations have been loaded. ", simulations.Count));
                    Console.WriteLine("");

                    // ASSERTION: We have loaded the simulations into memory. 
                }

                Application app = new Application(port, parameters);
                app.Start();

                Console.WriteLine(string.Format("Running Moksy on Port {0}. ", port));
                Console.WriteLine(string.Format("Navigate to: http://localhost:{0} for a sanity test.", port));

                if (simulationsSpecified)
                {
                    Task.Factory.StartNew(() =>
                    {
                        Proxy proxy = new Proxy(port);
                        proxy.Wait(20);

                        // We need to wait until the service has started. 
                        foreach (var simulation in simulations)
                        {
                            proxy.Add(simulation);
                        }
                    }, TaskCreationOptions.LongRunning
                    );
                }

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
            System.Console.WriteLine("Usage: Moksy.Host <PortNumber> [/file:SimulationFile.json] [/log]");
            System.Console.WriteLine();
            System.Console.WriteLine("   eg: Moksy.Host 10011");
            System.Console.WriteLine(@"   eg: Moksy.Host 10011 /file:C:\Temp\MySimulations.json /log");
            System.Console.WriteLine();
            System.Console.WriteLine("NOTE: To simplify matters, run the host as an Administrator so that no reservation needs to be created manually for the port. ");
        }

        static string ReadResource(string name)
        {
            System.Reflection.Assembly thisExe;
            thisExe = System.Reflection.Assembly.GetExecutingAssembly();
            var stream = thisExe.GetManifestResourceStream(name);
            using (var reader = new System.IO.StreamReader(stream, Encoding.UTF8))
            {
                string value = reader.ReadToEnd();
                return value;
            }
        }
    }
}
