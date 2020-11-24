using System;
using System.Text;
using System.Collections.Generic;
using Unity.AI.Planner;
using Unity.AI.Planner.Traits;
using Unity.AI.Planner.Jobs;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using PlanningAgent = Unity.AI.Planner.Traits.PlanningAgent;

namespace Generated.AI.Planner.StateRepresentation.FeedEaterProblemDefinition
{
    // Plans don't share key types to enforce a specific state representation
    public struct StateEntityKey : IEquatable<StateEntityKey>, IStateKey
    {
        public Entity Entity;
        public int HashCode;

        public bool Equals(StateEntityKey other) => Entity == other.Entity;

        public bool Equals(IStateKey other) => other is StateEntityKey key && Equals(key);

        public override int GetHashCode() => HashCode;

        public override string ToString() => $"StateEntityKey ({Entity} {HashCode})";

        public string Label => $"State{Entity}";
    }

    public static class TraitArrayIndex<T> where T : struct, ITrait
    {
        public static readonly int Index = -1;

        static TraitArrayIndex()
        {
            var typeIndex = TypeManager.GetTypeIndex<T>();
            if (typeIndex == TypeManager.GetTypeIndex<EaterTrait>())
                Index = 0;
            else if (typeIndex == TypeManager.GetTypeIndex<Location>())
                Index = 1;
            else if (typeIndex == TypeManager.GetTypeIndex<Moveable>())
                Index = 2;
            else if (typeIndex == TypeManager.GetTypeIndex<FeedTrait>())
                Index = 3;
            else if (typeIndex == TypeManager.GetTypeIndex<GoalTrait>())
                Index = 4;
            else if (typeIndex == TypeManager.GetTypeIndex<PlanningAgent>())
                Index = 5;
        }
    }

    public struct TraitBasedObject : ITraitBasedObject
    {
        public int Length => 6;

        public byte this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0:
                        return EaterTraitIndex;
                    case 1:
                        return LocationIndex;
                    case 2:
                        return MoveableIndex;
                    case 3:
                        return FeedTraitIndex;
                    case 4:
                        return GoalTraitIndex;
                    case 5:
                        return PlanningAgentIndex;
                }

                return Unset;
            }
            set
            {
                switch (i)
                {
                    case 0:
                        EaterTraitIndex = value;
                        break;
                    case 1:
                        LocationIndex = value;
                        break;
                    case 2:
                        MoveableIndex = value;
                        break;
                    case 3:
                        FeedTraitIndex = value;
                        break;
                    case 4:
                        GoalTraitIndex = value;
                        break;
                    case 5:
                        PlanningAgentIndex = value;
                        break;
                }
            }
        }

        public static readonly byte Unset = Byte.MaxValue;

        public static TraitBasedObject Default => new TraitBasedObject
        {
            EaterTraitIndex = Unset,
            LocationIndex = Unset,
            MoveableIndex = Unset,
            FeedTraitIndex = Unset,
            GoalTraitIndex = Unset,
            PlanningAgentIndex = Unset,
        };


        public byte EaterTraitIndex;
        public byte LocationIndex;
        public byte MoveableIndex;
        public byte FeedTraitIndex;
        public byte GoalTraitIndex;
        public byte PlanningAgentIndex;


        static readonly int s_EaterTraitTypeIndex = TypeManager.GetTypeIndex<EaterTrait>();
        static readonly int s_LocationTypeIndex = TypeManager.GetTypeIndex<Location>();
        static readonly int s_MoveableTypeIndex = TypeManager.GetTypeIndex<Moveable>();
        static readonly int s_FeedTraitTypeIndex = TypeManager.GetTypeIndex<FeedTrait>();
        static readonly int s_GoalTraitTypeIndex = TypeManager.GetTypeIndex<GoalTrait>();
        static readonly int s_PlanningAgentTypeIndex = TypeManager.GetTypeIndex<PlanningAgent>();

        public bool HasSameTraits(TraitBasedObject other)
        {
            for (var i = 0; i < Length; i++)
            {
                var traitIndex = this[i];
                var otherTraitIndex = other[i];
                if (traitIndex == Unset && otherTraitIndex != Unset || traitIndex != Unset && otherTraitIndex == Unset)
                    return false;
            }
            return true;
        }

        public bool HasTraitSubset(TraitBasedObject traitSubset)
        {
            for (var i = 0; i < Length; i++)
            {
                var requiredTrait = traitSubset[i];
                if (requiredTrait != Unset && this[i] == Unset)
                    return false;
            }
            return true;
        }

        // todo - replace with more efficient subset check
        public bool MatchesTraitFilter(NativeArray<ComponentType> componentTypes)
        {
            for (int i = 0; i < componentTypes.Length; i++)
            {
                var t = componentTypes[i];
                if (t == default || t.TypeIndex == 0)
                {
                    // This seems to be necessary for Burst compilation; Doesn't happen with non-Burst compilation
                }
                else if (t.TypeIndex == s_EaterTraitTypeIndex)
                {
                    if (t.AccessModeType == ComponentType.AccessMode.Exclude ^ EaterTraitIndex == Unset)
                        return false;
                }
                else if (t.TypeIndex == s_LocationTypeIndex)
                {
                    if (t.AccessModeType == ComponentType.AccessMode.Exclude ^ LocationIndex == Unset)
                        return false;
                }
                else if (t.TypeIndex == s_MoveableTypeIndex)
                {
                    if (t.AccessModeType == ComponentType.AccessMode.Exclude ^ MoveableIndex == Unset)
                        return false;
                }
                else if (t.TypeIndex == s_FeedTraitTypeIndex)
                {
                    if (t.AccessModeType == ComponentType.AccessMode.Exclude ^ FeedTraitIndex == Unset)
                        return false;
                }
                else if (t.TypeIndex == s_GoalTraitTypeIndex)
                {
                    if (t.AccessModeType == ComponentType.AccessMode.Exclude ^ GoalTraitIndex == Unset)
                        return false;
                }
                else if (t.TypeIndex == s_PlanningAgentTypeIndex)
                {
                    if (t.AccessModeType == ComponentType.AccessMode.Exclude ^ PlanningAgentIndex == Unset)
                        return false;
                }
                else
                    throw new ArgumentException($"Incorrect trait type used in object query: {t}");
            }

            return true;
        }

        public bool MatchesTraitFilter(ComponentType[] componentTypes)
        {
            for (int i = 0; i < componentTypes.Length; i++)
            {
                var t = componentTypes[i];
                if (t == default || t == null || t.TypeIndex == 0)
                {
                    // This seems to be necessary for Burst compilation; Doesn't happen with non-Burst compilation
                }
                else if (t.TypeIndex == s_EaterTraitTypeIndex)
                {
                    if (t.AccessModeType == ComponentType.AccessMode.Exclude ^ EaterTraitIndex == Unset)
                        return false;
                }
                else if (t.TypeIndex == s_LocationTypeIndex)
                {
                    if (t.AccessModeType == ComponentType.AccessMode.Exclude ^ LocationIndex == Unset)
                        return false;
                }
                else if (t.TypeIndex == s_MoveableTypeIndex)
                {
                    if (t.AccessModeType == ComponentType.AccessMode.Exclude ^ MoveableIndex == Unset)
                        return false;
                }
                else if (t.TypeIndex == s_FeedTraitTypeIndex)
                {
                    if (t.AccessModeType == ComponentType.AccessMode.Exclude ^ FeedTraitIndex == Unset)
                        return false;
                }
                else if (t.TypeIndex == s_GoalTraitTypeIndex)
                {
                    if (t.AccessModeType == ComponentType.AccessMode.Exclude ^ GoalTraitIndex == Unset)
                        return false;
                }
                else if (t.TypeIndex == s_PlanningAgentTypeIndex)
                {
                    if (t.AccessModeType == ComponentType.AccessMode.Exclude ^ PlanningAgentIndex == Unset)
                        return false;
                }
                else
                    throw new ArgumentException($"Incorrect trait type used in object query: {t}");
            }

            return true;
        }
    }

    public struct StateData : ITraitBasedStateData<TraitBasedObject, StateData>
    {
        public Entity StateEntity;
        public DynamicBuffer<TraitBasedObject> TraitBasedObjects;
        public DynamicBuffer<TraitBasedObjectId> TraitBasedObjectIds;

        public DynamicBuffer<EaterTrait> EaterTraitBuffer;
        public DynamicBuffer<Location> LocationBuffer;
        public DynamicBuffer<Moveable> MoveableBuffer;
        public DynamicBuffer<FeedTrait> FeedTraitBuffer;
        public DynamicBuffer<GoalTrait> GoalTraitBuffer;
        public DynamicBuffer<PlanningAgent> PlanningAgentBuffer;

        static readonly int s_EaterTraitTypeIndex = TypeManager.GetTypeIndex<EaterTrait>();
        static readonly int s_LocationTypeIndex = TypeManager.GetTypeIndex<Location>();
        static readonly int s_MoveableTypeIndex = TypeManager.GetTypeIndex<Moveable>();
        static readonly int s_FeedTraitTypeIndex = TypeManager.GetTypeIndex<FeedTrait>();
        static readonly int s_GoalTraitTypeIndex = TypeManager.GetTypeIndex<GoalTrait>();
        static readonly int s_PlanningAgentTypeIndex = TypeManager.GetTypeIndex<PlanningAgent>();

        public StateData(ExclusiveEntityTransaction transaction, Entity stateEntity)
        {
            StateEntity = stateEntity;
            TraitBasedObjects = transaction.GetBuffer<TraitBasedObject>(stateEntity);
            TraitBasedObjectIds = transaction.GetBuffer<TraitBasedObjectId>(stateEntity);

            EaterTraitBuffer = transaction.GetBuffer<EaterTrait>(stateEntity);
            LocationBuffer = transaction.GetBuffer<Location>(stateEntity);
            MoveableBuffer = transaction.GetBuffer<Moveable>(stateEntity);
            FeedTraitBuffer = transaction.GetBuffer<FeedTrait>(stateEntity);
            GoalTraitBuffer = transaction.GetBuffer<GoalTrait>(stateEntity);
            PlanningAgentBuffer = transaction.GetBuffer<PlanningAgent>(stateEntity);
        }

        public StateData(int jobIndex, EntityCommandBuffer.ParallelWriter entityCommandBuffer, Entity stateEntity)
        {
            StateEntity = stateEntity;
            TraitBasedObjects = entityCommandBuffer.AddBuffer<TraitBasedObject>(jobIndex, stateEntity);
            TraitBasedObjectIds = entityCommandBuffer.AddBuffer<TraitBasedObjectId>(jobIndex, stateEntity);

            EaterTraitBuffer = entityCommandBuffer.AddBuffer<EaterTrait>(jobIndex, stateEntity);
            LocationBuffer = entityCommandBuffer.AddBuffer<Location>(jobIndex, stateEntity);
            MoveableBuffer = entityCommandBuffer.AddBuffer<Moveable>(jobIndex, stateEntity);
            FeedTraitBuffer = entityCommandBuffer.AddBuffer<FeedTrait>(jobIndex, stateEntity);
            GoalTraitBuffer = entityCommandBuffer.AddBuffer<GoalTrait>(jobIndex, stateEntity);
            PlanningAgentBuffer = entityCommandBuffer.AddBuffer<PlanningAgent>(jobIndex, stateEntity);
        }

        public StateData Copy(int jobIndex, EntityCommandBuffer.ParallelWriter entityCommandBuffer)
        {
            var stateEntity = entityCommandBuffer.Instantiate(jobIndex, StateEntity);
            var traitBasedObjects = entityCommandBuffer.SetBuffer<TraitBasedObject>(jobIndex, stateEntity);
            traitBasedObjects.CopyFrom(TraitBasedObjects.AsNativeArray());
            var traitBasedObjectIds = entityCommandBuffer.SetBuffer<TraitBasedObjectId>(jobIndex, stateEntity);
            traitBasedObjectIds.CopyFrom(TraitBasedObjectIds.AsNativeArray());

            var EaterTraits = entityCommandBuffer.SetBuffer<EaterTrait>(jobIndex, stateEntity);
            EaterTraits.CopyFrom(EaterTraitBuffer.AsNativeArray());
            var Locations = entityCommandBuffer.SetBuffer<Location>(jobIndex, stateEntity);
            Locations.CopyFrom(LocationBuffer.AsNativeArray());
            var Moveables = entityCommandBuffer.SetBuffer<Moveable>(jobIndex, stateEntity);
            Moveables.CopyFrom(MoveableBuffer.AsNativeArray());
            var FeedTraits = entityCommandBuffer.SetBuffer<FeedTrait>(jobIndex, stateEntity);
            FeedTraits.CopyFrom(FeedTraitBuffer.AsNativeArray());
            var GoalTraits = entityCommandBuffer.SetBuffer<GoalTrait>(jobIndex, stateEntity);
            GoalTraits.CopyFrom(GoalTraitBuffer.AsNativeArray());
            var PlanningAgents = entityCommandBuffer.SetBuffer<PlanningAgent>(jobIndex, stateEntity);
            PlanningAgents.CopyFrom(PlanningAgentBuffer.AsNativeArray());

            return new StateData
            {
                StateEntity = stateEntity,
                TraitBasedObjects = traitBasedObjects,
                TraitBasedObjectIds = traitBasedObjectIds,

                EaterTraitBuffer = EaterTraits,
                LocationBuffer = Locations,
                MoveableBuffer = Moveables,
                FeedTraitBuffer = FeedTraits,
                GoalTraitBuffer = GoalTraits,
                PlanningAgentBuffer = PlanningAgents,
            };
        }

        public void AddObject(NativeArray<ComponentType> types, out TraitBasedObject traitBasedObject, TraitBasedObjectId objectId, FixedString64 name = default)
        {
            traitBasedObject = TraitBasedObject.Default;
#if DEBUG
            objectId.Name.CopyFrom(name);
#endif

            for (int i = 0; i < types.Length; i++)
            {
                var t = types[i];
                if (t.TypeIndex == s_EaterTraitTypeIndex)
                {
                    EaterTraitBuffer.Add(default);
                    traitBasedObject.EaterTraitIndex = (byte) (EaterTraitBuffer.Length - 1);
                }
                else if (t.TypeIndex == s_LocationTypeIndex)
                {
                    LocationBuffer.Add(default);
                    traitBasedObject.LocationIndex = (byte) (LocationBuffer.Length - 1);
                }
                else if (t.TypeIndex == s_MoveableTypeIndex)
                {
                    MoveableBuffer.Add(default);
                    traitBasedObject.MoveableIndex = (byte) (MoveableBuffer.Length - 1);
                }
                else if (t.TypeIndex == s_FeedTraitTypeIndex)
                {
                    FeedTraitBuffer.Add(default);
                    traitBasedObject.FeedTraitIndex = (byte) (FeedTraitBuffer.Length - 1);
                }
                else if (t.TypeIndex == s_GoalTraitTypeIndex)
                {
                    GoalTraitBuffer.Add(default);
                    traitBasedObject.GoalTraitIndex = (byte) (GoalTraitBuffer.Length - 1);
                }
                else if (t.TypeIndex == s_PlanningAgentTypeIndex)
                {
                    PlanningAgentBuffer.Add(default);
                    traitBasedObject.PlanningAgentIndex = (byte) (PlanningAgentBuffer.Length - 1);
                }
            }

            TraitBasedObjectIds.Add(objectId);
            TraitBasedObjects.Add(traitBasedObject);
        }

        public void AddObject(NativeArray<ComponentType> types, out TraitBasedObject traitBasedObject, out TraitBasedObjectId objectId, FixedString64 name = default)
        {
            objectId = new TraitBasedObjectId() { Id = ObjectId.GetNext() };
            AddObject(types, out traitBasedObject, objectId, name);
        }

        public void ConvertAndSetPlannerTrait(Entity sourceEntity, EntityManager sourceEntityManager,
            NativeArray<ComponentType> sourceTraitTypes, IDictionary<Entity, TraitBasedObjectId> entityToObjectId,
            ref TraitBasedObject traitBasedObject)
        {
            unsafe
            {
                foreach (var type in sourceTraitTypes)
                {
                    if (type == typeof(Generated.Semantic.Traits.EaterTraitData))
                    {
                        var traitData = sourceEntityManager.GetComponentData<Generated.Semantic.Traits.EaterTraitData>(sourceEntity);
                        var plannerTraitData = new EaterTrait();
                        UnsafeUtility.CopyStructureToPtr(ref traitData, UnsafeUtility.AddressOf(ref plannerTraitData));
                        SetTraitOnObject(plannerTraitData, ref traitBasedObject);
                    }
                    if (type == typeof(Generated.Semantic.Traits.LocationData))
                    {
                        var traitData = sourceEntityManager.GetComponentData<Generated.Semantic.Traits.LocationData>(sourceEntity);
                        var plannerTraitData = new Location();
                        UnsafeUtility.CopyStructureToPtr(ref traitData, UnsafeUtility.AddressOf(ref plannerTraitData));
                        SetTraitOnObject(plannerTraitData, ref traitBasedObject);
                    }
                }
            }
        }

        public void SetTraitOnObject(ITrait trait, ref TraitBasedObject traitBasedObject)
        {
            if (trait is EaterTrait EaterTraitTrait)
                SetTraitOnObject(EaterTraitTrait, ref traitBasedObject);
            else if (trait is Location LocationTrait)
                SetTraitOnObject(LocationTrait, ref traitBasedObject);
            else if (trait is Moveable MoveableTrait)
                SetTraitOnObject(MoveableTrait, ref traitBasedObject);
            else if (trait is FeedTrait FeedTraitTrait)
                SetTraitOnObject(FeedTraitTrait, ref traitBasedObject);
            else if (trait is GoalTrait GoalTraitTrait)
                SetTraitOnObject(GoalTraitTrait, ref traitBasedObject);
            else if (trait is PlanningAgent PlanningAgentTrait)
                SetTraitOnObject(PlanningAgentTrait, ref traitBasedObject);
            else 
                throw new ArgumentException($"Trait {trait} of type {trait.GetType()} is not supported in this state representation.");
        }

        public void SetTraitOnObjectAtIndex(ITrait trait, int traitBasedObjectIndex)
        {
            if (trait is EaterTrait EaterTraitTrait)
                SetTraitOnObjectAtIndex(EaterTraitTrait, traitBasedObjectIndex);
            else if (trait is Location LocationTrait)
                SetTraitOnObjectAtIndex(LocationTrait, traitBasedObjectIndex);
            else if (trait is Moveable MoveableTrait)
                SetTraitOnObjectAtIndex(MoveableTrait, traitBasedObjectIndex);
            else if (trait is FeedTrait FeedTraitTrait)
                SetTraitOnObjectAtIndex(FeedTraitTrait, traitBasedObjectIndex);
            else if (trait is GoalTrait GoalTraitTrait)
                SetTraitOnObjectAtIndex(GoalTraitTrait, traitBasedObjectIndex);
            else if (trait is PlanningAgent PlanningAgentTrait)
                SetTraitOnObjectAtIndex(PlanningAgentTrait, traitBasedObjectIndex);
            else 
                throw new ArgumentException($"Trait {trait} of type {trait.GetType()} is not supported in this state representation.");
        }


        public TTrait GetTraitOnObject<TTrait>(TraitBasedObject traitBasedObject) where TTrait : struct, ITrait
        {
            var traitBasedObjectTraitIndex = TraitArrayIndex<TTrait>.Index;
            if (traitBasedObjectTraitIndex == -1)
                throw new ArgumentException($"Trait {typeof(TTrait)} not supported in this plan");

            var traitBufferIndex = traitBasedObject[traitBasedObjectTraitIndex];
            if (traitBufferIndex == TraitBasedObject.Unset)
                throw new ArgumentException($"Trait of type {typeof(TTrait)} does not exist on object {traitBasedObject}.");

            return GetBuffer<TTrait>()[traitBufferIndex];
        }

        public bool HasTraitOnObject<TTrait>(TraitBasedObject traitBasedObject) where TTrait : struct, ITrait
        {
            var traitBasedObjectTraitIndex = TraitArrayIndex<TTrait>.Index;
            if (traitBasedObjectTraitIndex == -1)
                throw new ArgumentException($"Trait {typeof(TTrait)} not supported in this plan");

            var traitBufferIndex = traitBasedObject[traitBasedObjectTraitIndex];
            return traitBufferIndex != TraitBasedObject.Unset;
        }

        public void SetTraitOnObject<TTrait>(TTrait trait, ref TraitBasedObject traitBasedObject) where TTrait : struct, ITrait
        {
            var objectIndex = GetTraitBasedObjectIndex(traitBasedObject);
            if (objectIndex == -1)
                throw new ArgumentException($"Object {traitBasedObject} does not exist within the state data {this}.");

            var traitIndex = TraitArrayIndex<TTrait>.Index;
            var traitBuffer = GetBuffer<TTrait>();

            var bufferIndex = traitBasedObject[traitIndex];
            if (bufferIndex == TraitBasedObject.Unset)
            {
                traitBuffer.Add(trait);
                traitBasedObject[traitIndex] = (byte) (traitBuffer.Length - 1);

                TraitBasedObjects[objectIndex] = traitBasedObject;
            }
            else
            {
                traitBuffer[bufferIndex] = trait;
            }
        }

        public bool RemoveTraitOnObject<TTrait>(ref TraitBasedObject traitBasedObject) where TTrait : struct, ITrait
        {
            var objectTraitIndex = TraitArrayIndex<TTrait>.Index;
            var traitBuffer = GetBuffer<TTrait>();

            var traitBufferIndex = traitBasedObject[objectTraitIndex];
            if (traitBufferIndex == TraitBasedObject.Unset)
                return false;

            // last index
            var lastBufferIndex = traitBuffer.Length - 1;

            // Swap back and remove
            var lastTrait = traitBuffer[lastBufferIndex];
            traitBuffer[lastBufferIndex] = traitBuffer[traitBufferIndex];
            traitBuffer[traitBufferIndex] = lastTrait;
            traitBuffer.RemoveAt(lastBufferIndex);

            // Update index for object with last trait in buffer
            var numObjects = TraitBasedObjects.Length;
            for (int i = 0; i < numObjects; i++)
            {
                var otherTraitBasedObject = TraitBasedObjects[i];
                if (otherTraitBasedObject[objectTraitIndex] == lastBufferIndex)
                {
                    otherTraitBasedObject[objectTraitIndex] = traitBufferIndex;
                    TraitBasedObjects[i] = otherTraitBasedObject;
                    break;
                }
            }

            // Update traitBasedObject in buffer (ref is to a copy)
            for (int i = 0; i < numObjects; i++)
            {
                if (traitBasedObject.Equals(TraitBasedObjects[i]))
                {
                    traitBasedObject[objectTraitIndex] = TraitBasedObject.Unset;
                    TraitBasedObjects[i] = traitBasedObject;
                    return true;
                }
            }

            throw new ArgumentException($"TraitBasedObject {traitBasedObject} does not exist in the state container {this}.");
        }

        public bool RemoveObject(TraitBasedObject traitBasedObject)
        {
            var objectIndex = GetTraitBasedObjectIndex(traitBasedObject);
            if (objectIndex == -1)
                return false;


            RemoveTraitOnObject<EaterTrait>(ref traitBasedObject);
            RemoveTraitOnObject<Location>(ref traitBasedObject);
            RemoveTraitOnObject<Moveable>(ref traitBasedObject);
            RemoveTraitOnObject<FeedTrait>(ref traitBasedObject);
            RemoveTraitOnObject<GoalTrait>(ref traitBasedObject);
            RemoveTraitOnObject<PlanningAgent>(ref traitBasedObject);

            TraitBasedObjects.RemoveAt(objectIndex);
            TraitBasedObjectIds.RemoveAt(objectIndex);

            return true;
        }


        public TTrait GetTraitOnObjectAtIndex<TTrait>(int traitBasedObjectIndex) where TTrait : struct, ITrait
        {
            var traitBasedObjectTraitIndex = TraitArrayIndex<TTrait>.Index;
            if (traitBasedObjectTraitIndex == -1)
                throw new ArgumentException($"Trait {typeof(TTrait)} not supported in this state representation");

            var traitBasedObject = TraitBasedObjects[traitBasedObjectIndex];
            var traitBufferIndex = traitBasedObject[traitBasedObjectTraitIndex];
            if (traitBufferIndex == TraitBasedObject.Unset)
                throw new Exception($"Trait index for {typeof(TTrait)} is not set for object {traitBasedObject}");

            return GetBuffer<TTrait>()[traitBufferIndex];
        }

        public void SetTraitOnObjectAtIndex<T>(T trait, int traitBasedObjectIndex) where T : struct, ITrait
        {
            var traitBasedObjectTraitIndex = TraitArrayIndex<T>.Index;
            if (traitBasedObjectTraitIndex == -1)
                throw new ArgumentException($"Trait {typeof(T)} not supported in this state representation");

            var traitBasedObject = TraitBasedObjects[traitBasedObjectIndex];
            var traitBufferIndex = traitBasedObject[traitBasedObjectTraitIndex];
            var traitBuffer = GetBuffer<T>();
            if (traitBufferIndex == TraitBasedObject.Unset)
            {
                traitBuffer.Add(trait);
                traitBufferIndex = (byte)(traitBuffer.Length - 1);
                traitBasedObject[traitBasedObjectTraitIndex] = traitBufferIndex;
                TraitBasedObjects[traitBasedObjectIndex] = traitBasedObject;
            }
            else
            {
                traitBuffer[traitBufferIndex] = trait;
            }
        }

        public bool RemoveTraitOnObjectAtIndex<TTrait>(int traitBasedObjectIndex) where TTrait : struct, ITrait
        {
            var objectTraitIndex = TraitArrayIndex<TTrait>.Index;
            var traitBuffer = GetBuffer<TTrait>();

            var traitBasedObject = TraitBasedObjects[traitBasedObjectIndex];
            var traitBufferIndex = traitBasedObject[objectTraitIndex];
            if (traitBufferIndex == TraitBasedObject.Unset)
                return false;

            // last index
            var lastBufferIndex = traitBuffer.Length - 1;

            // Swap back and remove
            var lastTrait = traitBuffer[lastBufferIndex];
            traitBuffer[lastBufferIndex] = traitBuffer[traitBufferIndex];
            traitBuffer[traitBufferIndex] = lastTrait;
            traitBuffer.RemoveAt(lastBufferIndex);

            // Update index for object with last trait in buffer
            var numObjects = TraitBasedObjects.Length;
            for (int i = 0; i < numObjects; i++)
            {
                var otherTraitBasedObject = TraitBasedObjects[i];
                if (otherTraitBasedObject[objectTraitIndex] == lastBufferIndex)
                {
                    otherTraitBasedObject[objectTraitIndex] = traitBufferIndex;
                    TraitBasedObjects[i] = otherTraitBasedObject;
                    break;
                }
            }

            traitBasedObject[objectTraitIndex] = TraitBasedObject.Unset;
            TraitBasedObjects[traitBasedObjectIndex] = traitBasedObject;

            return true;
        }

        public bool RemoveTraitBasedObjectAtIndex(int traitBasedObjectIndex)
        {
            RemoveTraitOnObjectAtIndex<EaterTrait>(traitBasedObjectIndex);
            RemoveTraitOnObjectAtIndex<Location>(traitBasedObjectIndex);
            RemoveTraitOnObjectAtIndex<Moveable>(traitBasedObjectIndex);
            RemoveTraitOnObjectAtIndex<FeedTrait>(traitBasedObjectIndex);
            RemoveTraitOnObjectAtIndex<GoalTrait>(traitBasedObjectIndex);
            RemoveTraitOnObjectAtIndex<PlanningAgent>(traitBasedObjectIndex);

            TraitBasedObjects.RemoveAt(traitBasedObjectIndex);
            TraitBasedObjectIds.RemoveAt(traitBasedObjectIndex);

            return true;
        }


        public NativeArray<int> GetTraitBasedObjectIndices(NativeList<int> traitBasedObjectIndices, NativeArray<ComponentType> traitFilter)
        {
            var numObjects = TraitBasedObjects.Length;
            for (var i = 0; i < numObjects; i++)
            {
                var traitBasedObject = TraitBasedObjects[i];
                if (traitBasedObject.MatchesTraitFilter(traitFilter))
                    traitBasedObjectIndices.Add(i);
            }

            return traitBasedObjectIndices.AsArray();
        }

        public NativeArray<int> GetTraitBasedObjectIndices(NativeList<int> traitBasedObjectIndices, params ComponentType[] traitFilter)
        {
            var numObjects = TraitBasedObjects.Length;
            for (var i = 0; i < numObjects; i++)
            {
                var traitBasedObject = TraitBasedObjects[i];
                if (traitBasedObject.MatchesTraitFilter(traitFilter))
                    traitBasedObjectIndices.Add(i);
            }

            return traitBasedObjectIndices.AsArray();
        }

        public int GetTraitBasedObjectIndex(TraitBasedObject traitBasedObject)
        {
            for (int objectIndex = TraitBasedObjects.Length - 1; objectIndex >= 0; objectIndex--)
            {
                bool match = true;
                var other = TraitBasedObjects[objectIndex];
                for (int i = 0; i < traitBasedObject.Length && match; i++)
                {
                    match &= traitBasedObject[i] == other[i];
                }

                if (match)
                    return objectIndex;
            }

            return -1;
        }

        public int GetTraitBasedObjectIndex(TraitBasedObjectId traitBasedObjectId)
        {
            var objectIndex = -1;
            for (int i = TraitBasedObjectIds.Length - 1; i >= 0; i--)
            {
                if (TraitBasedObjectIds[i].Equals(traitBasedObjectId))
                {
                    objectIndex = i;
                    break;
                }
            }

            return objectIndex;
        }

        public TraitBasedObjectId GetTraitBasedObjectId(TraitBasedObject traitBasedObject)
        {
            var index = GetTraitBasedObjectIndex(traitBasedObject);
            return TraitBasedObjectIds[index];
        }

        public TraitBasedObjectId GetTraitBasedObjectId(int traitBasedObjectIndex)
        {
            return TraitBasedObjectIds[traitBasedObjectIndex];
        }

        public TraitBasedObject GetTraitBasedObject(TraitBasedObjectId traitBasedObject)
        {
            var index = GetTraitBasedObjectIndex(traitBasedObject);
            return TraitBasedObjects[index];
        }


        DynamicBuffer<T> GetBuffer<T>() where T : struct, ITrait
        {
            var index = TraitArrayIndex<T>.Index;
            switch (index)
            {
                case 0:
                    return EaterTraitBuffer.Reinterpret<T>();
                case 1:
                    return LocationBuffer.Reinterpret<T>();
                case 2:
                    return MoveableBuffer.Reinterpret<T>();
                case 3:
                    return FeedTraitBuffer.Reinterpret<T>();
                case 4:
                    return GoalTraitBuffer.Reinterpret<T>();
                case 5:
                    return PlanningAgentBuffer.Reinterpret<T>();
            }

            return default;
        }

        public bool Equals(IStateData other) => other is StateData otherData && Equals(otherData);

        public bool Equals(StateData rhsState)
        {
            if (StateEntity == rhsState.StateEntity)
                return true;

            // Easy check is to make sure each state has the same number of trait-based objects
            if (TraitBasedObjects.Length != rhsState.TraitBasedObjects.Length
                || EaterTraitBuffer.Length != rhsState.EaterTraitBuffer.Length
                || LocationBuffer.Length != rhsState.LocationBuffer.Length
                || MoveableBuffer.Length != rhsState.MoveableBuffer.Length
                || FeedTraitBuffer.Length != rhsState.FeedTraitBuffer.Length
                || GoalTraitBuffer.Length != rhsState.GoalTraitBuffer.Length
                || PlanningAgentBuffer.Length != rhsState.PlanningAgentBuffer.Length)
                return false;

            var objectMap = new ObjectCorrespondence(TraitBasedObjectIds.Length, Allocator.Temp);
            bool statesEqual = TryGetObjectMapping(rhsState, objectMap);
            objectMap.Dispose();

            return statesEqual;
        }

        bool ITraitBasedStateData<TraitBasedObject, StateData>.TryGetObjectMapping(StateData rhsState, ObjectCorrespondence objectMap)
        {
            // Easy check is to make sure each state has the same number of domain objects
            if (TraitBasedObjects.Length != rhsState.TraitBasedObjects.Length
                || EaterTraitBuffer.Length != rhsState.EaterTraitBuffer.Length
                || LocationBuffer.Length != rhsState.LocationBuffer.Length
                || MoveableBuffer.Length != rhsState.MoveableBuffer.Length
                || FeedTraitBuffer.Length != rhsState.FeedTraitBuffer.Length
                || GoalTraitBuffer.Length != rhsState.GoalTraitBuffer.Length
                || PlanningAgentBuffer.Length != rhsState.PlanningAgentBuffer.Length)
                return false;

            return TryGetObjectMapping(rhsState, objectMap);
        }

        internal bool TryGetObjectMapping(StateData rhsState, ObjectCorrespondence objectMap)
        {
            objectMap.Initialize(TraitBasedObjectIds, rhsState.TraitBasedObjectIds);

            bool statesEqual = true;
            var numObjects = TraitBasedObjects.Length;
            for (int lhsIndex = 0; lhsIndex < numObjects; lhsIndex++)
            {
                var lhsId = TraitBasedObjectIds[lhsIndex].Id;
                if (objectMap.TryGetValue(lhsId, out _)) // already matched
                    continue;

                // todo lhsIndex to start? would require swapping rhs on assignments, though
                bool matchFound = true;

                for (var rhsIndex = 0; rhsIndex < numObjects; rhsIndex++)
                {
                    var rhsId = rhsState.TraitBasedObjectIds[rhsIndex].Id;
                    if (objectMap.ContainsRHS(rhsId)) // skip if already assigned todo optimize this
                        continue;

                    objectMap.BeginNewTraversal();
                    objectMap.Add(lhsId, rhsId);

                    // Traversal comparing all reachable objects
                    matchFound = true;
                    while (objectMap.Next(out var lhsIdToEvaluate, out var rhsIdToEvaluate))
                    {
                        // match objects, queueing as needed
                        var lhsTraitBasedObject = TraitBasedObjects[objectMap.GetLHSIndex(lhsIdToEvaluate)];
                        var rhsTraitBasedObject = rhsState.TraitBasedObjects[objectMap.GetRHSIndex(rhsIdToEvaluate)];

                        if (!ObjectsMatchAttributes(lhsTraitBasedObject, rhsTraitBasedObject, rhsState) ||
                            !CheckRelationsAndQueueObjects(lhsTraitBasedObject, rhsTraitBasedObject, rhsState, objectMap))
                        {
                            objectMap.RevertTraversalChanges();

                            matchFound = false;
                            break;
                        }
                    }

                    if (matchFound)
                        break;
                }

                if (!matchFound)
                {
                    statesEqual = false;
                    break;
                }
            }

            return statesEqual;
        }

        bool ObjectsMatchAttributes(TraitBasedObject traitBasedObjectLHS, TraitBasedObject traitBasedObjectRHS, StateData rhsState)
        {
            if (!traitBasedObjectLHS.HasSameTraits(traitBasedObjectRHS))
                return false;

            if (traitBasedObjectLHS.EaterTraitIndex != TraitBasedObject.Unset
                && !EaterTraitTraitAttributesEqual(EaterTraitBuffer[traitBasedObjectLHS.EaterTraitIndex], rhsState.EaterTraitBuffer[traitBasedObjectRHS.EaterTraitIndex]))
                return false;


            if (traitBasedObjectLHS.LocationIndex != TraitBasedObject.Unset
                && !LocationTraitAttributesEqual(LocationBuffer[traitBasedObjectLHS.LocationIndex], rhsState.LocationBuffer[traitBasedObjectRHS.LocationIndex]))
                return false;






            return true;
        }
        
        bool EaterTraitTraitAttributesEqual(EaterTrait one, EaterTrait two)
        {
            return
                    one.EatCount == two.EatCount;
        }
        
        bool LocationTraitAttributesEqual(Location one, Location two)
        {
            return
                    one.Position == two.Position && 
                    one.Forward == two.Forward;
        }
        
        bool CheckRelationsAndQueueObjects(TraitBasedObject traitBasedObjectLHS, TraitBasedObject traitBasedObjectRHS, StateData rhsState, ObjectCorrespondence objectMap)
        {

            return true;
        }

        public override int GetHashCode()
        {
            // h = 3860031 + (h+y)*2779 + (h*y*2)   // from How to Hash a Set by Richard O’Keefe
            var stateHashValue = 3860031 + (397 + TraitBasedObjectIds.Length) * 2779 + (397 * TraitBasedObjectIds.Length * 2);

            int bufferLength;

            bufferLength = EaterTraitBuffer.Length;
            for (int i = 0; i < bufferLength; i++)
            {
                var element = EaterTraitBuffer[i];
                var value = 397
                    ^ element.EatCount.GetHashCode();
                stateHashValue = 3860031 + (stateHashValue + value) * 2779 + (stateHashValue * value * 2);
            }
            bufferLength = LocationBuffer.Length;
            for (int i = 0; i < bufferLength; i++)
            {
                var element = LocationBuffer[i];
                var value = 397
                    ^ element.Position.GetHashCode()
                    ^ element.Forward.GetHashCode();
                stateHashValue = 3860031 + (stateHashValue + value) * 2779 + (stateHashValue * value * 2);
            }
            bufferLength = MoveableBuffer.Length;
            for (int i = 0; i < bufferLength; i++)
            {
                var value = 397;
                stateHashValue = 3860031 + (stateHashValue + value) * 2779 + (stateHashValue * value * 2);
            }
            bufferLength = FeedTraitBuffer.Length;
            for (int i = 0; i < bufferLength; i++)
            {
                var value = 397;
                stateHashValue = 3860031 + (stateHashValue + value) * 2779 + (stateHashValue * value * 2);
            }
            bufferLength = GoalTraitBuffer.Length;
            for (int i = 0; i < bufferLength; i++)
            {
                var value = 397;
                stateHashValue = 3860031 + (stateHashValue + value) * 2779 + (stateHashValue * value * 2);
            }
            bufferLength = PlanningAgentBuffer.Length;
            for (int i = 0; i < bufferLength; i++)
            {
                var value = 397;
                stateHashValue = 3860031 + (stateHashValue + value) * 2779 + (stateHashValue * value * 2);
            }

            return stateHashValue;
        }

        public override string ToString()
        {
            if (StateEntity.Equals(default))
                return string.Empty;

            var sb = new StringBuilder();
            var numObjects = TraitBasedObjects.Length;
            for (var traitBasedObjectIndex = 0; traitBasedObjectIndex < numObjects; traitBasedObjectIndex++)
            {
                var traitBasedObject = TraitBasedObjects[traitBasedObjectIndex];
                sb.AppendLine(TraitBasedObjectIds[traitBasedObjectIndex].ToString());

                var i = 0;

                var traitIndex = traitBasedObject[i++];
                if (traitIndex != TraitBasedObject.Unset)
                    sb.AppendLine(EaterTraitBuffer[traitIndex].ToString());

                traitIndex = traitBasedObject[i++];
                if (traitIndex != TraitBasedObject.Unset)
                    sb.AppendLine(LocationBuffer[traitIndex].ToString());

                traitIndex = traitBasedObject[i++];
                if (traitIndex != TraitBasedObject.Unset)
                    sb.AppendLine(MoveableBuffer[traitIndex].ToString());

                traitIndex = traitBasedObject[i++];
                if (traitIndex != TraitBasedObject.Unset)
                    sb.AppendLine(FeedTraitBuffer[traitIndex].ToString());

                traitIndex = traitBasedObject[i++];
                if (traitIndex != TraitBasedObject.Unset)
                    sb.AppendLine(GoalTraitBuffer[traitIndex].ToString());

                traitIndex = traitBasedObject[i++];
                if (traitIndex != TraitBasedObject.Unset)
                    sb.AppendLine(PlanningAgentBuffer[traitIndex].ToString());

                sb.AppendLine();
            }

            return sb.ToString();
        }
    }

    public struct StateDataContext : ITraitBasedStateDataContext<TraitBasedObject, StateEntityKey, StateData>
    {
        public bool IsCreated;
        internal EntityCommandBuffer.ParallelWriter EntityCommandBuffer;
        internal EntityArchetype m_StateArchetype;
        internal int JobIndex;

        [ReadOnly,NativeDisableContainerSafetyRestriction] public BufferFromEntity<TraitBasedObject> TraitBasedObjects;
        [ReadOnly,NativeDisableContainerSafetyRestriction] public BufferFromEntity<TraitBasedObjectId> TraitBasedObjectIds;

        [ReadOnly,NativeDisableContainerSafetyRestriction] public BufferFromEntity<EaterTrait> EaterTraitData;
        [ReadOnly,NativeDisableContainerSafetyRestriction] public BufferFromEntity<Location> LocationData;
        [ReadOnly,NativeDisableContainerSafetyRestriction] public BufferFromEntity<Moveable> MoveableData;
        [ReadOnly,NativeDisableContainerSafetyRestriction] public BufferFromEntity<FeedTrait> FeedTraitData;
        [ReadOnly,NativeDisableContainerSafetyRestriction] public BufferFromEntity<GoalTrait> GoalTraitData;
        [ReadOnly,NativeDisableContainerSafetyRestriction] public BufferFromEntity<PlanningAgent> PlanningAgentData;

        [NativeDisableContainerSafetyRestriction,ReadOnly] ObjectCorrespondence m_ObjectCorrespondence;

        public StateDataContext(JobComponentSystem system, EntityArchetype stateArchetype)
        {
            EntityCommandBuffer = default;
            TraitBasedObjects = system.GetBufferFromEntity<TraitBasedObject>(true);
            TraitBasedObjectIds = system.GetBufferFromEntity<TraitBasedObjectId>(true);

            EaterTraitData = system.GetBufferFromEntity<EaterTrait>(true);
            LocationData = system.GetBufferFromEntity<Location>(true);
            MoveableData = system.GetBufferFromEntity<Moveable>(true);
            FeedTraitData = system.GetBufferFromEntity<FeedTrait>(true);
            GoalTraitData = system.GetBufferFromEntity<GoalTrait>(true);
            PlanningAgentData = system.GetBufferFromEntity<PlanningAgent>(true);

            m_StateArchetype = stateArchetype;
            JobIndex = 0;
            m_ObjectCorrespondence = default;
            IsCreated = true;
        }

        public StateData GetStateData(StateEntityKey stateKey)
        {
            var stateEntity = stateKey.Entity;

            return new StateData
            {
                StateEntity = stateEntity,
                TraitBasedObjects = TraitBasedObjects[stateEntity],
                TraitBasedObjectIds = TraitBasedObjectIds[stateEntity],

                EaterTraitBuffer = EaterTraitData[stateEntity],
                LocationBuffer = LocationData[stateEntity],
                MoveableBuffer = MoveableData[stateEntity],
                FeedTraitBuffer = FeedTraitData[stateEntity],
                GoalTraitBuffer = GoalTraitData[stateEntity],
                PlanningAgentBuffer = PlanningAgentData[stateEntity],
            };
        }

        public StateData CopyStateData(StateData stateData)
        {
            return stateData.Copy(JobIndex, EntityCommandBuffer);
        }

        public StateEntityKey GetStateDataKey(StateData stateData)
        {
            return new StateEntityKey { Entity = stateData.StateEntity, HashCode = stateData.GetHashCode()};
        }

        public void DestroyState(StateEntityKey stateKey)
        {
            EntityCommandBuffer.DestroyEntity(JobIndex, stateKey.Entity);
        }

        public StateData CreateStateData()
        {
            return new StateData(JobIndex, EntityCommandBuffer, EntityCommandBuffer.CreateEntity(JobIndex, m_StateArchetype));
        }

        public bool Equals(StateData x, StateData y)
        {
            if (x.TraitBasedObjectIds.Length != y.TraitBasedObjectIds.Length)
                return false;

            if (!m_ObjectCorrespondence.IsCreated)
                m_ObjectCorrespondence = new ObjectCorrespondence(x.TraitBasedObjectIds.Length, Allocator.Temp);

            return x.TryGetObjectMapping(y, m_ObjectCorrespondence);
        }

        public int GetHashCode(StateData obj)
        {
            return obj.GetHashCode();
        }
    }

    [DisableAutoCreation, AlwaysUpdateSystem]
    public class StateManager : JobComponentSystem, ITraitBasedStateManager<TraitBasedObject, StateEntityKey, StateData, StateDataContext>
    {
        public new EntityManager EntityManager
        {
            get
            {
                if (!m_EntityTransactionActive)
                    BeginEntityExclusivity();

                return ExclusiveEntityTransaction.EntityManager;
            }
        }

        ExclusiveEntityTransaction m_ExclusiveEntityTransaction;
        public ExclusiveEntityTransaction ExclusiveEntityTransaction
        {
            get
            {
                if (!m_EntityTransactionActive)
                    BeginEntityExclusivity();

                return m_ExclusiveEntityTransaction;
            }
        }

        StateDataContext m_StateDataContext;
        public StateDataContext StateDataContext
        {
            get
            {
                if (m_StateDataContext.IsCreated)
                    return m_StateDataContext;

                m_StateDataContext = new StateDataContext(this, m_StateArchetype);
                return m_StateDataContext;
            }
        }

        public event Action Destroying;

        List<EntityCommandBuffer> m_EntityCommandBuffers;
        EntityArchetype m_StateArchetype;
        bool m_EntityTransactionActive = false;

        protected override void OnCreate()
        {
            m_StateArchetype = base.EntityManager.CreateArchetype(typeof(State), typeof(TraitBasedObject), typeof(TraitBasedObjectId), typeof(HashCode),
                typeof(EaterTrait),
                typeof(Location),
                typeof(Moveable),
                typeof(FeedTrait),
                typeof(GoalTrait),
                typeof(PlanningAgent));

            m_EntityCommandBuffers = new List<EntityCommandBuffer>();
        }

        protected override void OnDestroy()
        {
            Destroying?.Invoke();
            EndEntityExclusivity();
            ClearECBs();
            base.OnDestroy();
        }

        public EntityCommandBuffer GetEntityCommandBuffer()
        {
            var ecb = new EntityCommandBuffer(Allocator.Persistent);
            m_EntityCommandBuffers.Add(ecb);
            return ecb;
        }

        public StateData CreateStateData()
        {
            var stateEntity = ExclusiveEntityTransaction.CreateEntity(m_StateArchetype);
            return new StateData(ExclusiveEntityTransaction, stateEntity);;
        }

        public StateData GetStateData(StateEntityKey stateKey, bool readWrite = false)
        {
            return !Enabled || !ExclusiveEntityTransaction.Exists(stateKey.Entity) ?
                default : new StateData(ExclusiveEntityTransaction, stateKey.Entity);
        }

        public void DestroyState(StateEntityKey stateKey)
        {
            var stateEntity = stateKey.Entity;
            if (ExclusiveEntityTransaction.Exists(stateEntity))
                ExclusiveEntityTransaction.DestroyEntity(stateEntity);
        }

        public StateEntityKey GetStateDataKey(StateData stateData)
        {
            return new StateEntityKey { Entity = stateData.StateEntity, HashCode = stateData.GetHashCode()};
        }

        public StateData CopyStateData(StateData stateData)
        {
            var copyStateEntity = ExclusiveEntityTransaction.Instantiate(stateData.StateEntity);
            return new StateData(ExclusiveEntityTransaction, copyStateEntity);
        }

        public StateEntityKey CopyState(StateEntityKey stateKey)
        {
            var copyStateEntity = ExclusiveEntityTransaction.Instantiate(stateKey.Entity);
            var stateData = new StateData(ExclusiveEntityTransaction, copyStateEntity);
            return new StateEntityKey { Entity = copyStateEntity, HashCode = stateData.GetHashCode()};
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var jobDependencyHandle = ExclusiveEntityTransaction.EntityManager.ExclusiveEntityTransactionDependency;
            if (jobDependencyHandle.IsCompleted)
            {
                jobDependencyHandle.Complete();
                ClearECBs();
            }

            return inputDeps;
        }

        void ClearECBs()
        {
            foreach (var ecb in m_EntityCommandBuffers)
            {
                ecb.Dispose();
            }
            m_EntityCommandBuffers.Clear();
        }

        public bool Equals(StateData x, StateData y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(StateData obj)
        {
            return obj.GetHashCode();
        }

        void BeginEntityExclusivity()
        {
            m_StateDataContext = new StateDataContext(this, m_StateArchetype);
            m_ExclusiveEntityTransaction = base.EntityManager.BeginExclusiveEntityTransaction();
            m_EntityTransactionActive = true;
        }

        void EndEntityExclusivity()
        {
            base.EntityManager.EndExclusiveEntityTransaction();
            m_EntityTransactionActive = false;
            m_StateDataContext = default;
        }
    }

    struct DestroyStatesJobScheduler : IDestroyStatesScheduler<StateEntityKey, StateData, StateDataContext, StateManager>
    {
        public StateManager StateManager { private get; set; }
        public NativeQueue<StateEntityKey> StatesToDestroy { private get; set; }

        public JobHandle Schedule(JobHandle inputDeps)
        {
            var stateDataContext = StateManager.StateDataContext;
            var ecb = StateManager.GetEntityCommandBuffer();
            stateDataContext.EntityCommandBuffer = ecb.AsParallelWriter();
            var destroyStatesJobHandle = new DestroyStatesJob<StateEntityKey, StateData, StateDataContext>()
            {
                StateDataContext = stateDataContext,
                StatesToDestroy = StatesToDestroy
            }.Schedule(inputDeps);

            var playbackECBJobHandle = new PlaybackSingleECBJob()
            {
                ExclusiveEntityTransaction = StateManager.ExclusiveEntityTransaction,
                EntityCommandBuffer = ecb
            }.Schedule(destroyStatesJobHandle);

            var entityManager = StateManager.ExclusiveEntityTransaction.EntityManager;
            entityManager.ExclusiveEntityTransactionDependency = playbackECBJobHandle;
            return playbackECBJobHandle;
        }
    }
}
