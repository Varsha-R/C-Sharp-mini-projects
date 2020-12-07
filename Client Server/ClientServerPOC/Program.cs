using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class SynchronousSocketClient
{
    public static void StartClient()
    { 
        byte[] bytes = new byte[1024];

        try
        { 
            IPAddress ipAddress = IPAddress.Parse("192.168.15.45");
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);
            
            try
            {
                byte[] msg;
                int bytesSent;
                int bytesRec;
                for (int i=0; i < 100; i++)
                {
                    Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    sender.Connect(remoteEP);
                    Console.WriteLine("Socket connected to {0}", sender.RemoteEndPoint.ToString());
                    if (i == 0)
                    {                        
                        msg = Encoding.ASCII.GetBytes("Starting Windows Reset. <EOF>");                        
                    }
                    else if( i == 1)
                    {
                        Process pSysReset = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = @"C:\Windows\Sysnative\cmd.exe",
                                Arguments = @"/C systemreset",
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,
                                StandardErrorEncoding = Encoding.UTF8,
                                StandardOutputEncoding = Encoding.UTF8,
                                WindowStyle = ProcessWindowStyle.Normal,
                                RedirectStandardInput = true,

                            }
                        };
                        pSysReset.Start();
                        msg = Encoding.ASCII.GetBytes("Triggered System Reset on the client <EOF>");
                    }
                    else if (i == 25 || i == 50)
                    {
                        msg = Encoding.ASCII.GetBytes("Waiting for delay [5 seconds] <EOF>");
                        Task.Delay(5000).Wait();
                    }
                    else
                    {
                        msg = Encoding.ASCII.GetBytes("Testing: "+ i +" <EOF>");
                    }
                    bytesSent = sender.Send(msg);
                    bytesRec = sender.Receive(bytes);
                    Console.WriteLine("Echoed test = {0}", Encoding.ASCII.GetString(bytes, 0, bytesRec));
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                }

            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    public static int Main(String[] args)
    {
        StartClient();
        return 0;
    }
}