using Alexandria.ItemAPI;
using BepInEx;
using Gungeon;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace OrdinaryMagicianItems
{
    [BepInPlugin(MOD_GUID, "Ordinary Magician Items", "1.0.7")]
    [BepInDependency(ETGModMainBehaviour.GUID)]
    [BepInDependency(Alexandria.Alexandria.GUID)]
    public class Plugin : BaseUnityPlugin
    {
        public const string MOD_GUID = "spapi.etg.ordinarymagicianitems";

        public static CustomSynergyType TwelveShotsSynergy;
        public static CustomSynergyType CheatAgainstTheImpossibleSynergy;
        public static CustomSynergyType IncidentResolverKitSynergy;
        public static CustomSynergyType ExtendSynergy;

        public void Start()
        {
            ETGModMainBehaviour.WaitForGameManagerStart(GMStart);
        }

        public void GMStart(GameManager man)
        {
            new Harmony(MOD_GUID).PatchAll();
            ETGMod.Assets.SetupSpritesFromAssembly(typeof(Plugin).Assembly, "OrdinaryMagicianItems/Resources/MTGAPISpriteRoot");

            TwelveShotsSynergy                  = ETGModCompatibility.ExtendEnum<CustomSynergyType>(MOD_GUID, "TWELVE_SHOTS");
            CheatAgainstTheImpossibleSynergy    = ETGModCompatibility.ExtendEnum<CustomSynergyType>(MOD_GUID, "CHEAT_AGAINST_THE_IMPOSSIBLE");
            IncidentResolverKitSynergy          = ETGModCompatibility.ExtendEnum<CustomSynergyType>(MOD_GUID, "INCIDENT_RESOLVER_KIT");
            ExtendSynergy                       = ETGModCompatibility.ExtendEnum<CustomSynergyType>(MOD_GUID, "EXTEND");

            MagicianPistol.Init();
            MiniHakkero.Init();
            Broom.Init();
            Shanghai.Init();

            var twelveShots = CustomSynergies.Add("Twelve Shots", ["witch_pistol", "spapi:magician_pistol"]);
            twelveShots.bonusSynergies = [TwelveShotsSynergy];
            var cheatAgainstTheImpossible = CustomSynergies.Add("Cheat against the Impossible", ["staff_of_firepower", "spapi:mini_hakkero"]);
            cheatAgainstTheImpossible.bonusSynergies = [CheatAgainstTheImpossibleSynergy];
            var incidentResolverKit = CustomSynergies.Add("Incident-Resolver kit", ["spapi:ordinary_broom", "backpack"]);
            incidentResolverKit.bonusSynergies = [IncidentResolverKitSynergy];
            var extend = CustomSynergies.Add("Extend!", ["spapi:shanghai"], ["heart_holster", "heart_lunchbox", "heart_locket", "heart_bottle", "heart_purse"]);
            extend.bonusSynergies = [ExtendSynergy];
            AmmoRegenSynergyProcessor processor = Game.Items["staff_of_firepower"].gameObject.AddComponent<AmmoRegenSynergyProcessor>();
            processor.PreventGainWhileFiring = false;
            processor.RequiredSynergy = CheatAgainstTheImpossibleSynergy;
            processor.AmmoPerSecond = 1f;
        }
    }
}