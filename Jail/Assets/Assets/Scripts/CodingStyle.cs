using System;
using UnityEditor;
using UnityEngine;

namespace Jail
{
    public class CodingStyle
    {
        [SerializeField] 
        bool isPrivate;
        bool IsPublic => !isPrivate;

        void Awake()
        {
            bool local_var = false;
            if (!local_var) return;
            
            if (local_var)
            {
                Console.WriteLine("hey");
            }
            else
            {
                //  comment
            }
        }

        public void DoThing()
        {
            //  code
            Component comp = default;
            if (comp != null) return;

            /*bool has_hit = Physics.Raycast(
                body.position,
                -Vector3.up,
                out RaycastHit hit,
                probeDistance,
                probeMask,
                QueryTriggerInteraction.Ignore
            );*/
        }
    }
}