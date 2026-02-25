using AxGrid;
using AxGrid.FSM;
using AxGrid.Model;

namespace TestTaskSolution
{
    [State("ReelAlign")]
    public class ReelAlignState : FSMState
    {
        [Enter]
        private void Enter()
        {
            Log.Info("ALIGN reel state entered");
            Settings.Invoke("Align");
        }

        [Bind("ReelAligned")]
        private void OnReelAligned()
        {
            Settings.Invoke("EmitCoins");
            Parent.Change("ReelIdle");
        }
    }
}
