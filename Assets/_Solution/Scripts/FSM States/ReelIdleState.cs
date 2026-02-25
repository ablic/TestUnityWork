using AxGrid;
using AxGrid.FSM;

namespace TestTaskSolution
{
    [State("ReelIdle")]
    public class ReelIdleState : FSMState
    {
        [Enter]
        private void Enter()
        {
            Log.Info("IDLE reel state entered");

            Model.EventManager.AddAction("OnStartButtonClick", OnStartButtonClicked);

            Model.Set("canStart", true);
            Model.Set("canStop", false);
        }

        [Exit]
        private void Exit()
        {
            Model.Set("canStart", false);
        }

        private void OnStartButtonClicked()
        {
            Parent.Change("ReelSpinning");
        }
    }
}
