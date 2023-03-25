using Server.Models;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

var ip = IPAddress.Parse("127.0.0.1");
var port = 45678;

var listener = new TcpListener(ip, port);

listener.Start(10);



StringBuilder builder = new StringBuilder();
builder.Append("\nPROCLIST".PadRight(30));
builder.Append("Displays running tasks and services");
builder.Append("\nKILL <process name>".PadRight(30));
builder.Append("Kills or stops a running process or application");
builder.Append("\nRUN <process name>".PadRight(30));
builder.Append("Runs or starts the given process or application");

string helpText = builder.ToString();



while (true)
{
    var client = await listener.AcceptTcpClientAsync();

    Console.WriteLine($"Client {client.Client.RemoteEndPoint} accepted");


    new Task(() =>
    {
        var stream = client.GetStream();
        var bw = new BinaryWriter(stream);
        var br = new BinaryReader(stream);
        while (true)
        {
            var jsonStr = br.ReadString();

            var command = JsonSerializer.Deserialize<Command>(jsonStr);

            if (command is null)
                return;

            switch (command.Text)
            {
                case CommandText.Help:
                    bw.Write(helpText);
                    stream.Flush();
                    break;
                case CommandText.Proclist:
                    var list = Process.GetProcesses()
                                      .Select(p => p.ProcessName)
                                      .ToList();
                    var jsonList = JsonSerializer.Serialize(list);

                    bw.Write(jsonList);
                    stream.Flush();
                    break;
                case CommandText.Kill:
                    bool canKill;
                    var processName = command.Parameter;

                    if (processName == null) canKill = false;
                    else
                    {

                        canKill = false;
                        var processes = Process.GetProcessesByName(processName);

                        if (processes.Length > 0)
                        {
                            try
                            {
                                foreach (var p in processes)
                                    p.Kill();

                                canKill = true;
                            }
                            catch (Exception) { }
                        }
                    }

                    bw.Write(canKill);
                    break;
                case CommandText.Run:
                    var canRun = false;
                    var procName = command.Parameter;

                    if (procName is not null)
                    {
                        try
                        {
                            Process.Start(procName);
                            canRun = true;
                        }
                        catch (Exception) { }
                    }

                    bw.Write(canRun);
                    break;
                case CommandText.Unkown:
                    break;
            }
        }
    }).Start();
}