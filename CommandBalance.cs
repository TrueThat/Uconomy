using System;
using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Core;
using Rocket.Unturned.Player;
using UnityEngine;
using SDG.Unturned;
using Steamworks;

namespace fr34kyn01535.Uconomy
{
    public class CommandBalance : IRocketCommand
    {
        public string Name
        {
            get { return "balance"; }
        }
        public string Help
        {
            get { return "Shows the current balance"; }
        }


        public AllowedCaller AllowedCaller
        {
            get
            {
                return AllowedCaller.Player;
            }
        }

        public string Syntax
        {
            get { return ""; }
        }

        public List<string> Aliases
        {
            get { return new List<string>(); }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>() { "uconomy.balance" };
            }
        }

        public void Execute(IRocketPlayer caller,params string[] command)
        {
            UnturnedPlayer CPlayer = UnturnedPlayer.FromName(caller.DisplayName);
            if(command.Length == 1)
            {
                if (caller.HasPermission("balance.check"))
                {
                    UnturnedPlayer target = UnturnedPlayer.FromName(command[0]);
                    if (target != null)
                    {
                        decimal balance = Uconomy.Instance.Database.GetBalance(target.Id);
                        //UnturnedChat.Say(caller, Uconomy.Instance.Translate("command_balance_show_otherPlayer", Uconomy.Instance.Configuration.Instance.MoneySymbol, balance, Uconomy.Instance.Configuration.Instance.MoneyName), UnturnedChat.GetColorFromName(Uconomy.MessageColor, Color.green));
                        ChatManager.serverSendMessage(Uconomy.Instance.Translate("command_balance_show_otherPlayer", Uconomy.Instance.Configuration.Instance.MoneySymbol, balance, Uconomy.Instance.Configuration.Instance.MoneyName).Replace('[', '<').Replace(']', '>'), Color.green, null, CPlayer.Player.channel.owner, EChatMode.SAY, Uconomy.Instance.Configuration.Instance.ImageURL, true);
                    }
                    else
                    {
                        //UnturnedChat.Say(caller, Uconomy.Instance.Translate("command_balance_error_player_not_found"), UnturnedChat.GetColorFromName(Uconomy.MessageColor, Color.green));
                        ChatManager.serverSendMessage(Uconomy.Instance.Translate("command_balance_error_player_not_found").Replace('[', '<').Replace(']', '>'), Color.green, null, CPlayer.Player.channel.owner, EChatMode.SAY, Uconomy.Instance.Configuration.Instance.ImageURL, true);
                    }
                }
                else
                {
                    //UnturnedChat.Say(caller, Uconomy.Instance.Translate("command_balance_check_noPermissions"), UnturnedChat.GetColorFromName(Uconomy.MessageColor, Color.green));
                    ChatManager.serverSendMessage(Uconomy.Instance.Translate("command_balance_check_noPermissions").Replace('[', '<').Replace(']', '>'), Color.green, null, CPlayer.Player.channel.owner, EChatMode.SAY, Uconomy.Instance.Configuration.Instance.ImageURL, true);
                }
                
            }
            else
            {
                decimal balance = Uconomy.Instance.Database.GetBalance(caller.Id);
                //UnturnedChat.Say(caller, Uconomy.Instance.Translations.Instance.Translate("command_balance_show", Uconomy.Instance.Configuration.Instance.MoneySymbol, balance, Uconomy.Instance.Configuration.Instance.MoneyName), UnturnedChat.GetColorFromName(Uconomy.MessageColor, Color.green));
                ChatManager.serverSendMessage(Uconomy.Instance.Translations.Instance.Translate("command_balance_show", Uconomy.Instance.Configuration.Instance.MoneySymbol, balance, Uconomy.Instance.Configuration.Instance.MoneyName).Replace('[', '<').Replace(']', '>'), Color.green, null, CPlayer.Player.channel.owner, EChatMode.SAY, Uconomy.Instance.Configuration.Instance.ImageURL, true);
            }
            
        }
    }
}
