using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using RTLTMPro;

public class PurchaseMenuHandler : MonoBehaviour
{
    public RectTransform title, image, amount, price, submitButton, backButton;
    public TMP_InputField amountText;
    public RTLTextMeshPro priceText;
    public GameObject successNotif, failNotif;

    PurchaseManager purchaseManager;
    GameModel gameModel;
    int amountInt;

    private void Start()
    {
        purchaseManager = new FakePurchaseManager();
    }

    void Update()
    {
        if (amountText.text != "" && int.TryParse(amountText.text, out amountInt) && amountText.text.Length <= 8)
        {
            priceText.text = (amountInt + 1).ToString();
        }
        else
        {
            priceText.text = "۰";
        }
    }

    public void OnSubmitClicked()
    {
        int purchasedAmount = purchaseManager.MakePurchase(amountInt, amountInt + 1);
        if (purchasedAmount > 0)
        {
            string json = File.ReadAllText(Application.dataPath + "/gameModel.json");
            gameModel = JsonUtility.FromJson<GameModel>(json);
            gameModel.IncrementCoins(purchasedAmount);

            json = JsonUtility.ToJson(gameModel);
            File.WriteAllText(Application.dataPath + "/gameModel.json", json);

            StartCoroutine(showSuccessNotif());
        }
        else if (purchasedAmount != 0)
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

    private void OnEnable()
    {
        AnimationManager();
    }

    void AnimationManager()
    {
        Vector3 initialPos = title.transform.position;
        initialPos.x -= 40;
        title.transform.position = initialPos;

        initialPos = image.transform.position;
        initialPos.x += 40;
        image.transform.position = initialPos;

        initialPos = amount.transform.position;
        initialPos.x -= 40;
        amount.transform.position = initialPos;

        initialPos = price.transform.position;
        initialPos.x += 40;
        price.transform.position = initialPos;

        initialPos = submitButton.transform.position;
        initialPos.x -= 40;
        submitButton.transform.position = initialPos;

        initialPos = backButton.transform.position;
        initialPos.x += 40;
        backButton.transform.position = initialPos;

        title.DOAnchorPosX(-1.5f, 1).SetEase(Ease.OutBounce);
        image.DOAnchorPosX(-1.5f, 1).SetEase(Ease.OutBounce);
        amount.DOAnchorPosX(-41, 1).SetEase(Ease.OutBounce);
        price.DOAnchorPosX(-5.5f, 1).SetEase(Ease.OutBounce);
        submitButton.DOAnchorPosX(-5.5f, 1).SetEase(Ease.OutBounce);
        backButton.DOAnchorPosX(-5.5f, 1).SetEase(Ease.OutBounce);
    }
}
