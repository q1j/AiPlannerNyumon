using System;
using System.Collections.Generic;
using Unity.Semantic.Traits;
using Unity.Collections;
using Unity.Entities;

namespace Generated.Semantic.Traits
{
    [Serializable]
    public partial struct GoalTraitData : ITraitData, IEquatable<GoalTraitData>
    {

        public bool Equals(GoalTraitData other)
        {
            return true;
        }

        public override string ToString()
        {
            return $"GoalTrait";
        }
    }
}
