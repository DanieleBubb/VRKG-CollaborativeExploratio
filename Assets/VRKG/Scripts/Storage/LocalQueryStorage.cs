using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class QueryEntry
{
    public string Name;
    public string Preview;
    public string CsvFileName;
    public string GraphicsProfile;
}

/* Manages a local file storage in a folder */
public class LocalQueryStorage : MonoBehaviour
{
    public string BaseFolder;
    public List<QueryEntry> CachedEntries;
    public bool ready;

    public string GetFullFilePath(string fileName)
    {
        return BaseFolder + fileName;
    }

    public bool IsReady
    {
        get
        {
            return ready;
        }
    }

    public void UpdateCache()
    {
        ready = false;
        UpdateCacheAsync();
    }
    
    async void UpdateCacheAsync()
    {
        await Task.Run(UpdateCachedEntries);
        ready = true;
    }

    void UpdateCachedEntries()
    {
        CachedEntries.Clear();
     
        var jsonFiles = Directory.GetFiles(BaseFolder, "*.json", SearchOption.TopDirectoryOnly);
        foreach (var curJsonFile in jsonFiles)
        {
            QueryEntry newEntry = JsonUtility.FromJson<QueryEntry>(File.ReadAllText(curJsonFile));
            if(File.Exists(BaseFolder + newEntry.CsvFileName))
                CachedEntries.Add(newEntry);
        }
    }

    public IEnumerator GetCachedEntries(UnityAction<List<QueryEntry>> callback)
    {
        if (CachedEntries.Count == 0)
        {
            UpdateCache();
            yield return new WaitUntil(() => ready);
        }

        callback(CachedEntries);
    }

    [ContextMenu ("Create Fake JSON")]
    void CreateFakeJson()
    {
        QueryEntry newEntry = new QueryEntry();
        newEntry.Name = "Van Gogh";
        newEntry.Preview = "Van Gogh Preview";
        newEntry.CsvFileName = "VanGoghKG.csv";
        string json = JsonUtility.ToJson(newEntry, true);
        File.WriteAllText(BaseFolder + "VanGoghKG.json", json);
    }
}
