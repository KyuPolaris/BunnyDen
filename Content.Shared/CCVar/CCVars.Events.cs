// SPDX-FileCopyrightText: 2025 VMSolidus <evilexecutive@gmail.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using Robust.Shared.Configuration;

namespace Content.Shared.CCVar;

public sealed partial class CCVars
{
    /// <summary>
    ///     Controls if the game should run station events
    /// </summary>
    public static readonly CVarDef<bool>
        EventsEnabled = CVarDef.Create("events.enabled", true, CVar.ARCHIVE | CVar.SERVERONLY);

    /// <summary>
    ///     Average time (in minutes) for when the ramping event scheduler should stop increasing the chaos modifier.
    ///     Close to how long you expect a round to last, so you'll probably have to tweak this on downstreams.
    /// </summary>
    public static readonly CVarDef<float>
        EventsRampingAverageEndTime = CVarDef.Create("events.ramping_average_end_time", 180f, CVar.ARCHIVE | CVar.SERVERONLY);

    /// <summary>
    ///     Average ending chaos modifier for the ramping event scheduler.
    ///     Max chaos chosen for a round will deviate from this
    /// </summary>
    public static readonly CVarDef<float>
        EventsRampingAverageChaos = CVarDef.Create("events.ramping_average_chaos", 0.6f, CVar.ARCHIVE | CVar.SERVERONLY);
}
