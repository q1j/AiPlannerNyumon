using System;
using System.Collections.Generic;
using Unity.Semantic.Traits;
using Unity.Collections;
using Unity.Entities;

namespace Generated.Semantic.Traits
{
    [Serializable]
    public partial struct EaterTraitData : ITraitData, IEquatable<EaterTraitData>
    {
        public System.Int32 EatCount;

        public bool Equals(EaterTraitData other)
        {
            return EatCount.Equals(other.EatCount);
        }

        public override string ToString()
        {
            return $"EaterTrait: {EatCount}";
        }
    }
}
