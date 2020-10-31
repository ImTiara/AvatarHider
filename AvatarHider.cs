using MelonLoader;
using System;
using System.Collections;
using UnhollowerBaseLib;
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
        public const string Version = "1.1.0";
        public const string DownloadLink = "https://github.com/ImTiara/AvatarHider/releases";
    }

    public class AvatarHider : MelonMod
    {

        private bool m_HideAvatars;
        private bool m_IgnoreFriends;
        private bool m_ExcludeShownAvatars;

        private float m_Distance;

        public override void OnApplicationStart()
        {
            MelonPrefs.RegisterCategory("AvatarHider", "Avatar Hider");

            MelonPrefs.RegisterBool("AvatarHider", "HideAvatars", false, "Hide Avatars");
            MelonPrefs.RegisterBool("AvatarHider", "IgnoreFriends", true, "Ignore Friends");
            MelonPrefs.RegisterBool("AvatarHider", "ExcludeShownAvatars", true, "Exclude Shown Avatars");
            MelonPrefs.RegisterFloat("AvatarHider", "HideDistance", 7f, "Distance (meters)");

            OnModSettingsApplied();

            MelonCoroutines.Start(AvatarScanner());
        }

        private IEnumerator AvatarScanner()
        {
            while (true)
            {
                if (m_HideAvatars && Manager.GetLocalVRCPlayer() != null)
                {
                    foreach (VRC.Player player in Manager.GetAllPlayers())
                    {
                        try
                        {
                            if (player == null || player.IsMe())
                                continue;

                            APIUser apiUser = player.prop_APIUser_0;
                            if (apiUser == null || (m_IgnoreFriends && apiUser.IsFriendsWith()) || (m_ExcludeShownAvatars && apiUser.IsShowingAvatar()))
                                continue;

                            GameObject avtrObject = player.GetAvatarObject();
                            if (avtrObject == null)
                                continue;

                            float dist = Vector3.Distance(Manager.GetLocalVRCPlayer().transform.position, avtrObject.transform.position);
                            bool isActive = avtrObject.active;

                            if (m_HideAvatars && isActive && dist > m_Distance)
                                avtrObject.SetActive(false);
                            else if (m_HideAvatars && !isActive && dist <= m_Distance)
                                avtrObject.SetActive(true);
                            else if (!m_HideAvatars && !isActive)
                                avtrObject.SetActive(true);
                        }
                        catch (Exception e)
                        {
                            MelonLogger.Log(ConsoleColor.Red, $"Failed to scan avatar: {e}");
                        }
                        yield return new WaitForSeconds(.2f);
                    }
                }
                yield return new WaitForEndOfFrame();
            }
        }

        public override void OnModSettingsApplied()
        {
            m_HideAvatars = MelonPrefs.GetBool("AvatarHider", "HideAvatars");
            m_IgnoreFriends = MelonPrefs.GetBool("AvatarHider", "IgnoreFriends");
            m_ExcludeShownAvatars = MelonPrefs.GetBool("AvatarHider", "ExcludeShownAvatars");
            m_Distance = MelonPrefs.GetFloat("AvatarHider", "HideDistance");

            UnHideAvatars();
        }

        private void UnHideAvatars()
        {
            try
            {
                foreach (VRC.Player player in Manager.GetAllPlayers())
                {
                    if (player == null || player.IsMe())
                        continue;

                    GameObject avtrObject = player.GetAvatarObject();
                    if (avtrObject == null || avtrObject.active)
                        continue;

                    avtrObject.SetActive(true);
                }
            }
            catch (Exception e)
            {
                MelonLogger.Log(ConsoleColor.Red, $"Failed to unhide avatar: {e}");
            }
        }
    }
}