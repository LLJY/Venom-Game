using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Obstacles.PressurePlate;
using Obstacles.TeslaCoil;
using Obstacles.Turret;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace World
{
    public enum WorldType
    {
        Act1,
        Act2,
        Act3
    }

    public class WorldManager : MonoBehaviour
    {
        public NavMeshSurface navMesh;
        public Vector3 worldScale;
        public GameObject ground;
        public Transform topWall;
        public Transform leftWall;
        public Transform player;
        public TurretManagedInstanceGroup turretGroup;
        public TeslaCoilManagedInstanceGroup teslaCoilGroup;
        public PressurePlateManagedInstanceGroup pressurePlateGroup;
        public WorldType worldType;
        public float obstacleRadius = 8;

        public Material act1Material;
        public Material act2Material;
        public Material act3Material;

        private void Awake()
        {
            // generate a new seed
            RandomSeedProvider.NextSeed();

            GenerateWorld();
            navMesh.BuildNavMesh();
        }

        private void GenerateWorld()
        {
            worldType = (WorldType) Random.Range(0, 3);
            // set world size randomly
            var scale = new Vector2(Random.Range(8, 30), Random.Range(8, 30));

            // scale the floor texture and world
            worldScale = new Vector3(scale.x, 10, scale.y);
            ground.transform.localScale = worldScale;
            var textureScale = scale * 4f;
            var groundRenderer = ground.GetComponent<Renderer>();
            switch (worldType)
            {
                case WorldType.Act1:
                    groundRenderer.material = act1Material;
                    break;
                case WorldType.Act2:
                    groundRenderer.material = act2Material;
                    break;
                case WorldType.Act3:
                    groundRenderer.material = act3Material;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            groundRenderer.material.SetTextureScale("_BaseColorMap", textureScale);
            
            var obstaclePos = GenerateObstaclePositions();
            foreach (var pos in obstaclePos)
            {
                switch (worldType)
                {
                    case WorldType.Act1:
                        turretGroup.Instantiate(pos);
                        break;
                    case WorldType.Act2:
                        pressurePlateGroup.Instantiate(pos);
                        break;
                    case WorldType.Act3:
                        teslaCoilGroup.Instantiate(pos);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private List<Vector3> GenerateObstaclePositions()
        {
            var obstaclePos = new List<Vector3>();
            var randomChance = Random.Range(0, 1);
            var obstacleDensity = Mathf.Min(Mathf.Pow(1.5f * randomChance, 7) + 30, 1) / 100;
            var worldSize = ground.GetComponent<Renderer>().bounds.size;
            var worldArea = worldSize.x * worldSize.z * (Mathf.PI / (2 * Mathf.Pow(1f, 1f / 3f)));
            var obstacleArea = Mathf.PI * obstacleRadius * obstacleRadius;
            var numberOfObstaclesFit = worldArea / obstacleArea;
            Debug.Log($"Number of obstacles fit {numberOfObstaclesFit}");
            var numberOfObstacles = Mathf.FloorToInt(numberOfObstaclesFit * obstacleDensity);

            for (int i = 0; i < numberOfObstacles; i++)
            {
                Vector3 randomPos = default;
                for (int j = 0; j < 10; j++)
                {
                    randomPos = new Vector3(Random.Range(-worldSize.x / 2, worldSize.x / 2), 0,
                        Random.Range(-worldSize.z / 2, worldSize.z / 2));
                    if (obstaclePos.Count == 0 ||
                        !obstaclePos.Any(x => Vector3.Distance(x, randomPos) < obstacleRadius)) break;
                    randomPos = Vector3.zero;
                }

                if (randomPos != Vector3.zero)
                {
                    obstaclePos.Add(randomPos);
                }
            }

            return obstaclePos;
        }

        private void Start()
        {
            // spawn the player near the assumed door position
            var playerPos = new Vector3(topWall.position.x + 10, 0, leftWall.position.z + 10);
            player.position = playerPos;
        }
        
        public void PreviousWorld()
        {
            RandomSeedProvider.PreviousSeed();
            SceneManager.LoadScene("StupidGameScene");
        }

        public void NextWorld()
        {
            RandomSeedProvider.NextSeed();
            SceneManager.LoadScene("StupidGameScene");
        }
    }
}