using System;
using System.Collections.Generic;
using ModularIrcBot;
using System.Text;

namespace Poker
{
    class PokerGame
    {
        string WaitingOnUser = "";
        List<string> players = new List<string>();

        enum GameState 
        {
            NoGame,         //Waiting for players
            StartHand,      //Shuffle and deal cards
            Round1Betting,  //Preflop betting
            Deal3,          //Deal flop
            Round2Betting,  //Flop betting
            Deal4,          //Deal one card
            Round3Betting,  //Bet on that card
            Deal5,          //Deal last card
            Round4Betting   //Bet on last card
        }

        //State = -1 = NO
        GameState gs = GameState.NoGame;
        IrcClient irc = null;

        public PokerGame(IrcClient irc)
        {
            this.irc = irc;
        }

        public void InputUserMSG(MSG msg)
        {
            if (!players.Contains(msg.from) && msg.message == ".join poker")
            {
                players.Add(msg.from);
            }
            else if (players.Contains(msg.from) && msg.message == ".poker check")
            {
                if (msg.from != WaitingOnUser)
                    irc.SendMessage(msg.from, "Please wait till its your turn to act, preacting is not implemented yet.");
            }
            else if (players.Contains(msg.from) && msg.message == ".poker fold")
            {
                if (msg.from != WaitingOnUser)
                    irc.SendMessage(msg.from, "Please wait till its your turn to act, preacting is not implemented yet.");

            }
            else if (players.Contains(msg.from) && msg.message == ".poker leave")
            {

            }
            else if (players.Contains(msg.from) && msg.message.StartsWith(".poker raise "))
            {
                if (msg.from != WaitingOnUser)
                    irc.SendMessage(msg.from, "Please wait till its your turn to act, preacting is not implemented yet.");
            }
        }
    }

    public class Poker : Module
    {
        List<PokerGame> games = new List<PokerGame>();
        PokerGame game = null;

        public override void AddBindings()
        {
            game = new PokerGame(irc);
            irc.OnPMRecvd += new IrcClient.MSGRecvd(irc_OnPMRecvd);
        }

        void irc_OnPMRecvd(MSG msg)
        {
            
        }

        public override void RemoveBindings()
        {
            irc.OnPMRecvd -= new IrcClient.MSGRecvd(irc_OnPMRecvd);
        }

        public override string GetName()
        {
            return "Poker";
        }

        public override string GetHelp()
        {
            return "Not yet implemented";
        }
    }
}
