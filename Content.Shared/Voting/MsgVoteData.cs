// SPDX-FileCopyrightText: 2021 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2021 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 mirrorcult <notzombiedude@gmail.com>
// SPDX-FileCopyrightText: 2022 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Voting
{
    public sealed class MsgVoteData : NetMessage
    {
        public override MsgGroups MsgGroup => MsgGroups.Command;

        public int VoteId;
        public bool VoteActive;
        public string VoteTitle = string.Empty;
        public string VoteInitiator = string.Empty;
        public TimeSpan StartTime; // Server RealTime.
        public TimeSpan EndTime; // Server RealTime.
        public bool PlayVoteSound;
        public (ushort votes, string name)[] Options = default!;
        public bool IsYourVoteDirty;
        public byte? YourVote;

        public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
        {
            VoteId = buffer.ReadVariableInt32();
            VoteActive = buffer.ReadBoolean();
            buffer.ReadPadBits();

            if (!VoteActive)
                return;

            VoteTitle = buffer.ReadString();
            VoteInitiator = buffer.ReadString();
            StartTime = TimeSpan.FromTicks(buffer.ReadInt64());
            EndTime = TimeSpan.FromTicks(buffer.ReadInt64());

            Options = new (ushort votes, string name)[buffer.ReadByte()];
            for (var i = 0; i < Options.Length; i++)
            {
                Options[i] = (buffer.ReadUInt16(), buffer.ReadString());
            }

            IsYourVoteDirty = buffer.ReadBoolean();
            if (IsYourVoteDirty)
            {
                YourVote = buffer.ReadBoolean() ? buffer.ReadByte() : null;
            }
        }

        public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
        {
            buffer.WriteVariableInt32(VoteId);
            buffer.Write(VoteActive);
            buffer.WritePadBits();

            if (!VoteActive)
                return;

            buffer.Write(VoteTitle);
            buffer.Write(VoteInitiator);
            buffer.Write(StartTime.Ticks);
            buffer.Write(EndTime.Ticks);

            buffer.Write((byte) Options.Length);
            foreach (var (votes, name) in Options)
            {
                buffer.Write(votes);
                buffer.Write(name);
            }

            buffer.Write(IsYourVoteDirty);
            if (IsYourVoteDirty)
            {
                buffer.Write(YourVote.HasValue);
                if (YourVote.HasValue)
                {
                    buffer.Write(YourVote.Value);
                }
            }
        }

        public override NetDeliveryMethod DeliveryMethod => NetDeliveryMethod.ReliableOrdered;
    }
}
