using DG.Tweening;
using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField] private Transform EndPoint;
    [SerializeField] private Transform StartPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MoveIn();
    }

    private void MoveIn()
    {
        gameObject.transform.DOMove(EndPoint.position,1).OnComplete(() =>
        {

            MoveOut();

        });
    }
    private void MoveOut()
    {
        gameObject.transform.DOMove(StartPoint.position, 1).OnComplete(() =>
        {

            MoveIn();

        });
    }

}
