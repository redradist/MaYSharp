using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TestServer;

namespace TestServer
{
    class Program
    {
        static void Main(string[] args)
        {
            LimitedConcurrencyTaskScheduler tcpServerScheduler = new LimitedConcurrencyTaskScheduler(2);
            // Create a TaskFactory and pass it our custom scheduler. 
            TaskFactory factory = new TaskFactory(tcpServerScheduler);
            Task acceptSocketTask = StartServer();
            acceptSocketTask.Wait();
            Console.WriteLine($"IsCompletedSuccessfully is {acceptSocketTask.IsCompletedSuccessfully}");
        }

        static async Task StartServer()
        {
            try
            {
                // Start the server
                Console.WriteLine("Starting TCP server ...");
                TcpAcceptConnectionHelper.StartServer("127.0.0.1", 49000);
                await TcpAcceptConnectionHelper.ListenAsync();
                Console.WriteLine("ListenAsync is finished !!");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"ex is {ex}");
            }
        }
    }
}
