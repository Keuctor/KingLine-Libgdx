﻿
using System.Collections.Generic;

public enum IType : int{ 
    RESOURCE,
    TOOL,
    HELMET,
    ARMOR,
    WEAPON,
}

public static class ItemRegistry
{
    private static Dictionary<int, IItemMaterial> Materials = new Dictionary<int, IItemMaterial>();
    static ItemRegistry()
    {
        Materials.Add((int)MaterialType.STONE, new ResourceItemMaterial("Stone"));
        Materials.Add((int)MaterialType.BONE, new ResourceItemMaterial("Bone"));
        Materials.Add((int)MaterialType.STONE_PICKAXE, new ToolItemMaterial("StonePickaxe", 1.25f));
        Materials.Add((int)MaterialType.IRON_PICKAXE, new ToolItemMaterial("IronPickaxe", 2.00f));
        Materials.Add((int)MaterialType.STEEL_PICKAXE, new ToolItemMaterial("SteelPickaxe", 3.5f));
        Materials.Add((int)MaterialType.PEASANT_CAP, new ArmorItemMaterial("PeasantCap", 10, EquipmentSlot.HELMET));
        Materials.Add((int)MaterialType.BONE_CLUP, new WeaponItemMaterial("BoneClub", 10));
        Materials.Add((int)MaterialType.PEASANT_CLOTHING, new ArmorItemMaterial("PeasantClothing", 10, EquipmentSlot.CHEST));
    }
    public static int GetMaterialId(MaterialType material)
    {
        return (int)material;
    }

    public static IItemMaterial GetItem(int id)
    {
        if (Materials.TryGetValue(id, out IItemMaterial material)) {
            return material;
        }
        return default;
    }
}