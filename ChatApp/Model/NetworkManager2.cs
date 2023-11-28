using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatApp.Model
{
    internal class NetworkManager2
    {
        private static async Task SendRequest(string ip, int port, CancellationToken cancellationToken = default)
        {
            string message = "ello";
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
            // Create and connect a dual-stack socket
            using Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            await socket.ConnectAsync(ip, port, cancellationToken);

            int bytesSent = 0;
            while (bytesSent < data.Length)
            {
                bytesSent += await socket.SendAsync(data.AsMemory(bytesSent), SocketFlags.None);
            }

            // Do minimalistic buffering assuming ASCII response
            byte[] responseBytes = new byte[256];
            char[] responseChars = new char[256];

            while (true)
            {
                int bytesReceived = await socket.ReceiveAsync(responseBytes, SocketFlags.None, cancellationToken);

                // Receiving 0 bytes means EOF has been reached
                if (bytesReceived == 0) break;

                   //////  //  //     //////   //      //    //    //  //
                  //       ////      //     //  //  //  //  ////  //  //
                 ////      //       ////   //////  //////  //  ////  
                //        //       //     //  //  //  //  //    //  //

                // Convert byteCount bytes to ASCII characters using the 'responseChars' buffer as destination
                int charCount = Encoding.ASCII.GetChars(responseBytes, 0, bytesReceived, responseChars, 0);

                // Print the contents of the 'responseChars' buffer to Console.Out
                await Console.Out.WriteAsync(responseChars.AsMemory(0, charCount), cancellationToken);
            }
        }
    }
}
