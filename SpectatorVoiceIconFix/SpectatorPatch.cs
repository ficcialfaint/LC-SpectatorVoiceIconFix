using System.Collections.Generic;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace SpectatorVoiceIconFix;

[HarmonyPatch(typeof(HUDManager))]
public class SpectatorPatch
{
    private static readonly int Speaking = Animator.StringToHash("speaking");

    [HarmonyPatch("UpdateSpectateBoxSpeakerIcons")]
    [HarmonyPostfix]
    private static void FixAudioIcon()
    {
        var spectatingPlayerBoxes = Traverse.Create(HUDManager.Instance).Field("spectatingPlayerBoxes").GetValue<Dictionary<Animator,PlayerControllerB>>();
        
        foreach (var player in spectatingPlayerBoxes)
        {
            if (!player.Value.isPlayerControlled && !player.Value.isPlayerDead) 
                continue;
            if (player.Value != GameNetworkManager.Instance.localPlayerController) 
                continue;
            if (string.IsNullOrEmpty(StartOfRound.Instance.voiceChatModule.LocalPlayerName)) 
                continue;

            if (StartOfRound.Instance.voiceChatModule.FindPlayer(StartOfRound.Instance.voiceChatModule.LocalPlayerName) is not { } voicePlayerState) 
                continue;
            
            player.Key.SetBool(Speaking, voicePlayerState.IsSpeaking && voicePlayerState.Amplitude > 0.005f && !StartOfRound.Instance.voiceChatModule.IsMuted);
        }
    }
}