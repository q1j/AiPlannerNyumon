using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.AI.Planner;
using Unity.AI.Planner.Traits;
using Unity.Burst;
using Generated.AI.Planner.StateRepresentation;
using Generated.AI.Planner.StateRepresentation.FeedEaterProblemDefinition;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace Generated.AI.Planner.Plans.FeedEaterProblemDefinition
{
    [BurstCompile]
    struct GoalEatAction : IJobParallelForDefer
    {
        public Guid ActionGuid;
        
        const int k_EaterIndex = 0;
        const int k_GoalIndex = 1;
        const int k_MaxArguments = 2;

        public static readonly string[] parameterNames = {
            "Eater",
            "Goal",
        };

        [ReadOnly] NativeArray<StateEntityKey> m_StatesToExpand;
        StateDataContext m_StateDataContext;

        // local allocations
        [NativeDisableContainerSafetyRestriction] NativeArray<ComponentType> EaterFilter;
        [NativeDisableContainerSafetyRestriction] NativeList<int> EaterObjectIndices;
        [NativeDisableContainerSafetyRestriction] NativeArray<ComponentType> GoalFilter;
        [NativeDisableContainerSafetyRestriction] NativeList<int> GoalObjectIndices;

        [NativeDisableContainerSafetyRestriction] NativeList<ActionKey> ArgumentPermutations;
        [NativeDisableContainerSafetyRestriction] NativeList<GoalEatActionFixupReference> TransitionInfo;

        bool LocalContainersInitialized => ArgumentPermutations.IsCreated;

        internal GoalEatAction(Guid guid, NativeList<StateEntityKey> statesToExpand, StateDataContext stateDataContext)
        {
            ActionGuid = guid;
            m_StatesToExpand = statesToExpand.AsDeferredJobArray();
            m_StateDataContext = stateDataContext;
            EaterFilter = default;
            EaterObjectIndices = default;
            GoalFilter = default;
            GoalObjectIndices = default;
            ArgumentPermutations = default;
            TransitionInfo = default;
        }

        void InitializeLocalContainers()
        {
            EaterFilter = new NativeArray<ComponentType>(3, Allocator.Temp){[0] = ComponentType.ReadWrite<EaterTrait>(),[1] = ComponentType.ReadWrite<Location>(),[2] = ComponentType.ReadWrite<Moveable>(),  };
            EaterObjectIndices = new NativeList<int>(2, Allocator.Temp);
            GoalFilter = new NativeArray<ComponentType>(2, Allocator.Temp){[0] = ComponentType.ReadWrite<GoalTrait>(),[1] = ComponentType.ReadWrite<Location>(),  };
            GoalObjectIndices = new NativeList<int>(2, Allocator.Temp);

            ArgumentPermutations = new NativeList<ActionKey>(4, Allocator.Temp);
            TransitionInfo = new NativeList<GoalEatActionFixupReference>(ArgumentPermutations.Length, Allocator.Temp);
        }

        public static int GetIndexForParameterName(string parameterName)
        {
            
            if (string.Equals(parameterName, "Eater", StringComparison.OrdinalIgnoreCase))
                 return k_EaterIndex;
            if (string.Equals(parameterName, "Goal", StringComparison.OrdinalIgnoreCase))
                 return k_GoalIndex;

            return -1;
        }

        void GenerateArgumentPermutations(StateData stateData, NativeList<ActionKey> argumentPermutations)
        {
            EaterObjectIndices.Clear();
            stateData.GetTraitBasedObjectIndices(EaterObjectIndices, EaterFilter);
            
            GoalObjectIndices.Clear();
            stateData.GetTraitBasedObjectIndices(GoalObjectIndices, GoalFilter);
            
            var LocationBuffer = stateData.LocationBuffer;
            var EaterTraitBuffer = stateData.EaterTraitBuffer;
            
            

            for (int i0 = 0; i0 < EaterObjectIndices.Length; i0++)
            {
                var EaterIndex = EaterObjectIndices[i0];
                var EaterObject = stateData.TraitBasedObjects[EaterIndex];
                
                
                if (!(EaterTraitBuffer[EaterObject.EaterTraitIndex].EatCount == 2))
                    continue;
                
                
            
            

            for (int i1 = 0; i1 < GoalObjectIndices.Length; i1++)
            {
                var GoalIndex = GoalObjectIndices[i1];
                var GoalObject = stateData.TraitBasedObjects[GoalIndex];
                
                if (!(LocationBuffer[EaterObject.LocationIndex].Position == LocationBuffer[GoalObject.LocationIndex].Position))
                    continue;
                
                
                

                var actionKey = new ActionKey(k_MaxArguments) {
                                                        ActionGuid = ActionGuid,
                                                       [k_EaterIndex] = EaterIndex,
                                                       [k_GoalIndex] = GoalIndex,
                                                    };
                argumentPermutations.Add(actionKey);
            
            }
            
            }
        }

        StateTransitionInfoPair<StateEntityKey, ActionKey, StateTransitionInfo> ApplyEffects(ActionKey action, StateEntityKey originalStateEntityKey)
        {
            var originalState = m_StateDataContext.GetStateData(originalStateEntityKey);
            var originalStateObjectBuffer = originalState.TraitBasedObjects;
            var originalEaterObject = originalStateObjectBuffer[action[k_EaterIndex]];

            var newState = m_StateDataContext.CopyStateData(originalState);
            var newEaterTraitBuffer = newState.EaterTraitBuffer;
            {
                    var @EaterTrait = newEaterTraitBuffer[originalEaterObject.EaterTraitIndex];
                    @EaterTrait.@EatCount += 1;
                    newEaterTraitBuffer[originalEaterObject.EaterTraitIndex] = @EaterTrait;
            }

            
            newState.RemoveTraitBasedObjectAtIndex(action[k_GoalIndex]);

            var reward = Reward(originalState, action, newState);
            var StateTransitionInfo = new StateTransitionInfo { Probability = 1f, TransitionUtilityValue = reward };
            var resultingStateKey = m_StateDataContext.GetStateDataKey(newState);

            return new StateTransitionInfoPair<StateEntityKey, ActionKey, StateTransitionInfo>(originalStateEntityKey, action, resultingStateKey, StateTransitionInfo);
        }

        float Reward(StateData originalState, ActionKey action, StateData newState)
        {
            var reward = 10f;

            return reward;
        }

        public void Execute(int jobIndex)
        {
            if (!LocalContainersInitialized)
                InitializeLocalContainers();

            m_StateDataContext.JobIndex = jobIndex;

            var stateEntityKey = m_StatesToExpand[jobIndex];
            var stateData = m_StateDataContext.GetStateData(stateEntityKey);

            ArgumentPermutations.Clear();
            GenerateArgumentPermutations(stateData, ArgumentPermutations);

            TransitionInfo.Clear();
            TransitionInfo.Capacity = math.max(TransitionInfo.Capacity, ArgumentPermutations.Length);
            for (var i = 0; i < ArgumentPermutations.Length; i++)
            {
                TransitionInfo.Add(new GoalEatActionFixupReference { TransitionInfo = ApplyEffects(ArgumentPermutations[i], stateEntityKey) });
            }

            // fixups
            var stateEntity = stateEntityKey.Entity;
            var fixupBuffer = m_StateDataContext.EntityCommandBuffer.AddBuffer<GoalEatActionFixupReference>(jobIndex, stateEntity);
            fixupBuffer.CopyFrom(TransitionInfo);
        }

        
        public static T GetEaterTrait<T>(StateData state, ActionKey action) where T : struct, ITrait
        {
            return state.GetTraitOnObjectAtIndex<T>(action[k_EaterIndex]);
        }
        
        public static T GetGoalTrait<T>(StateData state, ActionKey action) where T : struct, ITrait
        {
            return state.GetTraitOnObjectAtIndex<T>(action[k_GoalIndex]);
        }
        
    }

    public struct GoalEatActionFixupReference : IBufferElementData
    {
        internal StateTransitionInfoPair<StateEntityKey, ActionKey, StateTransitionInfo> TransitionInfo;
    }
}


