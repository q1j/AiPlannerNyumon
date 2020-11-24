using Unity.AI.Planner;
using Unity.Collections;
using Unity.Entities;
using Unity.AI.Planner.Traits;
using Generated.AI.Planner.StateRepresentation;
using Generated.AI.Planner.StateRepresentation.FeedEaterProblemDefinition;

namespace Generated.AI.Planner.Plans.FeedEaterProblemDefinition
{
    public struct FoodEaterTermination
    {
        public bool IsTerminal(StateData stateData)
        {
            var EaterFilter = new NativeArray<ComponentType>(1, Allocator.Temp){[0] = ComponentType.ReadWrite<EaterTrait>(),  };
            var EaterObjectIndices = new NativeList<int>(2, Allocator.Temp);
            stateData.GetTraitBasedObjectIndices(EaterObjectIndices, EaterFilter);
            var EaterTraitBuffer = stateData.EaterTraitBuffer;
            for (int i0 = 0; i0 < EaterObjectIndices.Length; i0++)
            {
                var EaterIndex = EaterObjectIndices[i0];
                var EaterObject = stateData.TraitBasedObjects[EaterIndex];
            
                
                if (!(EaterTraitBuffer[EaterObject.EaterTraitIndex].EatCount == 3))
                    continue;
                EaterObjectIndices.Dispose();
                EaterFilter.Dispose();
                return true;
            }
            EaterObjectIndices.Dispose();
            EaterFilter.Dispose();

            return false;
        }

        public float TerminalReward(StateData stateData)
        {
            var reward = 10f;

            return reward;
        }
    }
}
