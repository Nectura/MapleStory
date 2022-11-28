using Common.Database.Models;
using Common.Database.Models.Interfaces;
using Common.Database.Repositories.Interfaces;
using Common.Enums;
using Common.Interfaces.Inventory;
using Common.Models.Structs;
using Common.Networking;
using Common.Networking.Extensions;
using Common.Networking.Packets.Enums;
using Common.Networking.Packets.Interfaces;
using LoginServer.Packets.Enums;
using LoginServer.Packets.Models;

namespace LoginServer.Packets.Handlers;

public sealed class CharacterCreationHandler : IAsyncPacketHandler
{
    public EClientOperationCode Opcode { get; init; } = EClientOperationCode.CreateNewCharacter;

    public async Task HandlePacketAsync(IServiceScopeFactory scopeFactory, GameClient client, GameMessageBuffer buffer, CancellationToken cancellationToken = default)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var packetInstance = buffer.ParsePacketInstance<CharacterCreationPacket>();
        var repository = scope.ServiceProvider.GetRequiredService<ICharacterRepository>();
        var inventoryService = scope.ServiceProvider.GetRequiredService<IInventoryService>();
        if (await repository.AnyAsync(m => m.Name == packetInstance.Name, cancellationToken))
        {
            SendFailurePacket(client);
            return;
        }
        var inventory = new Inventory
        {
            EquippableTabSlots = 24,
            ConsumableTabSlots = 24,
            SetupTabSlots = 24,
            EtceteraTabSlots = 24,
            CashTabSlots = 96 // cash is maxed out cause they are sellouts, 96 being the highest amount of slots
        };
        var character = new Character
        {
            AccountId = client.Account!.Id,
            Name = packetInstance.Name,
            Face = packetInstance.Face,
            HairStyle = packetInstance.HairStyle,
            HairColor = (byte) packetInstance.HairColor,
            SkinColor = (byte) packetInstance.SkinColor,
            Inventory = inventory
        };
        await inventoryService.LoadAsync(character.InventoryId, cancellationToken);
        ApplyStartingConfiguration(character, packetInstance, inventoryService);
        repository.Add(character);
        await repository.SaveChangesAsync(cancellationToken);
        SendSuccessPacket(client, character);
    }

    private static void SendSuccessPacket(GameClient client, Character character)
    {
        client.Send(new GameMessageBuffer(EServerOperationCode.CreateNewCharacterResult)
            .WriteByte((byte)ELoginResult.Success)
            .WriteCharacterInfo(character)
        );
    }

    private static void SendFailurePacket(GameClient client)
    {
        client.Send(new GameMessageBuffer(EServerOperationCode.CreateNewCharacterResult)
            .WriteBool(false)
        );
    }
    
    private static void ApplyStartingConfiguration(ICharacter character, CharacterCreationPacket packetInstance, IInventoryService inventoryService)
    {
        if (!Enum.IsDefined(typeof(EJobCategory), packetInstance.JobCategory))
            throw new ArgumentException($"Unsupported Job Category: {packetInstance.JobCategory}"); // Note: potentially packet editing

        var jobCategory = (EJobCategory) packetInstance.JobCategory;

        inventoryService.TryAddItem(packetInstance.Top, 1, EInventoryTab.Equipment, out _);
        inventoryService.TryAddItem(packetInstance.Bottom, 1, EInventoryTab.Equipment, out _);
        inventoryService.TryAddItem(packetInstance.Shoes, 1, EInventoryTab.Equipment, out _);
        inventoryService.TryAddItem(packetInstance.Weapon, 1, EInventoryTab.Equipment, out _);
        
        switch (jobCategory)
        {
            case EJobCategory.CygnusKnights: // Knights of Cygnus
                character.Job = EJob.Noblesse;
                character.MapId = 130030000;
                inventoryService.TryAddItem(4161047, 1, EInventoryTab.Etcetera, out _);
                break;

            case EJobCategory.Explorer: // Adventurer
                character.Job = EJob.Beginner;
                character.MapId = /*specialJobType == 2 ? 3000600 : */10000;
                inventoryService.TryAddItem(4161001, 1, EInventoryTab.Etcetera, out _);
                break;

            case EJobCategory.Aran: // Aran
                character.Job = EJob.Legend;
                character.MapId = 914000000;
                inventoryService.TryAddItem(4161048, 1, EInventoryTab.Etcetera, out _);
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