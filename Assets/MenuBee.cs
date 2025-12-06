using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuBee : MonoBehaviour
{
    [SerializeField] private float speed = 30f;
    [SerializeField] private float movementRangeMin = 100f;
    [SerializeField] private float movementRangeMax = 300f;
    [SerializeField] private RectTransform backgroundTransform;
    [SerializeField] private Image beeImage;

    private void Start()
    {
        StartCoroutine(MoveBee());
    }

    private IEnumerator MoveBee()
    {
        Vector3 startPosition;
        Vector3 targetPosition;
        do
        {
            startPosition = transform.position;
            targetPosition = startPosition + new Vector3(Random.Range(-movementRangeMax, movementRangeMax), Random.Range(-movementRangeMax, movementRangeMax), 0);
            yield return null;
        }
        while (!IsPointInUIRect(targetPosition));
        int flip = (targetPosition.x < startPosition.x) ? 1 : -1;
        beeImage.transform.localScale = new Vector3(flip * beeImage.transform.localScale.x, beeImage.transform.localScale.y, beeImage.transform.localScale.z);
        float journeyLength = Vector3.Distance(startPosition, targetPosition);
        float startTime = Time.time;
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            float distCovered = (Time.time - startTime) * speed;
            float fractionOfJourney = distCovered / journeyLength;
            transform.position = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);
            yield return null;
        }
        yield return new WaitForSeconds(Random.Range(1f, 3f));
        StartCoroutine(MoveBee());
    }

    public bool IsPointInUIRect(Vector3 worldPoint)
    {
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldPoint);
        Debug.Log(RectTransformUtility.RectangleContainsScreenPoint(backgroundTransform, screenPoint, Camera.main));
        return RectTransformUtility.RectangleContainsScreenPoint(backgroundTransform, screenPoint, Camera.main);
    }
}
