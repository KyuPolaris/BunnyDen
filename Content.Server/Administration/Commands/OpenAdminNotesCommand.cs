// SPDX-FileCopyrightText: 2022 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Debug <49997488+DebugOk@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

using Content.Server.Administration.Notes;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server.Administration.Commands;

[AdminCommand(AdminFlags.ViewNotes)]
public sealed class OpenAdminNotesCommand : IConsoleCommand
{
    public const string CommandName = "adminnotes";

    public string Command => CommandName;
    public string Description => "Opens the admin notes panel.";
    public string Help => $"Usage: {Command} <notedPlayerUserId OR notedPlayerUsername>";

    public async void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (shell.Player is not { } player)
        {
            shell.WriteError("This does not work from the server console.");
            return;
        }

        Guid notedPlayer;

        switch (args.Length)
        {
            case 1 when Guid.TryParse(args[0], out notedPlayer):
                break;
            case 1:
                var locator = IoCManager.Resolve<IPlayerLocator>();
                var dbGuid = await locator.LookupIdByNameAsync(args[0]);

                if (dbGuid == null)
                {
                    shell.WriteError($"Unable to find {args[0]} netuserid");
                    return;
                }

                notedPlayer = dbGuid.UserId;
                break;
            default:
                shell.WriteError($"Invalid arguments.\n{Help}");
                return;
        }

        await IoCManager.Resolve<IAdminNotesManager>().OpenEui(player, notedPlayer);
    }
}
