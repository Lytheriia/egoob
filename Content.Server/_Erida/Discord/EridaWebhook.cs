// SPDX-FileCopyrightText: 2026 Lytheriia
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Server.Administration;
using Content.Server.Database;
using Content.Server.Discord;
using Content.Shared.CCVar;
using Robust.Server.Player;
using Robust.Shared.Configuration;
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

    public void PostInject()
    {
        _sawmill = Logger.GetSawmill("discord");

        // Inject faster, then CCVar. so check is it registered
        // Dont add another if's. 1 should be enough
        if (!_cfg.IsCVarRegistered(CCVars.DiscordBanWebhook.Name))
            return;

        _cfg.OnValueChanged(CCVars.DiscordBanWebhook,
            CreateWebhookHandler(wi => _webhookIdentifierBan = wi), true);
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
}

