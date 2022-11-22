using Common.Database.Models;
using Common.Database.Models.Interfaces;
using Common.Database.Repositories.Interfaces;
using Common.Enums;
using Common.Networking;
using Common.Networking.Extensions;
using Common.Networking.Packets.Enums;
using Common.Networking.Packets.Interfaces;
using LoginServer.Packets.Enums;
using LoginServer.Packets.Models;
using Newtonsoft.Json;

namespace LoginServer.Packets.Handlers;

public sealed class CharacterCreationHandler : IAsyncPacketHandler
{
    public EClientOperationCode Opcode { get; init; } = EClientOperationCode.CreateNewCharacter;

    public async Task HandlePacketAsync(IServiceScopeFactory scopeFactory, GameClient client, GameMessageBuffer buffer, CancellationToken cancellationToken = default)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var packetInstance = buffer.ParsePacketInstance<CharacterCreationPacket>();
        var repository = scope.ServiceProvider.GetRequiredService<ICharacterRepository>();
        if (await repository.AnyAsync(m => m.Name == packetInstance.Name, cancellationToken))
        {
            SendFailurePacket(client);
            return;
        }
        var character = new Character
        {
            AccountId = client.Account!.Id,
            Name = packetInstance.Name,
            Face = packetInstance.Face,
            HairStyle = packetInstance.HairStyle,
            HairColor = (byte) packetInstance.HairColor,
            SkinColor = (byte) packetInstance.SkinColor
        };
        ApplyStartingConfiguration(character, packetInstance);
        repository.Add(character);
        await repository.SaveChangesAsync(cancellationToken);
        SendSuccessPacket(client, character);
    }

    private static void SendSuccessPacket(GameClient client, Character character)
    {
        Console.WriteLine(JsonConvert.SerializeObject(character));
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
    
    private static void ApplyStartingConfiguration(ICharacter character, CharacterCreationPacket packetInstance)
    {
        if (!Enum.IsDefined(typeof(EJobCategory), packetInstance.JobCategory))
            throw new ArgumentException($"Unsupported Job Category: {packetInstance.JobCategory}"); // Note: potentially packet editing

        var jobCategory = (EJobCategory) packetInstance.JobCategory;
        
        switch (jobCategory)
        {
            case EJobCategory.CygnusKnights: // Knights of Cygnus
                character.Job = EJob.Noblesse;
                character.MapId = 130030000;
                //inventoryItemDTOs.Add(new ItemDTO { MapleId = 4161047, Slot = 1, Quantity = 1, Type = EItemType.Etcetera });
                break;

            case EJobCategory.Explorer: // Adventurer
                character.Job = EJob.Beginner;
                character.MapId = /*specialJobType == 2 ? 3000600 : */10000;
                //inventoryItemDTOs.Add(new ItemDTO { MapleId = 4161001, Slot = 1, Quantity = 1, Type = EItemType.Etcetera });
                break;

            case EJobCategory.Aran: // Aran
                character.Job = EJob.Legend;
                character.MapId = 914000000;
                //inventoryItemDTOs.Add(new ItemDTO { MapleId = 4161048, Slot = 1, Quantity = 1, Type = EItemType.Etcetera });
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

        character.EquipmentSlots = 24;
        character.UsableSlots = 24;
        character.SetupSlots = 24;
        character.EtceteraSlots = 24;
        character.CashSlots = 96; // cash is maxed out cause they are sellouts, 96 being the highest amount of slots

        character.Level = 1;
    }
}