using UnityEngine;

public class JarScript : MonoBehaviour
{
    public MoneyController moneyControllerRef;
    public GameObject JarGO;

    private void FixedUpdate()
    {
        GetComponent<TextMesh>().text = moneyControllerRef.currentMoneyInJar.ToString();
    }
}
