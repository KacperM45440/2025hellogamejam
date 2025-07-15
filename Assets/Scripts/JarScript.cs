using UnityEngine;

public class JarScript : MonoBehaviour//ca³y do usuniêcia
{
    public MoneyController moneyControllerRef;
    public GameObject JarGO;

    private void FixedUpdate()
    {
        GetComponent<TextMesh>().text = moneyControllerRef.currentMoneyInJar.ToString();
    }
}
