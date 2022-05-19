using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SerializationManager
{
    public static bool Save(string saveName, object saveData)
    {
        BinaryFormatter formatter = GetBinaryFormatter();

        if (!Directory.Exists(Application.persistentDataPath + "/saves"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/saves");
        }

        string path = Application.persistentDataPath + "/saves/" + saveName + ".sav";

        FileStream file = File.Create(path);

        formatter.Serialize(file, saveData);

        Debug.Log("You Saved");

        file.Close();

        return true;
    }

    public static SaveData Load(string saveName)
    {
        if (!File.Exists(Application.persistentDataPath + "/saves/" + saveName + ".sav")) { return null; }

        BinaryFormatter formatter = GetBinaryFormatter();

        FileStream file = File.OpenRead(Application.persistentDataPath + "/saves/" + saveName + ".sav");

        try
        {
            SaveData save = formatter.Deserialize(file) as SaveData;
            file.Close();
            return save;
        }

        catch
        {
            Debug.Log("Failed to Load File");
            file.Close();
            return null;
        }
    }

    public static BinaryFormatter GetBinaryFormatter()
    {
        BinaryFormatter formatter = new BinaryFormatter();

        return formatter;
    }
}
