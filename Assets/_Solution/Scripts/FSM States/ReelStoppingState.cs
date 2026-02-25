using AxGrid;
using AxGrid.FSM;
using AxGrid.Model;

namespace TestTaskSolution
{
    [State("ReelStopping")]
    public class ReelStoppingState : FSMState
    {
        [Enter]
        private void Enter()
        {
            Log.Info("STOPPING reel state entered");
            Settings.Invoke("StopSpin");
            Model.Set("canStop", false);
        } 

        [Bind("ReelStopped")]
        private void OnReelStopped()
        {
            Parent.Change("ReelAlign");
        }
    }
}
