using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using Jail.Utility;

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

    bool playerDetected = false;

    [SerializeField]
    GameObject backCollision = default;
    [SerializeField]
    GameObject topCollision = default;
    [SerializeField]
    BoxCollider topDetection = default, headDetection;

    [MethodButton("Build Ladder")]
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
        triggerToDetect.center = new Vector3(triggerToDetect.center.x, -((float)(ladderLength) / 2) + 0.5f + 0.05f, triggerToDetect.center.z);
        triggerToDetect.size = new Vector3(triggerToDetect.size.x, ladderLength + 0.1f, triggerToDetect.size.z);
        backCollider.center = new Vector3(backCollider.center.x, -((float)ladderLength / 2) + 0.5f, backCollider.center.z);
        backCollider.size = new Vector3(backCollider.size.x, ladderLength, backCollider.size.z);
    }

    void Awake()
    {
        backCollision.SetActive(false);
        topCollision.SetActive(true);
        BuildLadder();
    }

    void Update()
    {
        if (!playerDetected) return;

        if (IsHeadPlayerDetected() && Input.GetAxis("UpDown") >= -0.2f)
        {
            DesactiveClimbable();
        }
        else
        {
            ActiveClimbable();
        }
    }

    void ActiveClimbable()
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

    void OnTriggerEnter(Collider other)
    {
        if (LayerMaskUtils.HasLayer(playerMask, other.gameObject.layer))
        {
            playerDetected = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (LayerMaskUtils.HasLayer(playerMask, other.gameObject.layer))
        {
            playerDetected = false;
            DesactiveClimbable();
        }
    }

    bool IsPlayerOnTop()
    {
        Collider[] cols = Physics.OverlapBox(
            topDetection.transform.position + topDetection.center,
            topDetection.size / 2,
            topDetection.transform.rotation,
            playerMask
        );
        return cols.Length != 0;
    }

    bool IsHeadPlayerDetected()
    {
        Collider[] cols = Physics.OverlapBox(
            headDetection.transform.position + headDetection.center,
            headDetection.size / 2,
            headDetection.transform.rotation,
            playerMask
        );
        return cols.Length != 0;
    }
}
