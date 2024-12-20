﻿using AirHockey.Actors;
using AirHockey.Ambience.Effects;
using AirHockey.States;
namespace AirHockey.Handlers;

public class PowerupHandler : InputHandler
{
    public override bool Handle(InputContext context)
    {
        if (context.Player == null) return false;
        if (context.Player.Room.State is PausedState)
        {
            PassToNext(context);
            return false;
        }
        
        if (context.Inputs.GetValueOrDefault("powerup"))
        {
            context.Game.SoundEffects.AddEffect(new SoundEffect(SoundType.PowerupActivated, 0.2f));
            context.Player.UsePowerup();
        }

        PassToNext(context);
        return true;
    }
}