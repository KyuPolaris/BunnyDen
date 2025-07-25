# SPDX-FileCopyrightText: 2024 DEATHB4DEFEAT <77995199+DEATHB4DEFEAT@users.noreply.github.com>
# SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
#
# SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

cd ../../

mkdir Scripts/logs

rm Scripts/logs/Content.IntegrationTests.log
dotnet test Content.IntegrationTests/Content.IntegrationTests.csproj -c DebugOpt -- NUnit.ConsoleOut=0 NUnit.MapWarningTo=Failed > Scripts/logs/Content.IntegrationTests.log

echo "Tests complete. Press enter to continue."
read
