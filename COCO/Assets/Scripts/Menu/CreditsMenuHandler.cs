using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CreditsMenuHandler : MonoBehaviour
{
    public RectTransform title, hamed, shayan, backButton;

    private void OnEnable()
    {
        AnimationManager();
    }

    void AnimationManager()
    {
        Vector3 initialPos = title.transform.position;
        initialPos.x -= 40;
        title.transform.position = initialPos;

        initialPos = hamed.transform.position;
        initialPos.x += 40;
        hamed.transform.position = initialPos;

        initialPos = shayan.transform.position;
        initialPos.x -= 40;
        shayan.transform.position = initialPos;

        initialPos = backButton.transform.position;
        initialPos.x -= 40;
        backButton.transform.position = initialPos;

        title.DOAnchorPosX(-1.5f, 1).SetEase(Ease.OutBounce);
        hamed.DOAnchorPosX(-1.5f, 1).SetEase(Ease.OutBounce);
        shayan.DOAnchorPosX(31, 1).SetEase(Ease.OutBounce);
        backButton.DOAnchorPosX(-5.5f, 1).SetEase(Ease.OutBounce);
    }
}
