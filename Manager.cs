using Il2CppSystem.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRC;
using VRC.Core;
using VRC.Management;

namespace AvatarHider
{
    public static class Manager
    {

        public static void StopSpawnSounds(this GameObject avtrObject)
        {
            foreach (var audioSource in avtrObject.GetComponentsInChildren<AudioSource>().Where(audioSource => audioSource.isPlaying))
                audioSource.Stop();
        }

        public static VRCPlayer GetLocalVRCPlayer() => VRCPlayer.field_Internal_Static_VRCPlayer_0;

        public static GameObject GetAvatarObject(this Player p) => p.prop_VRCPlayer_0.prop_VRCAvatarManager_0.prop_GameObject_0;

        public static bool IsShowingAvatar(this APIUser apiUser)
        {
            foreach (var playerModeration in GetModerationManager().field_Private_List_1_ApiPlayerModeration_0)
                if (playerModeration.moderationType == ApiPlayerModeration.ModerationType.ShowAvatar && playerModeration.targetUserId == apiUser.id)
                    return true;

            return false;
        }

        public static System.Collections.Generic.List<Player> GetAllPlayers() => GetPlayerManager()?.prop_ArrayOf_Player_0.ToList();

        public static bool IsMe(this Player p) => p.name == GetLocalVRCPlayer().name;

        public static bool IsFriendsWith(this APIUser apiUser) => APIUser.CurrentUser.friendIDs.Contains(apiUser.id);

        public static ModerationManager GetModerationManager() => ModerationManager.prop_ModerationManager_0;

        public static PlayerManager GetPlayerManager() => PlayerManager.prop_PlayerManager_0;

    }
}
