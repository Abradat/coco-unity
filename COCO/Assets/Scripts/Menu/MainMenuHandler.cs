using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using DG.Tweening;

public class MainMenuHandler : MonoBehaviour
{
    public RectTransform playButton, purchaseButton, profileButton, creditsButton, quitButton;

    private void Start()
    {
        string path = Application.dataPath + "/gameModel.json";
        if (!File.Exists(path))
        {
            GameModel gameModel = new GameModel();
            string json = JsonUtility.ToJson(gameModel);
            File.WriteAllText(Application.dataPath + "/gameModel.json", json);
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quit Button was Pressed !");
        Application.Quit();
    }

    private void OnEnable()
    {
        AnimationManager();
    }

    void AnimationManager()
    {
        Vector3 initialPos = playButton.transform.position;
        initialPos.x -= 40;
        playButton.transform.position = initialPos;

        initialPos = purchaseButton.transform.position;
        initialPos.x += 40;
        purchaseButton.transform.position = initialPos;

        initialPos = profileButton.transform.position;
        initialPos.x -= 40;
        profileButton.transform.position = initialPos;

        initialPos = creditsButton.transform.position;
        initialPos.x += 40;
        creditsButton.transform.position = initialPos;

        initialPos = quitButton.transform.position;
        initialPos.x -= 40;
        quitButton.transform.position = initialPos;

        playButton.DOAnchorPosX(0, 1).SetEase(Ease.OutBounce);
        purchaseButton.DOAnchorPosX(0, 1).SetEase(Ease.OutBounce);
        profileButton.DOAnchorPosX(0, 1).SetEase(Ease.OutBounce);
        creditsButton.DOAnchorPosX(0, 1).SetEase(Ease.OutBounce);
        quitButton.DOAnchorPosX(0, 1).SetEase(Ease.OutBounce);
    }
}
