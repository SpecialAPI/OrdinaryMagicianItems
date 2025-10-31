using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OrdinaryMagicianItems
{
    public class ChangeReloadSpeedSynergyProcessor : GunBehaviour
    {
        public override void Update()
        {
            if(!PlayerOwner)
                return;

            var hasSynergy = PlayerOwner.HasActiveBonusSynergy(RequiredSynergy);
            if (hasSynergy && !m_processed)
            {
                m_processed = true;
                m_cachedReloadTime = gun.reloadTime;
                gun.reloadTime = SynergyReloadTime;
            }
            else if (!hasSynergy && m_processed)
            {
                m_processed = false;
                gun.reloadTime = m_cachedReloadTime;
            }
        }

        public float SynergyReloadTime;
        public CustomSynergyType RequiredSynergy;
        private bool m_processed;
        private float m_cachedReloadTime;
    }
}
