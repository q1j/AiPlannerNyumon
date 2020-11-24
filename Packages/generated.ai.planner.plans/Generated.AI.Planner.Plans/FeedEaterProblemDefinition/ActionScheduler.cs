using System;
using Unity.AI.Planner;
using Unity.AI.Planner.Traits;
using Unity.AI.Planner.Jobs;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Generated.AI.Planner.StateRepresentation;
using Generated.AI.Planner.StateRepresentation.FeedEaterProblemDefinition;

namespace Generated.AI.Planner.Plans.FeedEaterProblemDefinition
{
    public struct ActionScheduler :
        ITraitBasedActionScheduler<TraitBasedObject, StateEntityKey, StateData, StateDataContext, StateManager, ActionKey>
    {
        public static readonly Guid FeedEatActionGuid = Guid.NewGuid();
        public static readonly Guid GoalEatActionGuid = Guid.NewGuid();
        public static readonly Guid WalkActionGuid = Guid.NewGuid();

        // Input
        public NativeList<StateEntityKey> UnexpandedStates { get; set; }
        public StateManager StateManager { get; set; }

        // Output
        NativeQueue<StateTransitionInfoPair<StateEntityKey, ActionKey, StateTransitionInfo>> IActionScheduler<StateEntityKey, StateData, StateDataContext, StateManager, ActionKey>.CreatedStateInfo
        {
            set => m_CreatedStateInfo = value;
        }

        NativeQueue<StateTransitionInfoPair<StateEntityKey, ActionKey, StateTransitionInfo>> m_CreatedStateInfo;

        struct PlaybackECB : IJob
        {
            public ExclusiveEntityTransaction ExclusiveEntityTransaction;

            [ReadOnly]
            public NativeList<StateEntityKey> UnexpandedStates;
            public NativeQueue<StateTransitionInfoPair<StateEntityKey, ActionKey, StateTransitionInfo>> CreatedStateInfo;
            public EntityCommandBuffer FeedEatActionECB;
            public EntityCommandBuffer GoalEatActionECB;
            public EntityCommandBuffer WalkActionECB;

            public void Execute()
            {
                // Playback entity changes and output state transition info
                var entityManager = ExclusiveEntityTransaction;

                FeedEatActionECB.Playback(entityManager);
                for (int i = 0; i < UnexpandedStates.Length; i++)
                {
                    var stateEntity = UnexpandedStates[i].Entity;
                    var FeedEatActionRefs = entityManager.GetBuffer<FeedEatActionFixupReference>(stateEntity);
                    for (int j = 0; j < FeedEatActionRefs.Length; j++)
                        CreatedStateInfo.Enqueue(FeedEatActionRefs[j].TransitionInfo);
                    entityManager.RemoveComponent(stateEntity, typeof(FeedEatActionFixupReference));
                }

                GoalEatActionECB.Playback(entityManager);
                for (int i = 0; i < UnexpandedStates.Length; i++)
                {
                    var stateEntity = UnexpandedStates[i].Entity;
                    var GoalEatActionRefs = entityManager.GetBuffer<GoalEatActionFixupReference>(stateEntity);
                    for (int j = 0; j < GoalEatActionRefs.Length; j++)
                        CreatedStateInfo.Enqueue(GoalEatActionRefs[j].TransitionInfo);
                    entityManager.RemoveComponent(stateEntity, typeof(GoalEatActionFixupReference));
                }

                WalkActionECB.Playback(entityManager);
                for (int i = 0; i < UnexpandedStates.Length; i++)
                {
                    var stateEntity = UnexpandedStates[i].Entity;
                    var WalkActionRefs = entityManager.GetBuffer<WalkActionFixupReference>(stateEntity);
                    for (int j = 0; j < WalkActionRefs.Length; j++)
                        CreatedStateInfo.Enqueue(WalkActionRefs[j].TransitionInfo);
                    entityManager.RemoveComponent(stateEntity, typeof(WalkActionFixupReference));
                }
            }
        }

        public JobHandle Schedule(JobHandle inputDeps)
        {
            var entityManager = StateManager.ExclusiveEntityTransaction.EntityManager;
            var FeedEatActionDataContext = StateManager.StateDataContext;
            var FeedEatActionECB = StateManager.GetEntityCommandBuffer();
            FeedEatActionDataContext.EntityCommandBuffer = FeedEatActionECB.AsParallelWriter();
            var GoalEatActionDataContext = StateManager.StateDataContext;
            var GoalEatActionECB = StateManager.GetEntityCommandBuffer();
            GoalEatActionDataContext.EntityCommandBuffer = GoalEatActionECB.AsParallelWriter();
            var WalkActionDataContext = StateManager.StateDataContext;
            var WalkActionECB = StateManager.GetEntityCommandBuffer();
            WalkActionDataContext.EntityCommandBuffer = WalkActionECB.AsParallelWriter();

            var allActionJobs = new NativeArray<JobHandle>(4, Allocator.TempJob)
            {
                [0] = new FeedEatAction(FeedEatActionGuid, UnexpandedStates, FeedEatActionDataContext).Schedule(UnexpandedStates, 0, inputDeps),
                [1] = new GoalEatAction(GoalEatActionGuid, UnexpandedStates, GoalEatActionDataContext).Schedule(UnexpandedStates, 0, inputDeps),
                [2] = new WalkAction(WalkActionGuid, UnexpandedStates, WalkActionDataContext).Schedule(UnexpandedStates, 0, inputDeps),
                [3] = entityManager.ExclusiveEntityTransactionDependency
            };

            var allActionJobsHandle = JobHandle.CombineDependencies(allActionJobs);
            allActionJobs.Dispose();

            // Playback entity changes and output state transition info
            var playbackJob = new PlaybackECB()
            {
                ExclusiveEntityTransaction = StateManager.ExclusiveEntityTransaction,
                UnexpandedStates = UnexpandedStates,
                CreatedStateInfo = m_CreatedStateInfo,
                FeedEatActionECB = FeedEatActionECB,
                GoalEatActionECB = GoalEatActionECB,
                WalkActionECB = WalkActionECB,
            };

            var playbackJobHandle = playbackJob.Schedule(allActionJobsHandle);
            entityManager.ExclusiveEntityTransactionDependency = playbackJobHandle;

            return playbackJobHandle;
        }
    }
}
