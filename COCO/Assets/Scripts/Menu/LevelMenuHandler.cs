using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using DG.Tweening;

public class LevelMenuHandler : MonoBehaviour
{
    public List<Button> levelButtons;
    public RectTransform levelHeader, levelRectButtons, backButton;

    GameModel gameModel;

    private void Start()
    {
        string path = Application.dataPath + "/gameModel.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(Application.dataPath + "/gameModel.json");
            gameModel = JsonUtility.FromJson<GameModel>(json);

            for (int i = 0; i < gameModel.GetLevel(); i++)
            {
                levelButtons[i].interactable = true;
            }
        }
    }

    private void OnEnable()
    {
        AnimationManager();
    }

    public void EnterGame() {
        int selected_level = System.Convert.ToInt32(EventSystem.current.currentSelectedGameObject.name);
        PlayerPrefs.SetInt("selected_level", selected_level);
        SceneManager.LoadScene("GameScene");
    }

    void AnimationManager()
    {
        Vector3 initialPos = levelHeader.transform.position;
        initialPos.x -= 40;
        levelHeader.transform.position = initialPos;

        initialPos = levelRectButtons.transform.position;
        initialPos.x += 40;
        levelRectButtons.transform.position = initialPos;

        initialPos = backButton.transform.position;
        initialPos.x -= 40;
        backButton.transform.position = initialPos;

        levelHeader.DOAnchorPosX(28, 1).SetEase(Ease.OutBounce);
        levelRectButtons.DOAnchorPosX(0, 1).SetEase(Ease.OutBounce);
        backButton.DOAnchorPosX(36.5f, 1).SetEase(Ease.OutBounce);
    }
}
