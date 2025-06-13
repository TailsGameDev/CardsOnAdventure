using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Persistence
{
    protected void GenericSave<T>(string fileName, T objectData)
    {
        BinaryFormatter formater = new BinaryFormatter();
        string path = Application.persistentDataPath + fileName;
        FileStream stream = new FileStream(path, FileMode.Create);
        formater.Serialize(stream, objectData);
        stream.Close();
    }
    protected bool HasSave(string fileName)
    {
        return File.Exists(Application.persistentDataPath + fileName);
    }
    protected T GenericLoad<T>(string fileName)
    {
        string path = Application.persistentDataPath + fileName;
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
