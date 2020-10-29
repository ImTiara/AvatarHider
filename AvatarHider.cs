using MelonLoader;
using System;
using System.Collections;
using UnityEngine;
using VRC;
using VRC.Core;

namespace AvatarHider
{

    public static class BuildInfo
    {
        public const string Name = "AvatarHider";
        public const string Author = "ImTiara";
        public const string Company = null;
        public const string Version = "1.0.1";
        public const string DownloadLink = "https://github.com/ImTiara/AvatarHider/releases";
    }

    public class AvatarHider : MelonMod
    {

        private bool m_HideAvatars;
        private bool m_IgnoreFriends;
        private float m_Distance;

        public override void OnApplicationStart()
        {
            MelonPrefs.RegisterCategory("AvatarHider", "Avatar Hider");

            MelonPrefs.RegisterBool("AvatarHider", "HideAvatars", false, "Hide Avatars");
            MelonPrefs.RegisterBool("AvatarHider", "IgnoreFriends", true, "Ignore Friends");
            MelonPrefs.RegisterFloat("AvatarHider", "HideDistance", 7f, "Distance (meters)");

            OnModSettingsApplied();

            MelonCoroutines.Start(AvatarScanner());
        }

        private IEnumerator AvatarScanner()
        {
            while (true)
            {
                if (m_HideAvatars && GetLocalVRCPlayer() != null)
                {
                    foreach (VRC.Player player in PlayerManager.Method_Public_Static_ArrayOf_Player_0())
                    {
                        try
                        {
                            if (player == null || IsMe(player))
                                continue;

                            APIUser apiUser = player.prop_APIUser_0;
                            if (apiUser == null || (m_IgnoreFriends && IsFriendsWith(apiUser.id)))
                                continue;

                            GameObject avtrObject = GetAvatarObject(player);

                            if (avtrObject == null)
                                continue;

                            float dist = Vector3.Distance(GetLocalVRCPlayer().transform.position, avtrObject.transform.position);
                            bool isActive = avtrObject.active;

                            if (dist > m_Distance && isActive && m_HideAvatars)
                                avtrObject.SetActive(false);
                            else if (dist <= m_Distance && !isActive && m_HideAvatars)
                                avtrObject.SetActive(true);
                            else if (!m_HideAvatars && !isActive)
                                avtrObject.SetActive(true);
                        }
                        catch (Exception e)
                        {
                            MelonLogger.Log(ConsoleColor.Red, $"Failed to scan avatar: {e}");
                        }
                        yield return new WaitForSeconds(0.02f);
                    }
                }
                yield return new WaitForEndOfFrame();
            }
        }

        public override void OnModSettingsApplied()
        {
            m_HideAvatars = MelonPrefs.GetBool("AvatarHider", "HideAvatars");
            m_IgnoreFriends = MelonPrefs.GetBool("AvatarHider", "IgnoreFriends");
            m_Distance = MelonPrefs.GetFloat("AvatarHider", "HideDistance");

            UnHideAvatars();
        }

        private void UnHideAvatars()
        {
            try
            {
                foreach (VRC.Player player in PlayerManager.Method_Public_Static_ArrayOf_Player_0())
                {
                    if (player == null || IsMe(player)) continue;

                    GameObject avtrObject = GetAvatarObject(player);
                    if (avtrObject == null || avtrObject.active) continue;

                    avtrObject.SetActive(true);
                }
            }
            catch (Exception e)
            {
                MelonLogger.Log(ConsoleColor.Red, $"Failed to unhide avatar: {e}");
            }
        }

        private VRCPlayer GetLocalVRCPlayer() => VRCPlayer.field_Internal_Static_VRCPlayer_0;
        private GameObject GetAvatarObject(VRC.Player p) => p.prop_VRCPlayer_0.prop_VRCAvatarManager_0.prop_GameObject_0;
        private bool IsMe(VRC.Player p) => p.name == GetLocalVRCPlayer().name;
        private bool IsFriendsWith(string id) => APIUser.CurrentUser.friendIDs.Contains(id);
    }
}