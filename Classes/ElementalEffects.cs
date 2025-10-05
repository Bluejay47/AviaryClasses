using Kingmaker.Enums.Damage;
using System.Collections.Generic;

namespace AviaryClasses.Classes {
    public static class ElementalEffects {
        // Energy damage type VFX
        public static readonly string Lightning = "c31b3edbb9bde0e4c8ee5484b920f6dc";      // Lightning Bolt VFX
        public static readonly string Fire = "88029e10e5aa48444a15d238e4d0c87b";           // Ignition VFX
        public static readonly string Cold = "7980e876b0ef66d4fb2175d8aca27d95";           // Ray of Frost VFX
        public static readonly string Acid = "9a38d742801be084d89bd34318c600e8";           // Acid Splash VFX
        public static readonly string Sonic = "2644dac00cee8b840a35f2445c4dffd9";          // Sound Burst VFX
        public static readonly string NegativeEnergy = "f1325dbc42578b94b8e3a87c973ebdde"; // Channel Negative Energy VFX
        public static readonly string PositiveEnergy = "474cb5ac1a1048d4a9ee6ae62474d196"; // Cure Critical Wounds VFX
        public static readonly string Holy = "f3679ba2ed09dcd43ac908c615f837bb";           // Holy Smite VFX
        public static readonly string Unholy = "872f843a900d8f442896e5fdae6d44d1";         // Unholy Blight VFX
        public static readonly string Divine = "0c07afb9ee854184cb5110891324e3ad";         // Divine Favor VFX
        public static readonly string Magic = "4ac47ddb9fa1eaf43a1b6809980cfbd2";          // Magic Missile spell (AssetId)

        // Special effect VFX
        public static readonly string SwarmInfest = "ae929793962242cabda8f409fea59bcb";   // Swarm-That-Walks Infest projectile

        private static readonly Dictionary<DamageEnergyType, string> _energyToEffectMap = new Dictionary<DamageEnergyType, string> {
            { DamageEnergyType.Electricity, Lightning },
            { DamageEnergyType.Fire, Fire },
            { DamageEnergyType.Cold, Cold },
            { DamageEnergyType.Acid, Acid },
            { DamageEnergyType.Sonic, Sonic },
            { DamageEnergyType.NegativeEnergy, NegativeEnergy },
            { DamageEnergyType.PositiveEnergy, PositiveEnergy },
            { DamageEnergyType.Holy, Holy },
            { DamageEnergyType.Unholy, Unholy },
            { DamageEnergyType.Divine, Divine },
            { DamageEnergyType.Magic, Magic }
        };

        public static string GetEffectForEnergyType(DamageEnergyType energyType) {
            return _energyToEffectMap.TryGetValue(energyType, out string effect) ? effect : Lightning;
        }
    }
}