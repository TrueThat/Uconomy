﻿using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using Rocket.Unturned.Commands;
using Rocket.API.Extensions;
using System;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;
using SDG.Unturned;

namespace fr34kyn01535.Uconomy
{
    public class CommandPay : IRocketCommand
    {
        public string Help
        {
            get { return "Pays a specific player money from your account"; }
        }

        public string Name
        {
            get { return "pay"; }
        }

        public AllowedCaller AllowedCaller
        {
            get
            {
                return AllowedCaller.Both;
            }
        }

        public string Syntax
        {
            get { return "<player> <amount>"; }
        }

        public List<string> Aliases
        {
            get { return new List<string>(); }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>() { "uconomy.pay" };
            }
        }

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            UnturnedPlayer CPlayer = UnturnedPlayer.FromName(caller.DisplayName);
            if (command.Length != 2)
            {
                //UnturnedChat.Say(caller, Uconomy.Instance.Translations.Instance.Translate("command_pay_invalid"), UnturnedChat.GetColorFromName(Uconomy.MessageColor, Color.green));
                ChatManager.serverSendMessage(Uconomy.Instance.Translate("command_pay_invalid").Replace('[', '<').Replace(']', '>'), Color.green, null, CPlayer.Player.channel.owner, EChatMode.SAY, Uconomy.Instance.Configuration.Instance.ImageURL, true);
                return;
            }

            string otherPlayer = command.GetCSteamIDParameter(0)?.ToString();
            UnturnedPlayer otherPlayerOnline = UnturnedPlayer.FromName(command[0]);
            if (otherPlayerOnline != null) otherPlayer = otherPlayerOnline.Id;

            if (otherPlayer !=null)
            {
                if (caller.Id == otherPlayer)
                {
                    //UnturnedChat.Say(caller, Uconomy.Instance.Translations.Instance.Translate("command_pay_error_pay_self"), UnturnedChat.GetColorFromName(Uconomy.MessageColor, Color.green));
                    ChatManager.serverSendMessage(Uconomy.Instance.Translate("command_pay_error_pay_self").Replace('[', '<').Replace(']', '>'), Color.green, null, CPlayer.Player.channel.owner, EChatMode.SAY, Uconomy.Instance.Configuration.Instance.ImageURL, true);
                    return;
                }

                decimal amount = 0;
                if (!Decimal.TryParse(command[1], out amount) || amount <= 0)
                {
                    //UnturnedChat.Say(caller, Uconomy.Instance.Translations.Instance.Translate("command_pay_error_invalid_amount"), UnturnedChat.GetColorFromName(Uconomy.MessageColor, Color.green));
                    ChatManager.serverSendMessage(Uconomy.Instance.Translate("command_pay_error_invalid_amount").Replace('[', '<').Replace(']', '>'), Color.green, null, CPlayer.Player.channel.owner, EChatMode.SAY, Uconomy.Instance.Configuration.Instance.ImageURL, true);
                    return;
                }

                if (caller is ConsolePlayer)
                {
                    Uconomy.Instance.Database.IncreaseBalance(otherPlayer, amount);
                    if(otherPlayerOnline != null)
                        //UnturnedChat.Say(otherPlayerOnline, Uconomy.Instance.Translations.Instance.Translate("command_pay_console", amount, Uconomy.Instance.Configuration.Instance.MoneyName), UnturnedChat.GetColorFromName(Uconomy.MessageColor, Color.green));
                        ChatManager.serverSendMessage(Uconomy.Instance.Translations.Instance.Translate("command_pay_console", amount, Uconomy.Instance.Configuration.Instance.MoneyName).Replace('[', '<').Replace(']', '>'), Color.green, null, otherPlayerOnline.Player.channel.owner, EChatMode.SAY, Uconomy.Instance.Configuration.Instance.ImageURL, true);
                }
                else
                {

                    decimal myBalance = Uconomy.Instance.Database.GetBalance(caller.Id);
                    if ((myBalance - amount) <= 0)
                    {
                        //UnturnedChat.Say(caller, Uconomy.Instance.Translations.Instance.Translate("command_pay_error_cant_afford"), UnturnedChat.GetColorFromName(Uconomy.MessageColor, Color.green));
                        ChatManager.serverSendMessage(Uconomy.Instance.Translations.Instance.Translate("command_pay_error_cant_afford").Replace('[', '<').Replace(']', '>'), Color.green, null, CPlayer.Player.channel.owner, EChatMode.SAY, Uconomy.Instance.Configuration.Instance.ImageURL, true);
                        return;
                    }
                    else
                    {
                        Uconomy.Instance.Database.IncreaseBalance(caller.Id, -amount);
                        if(otherPlayerOnline != null)
                            //UnturnedChat.Say(caller, Uconomy.Instance.Translations.Instance.Translate("command_pay_private", otherPlayerOnline.CharacterName, amount, Uconomy.Instance.Configuration.Instance.MoneyName), UnturnedChat.GetColorFromName(Uconomy.MessageColor, Color.green));
                            ChatManager.serverSendMessage(Uconomy.Instance.Translations.Instance.Translate("command_pay_private", otherPlayerOnline.CharacterName, amount, Uconomy.Instance.Configuration.Instance.MoneyName).Replace('[', '<').Replace(']', '>'), Color.green, null, CPlayer.Player.channel.owner, EChatMode.SAY, Uconomy.Instance.Configuration.Instance.ImageURL, true);
                        else
                            //UnturnedChat.Say(caller, Uconomy.Instance.Translations.Instance.Translate("command_pay_private", otherPlayer, amount, Uconomy.Instance.Configuration.Instance.MoneyName), UnturnedChat.GetColorFromName(Uconomy.MessageColor, Color.green));
                            ChatManager.serverSendMessage(Uconomy.Instance.Translations.Instance.Translate("command_pay_private", otherPlayer, amount, Uconomy.Instance.Configuration.Instance.MoneyName).Replace('[', '<').Replace(']', '>'), Color.green, null, CPlayer.Player.channel.owner, EChatMode.SAY, Uconomy.Instance.Configuration.Instance.ImageURL, true);

                        Uconomy.Instance.Database.IncreaseBalance(otherPlayer, amount);
                        if (otherPlayerOnline != null)
                            //UnturnedChat.Say(otherPlayerOnline.CSteamID, Uconomy.Instance.Translations.Instance.Translate("command_pay_other_private", amount, Uconomy.Instance.Configuration.Instance.MoneyName, caller.DisplayName).Replace('[', '<').Replace(']', '>'), UnturnedChat.GetColorFromName(Uconomy.MessageColor, Color.green));
                            ChatManager.serverSendMessage(Uconomy.Instance.Translations.Instance.Translate("command_pay_other_private", amount, Uconomy.Instance.Configuration.Instance.MoneyName, caller.DisplayName).Replace('[', '<').Replace(']', '>'), Color.green, null, otherPlayerOnline.Player.channel.owner, EChatMode.SAY, Uconomy.Instance.Configuration.Instance.ImageURL, true);
                        Uconomy.Instance.HasBeenPayed((UnturnedPlayer)caller, otherPlayer, amount);
                    }
                }

            }
            else
            {
                //UnturnedChat.Say(caller, Uconomy.Instance.Translations.Instance.Translate("command_pay_error_player_not_found"));
                ChatManager.serverSendMessage(Uconomy.Instance.Translations.Instance.Translate("command_pay_error_player_not_found").Replace('[', '<').Replace(']', '>'), Color.green, null, CPlayer.Player.channel.owner, EChatMode.SAY, Uconomy.Instance.Configuration.Instance.ImageURL, true);
            }
        }
    }
}
