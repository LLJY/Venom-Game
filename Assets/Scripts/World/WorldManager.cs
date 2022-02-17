using System;
using System.Collections;
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
using UnityEngine.Video;
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
        public Transform bottomWall;
        public Transform rightWall;
        public Transform player;
        public TurretManagedInstanceGroup turretGroup;
        public TeslaCoilManagedInstanceGroup teslaCoilGroup;
        public PressurePlateManagedInstanceGroup pressurePlateGroup;
        public WorldType worldType;
        public float obstacleRadius = 8;
        public Material act1Material;
        public Material act2Material;
        public Material act3Material;
        public Vector2 obstacleMargin;
        
        private void Awake()
        {
            GenerateWorld();
            navMesh.BuildNavMesh();
        }

        private void Start()
        {
            // do this after everything has been done
            MovePlayerToStartPosition();
        }

        /// <summary>
        /// Moves the player to the starting position on the map
        /// </summary>
        private void MovePlayerToStartPosition()
        {
            /*
             * Disable cc to prevent it from interfering with position
             * modify position, then enable cc again
             */
            var cc = player.GetComponent<CharacterController>();
            cc.enabled = false;
            var playerPos = new Vector3(topWall.position.x +10, 0, leftWall.position.z +10);
            player.position = playerPos;
            cc.enabled = true;
        }

        /// <summary>
        /// Generates the world with RNG, scaling, spawning obstacles and TODO spawning mobs
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void GenerateWorld()
        {
            var groundRenderer = ground.GetComponent<Renderer>();
            // set world size randomly
            worldType = (WorldType) Random.Range(0, 3);
            var scale = new Vector2(Random.Range(8, 30), Random.Range(8, 30));

            // scale the floor texture and world
            worldScale = new Vector3(scale.x, 10, scale.y);
            ground.transform.localScale = worldScale;

            // then adjust the walls
            void AdjustWallsAfterScaling(Transform wall)
            {
                wall.parent = null;
                wall.localScale = new Vector3(2f, wall.localScale.y, wall.localScale.z);
            }
            AdjustWallsAfterScaling(topWall);
            AdjustWallsAfterScaling(bottomWall);
            AdjustWallsAfterScaling(leftWall);
            AdjustWallsAfterScaling(rightWall);
            groundRenderer.material = worldType switch
            {
                WorldType.Act1 => act1Material,
                WorldType.Act2 => act2Material,
                WorldType.Act3 => act3Material,
                _ => throw new ArgumentOutOfRangeException()
            };
            var textureScale = scale * 4f;
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

        /// <summary>
        /// Calculates the density of obstacles using the radius set in the inspector
        /// Usable area of circles in a rectangle is pi/2*root(3)
        /// </summary>
        /// <returns></returns>
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
                // make sure it does not loop more than 10 (arbitrary limit) times, or it will stall the editor and/or game.
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
        
        /// <summary>
        /// Resets the game scene and restarts it with the previous seed
        /// </summary>
        public static void PreviousWorld()
        {
            RandomSeedProvider.PreviousSeed();
            SceneManager.LoadScene("StupidGameScene");
        }
        /// <summary>
        /// Resets the game scene and restarts it with the next seed
        /// </summary>
        public static void NextWorld()
        {
            RandomSeedProvider.NextSeed();
            SceneManager.LoadScene("StupidGameScene");
        }
    }
}