using AxGrid;
using AxGrid.FSM;

namespace TestTaskSolution
{
    [State("ReelSpinning")]
    public class ReelSpinningState : FSMState
    {
        [Enter]
        private void Enter()
        {
            Log.Info("SPINNING reel state entered");
            Model.EventManager.AddAction("OnStopButtonClick", OnStopButtonClicked);
            Settings.Invoke("StartSpin");
        }

        [One(3f)]
        private void AllowStop()
        {
            Model.Set("canStop", true);
        }

        private void OnStopButtonClicked()
        {
            Parent.Change("ReelStopping");
        }
    }
}
