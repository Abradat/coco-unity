using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using DG.Tweening;

public class ProfileMenuHandler : MonoBehaviour
{
    public GameObject EmailGameObject, NameGameObject, UsernameGameObject;
    public string Email, Name, Username;
    public RectTransform headerLabel, image, editButton, nameInput, emailInput, usenameInput, backButton;

    GameModel gameModel;

    private void Start()
    {
        string json = File.ReadAllText(Application.dataPath + "/gameModel.json");
        gameModel = JsonUtility.FromJson<GameModel>(json);
        
        if (gameModel.GetName() != "" && gameModel.GetName().Length > 1)
        {
            NameGameObject.gameObject.GetComponent<TextMeshProUGUI>().text = gameModel.GetName();
        }

        if (gameModel.GetEmail() != "" && gameModel.GetEmail().Length > 1)
        {
            EmailGameObject.gameObject.GetComponent<TextMeshProUGUI>().text = gameModel.GetEmail();
        }

        if (gameModel.GetUsername() != "" && gameModel.GetUsername().Length > 1)
        {
            UsernameGameObject.gameObject.GetComponent<TextMeshProUGUI>().text = gameModel.GetUsername();
        }
    }

    private void OnDestroy()
    {
        Email = EmailGameObject.gameObject.GetComponent<TextMeshProUGUI>().text;
        Name = NameGameObject.gameObject.GetComponent<TextMeshProUGUI>().text;
        Username = UsernameGameObject.gameObject.GetComponent<TextMeshProUGUI>().text;

        gameModel.SetName(Name);
        gameModel.SetEmail(Email);
        gameModel.SetUsername(Username);

        string json = JsonUtility.ToJson(gameModel);
        File.WriteAllText(Application.dataPath + "/gameModel.json", json);
    }

    private void OnEnable()
    {
        AnimationManager();
    }

    void AnimationManager()
    {
        Vector3 initialPos = image.transform.position;
        initialPos.x -= 40;
        image.transform.position = initialPos;

        initialPos = editButton.transform.position;
        initialPos.x += 40;
        editButton.transform.position = initialPos;

        initialPos = nameInput.transform.position;
        initialPos.x -= 40;
        nameInput.transform.position = initialPos;

        initialPos = emailInput.transform.position;
        initialPos.x += 40;
        emailInput.transform.position = initialPos;

        initialPos = usenameInput.transform.position;
        initialPos.x -= 40;
        usenameInput.transform.position = initialPos;

        initialPos = backButton.transform.position;
        initialPos.x += 40;
        backButton.transform.position = initialPos;

        initialPos = headerLabel.transform.position;
        initialPos.x -= 40;
        headerLabel.transform.position = initialPos;

        headerLabel.DOAnchorPosX(29, 1).SetEase(Ease.OutBounce);
        image.DOAnchorPosX(29, 1).SetEase(Ease.OutBounce);
        editButton.DOAnchorPosX(29, 1).SetEase(Ease.OutBounce);
        nameInput.DOAnchorPosX(-0.5f, 1).SetEase(Ease.OutBounce);
        emailInput.DOAnchorPosX(-4, 1).SetEase(Ease.OutBounce);
        usenameInput.DOAnchorPosX(-11, 1).SetEase(Ease.OutBounce);
        backButton.DOAnchorPosX(29, 1).SetEase(Ease.OutBounce);
    }
}
