using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using System.Data;
using NUnit.Framework;

public class NewsletterClass : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleField;
    [SerializeField] private TextMeshProUGUI contentField;
    [SerializeField] private TextMeshProUGUI dateField;
    [SerializeField] private Image imageField;

    private int startingDay = 4;
    private string startingDate = "/07/20XX";

    public void InitializeArticle(int dayNum, string titleText, string contentText, string imageResourcesRef)
    {
        SetDate(dayNum);
        titleField.text = titleText;
        contentField.text = contentText;
        imageField.sprite = Resources.Load<Sprite>(imageResourcesRef);
    }

    private void SetDate(int dayNum)
    {
        string day = "0"+ (dayNum + startingDay).ToString();
        string date = day + startingDate;
        dateField.text = date;
    }
}
