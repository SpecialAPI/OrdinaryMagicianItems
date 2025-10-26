using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OrdinaryMagicianItems
{
    public class ChangeReloadSpeedSynergyProcessor : MonoBehaviour
    {
        public void Awake()
        {
            m_gun = GetComponent<Gun>();
        }

        public void Update()
        {
            bool flag = m_gun && m_gun.OwnerHasSynergy(RequiredSynergy);
            if (flag && !m_processed)
            {
                m_processed = true;
                m_cachedReloadTime = m_gun.reloadTime;
                m_gun.reloadTime = SynergyReloadTime;
            }
            else if (!flag && m_processed)
            {
                m_processed = false;
                m_gun.reloadTime = m_cachedReloadTime;
            }
        }

        public float SynergyReloadTime;
        public CustomSynergyType RequiredSynergy;
        private bool m_processed;
        private Gun m_gun;
        private float m_cachedReloadTime;
    }
}
