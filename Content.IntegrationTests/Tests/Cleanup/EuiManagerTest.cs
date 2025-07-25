// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Debug <49997488+DebugOk@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 TemporalOroboros <TemporalOroboros@gmail.com>
// SPDX-FileCopyrightText: 2023 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

using System.Linq;
using Content.Server.Administration.UI;
using Content.Server.EUI;
using Robust.Server.Player;

namespace Content.IntegrationTests.Tests.Cleanup;

public sealed class EuiManagerTest
{
    [Test]
    public async Task EuiManagerRecycleWithOpenWindowTest()
    {
        // Even though we are using the server EUI here, we actually want to see if the client EUIManager crashes
        for (var i = 0; i < 2; i++)
        {
            await using var pair = await PoolManager.GetServerClient(new PoolSettings
            {
                Connected = true,
                Dirty = true
            });
            var server = pair.Server;

            var sPlayerManager = server.ResolveDependency<IPlayerManager>();
            var eui = server.ResolveDependency<EuiManager>();

            await server.WaitAssertion(() =>
            {
                var clientSession = sPlayerManager.Sessions.Single();
                var ui = new AdminAnnounceEui();
                eui.OpenEui(ui, clientSession);
            });
            await pair.CleanReturnAsync();
        }
    }
}
