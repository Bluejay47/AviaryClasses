using Kingmaker.Enums.Damage;
using System.Collections.Generic;

namespace AviaryClasses.Classes {
    public static class ElementalEffects {
        public static readonly string Lightning = "c31b3edbb9bde0e4c8ee5484b920f6dc"; // Lightning Bolt VFX
        public static readonly string Fire = "88029e10e5aa48444a15d238e4d0c87b";      // Ignition VFX
        public static readonly string Cold = "7980e876b0ef66d4fb2175d8aca27d95";      // Ray of Frost VFX
        public static readonly string Acid = "9a38d742801be084d89bd34318c600e8";      // Acid Splash VFX

        private static readonly Dictionary<DamageEnergyType, string> _energyToEffectMap = new Dictionary<DamageEnergyType, string> {
            { DamageEnergyType.Electricity, Lightning },
            { DamageEnergyType.Fire, Fire },
            { DamageEnergyType.Cold, Cold },
            { DamageEnergyType.Acid, Acid }
        };

        public static string GetEffectForEnergyType(DamageEnergyType energyType) {
            return _energyToEffectMap.TryGetValue(energyType, out string effect) ? effect : Lightning;
        }
    }
}