using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;

public class Persistence : MonoBehaviour
{
    public void GenericSave<T>(string nomeArquivo, T dadosDoObjeto)
    {
        BinaryFormatter formater = new BinaryFormatter();
        string path = Application.persistentDataPath + nomeArquivo;
        FileStream stream = new FileStream(path, FileMode.Create);
        formater.Serialize(stream, dadosDoObjeto);
        stream.Close();
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
