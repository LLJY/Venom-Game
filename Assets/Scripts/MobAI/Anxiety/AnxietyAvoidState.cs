using System.Threading.Tasks;
using MobAI.Anxiety;
using UnityEngine;

namespace MobAI
{
    public class AnxietyAvoidState: State<AnxietyNpc>
    {
        private int _wallLayerMask = 1 << 3;
        public AnxietyAvoidState(AnxietyNpc behaviour) : base(behaviour)
        {
        }

        public override void Prepare()
        {
            Debug.Log("Anxiety Avoid State");
        }

        public override void CleanUp()
        {
        }

        public override void Update()
        {
        }

        public override void LateUpdate()
        {
        }

        public override void FixedUpdate()
        {
            /*
             * In the event that the character is heading towards a wall, try to avoid it by incrementing
             * the direction vector by 5 degrees and raycasting again.
             */
            var position = _behaviour.transform.position;
            var direction = position - _behaviour.playerTransform.position;
            var ray = new Ray(position, direction);
            for (int i = 0; Physics.Raycast(ray, 5, _wallLayerMask) && i < 72; i++)
            {
                direction = Quaternion.Euler(0, 5 * i, 0) * direction;
                ray = new Ray(position, direction);
            }
            _behaviour.agent.SetDestination(position + direction);
            
        }
    }
}