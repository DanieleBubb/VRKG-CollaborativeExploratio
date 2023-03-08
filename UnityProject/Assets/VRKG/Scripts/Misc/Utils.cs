using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

/*
 * MIT License

Copyright (c) 2023 Alberto Accardo, Daniele Monaco, Maria Angela Pellegrino, Vittorio Scarano, Carmine Spagnuolo

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

 */

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
