using System;
using System.Collections.Generic;
using Unity.Semantic.Traits;
using Unity.Collections;
using Unity.Entities;

namespace Generated.Semantic.Traits
{
    [Serializable]
    public partial struct FeedTraitData : ITraitData, IEquatable<FeedTraitData>
    {

        public bool Equals(FeedTraitData other)
        {
            return true;
        }

        public override string ToString()
        {
            return $"FeedTrait";
        }
    }
}
