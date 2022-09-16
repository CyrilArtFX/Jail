using Jail.Utility;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ladder : MonoBehaviour
{

    [SerializeField, Range(1, 100)]
    int ladderLength = 1;


    [Header("Assignations")]
    [SerializeField]
    BoxCollider triggerToDetect = default;
    [SerializeField]
    BoxCollider backCollider = default;
    [SerializeField]
    GameObject ladderMeshPrefab = default;
    [SerializeField]
    Transform ladderMeshParent = default;


    [SerializeField]
    LayerMask playerMask = 0;

    private bool playerDetected = false;

    [SerializeField]
    GameObject backCollision = default;
    [SerializeField]
    GameObject topCollision = default;
    [SerializeField]
    BoxCollider topDetection = default;

    public void BuildLadder()
    {
        List<Transform> children = ladderMeshParent.Cast<Transform>().ToList();

        int i;
        for (i = 0; i < ladderLength; i++)
        {
            Transform mesh;
            if (i > children.Count - 1)
            {
                mesh = Instantiate(ladderMeshPrefab, ladderMeshParent).transform;
                children.Add(mesh);
            }
            else
            {
                mesh = children[i];
            }

            mesh.localPosition = new Vector3(0.5f, -i, 0.0f);
        }

        //  remove last items
        for (int j = i; j < children.Count; j++)
        {
            Transform child = children[j];
            #if UNITY_EDITOR
                DestroyImmediate(child.gameObject);
            #else
                Destroy(child.gameObject);
            #endif
        }

        //  update collider
        triggerToDetect.center = new Vector3(0.0f, -((float) (ladderLength) / 2) + 0.5f + 0.05f, 0.0f);
        triggerToDetect.size = new Vector3(1.0f, (float) (ladderLength) + 0.1f, 0.5f);
        backCollider.center = new Vector3(0.75f, -((float) (ladderLength) / 2) + 0.5f, 0.0f);
        backCollider.size = new Vector3(0.5f, (float) (ladderLength), 1.0f);
    }

    private void Awake()
    {
        backCollision.SetActive(false);
        topCollision.SetActive(true);
        BuildLadder();
    }

    private void Update()
    {
        if (playerDetected)
        {
            if (Mathf.Abs(Input.GetAxis("UpDown")) > 0.2f)
            {
                if (Input.GetAxis("UpDown") > 0.2f)
                {
                    if (IsPlayerOnTop()) return;
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
        if (LayerMaskUtils.HasLayer(playerMask, other.gameObject.layer))
        {
            playerDetected = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (LayerMaskUtils.HasLayer(playerMask, other.gameObject.layer))
        {
            playerDetected = false;
        }
    }

    private bool IsPlayerOnTop()
    {
        Collider[] cols = Physics.OverlapBox(
            topDetection.transform.position + topDetection.center,
            topDetection.size / 2,
            topDetection.transform.rotation,
            playerMask
        );
        return cols.Length != 0;
    }
}
