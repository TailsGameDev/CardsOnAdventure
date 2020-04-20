using UnityEngine;
using System.IO;
using System.Collections;

public class ClassesPersistence : Persistence
{
    string fileName = "classes";

    public void SaveClasses( ClassesSerializable classesSerializable )
    {
        GenericSave(fileName, classesSerializable);
    }

    public bool DoSaveExists()
    {
        return File.Exists(Application.persistentDataPath + fileName);
    }

    public ClassesSerializable LoadClasses()
    {
        return GenericLoad<ClassesSerializable>(fileName);
    }
}
