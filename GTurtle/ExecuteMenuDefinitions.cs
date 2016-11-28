using Gemini.Framework.Menus;
using Gemini.Framework.ToolBars;
using Gemini.Modules.MainMenu;
using GTurtle.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTurtle
{
    public static class ExecuteMenuDefinitions
    {
        [Export]
        public static MenuDefinition DebugMenu =
           new MenuDefinition(
               MenuDefinitions.MainMenuBar,
               10,
               "Debug");

        [Export]
        public static MenuItemGroupDefinition DebugMenuGroup =
            new MenuItemGroupDefinition(
                DebugMenu,
                10);

        [Export]
        public static MenuItemDefinition PlayMenuItem =
            new CommandMenuItemDefinition<Play>(
                DebugMenuGroup,
                10);

        [Export]
        public static MenuItemDefinition ToggleBreakpointMenuItem =
            new CommandMenuItemDefinition<ToggleBreakpoint>(
                DebugMenuGroup,
                10);

        [Export]
        public static ToolBarDefinition DebugToolbar =
            new ToolBarDefinition(10, "DEBUG");

        [Export]
        public static ToolBarItemGroupDefinition DebugToolbarGroup =
            new ToolBarItemGroupDefinition(DebugToolbar, 10);

        [Export]
        public static ToolBarItemDefinition Play = new CommandToolBarItemDefinition<Play>(DebugToolbarGroup, 10, ToolBarItemDisplay.IconAndText);



    }
}
