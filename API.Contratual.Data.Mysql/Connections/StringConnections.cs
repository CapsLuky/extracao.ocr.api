using API.Contratual.CrossCutting.Common.Enum;
using API.Contratual.Dto;
using Microsoft.Extensions.Options;

namespace API.Contratual.Data.Mysql.Connections;

public class StringConnections
{
    // private Dictionary<StringConnectionEnum, string> _connections = new Dictionary<StringConnectionEnum, string>();
    //
    // private static readonly string makemConnection =
    //     Environment.GetEnvironmentVariable("DB_ADMCONTRATUAL_CONNECTION_STRING")
    //     + "Uid="
    //     + Environment.GetEnvironmentVariable("DB_ADMCONTRATUAL_USER")
    //     + "; Pwd="
    //     + Environment.GetEnvironmentVariable("DB_ADMCONTRATUAL_PWD")
    //     + ";";
    //
    // public StringConnections()
    // {
    //     _connections.Add(StringConnectionEnum.Makem, makemConnection);
    // }
    //
    // public string this[StringConnectionEnum key]
    // {
    //     get { return _connections[key]; }
    //     set { _connections[key] = value; }
    // }
}