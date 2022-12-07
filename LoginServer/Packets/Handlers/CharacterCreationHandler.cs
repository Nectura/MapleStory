using Common.Database.Models;
using Common.Database.Models.Interfaces;
using Common.Database.Repositories.Interfaces;
using Common.Database.WorkUnits.Interfaces;
using Common.Enums;
using Common.Interfaces.Inventory;
using Common.Models.Structs;
using Common.Networking;
using Common.Networking.Extensions;
using Common.Networking.Packets.Enums;
using Common.Networking.Packets.Interfaces;
using LoginServer.Configuration;
using LoginServer.Packets.Enums;
using LoginServer.Packets.Models;
using Microsoft.Extensions.Options;

namespace LoginServer.Packets.Handlers;

public sealed class CharacterCreationHandler : IAsyncPacketHandler
{
    public EClientOperationCode Opcode { get; init; } = EClientOperationCode.CreateNewCharacter;

    public async Task HandlePacketAsync(IServiceScopeFactory scopeFactory, GameClient client, GameMessageBuffer buffer, CancellationToken cancellationToken = default)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var packetInstance = buffer.ParsePacketInstance<CharacterCreationPacket>();
        var workUnit = scope.ServiceProvider.GetRequiredService<IInventoryWorkUnit>();
        var inventoryConfig = scope.ServiceProvider.GetRequiredService<IOptions<InventoryConfig>>();
        if (await workUnit.Characters.AnyAsync(m => m.Name == packetInstance.Name, cancellationToken))
        {
            SendFailurePacket(client);
            return;
        }
        var character = workUnit.Characters.Add(new Character
        {
            AccountId = client.Account.Id,
            Name = packetInstance.Name,
            Face = packetInstance.Face,
            HairStyle = packetInstance.HairStyle,
            HairColor = (byte)packetInstance.HairColor,
            SkinColor = (byte)packetInstance.SkinColor
        });
        SetStarterStats(character, packetInstance);
        await workUnit.CommitChangesAsync(cancellationToken);
        var inventory = workUnit.Inventories.Add(new Inventory
        {
            CharacterId = character.Id,
            EquippableTabSlots = inventoryConfig.Value.EquippableTabSlots,
            ConsumableTabSlots = inventoryConfig.Value.ConsumableTabSlots,
            SetupTabSlots = inventoryConfig.Value.SetupTabSlots,
            EtceteraTabSlots = inventoryConfig.Value.EtceteraTabSlots,
            CashTabSlots = inventoryConfig.Value.CashTabSlots
        });
        character.Inventory = inventory;
        await workUnit.CommitChangesAsync(cancellationToken);
        await client.InitializeInventoryServiceAsync(character.Id, inventory.Id, cancellationToken);
        var inventoryService = client.InventoryServices[character.Id];
        await inventoryService.LoadAsync(inventory.Id, cancellationToken);
        AddStarterGear(packetInstance, inventoryService);
        await inventoryService.SaveAsync(cancellationToken);
        SendSuccessPacket(client, character);
    }

    private static void SendSuccessPacket(GameClient client, ICharacter character)
    {
        client.Send(new GameMessageBuffer(EServerOperationCode.CreateNewCharacterResult)
            .WriteByte((byte)ELoginResult.Success)
            .WriteCharacterInfo(character, client.InventoryServices[character.Id])
        );
    }

    private static void SendFailurePacket(GameClient client)
    {
        client.Send(new GameMessageBuffer(EServerOperationCode.CreateNewCharacterResult)
            .WriteBool(false)
        );
    }

    private static void AddStarterGear(CharacterCreationPacket packetInstance, IInventoryService inventoryService)
    {
        if (!Enum.IsDefined(typeof(EJobCategory), packetInstance.JobCategory))
            throw new ArgumentException($"Unsupported Job Category: {packetInstance.JobCategory}"); // Note: potentially packet editing

        var jobCategory = (EJobCategory) packetInstance.JobCategory;
        
        inventoryService.TryAddItem(EInventoryTab.Equipment, packetInstance.Top, 1, out var topItem);
        inventoryService.TryAddItem(EInventoryTab.Equipment, packetInstance.Bottom, 1, out var bottomItem);
        inventoryService.TryAddItem(EInventoryTab.Equipment, packetInstance.Shoes, 1, out var shoesItem);
        inventoryService.TryAddItem(EInventoryTab.Equipment, packetInstance.Weapon, 1, out var weaponItem);

        inventoryService.TryEquipItem(topItem!, out _);
        inventoryService.TryEquipItem(bottomItem!, out _);
        inventoryService.TryEquipItem(shoesItem!, out _);
        inventoryService.TryEquipItem(weaponItem!, out _);
        
        switch (jobCategory)
        {
            case EJobCategory.CygnusKnights: // Knights of Cygnus
                inventoryService.TryAddItem(EInventoryTab.Etcetera, 4161047, 1, out _);
                break;

            case EJobCategory.Explorer: // Adventurer
                inventoryService.TryAddItem(EInventoryTab.Etcetera, 4161001, 1, out _);
                break;

            case EJobCategory.Aran: // Aran
                inventoryService.TryAddItem(EInventoryTab.Etcetera, 4161048, 1, out _);
                break;

            case EJobCategory.Resistance:
            case EJobCategory.Evan:
            default:
                throw new ArgumentException($"Unhandled Job Category: {packetInstance.JobCategory}"); // Note: potentially packet editing
        }
    }

    private static void SetStarterStats(ICharacter character, CharacterCreationPacket packetInstance)
    {
        if (!Enum.IsDefined(typeof(EJobCategory), packetInstance.JobCategory))
            throw new ArgumentException($"Unsupported Job Category: {packetInstance.JobCategory}"); // Note: potentially packet editing

        var jobCategory = (EJobCategory) packetInstance.JobCategory;

        switch (jobCategory)
        {
            case EJobCategory.CygnusKnights: // Knights of Cygnus
                character.Job = EJob.Noblesse;
                character.MapId = 130030000;
                break;

            case EJobCategory.Explorer: // Adventurer
                character.Job = EJob.Beginner;
                character.MapId = /*specialJobType == 2 ? 3000600 : */10000;
                break;

            case EJobCategory.Aran: // Aran
                character.Job = EJob.Legend;
                character.MapId = 914000000;
                break;

            case EJobCategory.Resistance:
            case EJobCategory.Evan:
            default:
                throw new ArgumentException($"Unhandled Job Category: {packetInstance.JobCategory}"); // Note: potentially packet editing
        }

        character.SubJob = packetInstance.SubJob;
        character.HitPoints = 50;
        character.MaxHitPoints = 50;

        character.ManaPoints = 5;
        character.MaxManaPoints = 5;

        character.Strength = 13;
        character.Dexterity = 4;
        character.Intelligence = 4;
        character.Luck = 4;

        character.BuddyLimit = 20;
        character.Level = 1;
    }
}