using UnityEngine;

public class SignScript : MonoBehaviour
{
    public bool isFrontSign;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isFrontSign = true;
            Debug.Log("看板に接触");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isFrontSign = false;
            Debug.Log("看板から離れる");
        }
    }
}

