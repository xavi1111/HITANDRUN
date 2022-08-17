using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
    
    private string dirPath;
    private string fileName;

    public FileDataHandler (string dirPath, string fileName)
    {
        this.dirPath = dirPath;
        this.fileName = fileName;
    }

    public GameData Load()
    {
        string path = Path.Combine(dirPath, fileName);
        GameData loadedData = null;
        if(File.Exists(path))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.Log("Error loading data");
            }
        }
            
        return loadedData;
    }

    public void Save(GameData data)
    {
        string path = Path.Combine(dirPath, fileName);
        try
        {
            Directory.CreateDirectory(dirPath);
            string dataToStore = JsonUtility.ToJson(data, true);
            using (FileStream stream =  new FileStream(path, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error saving data");
        }
    }
}
