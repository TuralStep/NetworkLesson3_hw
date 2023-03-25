using System.Text;

namespace Client;

public class CmdMessages
{

    StringBuilder sb = new();


    public string DefaultMsg()
    {
        return $"{Directory.GetCurrentDirectory()}>";
    }


    public string NoParameterNeeded(string commandName)
    {
        sb.Clear();

        sb.Append("\nCommand \'");
        sb.Append(commandName);
        sb.Append("\' does not accept any parameters\n\n");
        sb.Append(DefaultMsg());

        return sb.ToString();
    }

    public string MustEnterParameter(string commandName, string parameterName)
    {
        sb.Clear();

        sb.Append("\nYou must enter \'");
        sb.Append(parameterName);
        sb.Append("\' to use \'");
        sb.Append(commandName);
        sb.Append("\' command.\n\n");
        sb.Append(DefaultMsg());

        return sb.ToString();
    }


    public enum ProcessMsgType
    {
        SuccesfullKill,
        CantKill,
        SuccesfullRun,
        CantRun
    }

    public string ProcessMsgs(ProcessMsgType type)
    {
        sb.Clear();

        switch (type)
        {
            case ProcessMsgType.SuccesfullKill:
                sb.Append("\nProcess ended/killed succesfully.\n\n");
                break;
            case ProcessMsgType.CantKill:
                sb.Append("\nProcess can't be ended/killed.");
                sb.Append("\nCheck if process name is correct and");
                sb.Append("\nmake sure you have access to do it.\n\n");
                break;
            case ProcessMsgType.SuccesfullRun:
                sb.Append("\nProcess is started succesfully.\n\n");
                break;
            case ProcessMsgType.CantRun:
                sb.Append("\nProcess can't get started.");
                sb.Append("\nCheck if process name is correct and");
                sb.Append("\nmake sure you have access to do it.\n\n");
                break;
        }

        sb.Append(DefaultMsg());

        return sb.ToString();
    }


    public string ErrorMessage()
    {
        sb.Clear();

        sb.Append("\nSomething went wrong. But don't worry");
        sb.Append("\nit is our problem. Please report this issue");
        sb.Append("\nto \'turalozel@gmail.com\'. Until this issue");
        sb.Append("\nis fixed, you can try our other commands.\n\n");
        sb.Append(DefaultMsg());

        return sb.ToString();
    }


    public string Unknown(string command)
    {
        sb.Clear();

        sb.Append($"\'{command}\' is not recognized as an internal or");
        sb.Append("\nexternal command, operable program or batch file.");
        sb.Append("\nType \'help\' if you can't find what you need.\n\n");
        sb.Append(DefaultMsg());

        return sb.ToString();
    }

}
