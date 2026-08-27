// SPDX-FileCopyrightText: 2026 Lytheriia
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Threading.Tasks;
using Content.Server.Discord;
using Robust.Shared.Network;

namespace Content.Server._Erida.Discord;

public sealed partial class EridaWebhooks
{
    public void SendTokensChangedMessage(NetUserId adminId, NetUserId targetId, string value, bool isSet = false)
    {
        if (int.TryParse(value, out var number))
        {
            SendTokensChangedMessage(adminId, targetId, number, isSet);
        }
    }

    public void SendTokensChangedMessage(NetUserId adminId, NetUserId targetId, int value, bool isSet = false)
    {
        if (_webhookIdentifierTokens == null)
            return;

        SendTokenChanged(adminId, targetId, value, isSet);
    }

    private async void SendTokenChanged(NetUserId adminId, NetUserId targetId, int value, bool isSet = false)
    {
        var embed = CreateBaseEmbedWithNames(adminId, targetId);

        if (!isSet)
            if (value > 0)
            {
                embed.Title = Loc.GetString("tokens-webhook-title-add");
                embed.Color = WebhookEmbedColors[WebhookType.CoinsAdd];
            }
            else
            {
                embed.Title = Loc.GetString("tokens-webhook-title-rem");
                embed.Color = WebhookEmbedColors[WebhookType.CoinsRem];
            }
        else
        {
            embed.Title = Loc.GetString("tokens-webhook-title-set");
            embed.Color = WebhookEmbedColors[WebhookType.CoinsSet];
        }

        var balance = await GetBalance(targetId);

        embed.Fields.Add(new WebhookEmbedField()
        {
            Name = Loc.GetString("tokens-webhook-value-old"),
            Value = CodeBlocked((balance - value).ToString("N0")),
            Inline = true
        });

        embed.Fields.Add(new WebhookEmbedField()
        {
            Name = Loc.GetString("tokens-webhook-value"),
            Value = CodeBlocked(value.ToString("N0")),
            Inline = true
        });


        embed.Fields.Add(new WebhookEmbedField()
        {
            Name = Loc.GetString("tokens-webhook-value-new"),
            Value = CodeBlocked(balance.ToString("N0")),
            Inline = true
        });

        var payload = new WebhookPayload()
        {
            Username = Loc.GetString("erida-webhook-server-name"),
            Embeds = [
                embed
            ]
        };

        SendMessage(_webhookIdentifierTokens!.Value, payload);
    }

    private async Task<int> GetBalance(NetUserId? userId = null)
    {
        return userId == null ? 0 : await _serverDbManager.GetServerCurrency(userId!.Value);
    }
}
