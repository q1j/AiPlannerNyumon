using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.AI.Planner.Traits;

namespace Generated.AI.Planner.StateRepresentation
{
    [Serializable]
    public struct EaterTrait : ITrait, IBufferElementData, IEquatable<EaterTrait>
    {
        public const string FieldEatCount = "EatCount";
        public System.Int32 EatCount;

        public void SetField(string fieldName, object value)
        {
            switch (fieldName)
            {
                case nameof(EatCount):
                    EatCount = (System.Int32)value;
                    break;
                default:
                    throw new ArgumentException($"Field \"{fieldName}\" does not exist on trait EaterTrait.");
            }
        }

        public object GetField(string fieldName)
        {
            switch (fieldName)
            {
                case nameof(EatCount):
                    return EatCount;
                default:
                    throw new ArgumentException($"Field \"{fieldName}\" does not exist on trait EaterTrait.");
            }
        }

        public bool Equals(EaterTrait other)
        {
            return EatCount == other.EatCount;
        }

        public override string ToString()
        {
            return $"EaterTrait\n  EatCount: {EatCount}";
        }
    }
}
