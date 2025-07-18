// SPDX-FileCopyrightText: 2023 Tim Falken <timfalken@hotmail.com>
// SPDX-FileCopyrightText: 2024 Fansana <116083121+Fansana@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Mnemotechnican <69920617+Mnemotechnician@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 ShatteredSwords <135023515+ShatteredSwords@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 VMSolidus <evilexecutive@gmail.com>
// SPDX-FileCopyrightText: 2025 BlitzTheSquishy <73762869+BlitzTheSquishy@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using Content.Client.Message;
using Content.Shared._DV.CartridgeLoader.Cartridges;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Prototypes;
using static Content.Client._DV.CartridgeLoader.Cartridges.CrimeAssistUi;

namespace Content.Client._DV.CartridgeLoader.Cartridges;

[GenerateTypedNameReferences]
public sealed partial class CrimeAssistUiFragment : BoxContainer
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

    private CrimeAssistPage _currentPage;
    private List<CrimeAssistPage>? _pages;

    public CrimeAssistUiFragment()
    {
        RobustXamlLoader.Load(this);
        IoCManager.InjectDependencies(this);

        Orientation = LayoutOrientation.Vertical;
        HorizontalExpand = true;
        VerticalExpand = true;

        _pages = new List<CrimeAssistPage>(_prototypeManager.EnumeratePrototypes<CrimeAssistPage>());

        _currentPage = FindPageById("mainmenu");
        UpdateUI(_currentPage);

        StartButton.OnPressed += _ => UpdateUI(FindPageById(FindPageById("mainmenu").OnStart!));
        HomeButton.OnPressed += _ => UpdateUI(FindPageById("mainmenu"));
        YesButton.OnPressed += _ => AdvanceState(_currentPage!, true);
        NoButton.OnPressed += _ => AdvanceState(_currentPage!, false);
    }

    public void AdvanceState(CrimeAssistPage currentPage, bool yesPressed)
    {
        UpdateUI(yesPressed ? FindPageById(currentPage.OnYes!) : FindPageById(currentPage.OnNo!));
    }

    public void UpdateUI(CrimeAssistPage page)
    {
        _currentPage = page;
        bool isResult = page.LocKeyPunishment != null;

        StartButton.Visible = page.OnStart != null;
        YesButton.Visible = page.OnYes != null;
        NoButton.Visible = page.OnNo != null;
        HomeButton.Visible = page.OnStart == null;
        Explanation.Visible = page.OnStart == null;

        Subtitle.Visible = page.LocKeySeverity != null;
        Punishment.Visible = page.LocKeyPunishment != null;

        if (!isResult)
        {
            string question = $"\n[font size=15]{Loc.GetString(page.LocKey!)}[/font]";

            if (question.ToLower().Contains("sophont"))
            {
                string sophontExplanation = Loc.GetString("crime-assist-sophont-explanation");
                question += $"\n[font size=8][color=#999999]{sophontExplanation}[/color][/font]";
            }

            Title.SetMarkup(question);
            Subtitle.SetMarkup(string.Empty);
            Explanation.SetMarkup(string.Empty);
            Punishment.SetMarkup(string.Empty);
        }
        else
        {
            string color = page.LocKeySeverity! switch
            {
                "crime-assist-crimetype-innocent" => "#39a300",
                "crime-assist-crimetype-misdemeanour" => "#7b7b30",
                "crime-assist-crimetype-felony" => "#7b5430",
                "crime-assist-crimetype-capital" => "#7b2e30",
                _ => "#ff00ff"
            };

            Title.SetMarkup("\n[bold][font size=23][color=#a4885c]" + Loc.GetString(page.LocKeyTitle!) + "[/color][/font][/bold]");
            Subtitle.SetMarkup($"\n[font size=19][color={color}]" + Loc.GetString(page.LocKeySeverity!) + "[/color][/font]");
            Explanation.SetMarkup("\n[title]" + Loc.GetString(page.LocKeyDescription!) + "[/title]\n");
            Punishment.SetMarkup("[bold][font size=15]" + Loc.GetString(page.LocKeyPunishment!) + "[/font][/bold]");
        }
    }

    private CrimeAssistPage FindPageById(string id)
    {
        return _pages?.Find(o => o.ID == id)!;
    }
}
