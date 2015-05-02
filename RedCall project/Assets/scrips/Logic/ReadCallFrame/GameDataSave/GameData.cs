using UnityEngine;
using System.Collections;

public class GameData
{
    public string Account { get; set; }
    public string Password { get; set; }
    public bool HasVideoPlayedBefore { get; set; }
    public string Key { get; set; }
    public string ClientVersion { get; set; }
    public bool IsClientVersionInitialed { get; set; }
}
