using AxGrid;
using AxGrid.Base;
using AxGrid.FSM;
using UnityEngine;

namespace TestTaskSolution
{
    public class FSMInitializator : MonoBehaviourExt
    {
        [OnAwake]
        private void Init()
        {
            Settings.Fsm = new FSM();
            
            Settings.Fsm.Add(new ReelIdleState());
            Settings.Fsm.Add(new ReelSpinningState());
            Settings.Fsm.Add(new ReelStoppingState());
            Settings.Fsm.Add(new ReelAlignState());
        }

        [OnStart]
        private void StartFsm()
        {
            Settings.Fsm.Start("ReelIdle");
        }

        [OnUpdate]
        private void UpdateFsm()
        {
            Settings.Fsm.Update(Time.deltaTime);
        }
    }
}
