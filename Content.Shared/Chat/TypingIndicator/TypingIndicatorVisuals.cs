// SPDX-FileCopyrightText: 2022 Alex Evgrashin <aevgrashin@yandex.ru>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Chat.TypingIndicator;

[Serializable, NetSerializable]
public enum TypingIndicatorVisuals : byte
{
    IsTyping
}

[Serializable]
public enum TypingIndicatorLayers : byte
{
    Base
}
