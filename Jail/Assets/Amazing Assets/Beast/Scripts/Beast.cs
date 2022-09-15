using UnityEngine;
using System.Collections.Generic;

namespace AmazingAssets.Beast
{
    static public class Beast
    {
        public static Mesh GenerateSmoothNormals(this Mesh sourceMesh)
        {
            if (sourceMesh == null)
            {
                Debug.LogError("GenerateSmoothNormals: 'sourceMesh' mesh is NULL.\n");
                return null;
            }
            if (sourceMesh.normals == null || sourceMesh.normals.Length != sourceMesh.vertexCount)
            {
                Debug.LogError("GenerateSmoothNormals: 'sourceMesh' mesh has no normals.\n");
                return null;
            }


            Mesh newMesh = UnityEngine.Object.Instantiate(sourceMesh);
            newMesh.name = newMesh.name.Replace("(Clone)", string.Empty);
            newMesh.name += " (Smooth Normals)";


            Dictionary<Vector3, Vector3> smoothNormalsHash = new Dictionary<Vector3, Vector3>();
            for (int i = 0; i < newMesh.vertexCount; i++)
            {
                Vector3 key = newMesh.vertices[i];

                if (smoothNormalsHash.ContainsKey(key))
                {
                    smoothNormalsHash[key] = (smoothNormalsHash[key] + newMesh.normals[i]).normalized;
                }
                else
                {
                    smoothNormalsHash.Add(key, newMesh.normals[i]);
                }
            }


            List<Vector3> smoothNormals = new List<Vector3>(sourceMesh.normals);
            for (int i = 0; i < newMesh.vertexCount; i++)
            {
                smoothNormals[i] = smoothNormalsHash[newMesh.vertices[i]];
            }

            newMesh.SetUVs(3, smoothNormals);

            return newMesh;
        }
    }
}