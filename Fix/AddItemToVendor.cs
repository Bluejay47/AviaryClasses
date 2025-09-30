using BlueprintCore.Utils;
using BlueprintCore.Blueprints.Configurators.Items;
using BlueprintCore.Blueprints.References;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Loot;
using System;
using AviaryClasses.Items;

namespace AviaryClasses.Fix {

  public class VendorPatch {

    private static readonly LogWrapper Logger = LogWrapper.Get("VendorPatch");

    public static void Configure() {

      try {
        // Create LootItem for the custom item
        var lootItem = new LootItem() {
          m_Item = BlueprintTool.GetRef<BlueprintItemReference>(TriceratopsStatuetteItemAlt.itemGuid)
        };

        // Use BlueprintCore's configurator to modify the equipment vendor table
        // This adds the item to Gemyl Hawkes or Joran Vahne's equipment inventory
        SharedVendorTableConfigurator.For(SharedVendorTableRefs.Equipment_DefendersHeartVendorTable)
          .AddLootItemsPackFixed(
            count: 1,
            item: lootItem
          )
          .Configure();

      } catch (Exception ex) {
        Logger.Error("Failed to add item to vendor: " + ex.ToString());
      }

    }
  }
}