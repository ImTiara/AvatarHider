using Il2CppSystem.Collections.Generic;
using UnityEngine;
using VRC;
using VRC.Core;

namespace AvatarHider
{
    public static class Manager
    {

        public static VRCPlayer GetLocalVRCPlayer() => VRCPlayer.field_Internal_Static_VRCPlayer_0;

        public static GameObject GetAvatarObject(this Player p) => p.prop_VRCPlayer_0.prop_VRCAvatarManager_0.prop_GameObject_0;

        public static bool IsShowingAvatar(this APIUser apiUser)
        {
            foreach (var playerModeration in GetModerationManager().field_Private_List_1_ApiPlayerModeration_0)
                if (playerModeration.moderationType == ApiPlayerModeration.ModerationType.ShowAvatar && playerModeration.targetUserId == apiUser.id)
                    return true;

            return false;
        }

        public static Player[] GetAllPlayers()
        {
            List<Player> list = GetPlayerManager()?.field_Private_List_1_Player_0;

            if (list == null)
                return new Player[0];

            lock (list)
                return list.ToArray();
        }

        public static bool IsMe(this Player p) => p.name == GetLocalVRCPlayer().name;

        public static bool IsFriendsWith(this APIUser apiUser) => APIUser.CurrentUser.friendIDs.Contains(apiUser.id);

        public static ObjectPublicObLi1ApSiLi1ApBoSiUnique GetModerationManager() => ObjectPublicObLi1ApSiLi1ApBoSiUnique.prop_ObjectPublicObLi1ApSiLi1ApBoSiUnique_0;

        public static PlayerManager GetPlayerManager() => PlayerManager.prop_PlayerManager_0;

    }
}
