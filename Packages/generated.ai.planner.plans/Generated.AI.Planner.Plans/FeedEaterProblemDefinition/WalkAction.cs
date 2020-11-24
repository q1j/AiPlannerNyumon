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
    struct WalkAction : IJobParallelForDefer
    {
        public Guid ActionGuid;
        
        const int k_EaterIndex = 0;
        const int k_UnmovableTargetIndex = 1;
        const int k_MaxArguments = 2;

        public static readonly string[] parameterNames = {
            "Eater",
            "UnmovableTarget",
        };

        [ReadOnly] NativeArray<StateEntityKey> m_StatesToExpand;
        StateDataContext m_StateDataContext;

        // local allocations
        [NativeDisableContainerSafetyRestriction] NativeArray<ComponentType> EaterFilter;
        [NativeDisableContainerSafetyRestriction] NativeList<int> EaterObjectIndices;
        [NativeDisableContainerSafetyRestriction] NativeArray<ComponentType> UnmovableTargetFilter;
        [NativeDisableContainerSafetyRestriction] NativeList<int> UnmovableTargetObjectIndices;

        [NativeDisableContainerSafetyRestriction] NativeList<ActionKey> ArgumentPermutations;
        [NativeDisableContainerSafetyRestriction] NativeList<WalkActionFixupReference> TransitionInfo;

        bool LocalContainersInitialized => ArgumentPermutations.IsCreated;

        internal WalkAction(Guid guid, NativeList<StateEntityKey> statesToExpand, StateDataContext stateDataContext)
        {
            ActionGuid = guid;
            m_StatesToExpand = statesToExpand.AsDeferredJobArray();
            m_StateDataContext = stateDataContext;
            EaterFilter = default;
            EaterObjectIndices = default;
            UnmovableTargetFilter = default;
            UnmovableTargetObjectIndices = default;
            ArgumentPermutations = default;
            TransitionInfo = default;
        }

        void InitializeLocalContainers()
        {
            EaterFilter = new NativeArray<ComponentType>(3, Allocator.Temp){[0] = ComponentType.ReadWrite<EaterTrait>(),[1] = ComponentType.ReadWrite<Location>(),[2] = ComponentType.ReadWrite<Moveable>(),  };
            EaterObjectIndices = new NativeList<int>(2, Allocator.Temp);
            UnmovableTargetFilter = new NativeArray<ComponentType>(2, Allocator.Temp){[0] = ComponentType.ReadWrite<Location>(), [1] = ComponentType.Exclude<Moveable>(),  };
            UnmovableTargetObjectIndices = new NativeList<int>(2, Allocator.Temp);

            ArgumentPermutations = new NativeList<ActionKey>(4, Allocator.Temp);
            TransitionInfo = new NativeList<WalkActionFixupReference>(ArgumentPermutations.Length, Allocator.Temp);
        }

        public static int GetIndexForParameterName(string parameterName)
        {
            
            if (string.Equals(parameterName, "Eater", StringComparison.OrdinalIgnoreCase))
                 return k_EaterIndex;
            if (string.Equals(parameterName, "UnmovableTarget", StringComparison.OrdinalIgnoreCase))
                 return k_UnmovableTargetIndex;

            return -1;
        }

        void GenerateArgumentPermutations(StateData stateData, NativeList<ActionKey> argumentPermutations)
        {
            EaterObjectIndices.Clear();
            stateData.GetTraitBasedObjectIndices(EaterObjectIndices, EaterFilter);
            
            UnmovableTargetObjectIndices.Clear();
            stateData.GetTraitBasedObjectIndices(UnmovableTargetObjectIndices, UnmovableTargetFilter);
            
            var LocationBuffer = stateData.LocationBuffer;
            
            

            for (int i0 = 0; i0 < EaterObjectIndices.Length; i0++)
            {
                var EaterIndex = EaterObjectIndices[i0];
                var EaterObject = stateData.TraitBasedObjects[EaterIndex];
                
                
                
            
            

            for (int i1 = 0; i1 < UnmovableTargetObjectIndices.Length; i1++)
            {
                var UnmovableTargetIndex = UnmovableTargetObjectIndices[i1];
                var UnmovableTargetObject = stateData.TraitBasedObjects[UnmovableTargetIndex];
                
                if (!(LocationBuffer[EaterObject.LocationIndex].Position != LocationBuffer[UnmovableTargetObject.LocationIndex].Position))
                    continue;
                
                

                var actionKey = new ActionKey(k_MaxArguments) {
                                                        ActionGuid = ActionGuid,
                                                       [k_EaterIndex] = EaterIndex,
                                                       [k_UnmovableTargetIndex] = UnmovableTargetIndex,
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
            var originalUnmovableTargetObject = originalStateObjectBuffer[action[k_UnmovableTargetIndex]];

            var newState = m_StateDataContext.CopyStateData(originalState);
            var newLocationBuffer = newState.LocationBuffer;
            {
                    var @Location = newLocationBuffer[originalEaterObject.LocationIndex];
                    @Location.Position = newLocationBuffer[originalUnmovableTargetObject.LocationIndex].Position;
                    newLocationBuffer[originalEaterObject.LocationIndex] = @Location;
            }

            

            var reward = Reward(originalState, action, newState);
            var StateTransitionInfo = new StateTransitionInfo { Probability = 1f, TransitionUtilityValue = reward };
            var resultingStateKey = m_StateDataContext.GetStateDataKey(newState);

            return new StateTransitionInfoPair<StateEntityKey, ActionKey, StateTransitionInfo>(originalStateEntityKey, action, resultingStateKey, StateTransitionInfo);
        }

        float Reward(StateData originalState, ActionKey action, StateData newState)
        {
            var reward = 0f;
            {
                var param0 = originalState.GetTraitOnObjectAtIndex<Unity.AI.Planner.Traits.Location>(action[0]);
                var param1 = originalState.GetTraitOnObjectAtIndex<Unity.AI.Planner.Traits.Location>(action[1]);
                reward -= new global::Unity.AI.Planner.Navigation.LocationDistance().RewardModifier( param0, param1);
            }

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
                TransitionInfo.Add(new WalkActionFixupReference { TransitionInfo = ApplyEffects(ArgumentPermutations[i], stateEntityKey) });
            }

            // fixups
            var stateEntity = stateEntityKey.Entity;
            var fixupBuffer = m_StateDataContext.EntityCommandBuffer.AddBuffer<WalkActionFixupReference>(jobIndex, stateEntity);
            fixupBuffer.CopyFrom(TransitionInfo);
        }

        
        public static T GetEaterTrait<T>(StateData state, ActionKey action) where T : struct, ITrait
        {
            return state.GetTraitOnObjectAtIndex<T>(action[k_EaterIndex]);
        }
        
        public static T GetUnmovableTargetTrait<T>(StateData state, ActionKey action) where T : struct, ITrait
        {
            return state.GetTraitOnObjectAtIndex<T>(action[k_UnmovableTargetIndex]);
        }
        
    }

    public struct WalkActionFixupReference : IBufferElementData
    {
        internal StateTransitionInfoPair<StateEntityKey, ActionKey, StateTransitionInfo> TransitionInfo;
    }
}


