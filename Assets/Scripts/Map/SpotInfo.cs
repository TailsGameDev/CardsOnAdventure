using System.Collections.Generic;

public class SpotInfo
{
    public string GOName;
    public bool Cleared;
    public int PlayLvlBtnIndex;
    public List<int> AntecessorsIndexes;
    public bool Visited = false;

    public SpotInfo(string goName, bool cleared, int playLvlBtnIndex, List<int> antecessorsIndexes)
    {
        GOName = goName;
        Cleared = cleared;
        PlayLvlBtnIndex = playLvlBtnIndex;
        AntecessorsIndexes = antecessorsIndexes;
    }

    public SpotInfo GetInfoFromTreeOrGetNull(string desiredName, List<SpotInfo> treeNodes)
    {
        for (int i = 0; i < treeNodes.Count; i++)
        {
            if (treeNodes[i].GOName == desiredName)
            {
                return treeNodes[i];
            }
        }
        return null;
    }

    public void LogInfo()
    {
        string antecessors = "; antecessors: ";
        foreach (int a in AntecessorsIndexes)
        {
            antecessors += a + " and ";
        }
        antecessors += "that's all.";
        L.og("[Spot] " + GOName + "; btnIndex: " + PlayLvlBtnIndex + "; cleared: " + Cleared + antecessors, this);
    }
}