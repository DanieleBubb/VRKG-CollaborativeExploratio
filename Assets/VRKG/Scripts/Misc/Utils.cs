using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;


public class Utils
{
    public static IEnumerator GetHttpRequest(string uri, UnityAction<string, UnityWebRequest.Result, string> onFinished)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                case UnityWebRequest.Result.ProtocolError:
                    onFinished(uri, webRequest.result, webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    onFinished(uri, webRequest.result, webRequest.downloadHandler.text);
                    break;
            }
        }
    }

    public static List<string> GetRegexMatches(string text, string regex)
    {
        Regex rx = new Regex(regex, RegexOptions.IgnoreCase);
        MatchCollection matches = rx.Matches(text);
        List<string> textMatches = new List<string>();
        if (matches.Count > 0)
        {
            foreach (Match match in matches)
            {
                GroupCollection groupCollection = match.Groups;
                textMatches.Add(groupCollection[1].ToString().Trim());
            }
        }

        return textMatches;
    }

    public static string AddEllipsis(string text, int maxLength)
    {
        if (text.Length > maxLength)
        {
            return text.Substring(0, maxLength - 3) + "...";
        }

        return text;
    }

}
