using Client;
using Client.Models;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using static Client.CmdMessages;

var ip = "127.0.0.1";
var port = 45678;

var client = new TcpClient(ip, port);

var stream = client.GetStream();

var bw = new BinaryWriter(stream);
var br = new BinaryReader(stream);

string userCommand;

var CmdMsg = new CmdMessages();
Console.Write(CmdMsg.DefaultMsg());

while (true)
{
    userCommand = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(userCommand))
    {
        Console.Write(CmdMsg.DefaultMsg());
        continue;
    }

    userCommand = userCommand.Trim();

    var temp = userCommand.Split(' ');

    var commandProperties = new List<string>();

    for (int i = 0; i < temp.Length; i++)
    {
        if (!string.IsNullOrWhiteSpace(temp[i]))
            commandProperties.Add(temp[i].Trim());
    }


    CommandText commandText = commandProperties[0].ToLower() switch
    {
        "help" => CommandText.Help,
        "proclist" => CommandText.Proclist,
        "kill" => CommandText.Kill,
        "run" => CommandText.Run,
        _ => CommandText.Unkown
    };

    string? commandParameter = default;
    if (commandProperties.Count == 2)
        commandParameter = commandProperties[1];


    var command = new Command()
    {
        Parameter = commandParameter,
        Text = commandText
    };


    switch (commandText)
    {
        case CommandText.Help:
            {
                if (!string.IsNullOrWhiteSpace(command.Parameter))
                {
                    Console.Write(CmdMsg.NoParameterNeeded("help"));
                    continue;
                }

                var jsonStr = JsonSerializer.Serialize(command);

                bw.Write(jsonStr);

                await Task.Delay(50);

                var response = br.ReadString();
                Console.WriteLine(response);
                Console.Write(CmdMsg.DefaultMsg());
                break;
            }
        case CommandText.Proclist:
            {
                if (!string.IsNullOrWhiteSpace(command.Parameter))
                {
                    Console.Write(CmdMsg.NoParameterNeeded("proclist"));
                    continue;
                }
                var jsonStr = JsonSerializer.Serialize(command);

                bw.Write(jsonStr);

                await Task.Delay(50);

                var response = br.ReadString();
                var list = JsonSerializer.Deserialize<List<string>>(response);

                if (list is null)
                {
                    Console.Write(CmdMsg.ErrorMessage());
                    continue;
                }
                Console.WriteLine();
                foreach (var processName in list)
                    Console.WriteLine(processName);

                Console.Write(CmdMsg.DefaultMsg());
                break;
            }
        case CommandText.Kill:
            {

                if (string.IsNullOrWhiteSpace(command.Parameter))
                {
                    Console.Write(CmdMsg.MustEnterParameter("kill", "process name"));
                    continue;
                }
                var jsonStr = JsonSerializer.Serialize(command);

                bw.Write(jsonStr);

                await Task.Delay(50);

                var response = br.ReadBoolean();
                if (response is true)
                    Console.Write(CmdMsg.ProcessMsgs(ProcessMsgType.SuccesfullKill));
                else
                    Console.Write(CmdMsg.ProcessMsgs(ProcessMsgType.CantKill));
                break;
            }
        case CommandText.Run:
            {

                if (string.IsNullOrWhiteSpace(command.Parameter))
                {
                    Console.Write(CmdMsg.MustEnterParameter("run", "process name"));
                    continue;
                }
                var jsonStr = JsonSerializer.Serialize(command);

                bw.Write(jsonStr);

                await Task.Delay(50);

                var response = br.ReadBoolean();
                if (response is true)
                    Console.Write(CmdMsg.ProcessMsgs(ProcessMsgType.SuccesfullRun));
                else
                    Console.Write(CmdMsg.ProcessMsgs(ProcessMsgType.CantRun));
                break;
            }
        case CommandText.Unkown:
            {
                Console.Write(CmdMsg.Unknown(userCommand));
                break;
            }

    }

}