using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Persistence
{
    public void GenericSave<T>(string nomeArquivo, T dadosDoObjeto)
    {
        BinaryFormatter formater = new BinaryFormatter();
        string path = Application.persistentDataPath + nomeArquivo;
        FileStream stream = new FileStream(path, FileMode.Create);
        formater.Serialize(stream, dadosDoObjeto);
        stream.Close();
    }

    
    public bool DoesSaveExist(string fileName)
    {
        return File.Exists(Application.persistentDataPath + fileName);
    }
    
    public T GenericLoad<T>(string nomeArquivo)
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
