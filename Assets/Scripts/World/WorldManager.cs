using System;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace World
{
    public class WorldManager: MonoBehaviour
    {
        public NavMeshSurface navMesh;
        public Vector2 WorldScale;
        public GameObject ground;
        private void Start()
        {
            // generate a new seed
            RandomSeedProvider.NextSeed();
            
            // set world size randomly
            var scale = new Vector2(Random.Range(8, 30), Random.Range(8, 30));
            
            // scale the floor texture and world
            ground.transform.localScale = new Vector3(scale.x, 10, scale.y);
            var textureScale =  scale * 7.5f;
            ground.GetComponent<Renderer>().material.SetTextureScale("_Tex1",textureScale);
            navMesh.BuildNavMesh();
        }
    }
}