using UnityEngine;

public class JarScript : MonoBehaviour//ca�y do usuni�cia
{
    public MoneyController moneyControllerRef;
    public GameObject JarGO;

    private void FixedUpdate()
    {
        GetComponent<TextMesh>().text = moneyControllerRef.currentMoneyInJar.ToString();
    }
}
