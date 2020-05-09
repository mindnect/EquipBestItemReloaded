﻿using Helpers;
using SandBox.GauntletUI;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Library;
using static TaleWorlds.Core.ItemObject;

namespace EquipBestItem
{
    public class EquipBestItemViewModel : ViewModel
    {

        public static InventoryGauntletScreen InventoryScreen { get; set; }

        public static bool IsHelmButtonEnabled { get; set; }
        public static bool IsCloakButtonEnabled { get; set; }
        public static bool IsArmorButtonEnabled { get; set; }
        public static bool IsGloveButtonEnabled { get; set; }
        public static bool IsBootButtonEnabled { get; set; }
        public static bool IsMountButtonEnabled { get; set; }
        public static bool IsHarnessButtonEnabled { get; set; }
        public static bool IsWeapon1ButtonEnabled { get; set; }
        public static bool IsWeapon2ButtonEnabled { get; set; }
        public static bool IsWeapon3ButtonEnabled { get; set; }
        public static bool IsWeapon4ButtonEnabled { get; set; }
        public static bool IsEquipCurrentCharacterButtonEnabled { get; set; }

        public static bool IsLeftPanelLocked { get; set; }
        public static bool IsRightPanelLocked { get; set; }
        public static List<string> ItemUsageList { get; private set; }

        public static bool IsInWarSet { get; set; }

        public static SPItemVM BestHelm;
        public static SPItemVM BestCloak;
        public static SPItemVM BestArmor;
        public static SPItemVM BestGlove;
        public static SPItemVM BestBoot;
        public static SPItemVM BestMount;
        public static SPItemVM BestHarness;
        public static SPItemVM BestWeapon1;
        public static SPItemVM BestWeapon2;
        public static SPItemVM BestWeapon3;
        public static SPItemVM BestWeapon4;

        private static CharacterSettings _characterSettings { get; set; }
        public static string CurrentCharacterName { get; set; }

        public static SPInventoryVM _inventory;

        public static bool NewGame;

        public static CharacterObject currentCharacter;

        public EquipBestItemViewModel()
        {
            _inventory = EquipBestItemViewModel.InventoryScreen.GetField("_dataSource") as SPInventoryVM;
            ItemUsageList = new List<string>();
            UpdateCurrentCharacterName();
        }

        public static string GetItemUsage(SPItemVM item)
        {
            if (item == null || item.ItemRosterElement.IsEmpty || item.ItemRosterElement.EquipmentElement.IsEmpty || item.ItemRosterElement.EquipmentElement.Item.WeaponComponent == null)
                return "";
            string value = item.ItemRosterElement.EquipmentElement.Item.PrimaryWeapon.ItemUsage;
            return value;
        }

        private static bool IsArmorBetter(SPItemVM currentItem, SPItemVM characterSlotItem, SPItemVM bestItem, CharacterSettings.ArmorSlot armorSlot)
        {
            return ArmorIndexCalculation(currentItem, armorSlot) > ArmorIndexCalculation(characterSlotItem, armorSlot) &&
                                            ArmorIndexCalculation(currentItem, armorSlot) > ArmorIndexCalculation(bestItem, armorSlot) &&
                                            ArmorIndexCalculation(currentItem, armorSlot) != 0f;
        }

        private static bool IsWeaponBetter(SPItemVM currentItem, SPItemVM characterSlotItem, SPItemVM bestItem, CharacterSettings.WeaponSlot weaponSlot)
        {
            if (characterSlotItem != null && !characterSlotItem.ItemRosterElement.IsEmpty)
                if (currentItem.ItemRosterElement.EquipmentElement.Item.WeaponComponent.PrimaryWeapon.WeaponClass == characterSlotItem.ItemRosterElement.EquipmentElement.Item.WeaponComponent.PrimaryWeapon.WeaponClass &&
                        GetItemUsage(currentItem) == GetItemUsage(characterSlotItem))
                    if (WeaponIndexCalculation(currentItem, weaponSlot) > WeaponIndexCalculation(characterSlotItem, weaponSlot) &&
                        WeaponIndexCalculation(currentItem, weaponSlot) > WeaponIndexCalculation(bestItem, weaponSlot) &&
                            WeaponIndexCalculation(currentItem, weaponSlot) != 0f)
                    {
                        return true;
                    }
            return false;
        }

        private static void FindBestItemsFromSide(MBBindingList<SPItemVM> inventory)
        {
            foreach (SPItemVM item in inventory)
            {
                if (IsCamelHarness(item) || IsCamel(item))
                    continue;
                if (_inventory.IsInWarSet)
                {
                    if (item.IsEquipableItem && item.CanCharacterUseItem)
                    {
                        if (item.ItemRosterElement.EquipmentElement.Item.ArmorComponent != null)
                        {
                            switch (item.ItemRosterElement.EquipmentElement.Item.ItemType)
                            {
                                case ItemTypeEnum.HeadArmor:
                                    {
                                        if (IsArmorBetter(item, _inventory.CharacterHelmSlot, BestHelm, CharacterSettings.ArmorSlot.Helm))
                                        {
                                            BestHelm = item;
                                        }
                                        break;
                                    }
                                case ItemTypeEnum.Cape:
                                    {
                                        if (IsArmorBetter(item, _inventory.CharacterCloakSlot, BestCloak, CharacterSettings.ArmorSlot.Cloak))
                                        {
                                            BestCloak = item;
                                        }
                                        break;
                                    }
                                case ItemTypeEnum.BodyArmor:
                                    {
                                        if (IsArmorBetter(item, _inventory.CharacterTorsoSlot, BestArmor, CharacterSettings.ArmorSlot.Armor))
                                        {
                                            BestArmor = item;
                                        }
                                        break;
                                    }
                                case ItemTypeEnum.HandArmor:
                                    {
                                        if (IsArmorBetter(item, _inventory.CharacterGloveSlot, BestGlove, CharacterSettings.ArmorSlot.Glove))
                                        {
                                            BestGlove = item;
                                        }
                                        break;
                                    }
                                case ItemTypeEnum.LegArmor:
                                    {
                                        if (IsArmorBetter(item, _inventory.CharacterBootSlot, BestBoot, CharacterSettings.ArmorSlot.Boot))
                                        {
                                            BestBoot = item;
                                        }
                                        break;
                                    }
                                case ItemTypeEnum.HorseHarness:
                                    {
                                        if (IsArmorBetter(item, _inventory.CharacterMountArmorSlot, BestHarness, CharacterSettings.ArmorSlot.Harness))
                                        {
                                            BestHarness = item;
                                        }
                                        break;
                                    }
                                default:
                                    break;
                            }
                        }
                        if (item.ItemRosterElement.EquipmentElement.Item.WeaponComponent != null)
                        {
                            if (IsWeaponBetter(item, _inventory.CharacterWeapon1Slot, BestWeapon1, CharacterSettings.WeaponSlot.Weapon1))
                            {
                                BestWeapon1 = item;
                            }
                            if (IsWeaponBetter(item, _inventory.CharacterWeapon2Slot, BestWeapon2, CharacterSettings.WeaponSlot.Weapon2))
                            {
                                BestWeapon2 = item;
                            }
                            if (IsWeaponBetter(item, _inventory.CharacterWeapon3Slot, BestWeapon3, CharacterSettings.WeaponSlot.Weapon3))
                            {
                                BestWeapon3 = item;
                            }
                            if (IsWeaponBetter(item, _inventory.CharacterWeapon4Slot, BestWeapon4, CharacterSettings.WeaponSlot.Weapon4))
                            {
                                BestWeapon4 = item;
                            }
                        }
                        if (item.ItemRosterElement.EquipmentElement.Item.HorseComponent != null)
                        {

                            if (MountIndexCalculation(item) > MountIndexCalculation(_inventory.CharacterMountSlot) &&
                                MountIndexCalculation(item) > MountIndexCalculation(BestMount) &&
                                            MountIndexCalculation(item) != 0f)
                            {
                                BestMount = item;
                            }
                        }
                    }
                }
                else
                {
                    if (item.IsEquipableItem && item.CanCharacterUseItem && item.IsCivilianItem)
                    {
                        if (item.ItemRosterElement.EquipmentElement.Item.ArmorComponent != null)
                        {
                            switch (item.ItemRosterElement.EquipmentElement.Item.ItemType)
                            {
                                case ItemTypeEnum.HeadArmor:
                                    {
                                        if (IsArmorBetter(item, _inventory.CharacterHelmSlot, BestHelm, CharacterSettings.ArmorSlot.Helm))
                                        {
                                            BestHelm = item;
                                        }
                                        break;
                                    }
                                case ItemTypeEnum.Cape:
                                    {
                                        if (IsArmorBetter(item, _inventory.CharacterCloakSlot, BestCloak, CharacterSettings.ArmorSlot.Cloak))
                                        {
                                            BestCloak = item;
                                        }
                                        break;
                                    }
                                case ItemTypeEnum.BodyArmor:
                                    {
                                        if (IsArmorBetter(item, _inventory.CharacterTorsoSlot, BestArmor, CharacterSettings.ArmorSlot.Armor))
                                        {
                                            BestArmor = item;
                                        }
                                        break;
                                    }
                                case ItemTypeEnum.HandArmor:
                                    {
                                        if (IsArmorBetter(item, _inventory.CharacterGloveSlot, BestGlove, CharacterSettings.ArmorSlot.Glove))
                                        {
                                            BestGlove = item;
                                        }
                                        break;
                                    }
                                case ItemTypeEnum.LegArmor:
                                    {
                                        if (IsArmorBetter(item, _inventory.CharacterBootSlot, BestBoot, CharacterSettings.ArmorSlot.Boot))
                                        {
                                            BestBoot = item;
                                        }
                                        break;
                                    }
                                case ItemTypeEnum.HorseHarness:
                                    {
                                        if (IsArmorBetter(item, _inventory.CharacterMountArmorSlot, BestHarness, CharacterSettings.ArmorSlot.Harness))
                                        {
                                            BestHarness = item;
                                        }
                                        break;
                                    }
                                default:
                                    break;
                            }
                        }
                        if (item.ItemRosterElement.EquipmentElement.Item.WeaponComponent != null)
                        {
                            if (IsWeaponBetter(item, _inventory.CharacterWeapon1Slot, BestWeapon1, CharacterSettings.WeaponSlot.Weapon1))
                            {
                                BestWeapon1 = item;
                            }
                            if (IsWeaponBetter(item, _inventory.CharacterWeapon2Slot, BestWeapon2, CharacterSettings.WeaponSlot.Weapon2))
                            {
                                BestWeapon2 = item;
                            }
                            if (IsWeaponBetter(item, _inventory.CharacterWeapon3Slot, BestWeapon3, CharacterSettings.WeaponSlot.Weapon3))
                            {
                                BestWeapon3 = item;
                            }
                            if (IsWeaponBetter(item, _inventory.CharacterWeapon4Slot, BestWeapon4, CharacterSettings.WeaponSlot.Weapon4))
                            {
                                BestWeapon4 = item;
                            }
                        }
                        if (item.ItemRosterElement.EquipmentElement.Item.HorseComponent != null)
                        {

                            if (MountIndexCalculation(item) > MountIndexCalculation(_inventory.CharacterMountSlot) &&
                                MountIndexCalculation(item) > MountIndexCalculation(BestMount) &&
                                            MountIndexCalculation(item) != 0f)
                            {
                                BestMount = item;
                            }
                        }
                    }
                }
            }
        }

        public static void FindBestItems()
        {
            _characterSettings = SettingsLoader.Instance.GetCharacterSettingsByName(CurrentCharacterName);

            NullBestItems();

            if (!SettingsLoader.Instance.Settings.IsRightPanelLocked)
            {
                FindBestItemsFromSide(_inventory.RightItemListVM);
            }
            if (!SettingsLoader.Instance.Settings.IsLeftPanelLocked)
            {
                FindBestItemsFromSide(_inventory.LeftItemListVM);
            }

            ButtonStatusUpdate();
        }

        private static bool IsCamel(SPItemVM item)
        {
            if (item != null)
                if (!item.ItemRosterElement.IsEmpty)
                    if (!item.ItemRosterElement.EquipmentElement.IsEmpty)
                        if (item.ItemRosterElement.EquipmentElement.Item.HasHorseComponent)
                            if (item.ItemRosterElement.EquipmentElement.Item.HorseComponent.Monster.MonsterUsage == "camel")
                                return true;
            return false;
        }

        private static bool IsCamelHarness(SPItemVM item)
        {
            if (item != null && item.StringId.StartsWith("camel_sadd"))
                return true;
            return false;
        }

        public static void ButtonStatusUpdate()
        {
            if (SettingsLoader.Instance.Settings.IsEnabledStandardButtons)
            {
                IsHelmButtonEnabled = (BestHelm != null);
                IsCloakButtonEnabled = (BestCloak != null);
                IsArmorButtonEnabled = (BestArmor != null);
                IsGloveButtonEnabled = (BestGlove != null);
                IsBootButtonEnabled = (BestBoot != null);
                IsMountButtonEnabled = (BestMount != null);
                IsHarnessButtonEnabled = (BestHarness != null) && (!IsCamel(_inventory.CharacterMountSlot));
                IsWeapon1ButtonEnabled = (BestWeapon1 != null);
                IsWeapon2ButtonEnabled = (BestWeapon2 != null);
                IsWeapon3ButtonEnabled = (BestWeapon3 != null);
                IsWeapon4ButtonEnabled = (BestWeapon4 != null);
            }
        }

        static void NullBestItems()
        {
            BestArmor = null;
            BestBoot = null;
            BestCloak = null;
            BestGlove = null;
            BestHarness = null;
            BestHelm = null;
            BestMount = null;
            BestWeapon1 = null;
            BestWeapon2 = null;
            BestWeapon3 = null;
            BestWeapon4 = null;
        }

        public static void UpdateCurrentCharacterName()
        {
            if (_inventory.CurrentCharacterName != null)
                CurrentCharacterName = _inventory.CurrentCharacterName;
            else
                CurrentCharacterName = Hero.MainHero.Name.ToString();
        }

        public static void EquipEveryCharacter()
        {
            foreach (TroopRosterElement rosterElement in InventoryManager.MyInventoryLogic.RightMemberRoster)
            {
                if (rosterElement.Character.IsHero)
                    EquipCharacter(rosterElement.Character);
            }
        }

        public static void EquipCharacterEquipment(CharacterObject character, Equipment equipment, bool isCivilian)
        {
            for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumEquipmentSetSlots; equipmentIndex++)
            {
                if (equipment[equipmentIndex].IsEmpty && equipmentIndex < EquipmentIndex.NonWeaponItemBeginSlot ||
                    equipment[EquipmentIndex.Horse].IsEmpty && equipmentIndex == EquipmentIndex.HorseHarness)
                    continue;

                EquipmentElement bestLeftEquipmentElement;
                EquipmentElement bestRightEquipmentElement;

                if (!SettingsLoader.Instance.Settings.IsLeftPanelLocked)
                {
                    bestLeftEquipmentElement = GetBetterItemFromSide(_inventory.LeftItemListVM, equipment[equipmentIndex], equipmentIndex, isCivilian, character);
                }
                if (!SettingsLoader.Instance.Settings.IsRightPanelLocked)
                {
                    bestRightEquipmentElement = GetBetterItemFromSide(_inventory.RightItemListVM, equipment[equipmentIndex], equipmentIndex, isCivilian, character);
                }

                if (!equipment[equipmentIndex].IsEmpty && (bestLeftEquipmentElement.Item != null || bestRightEquipmentElement.Item != null))
                {
                    TransferCommand transferCommand = TransferCommand.Transfer(
                        1,
                        InventoryLogic.InventorySide.Equipment,
                        InventoryLogic.InventorySide.PlayerInventory,
                        new ItemRosterElement(equipment[equipmentIndex], 1),
                        equipmentIndex,
                        EquipmentIndex.None,
                        character,
                        isCivilian
                    );
                    InventoryManager.MyInventoryLogic.AddTransferCommand(transferCommand);
                }



                if (bestLeftEquipmentElement.Item != null || bestRightEquipmentElement.Item != null)
                    if (ItemIndexCalculation(bestLeftEquipmentElement, equipmentIndex) > ItemIndexCalculation(bestRightEquipmentElement, equipmentIndex))
                    {
                        TransferCommand equipCommand = TransferCommand.Transfer(
                            1,
                            InventoryLogic.InventorySide.OtherInventory,
                            InventoryLogic.InventorySide.Equipment,
                            new ItemRosterElement(bestLeftEquipmentElement, 1),
                            EquipmentIndex.None,
                            equipmentIndex,
                            character,
                            isCivilian
                        );

                        EquipMessage(equipmentIndex, character);
                        InventoryManager.MyInventoryLogic.AddTransferCommand(equipCommand);
                    }
                    else
                    {
                        TransferCommand equipCommand = TransferCommand.Transfer(
                            1,
                            InventoryLogic.InventorySide.PlayerInventory,
                            InventoryLogic.InventorySide.Equipment,
                            new ItemRosterElement(bestRightEquipmentElement, 1),
                            EquipmentIndex.None,
                            equipmentIndex,
                            character,
                            isCivilian
                        );

                        EquipMessage(equipmentIndex, character);
                        InventoryManager.MyInventoryLogic.AddTransferCommand(equipCommand);
                    }
                _inventory.GetMethod("ExecuteRemoveZeroCounts");
            }
            _inventory.GetMethod("RefreshInformationValues");
        }

        public static void EquipCharacter(CharacterObject character)
        {
            _characterSettings = SettingsLoader.Instance.GetCharacterSettingsByName(character.Name.ToString());

            if (_inventory.IsInWarSet)
            {
                Equipment battleEquipment = character.FirstBattleEquipment;
                EquipCharacterEquipment(character, battleEquipment, false);
            }
            else
            {
                Equipment civilEquipment = character.FirstCivilianEquipment;
                EquipCharacterEquipment(character, civilEquipment, true);
            }
        }

        public static void EquipMessage(EquipmentIndex equipmentIndex, CharacterObject character)
        {
            switch (equipmentIndex)
            {
                case EquipmentIndex.Weapon0:
                    InformationManager.DisplayMessage(new InformationMessage(character.Name + " equips weapon in the first slot"));
                    break;
                case EquipmentIndex.Weapon1:
                    InformationManager.DisplayMessage(new InformationMessage(character.Name + " equips weapon in the second slot"));
                    break;
                case EquipmentIndex.Weapon2:
                    InformationManager.DisplayMessage(new InformationMessage(character.Name + " equips weapon in the third slot"));
                    break;
                case EquipmentIndex.Weapon3:
                    InformationManager.DisplayMessage(new InformationMessage(character.Name + " equips weapon in the fourth slot"));
                    break;
                case EquipmentIndex.Head:
                    InformationManager.DisplayMessage(new InformationMessage(character.Name + " equips helmet"));
                    break;
                case EquipmentIndex.Body:
                    InformationManager.DisplayMessage(new InformationMessage(character.Name + " equips body armor"));
                    break;
                case EquipmentIndex.Leg:
                    InformationManager.DisplayMessage(new InformationMessage(character.Name + " equips boots"));
                    break;
                case EquipmentIndex.Gloves:
                    InformationManager.DisplayMessage(new InformationMessage(character.Name + " equips gloves"));
                    break;
                case EquipmentIndex.Cape:
                    InformationManager.DisplayMessage(new InformationMessage(character.Name + " equips cape"));
                    break;
                case EquipmentIndex.Horse:
                    InformationManager.DisplayMessage(new InformationMessage(character.Name + " equips horse"));
                    break;
                case EquipmentIndex.HorseHarness:
                    InformationManager.DisplayMessage(new InformationMessage(character.Name + " equips horse harness"));
                    break;
                default:
                    break;
            }
        }

        public static EquipmentElement GetBetterItemFromSide(MBBindingList<SPItemVM> itemListVM, EquipmentElement equipmentElement, EquipmentIndex slot, bool isCivilian, CharacterObject character)
        {
            EquipmentElement bestEquipmentElement;

            foreach (SPItemVM item in itemListVM)
            {
                if (IsCamel(item) || IsCamelHarness(item))
                    continue;
                if (isCivilian)
                {
                    if (slot < EquipmentIndex.NonWeaponItemBeginSlot &&
                        item.ItemRosterElement.EquipmentElement.Item.PrimaryWeapon != null &&
                        item.IsEquipableItem &&
                        item.IsCivilianItem &&
                        CharacterHelper.CanUseItem(character, item.ItemRosterElement.EquipmentElement)
                        )
                    {
                        if (equipmentElement.Item.WeaponComponent.PrimaryWeapon.WeaponClass == item.ItemRosterElement.EquipmentElement.Item.PrimaryWeapon.WeaponClass &&
                            GetItemUsage(item) == equipmentElement.Item.PrimaryWeapon.ItemUsage)
                            if (bestEquipmentElement.IsEmpty)
                                if (ItemIndexCalculation(item.ItemRosterElement.EquipmentElement, slot) > ItemIndexCalculation(equipmentElement, slot) &&
                                    ItemIndexCalculation(item.ItemRosterElement.EquipmentElement, slot) != 0f)
                                    bestEquipmentElement = item.ItemRosterElement.EquipmentElement;
                                else
                                    continue;
                            else
                                if (ItemIndexCalculation(item.ItemRosterElement.EquipmentElement, slot) > ItemIndexCalculation(bestEquipmentElement, slot) &&
                                    ItemIndexCalculation(item.ItemRosterElement.EquipmentElement, slot) != 0f)
                                bestEquipmentElement = item.ItemRosterElement.EquipmentElement;
                    }
                    else if (item.ItemType == slot && item.IsEquipableItem && item.IsCivilianItem &&
                        CharacterHelper.CanUseItem(character, item.ItemRosterElement.EquipmentElement))
                        if (bestEquipmentElement.IsEmpty)
                            if (ItemIndexCalculation(item.ItemRosterElement.EquipmentElement, slot) > ItemIndexCalculation(equipmentElement, slot) &&
                                ItemIndexCalculation(item.ItemRosterElement.EquipmentElement, slot) != 0f)
                                bestEquipmentElement = item.ItemRosterElement.EquipmentElement;
                            else
                                continue;
                        else
                            if (ItemIndexCalculation(item.ItemRosterElement.EquipmentElement, slot) > ItemIndexCalculation(bestEquipmentElement, slot) &&
                                ItemIndexCalculation(item.ItemRosterElement.EquipmentElement, slot) != 0f)
                            bestEquipmentElement = item.ItemRosterElement.EquipmentElement;
                }
                else
                {
                    if (slot < EquipmentIndex.NonWeaponItemBeginSlot && item.ItemRosterElement.EquipmentElement.Item.PrimaryWeapon != null && item.IsEquipableItem &&
                        CharacterHelper.CanUseItem(character, item.ItemRosterElement.EquipmentElement))
                    {
                        if (equipmentElement.Item.WeaponComponent.PrimaryWeapon.WeaponClass == item.ItemRosterElement.EquipmentElement.Item.PrimaryWeapon.WeaponClass &&
                            GetItemUsage(item) == equipmentElement.Item.PrimaryWeapon.ItemUsage)
                            if (bestEquipmentElement.IsEmpty)
                                if (ItemIndexCalculation(item.ItemRosterElement.EquipmentElement, slot) > ItemIndexCalculation(equipmentElement, slot) &&
                                    ItemIndexCalculation(item.ItemRosterElement.EquipmentElement, slot) != 0f)
                                    bestEquipmentElement = item.ItemRosterElement.EquipmentElement;
                                else
                                    continue;
                            else
                                if (ItemIndexCalculation(item.ItemRosterElement.EquipmentElement, slot) > ItemIndexCalculation(bestEquipmentElement, slot) &&
                                    ItemIndexCalculation(item.ItemRosterElement.EquipmentElement, slot) != 0f)
                                bestEquipmentElement = item.ItemRosterElement.EquipmentElement;
                    }
                    else if (item.ItemType == slot && item.IsEquipableItem &&
                        CharacterHelper.CanUseItem(character, item.ItemRosterElement.EquipmentElement))
                        if (bestEquipmentElement.IsEmpty)
                            if (ItemIndexCalculation(item.ItemRosterElement.EquipmentElement, slot) > ItemIndexCalculation(equipmentElement, slot) &&
                                ItemIndexCalculation(item.ItemRosterElement.EquipmentElement, slot) != 0f)
                                bestEquipmentElement = item.ItemRosterElement.EquipmentElement;
                            else
                                continue;
                        else
                            if (ItemIndexCalculation(item.ItemRosterElement.EquipmentElement, slot) > ItemIndexCalculation(bestEquipmentElement, slot) &&
                                ItemIndexCalculation(item.ItemRosterElement.EquipmentElement, slot) != 0f)
                            bestEquipmentElement = item.ItemRosterElement.EquipmentElement;
                }
            }

            return bestEquipmentElement;
        }

        //public static EquipmentElement GetBetterItem(EquipmentElement equipmentElement, EquipmentIndex slot)
        //{
        //    EquipmentElement bestEquipmentElement;

        //    if (!SettingsLoader.Instance.Settings.IsLeftPanelLocked)
        //    {
        //        bestEquipmentElement = GetBetterItemFromSide(_inventory.LeftItemListVM, equipmentElement, slot, bestEquipmentElement);
        //    }
        //    if (!SettingsLoader.Instance.Settings.IsRightPanelLocked)
        //    {
        //        bestEquipmentElement = GetBetterItemFromSide(_inventory.RightItemListVM, equipmentElement, slot, bestEquipmentElement);
        //    }

        //    return bestEquipmentElement;
        //}

        public static int GetEquipmentSlot(EquipmentIndex slot)
        {
            switch (slot)
            {
                case EquipmentIndex.Weapon0:
                    return 0;
                case EquipmentIndex.Weapon1:
                    return 1;
                case EquipmentIndex.Weapon2:
                    return 2;
                case EquipmentIndex.Weapon3:
                    return 3;
                case EquipmentIndex.Head:
                    return 0;
                case EquipmentIndex.Cape:
                    return 1;
                case EquipmentIndex.Body:
                    return 2;
                case EquipmentIndex.Gloves:
                    return 3;
                case EquipmentIndex.Leg:
                    return 4;
                case EquipmentIndex.Horse:
                    return 0;
                case EquipmentIndex.HorseHarness:
                    return 5;
                default:
                    return 0;
            }
        }

        private static float ItemIndexCalculation(EquipmentElement sourceItem, EquipmentIndex slot)
        {

            if (sourceItem.IsEmpty)
                return -9999f;

            float value = 0f;

            if (sourceItem.Item.HasArmorComponent)
            {
                ArmorComponent armorComponentItem = sourceItem.Item.ArmorComponent;
                FilterArmorSettings filterArmor = _characterSettings.FilterArmor[GetEquipmentSlot(slot)];

                float sum =
                    Math.Abs(filterArmor.HeadArmor) +
                    Math.Abs(filterArmor.ArmArmor) +
                    Math.Abs(filterArmor.ArmorBodyArmor) +
                    Math.Abs(filterArmor.ArmorWeight) +
                    Math.Abs(filterArmor.LegArmor);

                ItemModifier mod =
                    sourceItem.ItemModifier;

                int HeadArmor = armorComponentItem.HeadArmor,
                    BodyArmor = armorComponentItem.BodyArmor,
                    LegArmor = armorComponentItem.LegArmor,
                    ArmArmor = armorComponentItem.ArmArmor;
                float Weight = sourceItem.Weight;

                if (mod != null)
                {
                    HeadArmor = mod.ModifyArmor(HeadArmor);
                    BodyArmor = mod.ModifyArmor(BodyArmor);
                    LegArmor = mod.ModifyArmor(LegArmor);
                    ArmArmor = mod.ModifyArmor(ArmArmor);
                    //Weight *= mod.WeightMultiplier;
                }

                value = (
                    HeadArmor * filterArmor.HeadArmor +
                    BodyArmor * filterArmor.ArmorBodyArmor +
                    LegArmor * filterArmor.LegArmor +
                    ArmArmor * filterArmor.ArmArmor +
                    Weight * filterArmor.ArmorWeight
                ) / sum;

#if DEBUG
                InformationManager.DisplayMessage(new InformationMessage(String.Format("{0}: HA {1}, BA {2}, LA {3}, AA {4}, W {5}",
                                sourceItem.Item.Name, HeadArmor, BodyArmor, LegArmor, ArmArmor, Weight)));

                InformationManager.DisplayMessage(new InformationMessage("Total score: " + value)); 
#endif

                return value;
            }

            if (sourceItem.Item.PrimaryWeapon != null)
            {
                WeaponComponentData primaryWeaponItem = sourceItem.Item.PrimaryWeapon;
                FilterWeaponSettings filterWeapon = _characterSettings.FilterWeapon[GetEquipmentSlot(slot)];
                float sum =
                    Math.Abs(filterWeapon.Accuracy) +
                    Math.Abs(filterWeapon.WeaponBodyArmor) +
                    Math.Abs(filterWeapon.Handling) +
                    Math.Abs(filterWeapon.MaxDataValue) +
                    Math.Abs(filterWeapon.MissileSpeed) +
                    Math.Abs(filterWeapon.SwingDamage) +
                    Math.Abs(filterWeapon.SwingSpeed) +
                    Math.Abs(filterWeapon.ThrustDamage) +
                    Math.Abs(filterWeapon.ThrustSpeed) +
                    Math.Abs(filterWeapon.WeaponLength) +
                    Math.Abs(filterWeapon.WeaponWeight);

                int Accuracy = primaryWeaponItem.Accuracy,
                    BodyArmor = primaryWeaponItem.BodyArmor,
                    Handling = primaryWeaponItem.Handling,
                    MaxDataValue = primaryWeaponItem.MaxDataValue,
                    MissileSpeed = primaryWeaponItem.MissileSpeed,
                    SwingDamage = primaryWeaponItem.SwingDamage,
                    SwingSpeed = primaryWeaponItem.SwingSpeed,
                    ThrustDamage = primaryWeaponItem.ThrustDamage,
                    ThrustSpeed = primaryWeaponItem.ThrustSpeed,
                    WeaponLength = primaryWeaponItem.WeaponLength;
                float WeaponWeight = sourceItem.Weight;

                ItemModifier mod = sourceItem.ItemModifier;
                if (mod != null)
                {
                    BodyArmor = mod.ModifyArmor(BodyArmor);
                    MissileSpeed = mod.ModifyMissileSpeed(MissileSpeed);
                    SwingDamage = mod.ModifyDamage(SwingDamage);
                    SwingSpeed = mod.ModifySpeed(SwingSpeed);
                    ThrustDamage = mod.ModifyDamage(ThrustDamage);
                    ThrustSpeed = mod.ModifySpeed(ThrustSpeed);
                    MaxDataValue += mod.HitPoints;
                    //WeaponWeight *= mod.WeightMultiplier;

                }

                var weights = _characterSettings.FilterWeapon[GetEquipmentSlot(slot)];
                value = (
                    Accuracy * weights.Accuracy +
                    BodyArmor * weights.WeaponBodyArmor +
                    Handling * weights.Handling +
                    MaxDataValue * weights.MaxDataValue +
                    MissileSpeed * weights.MissileSpeed +
                    SwingDamage * weights.SwingDamage +
                    SwingSpeed * weights.SwingSpeed +
                    ThrustDamage * weights.ThrustDamage +
                    ThrustSpeed * weights.ThrustSpeed +
                    WeaponLength * weights.WeaponLength +
                    WeaponWeight * weights.WeaponWeight
                ) / sum;


#if DEBUG
                InformationManager.DisplayMessage(new InformationMessage(String.Format("{0}: Acc {1}, BA {2}, HL {3}, HP {4}, MS {5}, SD {6}, SS {7}, TD {8}, TS {9}, WL {10}, W {11}",
                                sourceItem.Item.Name, Accuracy, BodyArmor, Handling, MaxDataValue, MissileSpeed, SwingDamage, SwingSpeed, ThrustDamage, ThrustSpeed, WeaponLength, WeaponWeight)));

                InformationManager.DisplayMessage(new InformationMessage("Total score: " + value)); 
#endif

                return value;
            }

            if (sourceItem.Item.HasHorseComponent)
            {
                HorseComponent horseComponentItem = sourceItem.Item.HorseComponent;
                FilterMountSettings filterMount = _characterSettings.FilterMount;

                float sum =
                    Math.Abs(filterMount.ChargeDamage) +
                    Math.Abs(filterMount.HitPoints) +
                    Math.Abs(filterMount.Maneuver) +
                    Math.Abs(filterMount.Speed);

                int ChargeDamage = horseComponentItem.ChargeDamage,
                    HitPoints = horseComponentItem.HitPoints,
                    Maneuver = horseComponentItem.Maneuver,
                    Speed = horseComponentItem.Speed;

                ItemModifier mod =
                    sourceItem.ItemModifier;
                if (mod != null)
                {
                    ChargeDamage = mod.ModifyHorseCharge(ChargeDamage);
                    Maneuver = mod.ModifyHorseManuever(Maneuver);
                    Speed = mod.ModifyHorseSpeed(Speed);
                }

                var weights = _characterSettings.FilterMount;
                value = (
                    ChargeDamage * weights.ChargeDamage +
                    HitPoints * weights.HitPoints +
                    Maneuver * weights.Maneuver +
                    Speed * weights.Speed
                ) / sum;


#if DEBUG
                InformationManager.DisplayMessage(new InformationMessage(String.Format("{0}: CD {1}, HP {2}, MR {3}, SD {4}",
                                sourceItem.Item.Name, ChargeDamage, HitPoints, Maneuver, Speed)));

                InformationManager.DisplayMessage(new InformationMessage("Total score: " + value)); 
#endif

                return value;
            }

            return value;
        }



        private static float WeaponIndexCalculation(SPItemVM item1, int slotNumber)
        {
            if (item1 == null ||
                item1.ItemRosterElement.IsEmpty ||
                item1.ItemRosterElement.EquipmentElement.IsEmpty ||
                item1.ItemRosterElement.EquipmentElement.Item.PrimaryWeapon == null)
                return -9999f;

            WeaponComponentData primaryWeaponItem1 = item1.ItemRosterElement.EquipmentElement.Item.PrimaryWeapon;
            FilterWeaponSettings filterWeapon = _characterSettings.FilterWeapon[slotNumber];
            float sum =
                Math.Abs(filterWeapon.Accuracy) +
                Math.Abs(filterWeapon.WeaponBodyArmor) +
                Math.Abs(filterWeapon.Handling) +
                Math.Abs(filterWeapon.MaxDataValue) +
                Math.Abs(filterWeapon.MissileSpeed) +
                Math.Abs(filterWeapon.SwingDamage) +
                Math.Abs(filterWeapon.SwingSpeed) +
                Math.Abs(filterWeapon.ThrustDamage) +
                Math.Abs(filterWeapon.ThrustSpeed) +
                Math.Abs(filterWeapon.WeaponLength) +
                Math.Abs(filterWeapon.WeaponWeight);

            int Accuracy = primaryWeaponItem1.Accuracy,
                BodyArmor = primaryWeaponItem1.BodyArmor,
                Handling = primaryWeaponItem1.Handling,
                MaxDataValue = primaryWeaponItem1.MaxDataValue,
                MissileSpeed = primaryWeaponItem1.MissileSpeed,
                SwingDamage = primaryWeaponItem1.SwingDamage,
                SwingSpeed = primaryWeaponItem1.SwingSpeed,
                ThrustDamage = primaryWeaponItem1.ThrustDamage,
                ThrustSpeed = primaryWeaponItem1.ThrustSpeed,
                WeaponLength = primaryWeaponItem1.WeaponLength;
            float WeaponWeight = item1.ItemRosterElement.EquipmentElement.Weight;

            ItemModifier mod = item1.ItemRosterElement.EquipmentElement.ItemModifier;
            if (mod != null)
            {
                BodyArmor = mod.ModifyArmor(BodyArmor);
                MissileSpeed = mod.ModifyMissileSpeed(MissileSpeed);
                SwingDamage = mod.ModifyDamage(SwingDamage);
                SwingSpeed = mod.ModifySpeed(SwingSpeed);
                ThrustDamage = mod.ModifyDamage(ThrustDamage);
                ThrustSpeed = mod.ModifySpeed(ThrustSpeed);
                MaxDataValue += mod.HitPoints;
                //WeaponWeight *= mod.WeightMultiplier;

            }

            var weights = _characterSettings.FilterWeapon[slotNumber];
            float value = (
                Accuracy * weights.Accuracy +
                BodyArmor * weights.WeaponBodyArmor +
                Handling * weights.Handling +
                MaxDataValue * weights.MaxDataValue +
                MissileSpeed * weights.MissileSpeed +
                SwingDamage * weights.SwingDamage +
                SwingSpeed * weights.SwingSpeed +
                ThrustDamage * weights.ThrustDamage +
                ThrustSpeed * weights.ThrustSpeed +
                WeaponLength * weights.WeaponLength +
                WeaponWeight * weights.WeaponWeight
            ) / sum;

#if DEBUG
            InformationManager.DisplayMessage(new InformationMessage(String.Format("{0}: Acc {1}, BA {2}, HL {3}, HP {4}, MS {5}, SD {6}, SS {7}, TD {8}, TS {9}, WL {10}, W {11}",
                        item1.ItemDescription, Accuracy, BodyArmor, Handling, MaxDataValue, MissileSpeed, SwingDamage, SwingSpeed, ThrustDamage, ThrustSpeed, WeaponLength, WeaponWeight)));

            InformationManager.DisplayMessage(new InformationMessage("Total score: " + value)); 
#endif

            return value;
        }

        private static float WeaponIndexCalculation(SPItemVM item1, CharacterSettings.WeaponSlot weaponSlot)
        {
            return WeaponIndexCalculation(item1, (int)weaponSlot);
        }

        private static float ArmorIndexCalculation(SPItemVM item1, int slotNumber)
        {
            if (item1 == null ||
                item1.ItemRosterElement.IsEmpty ||
                item1.ItemRosterElement.EquipmentElement.IsEmpty ||
                item1.ItemRosterElement.EquipmentElement.Item.ArmorComponent == null)
                return -9999f;

            ArmorComponent armorComponentItem1 = item1.ItemRosterElement.EquipmentElement.Item.ArmorComponent;
            FilterArmorSettings filterArmor = _characterSettings.FilterArmor[slotNumber];

            float sum =
                Math.Abs(filterArmor.HeadArmor) +
                Math.Abs(filterArmor.ArmArmor) +
                Math.Abs(filterArmor.ArmorBodyArmor) +
                Math.Abs(filterArmor.ArmorWeight) +
                Math.Abs(filterArmor.LegArmor);

            ItemModifier mod =
                item1.ItemRosterElement.EquipmentElement.ItemModifier;

            int HeadArmor = armorComponentItem1.HeadArmor,
                BodyArmor = armorComponentItem1.BodyArmor,
                LegArmor = armorComponentItem1.LegArmor,
                ArmArmor = armorComponentItem1.ArmArmor;
            float Weight = item1.ItemRosterElement.EquipmentElement.Weight;

            if (mod != null)
            {
                HeadArmor = mod.ModifyArmor(HeadArmor);
                BodyArmor = mod.ModifyArmor(BodyArmor);
                LegArmor = mod.ModifyArmor(LegArmor);
                ArmArmor = mod.ModifyArmor(ArmArmor);
                //Weight *= mod.WeightMultiplier;
            }

            float value = (
                HeadArmor * filterArmor.HeadArmor +
                BodyArmor * filterArmor.ArmorBodyArmor +
                LegArmor * filterArmor.LegArmor +
                ArmArmor * filterArmor.ArmArmor +
                Weight * filterArmor.ArmorWeight
            ) / sum;

#if DEBUG
            InformationManager.DisplayMessage(new InformationMessage(String.Format("{0}: HA {1}, BA {2}, LA {3}, AA {4}, W {5}",
                        item1.ItemDescription, HeadArmor, BodyArmor, LegArmor, ArmArmor, Weight)));

            InformationManager.DisplayMessage(new InformationMessage("Total score: " + value)); 
#endif

            return value;
        }

        private static float ArmorIndexCalculation(SPItemVM item1, CharacterSettings.ArmorSlot armorSlot)
        {
            return ArmorIndexCalculation(item1, (int)armorSlot);
        }

        private static float MountIndexCalculation(SPItemVM item1)
        {
            if (item1 == null ||
                item1.ItemRosterElement.IsEmpty ||
                item1.ItemRosterElement.EquipmentElement.IsEmpty ||
                item1.ItemRosterElement.EquipmentElement.Item.HorseComponent == null)
                return -9999f;

            HorseComponent horseComponentItem1 = item1.ItemRosterElement.EquipmentElement.Item.HorseComponent;
            FilterMountSettings filterMount = _characterSettings.FilterMount;

            float sum =
                Math.Abs(filterMount.ChargeDamage) +
                Math.Abs(filterMount.HitPoints) +
                Math.Abs(filterMount.Maneuver) +
                Math.Abs(filterMount.Speed);

            int ChargeDamage = horseComponentItem1.ChargeDamage,
                HitPoints = horseComponentItem1.HitPoints,
                Maneuver = horseComponentItem1.Maneuver,
                Speed = horseComponentItem1.Speed;

            ItemModifier mod =
                item1.ItemRosterElement.EquipmentElement.ItemModifier;
            if (mod != null)
            {
                ChargeDamage = mod.ModifyHorseCharge(ChargeDamage);
                Maneuver = mod.ModifyHorseManuever(Maneuver);
                Speed = mod.ModifyHorseSpeed(Speed);
            }

            var weights = _characterSettings.FilterMount;
            float value = (
                ChargeDamage * weights.ChargeDamage +
                HitPoints * weights.HitPoints +
                Maneuver * weights.Maneuver +
                Speed * weights.Speed
            ) / sum;

#if DEBUG
            InformationManager.DisplayMessage(new InformationMessage(String.Format("{0}: CD {1}, HP {2}, MR {3}, SD {4}",
                        item1.ItemDescription, ChargeDamage, HitPoints, Maneuver, Speed)));

            InformationManager.DisplayMessage(new InformationMessage("Total score: " + value)); 
#endif

            return value;
        }

        public override void RefreshValues()
        {
            base.RefreshValues();
        }

        public static CharacterObject GetCharacterByName(string name)
        {
            foreach (TroopRosterElement rosterElement in InventoryManager.MyInventoryLogic.RightMemberRoster)
            {
                if (rosterElement.Character.IsHero && rosterElement.Character.Name.ToString() == name)
                    return rosterElement.Character;
            }
            return null;
        }

        public static void UpdateValues()
        {
            FindBestItems();

#if DEBUG
            InformationManager.DisplayMessage(new InformationMessage("EBIViewModel UpdateValue() " + Game.Current.ApplicationTime.ToString())); 
#endif
        }

        public static void EquipBestHelm()
        {
            _inventory.GetMethod("ProcessEquipItem", BestHelm);
            BestHelm = null;
            //this.RefreshValues();
        }

        public static void EquipBestCloak()
        {
            _inventory.GetMethod("ProcessEquipItem", BestCloak);
            BestCloak = null;
            //this.RefreshValues();
        }

        public static void EquipBestArmor()
        {
            _inventory.GetMethod("ProcessEquipItem", BestArmor);
            BestArmor = null;
            //this.RefreshValues();
        }

        public static void EquipBestGlove()
        {
            _inventory.GetMethod("ProcessEquipItem", BestGlove);
            BestGlove = null;
            //this.RefreshValues();
        }
        public static void EquipBestBoot()
        {
            _inventory.GetMethod("ProcessEquipItem", BestBoot);
            BestBoot = null;
            //this.RefreshValues();
        }

        public static void EquipBestMount()
        {
            _inventory.GetMethod("ProcessEquipItem", BestMount);
            BestMount = null;
            //this.RefreshValues();
        }
        public static void EquipBestHarness()
        {
            _inventory.GetMethod("ProcessEquipItem", BestHarness);
            BestHarness = null;
            //this.RefreshValues();
        }

        public static void EquipBestWeapon1()
        {
            _inventory.GetMethod("UnequipEquipment", _inventory.CharacterWeapon1Slot);
            _inventory.GetMethod("ProcessEquipItem", BestWeapon1);
            BestWeapon1 = null;
            UpdateValues();
        }
        public static void EquipBestWeapon2()
        {
            _inventory.GetMethod("UnequipEquipment", _inventory.CharacterWeapon2Slot);
            _inventory.GetMethod("ProcessEquipItem", BestWeapon2);
            BestWeapon2 = null;
            UpdateValues();
        }

        public static void EquipBestWeapon3()
        {
            _inventory.GetMethod("UnequipEquipment", _inventory.CharacterWeapon3Slot);
            _inventory.GetMethod("ProcessEquipItem", BestWeapon3);
            BestWeapon3 = null;
            UpdateValues();
        }
        public static void EquipBestWeapon4()
        {
            _inventory.GetMethod("UnequipEquipment", _inventory.CharacterWeapon4Slot);
            _inventory.GetMethod("ProcessEquipItem", BestWeapon4);
            BestWeapon4 = null;
            UpdateValues();
        }

        public static bool IsAllBestItemsNull()
        {
            if (BestHelm != null)
            {
                return false;
            }
            if (BestCloak != null)
            {
                return false;
            }
            if (BestArmor != null)
            {
                return false;
            }
            if (BestGlove != null)
            {
                return false;
            }
            if (BestBoot != null)
            {
                return false;
            }
            if (BestMount != null)
            {
                return false;
            }
            if (BestHarness != null)
            {
                return false;
            }
            if (BestWeapon1 != null)
            {
                return false;
            }
            if (BestWeapon2 != null)
            {
                return false;
            }
            if (BestWeapon3 != null)
            {
                return false;
            }
            if (BestWeapon4 != null)
            {
                return false;
            }
            return true;
        }

        //public static void EquipAll()
        //{
        //    if (BestHelm != null)
        //    {
        //        EquipBestHelm();
        //        return;
        //    }
        //    if (BestCloak != null)
        //    {
        //        EquipBestCloak();
        //        return;
        //    }
        //    if (BestArmor != null)
        //    {
        //        EquipBestArmor();
        //        return;
        //    }
        //    if (BestGlove != null)
        //    {
        //        EquipBestGlove();
        //        return;
        //    }
        //    if (BestBoot != null)
        //    {
        //        EquipBestBoot();
        //        return;
        //    }
        //    if (BestMount != null)
        //    {
        //        EquipBestMount();
        //        return;
        //    }
        //    if (BestHarness != null)
        //    {
        //        EquipBestHarness();
        //        return;
        //    }
        //    if (BestWeapon1 != null)
        //    {
        //        EquipBestWeapon1();
        //        return;
        //    }
        //    if (BestWeapon2 != null)
        //    {
        //        EquipBestWeapon2();
        //        return;
        //    }
        //    if (BestWeapon3 != null)
        //    {
        //        EquipBestWeapon3();
        //        return;
        //    }
        //    if (BestWeapon4 != null)
        //    {
        //        EquipBestWeapon4();
        //        return;
        //    }
        //}

    }
}