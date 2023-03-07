using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

/* Handles http requests and response to download CSV and JSON files from the remote server */
public class FileServerStorage : MonoBehaviour
{
    public string FileListURL;
    public string BaseFileURL;
    public string Regex;
    private List<QueryEntry> cachedEntries;
    private bool ready;
    private int csvRequestsCounter;
    private int jsonRequestsCounter;
    private UnityAction<string> lastQueryContentCallback;

    private void Awake()
    {
        cachedEntries = new List<QueryEntry>();
    }

    public string GetFullUrl(string fileName)
    {
        return BaseFileURL + fileName;
    }

    public void UpdateCache()
    {
        ready = false;
        cachedEntries.Clear();
        StartCoroutine(Utils.GetHttpRequest(FileListURL, OnRemoteFileListRetrieved));
    }

    void OnRemoteFileListRetrieved(string uri, UnityWebRequest.Result result, string text)
    {
        if (result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning("Impossible to connect to file server");
            return;
        }

        List<string> jsonFiles = Utils.GetRegexMatches(text, Regex);
        jsonRequestsCounter = 0;
        foreach (var curJson in jsonFiles)
        {
            StartCoroutine(Utils.GetHttpRequest(GetFullUrl(curJson), OnRemoteJsonRetrieved));
            ++jsonRequestsCounter;
        }
        StartCoroutine(WaitForJsonsAndDownloadCsv());
    }
    
    void OnRemoteJsonRetrieved(string uri, UnityWebRequest.Result result, string text)
    {
        if (result == UnityWebRequest.Result.Success)
        {
            QueryEntry newEntry = JsonUtility.FromJson<QueryEntry>(text);
            if (newEntry != null && newEntry.CsvFileName != null)
            {
                Debug.Log("New entry " + newEntry.Name);
                cachedEntries.Add(newEntry);
            }
        }
        --jsonRequestsCounter;
    }

    IEnumerator WaitForJsonsAndDownloadCsv()
    {
        yield return new WaitUntil(() => jsonRequestsCounter == 0);
        csvRequestsCounter = 0;
        foreach (var curEntry in cachedEntries)
        {
            StartCoroutine(Utils.GetHttpRequest(GetFullUrl(curEntry.CsvFileName), OnRemoteCsvRetrieved));
            ++csvRequestsCounter;
        }

        StartCoroutine(WaitForCsvDownloaded());
    }
    
    void OnRemoteCsvRetrieved(string uri, UnityWebRequest.Result result, string text)
    {
        if (result != UnityWebRequest.Result.Success)
        {
            string csvFile = Path.GetFileName(uri);
            Debug.Log(csvFile + " not found, deleting");
            cachedEntries.RemoveAll(e => e.CsvFileName.Equals(csvFile));
        }
        --csvRequestsCounter;
    }

    IEnumerator WaitForCsvDownloaded()
    {
        yield return new WaitUntil(() => csvRequestsCounter == 0);
        ready = true;
    }
    
    public IEnumerator GetCachedEntries(UnityAction<List<QueryEntry>> callback)
    {
        if (cachedEntries.Count == 0)
        {
            UpdateCache();
            yield return new WaitUntil(() => ready);
        }

        callback(cachedEntries);
    }
    
    public IEnumerator GetQueryContent(QueryEntry query, UnityAction<string> callback)
    {
        lastQueryContentCallback = callback;
        StartCoroutine(Utils.GetHttpRequest(GetFullUrl(query.CsvFileName), OnRemoteQueryRetrieved));
        yield return null;
    }

    void OnRemoteQueryRetrieved(string uri, UnityWebRequest.Result result, string text)
    {
        if (result == UnityWebRequest.Result.Success)
        {
            lastQueryContentCallback(text);
        }
        else
        {
            Debug.LogError("Unable to get csv: " + uri);
        }
    }
    
}
