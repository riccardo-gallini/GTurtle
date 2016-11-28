using Gemini.Framework.Commands;
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
using System.Windows.Input;

namespace GTurtle
{
    public static class ExecuteMenuDefinitions
    {
        [Export] public static MenuDefinition DebugMenu = new MenuDefinition(MenuDefinitions.MainMenuBar, 10, "Execute");
        [Export] public static MenuItemGroupDefinition DebugMenuGroup = new MenuItemGroupDefinition(DebugMenu, 10);
        [Export] public static MenuItemDefinition PlayMenuItem = new CommandMenuItemDefinition<Play>(DebugMenuGroup, 10);
        [Export] public static MenuItemDefinition PauseMenuItem = new CommandMenuItemDefinition<Pause>(DebugMenuGroup, 20);
        [Export] public static MenuItemDefinition StopMenuItem = new CommandMenuItemDefinition<Stop>(DebugMenuGroup, 30);
        [Export] public static MenuItemDefinition StepIntoMenuItem = new CommandMenuItemDefinition<StepInto>(DebugMenuGroup, 40);
        [Export] public static MenuItemDefinition StepOverMenuItem = new CommandMenuItemDefinition<StepOver>(DebugMenuGroup, 50);
        [Export] public static MenuItemDefinition StepOutMenuItem = new CommandMenuItemDefinition<StepOut>(DebugMenuGroup, 60);
        [Export] public static MenuItemDefinition ToggleBreakpointMenuItem = new CommandMenuItemDefinition<ToggleBreakpoint>(DebugMenuGroup, 70);
        
        [Export] public static ToolBarDefinition DebugToolbar = new ToolBarDefinition(10, "Execute");
        [Export] public static ToolBarItemGroupDefinition DebugToolbarGroup = new ToolBarItemGroupDefinition(DebugToolbar, 10);
        public static ToolBarItemDefinition PlayToolBarItem = new CommandToolBarItemDefinition<Play>(DebugToolbarGroup, 10, ToolBarItemDisplay.IconAndText);
        [Export] public static ToolBarItemDefinition PauseToolBarItem = new CommandToolBarItemDefinition<Pause>(DebugToolbarGroup, 20, ToolBarItemDisplay.IconAndText);
        [Export] public static ToolBarItemDefinition StopToolBarItem = new CommandToolBarItemDefinition<Stop>(DebugToolbarGroup, 30, ToolBarItemDisplay.IconAndText);
        [Export] public static ToolBarItemDefinition StepIntoToolBarItem = new CommandToolBarItemDefinition<StepInto>(DebugToolbarGroup, 40, ToolBarItemDisplay.IconAndText);
        [Export] public static ToolBarItemDefinition StepOverToolBarItem = new CommandToolBarItemDefinition<StepOver>(DebugToolbarGroup, 50, ToolBarItemDisplay.IconAndText);
        [Export] public static ToolBarItemDefinition StepOutToolBarItem = new CommandToolBarItemDefinition<StepOut>(DebugToolbarGroup, 60, ToolBarItemDisplay.IconAndText);


    }
}
