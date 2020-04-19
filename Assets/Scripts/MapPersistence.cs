using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public class MapPersistence : MonoBehaviour
{
    protected static MapPersistence instance;

    private void Awake()
    {
        if (instance == null){
            instance = this;
        }
    }

    public void SaveMap(string mapName, MapInfo mapInfo)
    {
        L.og("Saving Map!!", this);
        GenericSave(mapName, mapInfo);
    }

    public bool DoSaveExists(string mapName)
    {
        return File.Exists(Application.persistentDataPath + mapName);
    }

    public MapInfo LoadMap(string mapName)
    {
        L.og("Loading Map!!", this);
        return GenericLoad<MapInfo>(mapName);
    }

    private void GenericSave<T>(string nomeArquivo, T dadosDoObjeto)
    {
        BinaryFormatter formater = new BinaryFormatter();
        string path = Application.persistentDataPath + nomeArquivo;
        FileStream stream = new FileStream(path, FileMode.Create);
        formater.Serialize(stream, dadosDoObjeto);
        stream.Close();
    }

    private T GenericLoad<T>(string nomeArquivo)
    {
        string path = Application.persistentDataPath + nomeArquivo;
        if (File.Exists(path))
        {
            BinaryFormatter formater = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            T dados = (T)formater.Deserialize(stream);
            stream.Close();

            return dados;
        }
        else
        {
            L.ogError("there is no save!!!!!!!!!!!!", this);
            return default(T);
        }
    }
}