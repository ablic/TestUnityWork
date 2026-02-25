using AxGrid.Base;
using AxGrid.Model;
using UnityEngine;

namespace TestTaskSolution
{
    public class CoinEmitter : MonoBehaviourExtBind
    {
        [SerializeField] private ParticleSystem partSystem;

        [Bind("EmitCoins")]
        private void StartEmit()
        {
            partSystem.Play();
        }
    }
}
