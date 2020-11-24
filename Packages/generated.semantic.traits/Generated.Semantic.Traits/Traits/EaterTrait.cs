using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Semantic.Traits;
using Unity.Entities;
using UnityEngine;

namespace Generated.Semantic.Traits
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [AddComponentMenu("Semantic/Traits/EaterTrait (Trait)")]
    [RequireComponent(typeof(SemanticObject))]
    public partial class EaterTrait : MonoBehaviour, ITrait
    {
        public System.Int32 EatCount
        {
            get
            {
                if (m_EntityManager != default && m_EntityManager.HasComponent<EaterTraitData>(m_Entity))
                {
                    m_p2 = m_EntityManager.GetComponentData<EaterTraitData>(m_Entity).EatCount;
                }

                return m_p2;
            }
            set
            {
                EaterTraitData data = default;
                var dataActive = m_EntityManager != default && m_EntityManager.HasComponent<EaterTraitData>(m_Entity);
                if (dataActive)
                    data = m_EntityManager.GetComponentData<EaterTraitData>(m_Entity);
                data.EatCount = m_p2 = value;
                if (dataActive)
                    m_EntityManager.SetComponentData(m_Entity, data);
            }
        }
        public EaterTraitData Data
        {
            get => m_EntityManager != default && m_EntityManager.HasComponent<EaterTraitData>(m_Entity) ?
                m_EntityManager.GetComponentData<EaterTraitData>(m_Entity) : GetData();
            set
            {
                if (m_EntityManager != default && m_EntityManager.HasComponent<EaterTraitData>(m_Entity))
                    m_EntityManager.SetComponentData(m_Entity, value);
            }
        }

        #pragma warning disable 649
        [SerializeField]
        [InspectorName("EatCount")]
        System.Int32 m_p2 = 0;
        #pragma warning restore 649

        EntityManager m_EntityManager;
        World m_World;
        Entity m_Entity;

        EaterTraitData GetData()
        {
            EaterTraitData data = default;
            data.EatCount = m_p2;

            return data;
        }

        
        void OnEnable()
        {
            // Handle the case where this trait is added after conversion
            var semanticObject = GetComponent<SemanticObject>();
            if (semanticObject && !semanticObject.Entity.Equals(default))
                Convert(semanticObject.Entity, semanticObject.EntityManager, null);
        }

        public void Convert(Entity entity, EntityManager destinationManager, GameObjectConversionSystem _)
        {
            m_Entity = entity;
            m_EntityManager = destinationManager;
            m_World = destinationManager.World;

            if (!destinationManager.HasComponent(entity, typeof(EaterTraitData)))
            {
                destinationManager.AddComponentData(entity, GetData());
            }
        }

        void OnDestroy()
        {
            if (m_World != default && m_World.IsCreated)
            {
                m_EntityManager.RemoveComponent<EaterTraitData>(m_Entity);
                if (m_EntityManager.GetComponentCount(m_Entity) == 0)
                    m_EntityManager.DestroyEntity(m_Entity);
            }
        }

        void OnValidate()
        {

            // Commit local fields to backing store
            Data = GetData();
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            TraitGizmos.DrawGizmoForTrait(nameof(EaterTraitData), gameObject,Data);
        }
#endif
    }
}
