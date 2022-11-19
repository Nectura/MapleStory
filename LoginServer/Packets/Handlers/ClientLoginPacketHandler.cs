using Common.Database.Models;
using Common.Database.Models.Interfaces;
using Common.Database.WorkUnits.Interfaces;
using Common.Networking;
using Common.Networking.Extensions;
using Common.Networking.Packets.Enums;
using Common.Networking.Packets.Interfaces;
using Common.Services.Interfaces;
using LoginServer.Configuration;
using LoginServer.Packets.Enums;
using LoginServer.Packets.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LoginServer.Packets.Handlers;

public sealed class ClientLoginPacketHandler : IAsyncPacketHandler
{
    public EClientOperationCode Opcode { get; init; } = EClientOperationCode.ClientLogin;

    public async Task HandlePacketAsync(IServiceScopeFactory scopeFactory, GameClient client, GameMessageBuffer buffer,
        CancellationToken cancellationToken = default)
    {
        await using var scope = scopeFactory.CreateAsyncScope();

        var packetInstance = buffer.ParsePacketInstance<AccountLoginPacket>();
        var workUnit = scope.ServiceProvider.GetRequiredService<IAccountWorkUnit>();
        var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
        var loginConfig = scope.ServiceProvider.GetRequiredService<IOptions<LoginConfig>>().Value;

        var account = await workUnit.Accounts
            .Query(m => m.UserName == packetInstance.UserName)
            .FirstOrDefaultAsync(cancellationToken);

        if (account == default)
        {
            if (!loginConfig.AutoRegister)
            {
                client.Send(new GameMessageBuffer(EServerOperationCode.CheckPasswordResult)
                    .WriteByte((byte) ELoginResult.Unregistered));
                return;
            }
            
            var (saltHash, passHash) = await authService.HashInputAsync(packetInstance.Password, cancellationToken);
            
            account = workUnit.Accounts.Add(new Account
            {
                UserName = packetInstance.UserName,
                PasswordHash = passHash,
                PasswordSaltHash = saltHash,
                LastKnownIpAddress = client.IpAddress.ToString(),
                LastLoggedInAt = DateTime.UtcNow
            });
            
            await workUnit.CommitChangesAsync(cancellationToken);
            
            SendSuccessfulLoginPackets(client, account); // should be EULA instead, we skip this in EMS tho

            return;
        }

        if (!await authService.CompareHashesAsync(packetInstance.Password, account.PasswordSaltHash, account.PasswordHash, cancellationToken))
        {
            client.Send(new GameMessageBuffer(EServerOperationCode.CheckPasswordResult)
                .WriteByte((byte) ELoginResult.IncorrectPassword));
            return;
        }

        /*if (!account.HasAcceptedEula)
        {
                client.Send(new GameMessageBuffer(EServerOperationCode.CheckPasswordResult)
                    .WriteByte((byte) ELoginResult.LicenceAgreement));

            return;
        }*/

        account.LastKnownIpAddress = client.IpAddress.ToString();
        account.LastLoggedInAt = DateTime.UtcNow;

        await workUnit.CommitChangesAsync(cancellationToken);

        SendSuccessfulLoginPackets(client, account);
    }

    private void SendSuccessfulLoginPackets(GameClient client, IAccount account)
    {
        SendLoginSuccessResult(client, account);
        SendWorldInfo(client);
        SendEndOfWorldInfo(client);
    }

    private void SendLoginSuccessResult(GameClient client, IAccount account)
    {
        client.Send(new GameMessageBuffer(EServerOperationCode.CheckPasswordResult)
            .WriteByte((byte)ELoginResult.Success)
            .WriteInt(account.Id)
            .WriteByte()
            .WriteByte()
            .WriteUShort()
            .WriteByte()
            .WriteString(account.UserName)
            .WriteByte()
            .WriteByte()
            .WriteLong()
            .WriteLong()
            .WriteInt()
            .WriteByte()
            .WriteByte());
    }

    private void SendWorldInfo(GameClient client)
    {
        var responseBuffer = new GameMessageBuffer(EServerOperationCode.WorldInformation);
        responseBuffer
            .WriteByte(1) // worldId
            .WriteString("1") // worldId
            .WriteByte() // worldState
            .WriteString("Event Message") // eventMsg
            .WriteUShort(1) // expRate
            .WriteUShort(1) // dropRate
            .WriteByte(20); // channelCount
        for (byte i = 0; i < 20;) // foreach channel
        {
            responseBuffer
                .WriteString($"1-{++i}") // channel name [WorldId-ChannelIndex+1]
                .WriteInt() // connected peers count
                .WriteByte(1) // world id
                .WriteByte(i) // channel index
                .WriteBool(); // is adult channel
        }
        responseBuffer.WriteShort(); // balloon count
        client.Send(responseBuffer);
    }

    private void SendEndOfWorldInfo(GameClient client)
    {
        client.Send(new GameMessageBuffer(EServerOperationCode.WorldInformation)
            .WriteByte(byte.MaxValue));
    }
}