// SPDX-FileCopyrightText: 2026 Lytheriia
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Threading.Tasks;
using Content.Server.Administration;
using Content.Server.Database;
using Content.Server.Discord;
using Content.Server.Roles;
using Content.Shared._Erida.CCVar;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Server._Erida.Discord;

public sealed partial class EridaWebhooks : IPostInjectInit
{
    [Dependency] private readonly DiscordWebhook _discord = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly IPlayerLocator _playerLocator = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IServerDbManager _serverDbManager = default!;

    private ISawmill _sawmill = default!;

    private WebhookIdentifier? _webhookIdentifierBan;
    private WebhookIdentifier? _webhookIdentifierPlayTime;

    public void PostInject()
    {
        _sawmill = Logger.GetSawmill("discord");

        // Inject faster, then CCVar. so check is it registered
        // Dont add another if's. 1 should be enough
        if (!_cfg.IsCVarRegistered(ECCVars.DiscordBanWebhook.Name))
            return;

        _cfg.OnValueChanged(ECCVars.DiscordBanWebhook,
            CreateWebhookHandler(wi => _webhookIdentifierBan = wi), true);

        _cfg.OnValueChanged(ECCVars.DiscordPlayTimeWebhook,
            CreateWebhookHandler(wi => _webhookIdentifierPlayTime = wi), true);
    }

    private Action<string> CreateWebhookHandler(Action<WebhookIdentifier?> setIdentifier)
    {
        return async url =>
        {
            setIdentifier(null);

            if (string.IsNullOrEmpty(url))
                return;

            try
            {
                if (await _discord.GetWebhook(url) is not { } identifier)
                    return;

                setIdentifier(identifier.ToIdentifier());
            }
            catch (Exception e)
            {
                _sawmill.Error($"Error resolving webhook identifier: {e}");
            }
        };
    }

    #region Shared data

    private static WebhookEmbedField EmbedSpacer => new()
    {
        Name = "\u200b",
        Value = "\u200b",
        Inline = true,
    };

    private static readonly string NOT_FOUND = Loc.GetString("ban-webhook-unknown-error");

    private static readonly Dictionary<WebhookType, int> WebhookEmbedColors = new()
    {
        { WebhookType.PlayTimeAdd, ColorToDiscordInt(Color.FromHex("#009455")) },
        { WebhookType.PlayTimeRem, ColorToDiscordInt(Color.FromHex("#007041")) },
        { WebhookType.PlayTimeSet, ColorToDiscordInt(Color.FromHex("#3d9a73")) },
        { WebhookType.CoinsAdd, ColorToDiscordInt(Color.FromHex("#009E98")) },
        { WebhookType.CoinsRem, ColorToDiscordInt(Color.FromHex("#00706C")) }
    };

    private enum WebhookType : byte
    {
        PlayTimeAdd,
        PlayTimeRem,
        PlayTimeSet,
        CoinsAdd,
        CoinsRem
    }

    #endregion
    #region Shared functions

    private async Task<string> GetAdminName(NetUserId? id)
    {
        if (id is not { } admin)
            return Loc.GetString("erida-webhook-unknown");

        if (_playerManager.TryGetPlayerData(admin, out var adminData))
            return adminData.UserName;

        var locatedData = await _playerLocator.LookupIdAsync(admin);
        return locatedData?.Username ?? Loc.GetString("erida-webhook-unknown");
    }

    private string CodeBlockedSmall(string value)
    {
        return $"``{value}``";
    }

    private string CodeBlocked(string value)
    {
        return $"```{value}```";
    }

    private static int ColorToDiscordInt(Color color)
    {
        return (color.RByte << 16) | (color.GByte << 8) | color.BByte;
    }
    #endregion
}

