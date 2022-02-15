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
        public Transform topWall;
        public Transform leftWall;
        public Transform player;
        private void Awake()
        {
            // generate a new seed
            RandomSeedProvider.NextSeed();
            
            // set world size randomly
            var scale = new Vector2(Random.Range(8, 30), Random.Range(8, 30));
            
            // scale the floor texture and world
            ground.transform.localScale = new Vector3(scale.x, 10, scale.y);
            var textureScale =  scale * 5f;
            ground.GetComponent<Renderer>().material.SetTextureScale("_BaseColorMap",textureScale);
            navMesh.BuildNavMesh();
        }

        private void Start()
        {
            // spawn the player near the assumed door position
            player.position = new Vector3(topWall.position.x + 10, 0, leftWall.position.z + 4);
        }
    }
}