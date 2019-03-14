using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class handle : MonoBehaviour
{
    public Text readText;
    public InputField setText;

    string setURL = "http://localhost:8888/PostName.php?name=";
    string getURL = "http://localhost:8888/ReadName.php";

    public void SetText()
    {
        StartCoroutine(SetTheText (setText.text));

    }
    IEnumerator SetTheText(string text)
    {
        byte[] myData = System.Text.Encoding.UTF8.GetBytes(text);
        using (UnityWebRequest www = UnityWebRequest.Put(setURL+text, myData))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Upload complete!");
            }
        }
    }

    public void GetText()
    {
        StartCoroutine(GetTheText());

    }
    IEnumerator GetTheText()
    {
        Debug.Log("entered function");
        string URL = getURL;
        UnityWebRequest www = UnityWebRequest.Get(URL);
        yield return www.SendWebRequest();

        if (www.isNetworkError) // Error
        {
            Debug.Log(www.error);
        }
        else // Success
        {
            Debug.Log(www.downloadHandler.text);
        }
        readText.text = www.downloadHandler.text;
    }

}
