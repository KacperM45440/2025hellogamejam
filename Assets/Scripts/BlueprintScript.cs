using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class BlueprintScript : InteractableObject
{
    [SerializeField] private Collider colliderRef;
    public CraftingMgr craftingMgr;
    public Hand handRef;

    public Button hideBlueprintButton;

    public override void Interact()
    {
        PullOutBlueprint();
        base.Interact();
    }

    public void ToggleCollider(bool value)
    {
        if (value)
        {
            colliderRef.enabled = true;
        }
        else
        {
            colliderRef.enabled = false;
            SetOutline(false);
        }
    }

    public void PullOutBlueprint()
    {
        hideBlueprintButton.gameObject.SetActive(true);
        CameraController.Instance.ShowBlueprint();
        handRef.SetUIHandBlock(true);
    }

    public void PutBlueprintAway()
    {
        hideBlueprintButton.gameObject.SetActive(false);
        CameraController.Instance.HideBlueprint();
        handRef.SetUIHandBlock(false);
    }
}
