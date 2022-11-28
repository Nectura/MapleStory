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
            .Include(m => m.Characters)
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
            
            SendLoginSuccessResult(client, account); // should be EULA instead, we skip this in EMS tho

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

        SendLoginSuccessResult(client, account);
    }

    private void SendLoginSuccessResult(GameClient client, IAccount account)
    {
        client.Account = account;
        client.Send(new GameMessageBuffer(EServerOperationCode.CheckPasswordResult)
            .WriteByte((byte)ELoginResult.Success)
            .WriteUInt(account.Id)
            .WriteByte() // acc gender?
            .WriteBool() // gm bool
            .WriteUShort() // gm byte
            .WriteByte() // country code
            .WriteString(account.UserName)
            .WriteByte() // ?
            .WriteByte() // isQUietBan
            .WriteLong() // quietbantimestamp
            .WriteLong() // creationTimeStamp
            .WriteInt() // 1: remove select world u want to play in
            .WriteByte() // pin bool
            .WriteByte()); // pic byte
    }
}