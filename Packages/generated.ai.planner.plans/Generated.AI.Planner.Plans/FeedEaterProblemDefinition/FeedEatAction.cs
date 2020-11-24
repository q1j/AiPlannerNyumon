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
    struct FeedEatAction : IJobParallelForDefer
    {
        public Guid ActionGuid;
        
        const int k_EaterIndex = 0;
        const int k_FeedIndex = 1;
        const int k_MaxArguments = 2;

        public static readonly string[] parameterNames = {
            "Eater",
            "Feed",
        };

        [ReadOnly] NativeArray<StateEntityKey> m_StatesToExpand;
        StateDataContext m_StateDataContext;

        // local allocations
        [NativeDisableContainerSafetyRestriction] NativeArray<ComponentType> EaterFilter;
        [NativeDisableContainerSafetyRestriction] NativeList<int> EaterObjectIndices;
        [NativeDisableContainerSafetyRestriction] NativeArray<ComponentType> FeedFilter;
        [NativeDisableContainerSafetyRestriction] NativeList<int> FeedObjectIndices;

        [NativeDisableContainerSafetyRestriction] NativeList<ActionKey> ArgumentPermutations;
        [NativeDisableContainerSafetyRestriction] NativeList<FeedEatActionFixupReference> TransitionInfo;

        bool LocalContainersInitialized => ArgumentPermutations.IsCreated;

        internal FeedEatAction(Guid guid, NativeList<StateEntityKey> statesToExpand, StateDataContext stateDataContext)
        {
            ActionGuid = guid;
            m_StatesToExpand = statesToExpand.AsDeferredJobArray();
            m_StateDataContext = stateDataContext;
            EaterFilter = default;
            EaterObjectIndices = default;
            FeedFilter = default;
            FeedObjectIndices = default;
            ArgumentPermutations = default;
            TransitionInfo = default;
        }

        void InitializeLocalContainers()
        {
            EaterFilter = new NativeArray<ComponentType>(3, Allocator.Temp){[0] = ComponentType.ReadWrite<EaterTrait>(),[1] = ComponentType.ReadWrite<Location>(),[2] = ComponentType.ReadWrite<Moveable>(),  };
            EaterObjectIndices = new NativeList<int>(2, Allocator.Temp);
            FeedFilter = new NativeArray<ComponentType>(2, Allocator.Temp){[0] = ComponentType.ReadWrite<FeedTrait>(),[1] = ComponentType.ReadWrite<Location>(),  };
            FeedObjectIndices = new NativeList<int>(2, Allocator.Temp);

            ArgumentPermutations = new NativeList<ActionKey>(4, Allocator.Temp);
            TransitionInfo = new NativeList<FeedEatActionFixupReference>(ArgumentPermutations.Length, Allocator.Temp);
        }

        public static int GetIndexForParameterName(string parameterName)
        {
            
            if (string.Equals(parameterName, "Eater", StringComparison.OrdinalIgnoreCase))
                 return k_EaterIndex;
            if (string.Equals(parameterName, "Feed", StringComparison.OrdinalIgnoreCase))
                 return k_FeedIndex;

            return -1;
        }

        void GenerateArgumentPermutations(StateData stateData, NativeList<ActionKey> argumentPermutations)
        {
            EaterObjectIndices.Clear();
            stateData.GetTraitBasedObjectIndices(EaterObjectIndices, EaterFilter);
            
            FeedObjectIndices.Clear();
            stateData.GetTraitBasedObjectIndices(FeedObjectIndices, FeedFilter);
            
            var LocationBuffer = stateData.LocationBuffer;
            
            

            for (int i0 = 0; i0 < EaterObjectIndices.Length; i0++)
            {
                var EaterIndex = EaterObjectIndices[i0];
                var EaterObject = stateData.TraitBasedObjects[EaterIndex];
                
                
                
            
            

            for (int i1 = 0; i1 < FeedObjectIndices.Length; i1++)
            {
                var FeedIndex = FeedObjectIndices[i1];
                var FeedObject = stateData.TraitBasedObjects[FeedIndex];
                
                if (!(LocationBuffer[EaterObject.LocationIndex].Position == LocationBuffer[FeedObject.LocationIndex].Position))
                    continue;
                
                

                var actionKey = new ActionKey(k_MaxArguments) {
                                                        ActionGuid = ActionGuid,
                                                       [k_EaterIndex] = EaterIndex,
                                                       [k_FeedIndex] = FeedIndex,
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

            
            newState.RemoveTraitBasedObjectAtIndex(action[k_FeedIndex]);

            var reward = Reward(originalState, action, newState);
            var StateTransitionInfo = new StateTransitionInfo { Probability = 1f, TransitionUtilityValue = reward };
            var resultingStateKey = m_StateDataContext.GetStateDataKey(newState);

            return new StateTransitionInfoPair<StateEntityKey, ActionKey, StateTransitionInfo>(originalStateEntityKey, action, resultingStateKey, StateTransitionInfo);
        }

        float Reward(StateData originalState, ActionKey action, StateData newState)
        {
            var reward = 1f;

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
                TransitionInfo.Add(new FeedEatActionFixupReference { TransitionInfo = ApplyEffects(ArgumentPermutations[i], stateEntityKey) });
            }

            // fixups
            var stateEntity = stateEntityKey.Entity;
            var fixupBuffer = m_StateDataContext.EntityCommandBuffer.AddBuffer<FeedEatActionFixupReference>(jobIndex, stateEntity);
            fixupBuffer.CopyFrom(TransitionInfo);
        }

        
        public static T GetEaterTrait<T>(StateData state, ActionKey action) where T : struct, ITrait
        {
            return state.GetTraitOnObjectAtIndex<T>(action[k_EaterIndex]);
        }
        
        public static T GetFeedTrait<T>(StateData state, ActionKey action) where T : struct, ITrait
        {
            return state.GetTraitOnObjectAtIndex<T>(action[k_FeedIndex]);
        }
        
    }

    public struct FeedEatActionFixupReference : IBufferElementData
    {
        internal StateTransitionInfoPair<StateEntityKey, ActionKey, StateTransitionInfo> TransitionInfo;
    }
}


