using UnityEngine;

public class Ladder : MonoBehaviour
{
    [SerializeField] LayerMask playerMask = 0;

    private bool playerDetected = false;

    [SerializeField] GameObject backCollision = default;
    [SerializeField] GameObject topCollision = default;
    [SerializeField] BoxCollider topDetection = default;

    private void Awake()
    {
        backCollision.SetActive(false);
        topCollision.SetActive(true);
    }

    private void Update()
    {
        if(playerDetected)
        {
            if (Mathf.Abs(Input.GetAxis("UpDown")) > 0.2f)
            {
                if(Input.GetAxis("UpDown") > 0.2f)
                {
                    if(IsPlayerOnTop()) return;
                    ActiveClimbable();
                }
                else
                {
                    ActiveClimbable();
                }
            }
        }
        else
        {
            DesactiveClimbable();
        }
    }

    private void ActiveClimbable()
    {
        backCollision.SetActive(true);
        topCollision.SetActive(false);
    }

    public void DesactiveClimbable()
    {
        StopAllCoroutines();
        backCollision.SetActive(false);
        topCollision.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((playerMask & (1 << other.gameObject.layer)) != 0)
        {
            playerDetected = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((playerMask & (1 << other.gameObject.layer)) != 0)
        {
            playerDetected = false;
        }
    }

    private bool IsPlayerOnTop()
    {
        Collider[] cols = Physics.OverlapBox(topDetection.transform.position + topDetection.center, topDetection.size / 2, topDetection.transform.rotation, playerMask);
        return cols.Length != 0;
    }
}
