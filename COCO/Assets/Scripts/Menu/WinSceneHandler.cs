using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinSceneHandler : MonoBehaviour
{
    public GameObject mainMenu, winMenu, profileMenu, levelMenu;
    public GameObject particlePrefab;
    public int win_or_lose;
    public AudioSource WonSound;
    public GameObject successNotif, failNotif;

    AdvertiseManager advertiseManager;
    GameModel gameModel;
    GameConfig gameConfig;

    private void Start()
    {
        gameConfig = Resources.Load<GameConfig>("gameConfig");
        advertiseManager = new FakeAdvertiseManager();
        win_or_lose = PlayerPrefs.GetInt("win_or_lose");
        if(win_or_lose == 1)
        {   
            winMenu.SetActive(true);
            mainMenu.SetActive(false);
            profileMenu.SetActive(false);
            levelMenu.SetActive(false);
            WonSound.Play();
            Instantiate(particlePrefab, new Vector3(0, 0, 30), Quaternion.identity);
            Instantiate(particlePrefab, new Vector3(7, 0, 30), Quaternion.identity);
            Instantiate(particlePrefab, new Vector3(-7, 0, 30), Quaternion.identity);
            Instantiate(particlePrefab, new Vector3(0, 5, 30), Quaternion.identity);
            PlayerPrefs.SetInt("win_or_lose", 0);

            string json = File.ReadAllText(Application.dataPath + "/gameModel.json");
            gameModel = JsonUtility.FromJson<GameModel>(json);
            gameModel.IncrementCoins(gameConfig.levelPrize);

            json = JsonUtility.ToJson(gameModel);
            File.WriteAllText(Application.dataPath + "/gameModel.json", json);
        }
        else
        {
            mainMenu.SetActive(true);
            profileMenu.SetActive(false);
            levelMenu.SetActive(false);
            winMenu.SetActive(false);
        }
    }

    public void OnShowAdvertiseClicked()
    {
        bool status = advertiseManager.ShowAdvertise();
        if (status)
        {
            StartCoroutine(showSuccessNotif());

            string json = File.ReadAllText(Application.dataPath + "/gameModel.json");
            gameModel = JsonUtility.FromJson<GameModel>(json);
            gameModel.IncrementCoins(gameConfig.levelPrize);

            json = JsonUtility.ToJson(gameModel);
            File.WriteAllText(Application.dataPath + "/gameModel.json", json);
        }
        else
        {
            StartCoroutine(showFailNotif());
        }
    }

    IEnumerator showSuccessNotif()
    {
        successNotif.SetActive(true);

        //Wait for 3 seconds
        yield return new WaitForSecondsRealtime(3);

        successNotif.SetActive(false);
    }

    IEnumerator showFailNotif()
    {
        failNotif.SetActive(true);

        //Wait for 3 seconds
        yield return new WaitForSecondsRealtime(3);

        failNotif.SetActive(false);
    }
}
