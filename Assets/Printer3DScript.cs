using System.Collections;
using TMPro;
using UnityEngine;
using static UnityEditor.Progress;
using static UnityEngine.Rendering.DebugUI;

public class Printer3DScript : InteractableObject
{
    public InventoryController inventoryControllerRef;

    [SerializeField] private Transform printSpot;
    [SerializeField] private GameObject printableFrame;
    [SerializeField] private Collider colliderRef;
    [SerializeField] private TextMeshProUGUI descriptionRef;

    private Outline outlineRef;
    private Animator animatorRef;

    private void Start()
    {
        animatorRef = GetComponent<Animator>();
        outlineRef = GetComponent<Outline>();

        SetReadyToPrint(false);
        outlineRef.enabled = false;
    }

    public override void Interact()
    {
        interactable = false;

        animatorRef.SetTrigger("Print");
    }

    public void SpawnGunFrame()
    {
        inventoryControllerRef.AddToInventory(printableFrame);

        GameObject newItem = Instantiate(printableFrame);
        newItem.transform.position = printSpot.position;
        SetReadyToPrint(false);
    }

    public void SetReadyToPrint(bool value)
    {
        interactable = value;

        SetCollider(value);
        UpdateDescription(value);
    }

    public override void SetOutline(bool value)
    {
        if (isOutline == value)
        {
            return;
        }

        isOutline = value;
        outlineRef.enabled = value;
    }

    public virtual void UpdateDescription(bool value)
    {
        if (value)
        {
            descriptionRef.text = "Out of gun frames!\nCLICK TO 3D PRINT";
            return;
        }

        descriptionRef.text = "";
    }

    private void SetCollider(bool value)
    {
        colliderRef.enabled = value;
    }
}
