using System.Collections;
using UnityEngine;

namespace Jail.Interactables.ZapTurret
{
    public class ZapTurretProjectile : MonoBehaviour
    {
        public ZapTurretChainer Chainer { get; set; }
        public Transform Target { get; set; }
        public Transform TurretAnchor { get; set; }
        public bool IsReturning => isReturning;

        Vector3 target;

        [SerializeField]
        float moveSpeed = 25.0f, returningSpeed = 15.0f;
        [SerializeField]
        float timeBeforeReturn = 1.0f;
        
        bool isReturning = false, isPaused = false;

        Coroutine returnCoroutine;

        public void SetPause(bool pause)
        {
            isPaused = pause;
        }
        
        public void ReturnToTurret()
        {
            isReturning = true;
            returnCoroutine = StartCoroutine(CoroutinePauseForTime(timeBeforeReturn));
        }

        IEnumerator CoroutinePauseForTime(float time)
        {
            isPaused = true;

            yield return new WaitForSeconds(time);

            isPaused = false;
        }

        public void Chase(Transform target)
        {
            if (returnCoroutine != null)
                StopCoroutine(returnCoroutine);

            Target = target;
            isReturning = false;
            isPaused = false;
        }

        void UpdateReturningMovement()
        {
            /*var nodes = Chainer.Nodes;
            int node_id = nodes.Count == 2 ? 1 : nodes.Count - 2;
            SplineNode node = nodes[node_id];
            print("moving to node " + node_id + "/" + (nodes.Count - 1));
            Vector3 node_pos = Chainer.transform.TransformPoint(node.Position);

            //float d = Mathf.Min(1.0f, Chainer.Spline.Length);
            //CurveSample sample = Chainer.Spline.GetSampleAtDistance(d);
            Vector3 target = node_pos;//Chainer.transform.TransformPoint(sample.location);

            //  move towards target
            transform.position = Vector3.MoveTowards(transform.position, target, Time.fixedDeltaTime * returningSpeed);
        
            //  delete node
            if ((transform.position - node_pos).sqrMagnitude <= 0)
            {
                //  last node (e.g. turret's anchor), it's the end
                if (nodes.Count == 2)
                {
                    isPaused = true;
                    //Destroy(gameObject);
                }
                //  remove node
                else
                {
                    Chainer.RemoveNodeAt(node_id);
                }
            }*/

            //Vector3 direction = Chainer.Spline.transform.TransformDirection(Chainer.Spline.GetDirection(0.0f));
            //transform.position += direction * Time.fixedDeltaTime * returningSpeed;

            //  move towards target
            Chainer.SplineChainer.Ratio -= Time.fixedDeltaTime * returningSpeed;
            target = Chainer.SplineChainer.Spline.GetPoint(Chainer.SplineChainer.Ratio);
            transform.position = target;
            //transform.position = Vector3.MoveTowards(transform.position, target, Time.fixedDeltaTime * returningSpeed);
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, target);
            Gizmos.DrawWireSphere(target, .5f);
        }

        void UpdateChase()
        {
            transform.position = Vector3.MoveTowards(transform.position, Target.position, Time.fixedDeltaTime * moveSpeed);
            //transform.LookAt(Target);
        }

        void FixedUpdate()
        {
            if (isPaused) return;

            if (isReturning)
            {
                UpdateReturningMovement();
            }
            else
            {
                UpdateChase();
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject != Player.instance.Spirit) return;

            Player.instance.GoBackToNormalForm();
            ReturnToTurret();
        }
    }
}