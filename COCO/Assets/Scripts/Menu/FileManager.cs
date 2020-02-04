using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;

public class FileManager : MonoBehaviour
{
    string path;
    GameModel gameModel;

    public RawImage image;

    private void Start()
    {
        string json = File.ReadAllText(Application.dataPath + "/gameModel.json");
        gameModel = JsonUtility.FromJson<GameModel>(json);

        if (gameModel.GetAvatar() != "")
        {
            path = gameModel.GetAvatar();
        }
        else
        {
            path = "";
        }
        UpdateImage();
    }

    public void OpenExplorer()
    {
        path = EditorUtility.OpenFilePanel("Overwrite with png", "", "jpg");
        GetImage();
    }

    void GetImage()
    {
        if(path != null)
        {
            UpdateImage();
        }
    }

    void UpdateImage()
    {
        WWW www = new WWW("file:///" + path);
        image.texture = www.texture;
    }

    private void OnDestroy()
    {
        gameModel.SetAvatar(path);
        string json = JsonUtility.ToJson(gameModel);

        File.WriteAllText(Application.dataPath + "/gameModel.json", json);
    }
}
