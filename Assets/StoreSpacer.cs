using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StoreSpacer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameField;
    [SerializeField] private GameObject hideButton;
    [SerializeField] private List<GameObject> childrenItems = new List<GameObject>();

    private bool hidden = false;

    public void InitializeSpacer(string categoryName)
    {
        nameField.text = categoryName;
    }

    public void AddChildItem(GameObject item)
    {
        childrenItems.Add(item);
    }

    public void ToggleHideChildren()
    {
        hidden = !hidden;
        hideButton.transform.rotation = Quaternion.Euler(0, 0, hidden ? 90 : -90);

        foreach (GameObject item in childrenItems)
        {
            Debug.Log($"Setting item {item.name} active: {!hidden}");
            item.SetActive(!hidden);
        }
    }
}
