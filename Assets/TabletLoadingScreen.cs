using DG.Tweening;
using System.Collections;
using UnityEngine;

public class TabletLoadingScreen : MonoBehaviour
{
    [SerializeField] private GameObject confirmationScreen;
    [SerializeField] private GameObject loadingIcon;
    [SerializeField] private TabletController tabletControllerRef;

    public void StartLoading(){
        loadingIcon.transform.DORotate(new Vector3(0, 0, 360), 1f, RotateMode.FastBeyond360)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1);
        StartCoroutine(ShowConfirmationScreen());
    }

    private IEnumerator ShowConfirmationScreen()
    {
        yield return new WaitForSeconds(3f);
        confirmationScreen.SetActive(true);
        gameObject.SetActive(false);
        tabletControllerRef.DeliveryCompleted();
    }
}
