using MelonLoader;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using VRC.Core;

namespace AvatarHider
{

    public static class BuildInfo
    {
        public const string Name = "AvatarHider";
        public const string Author = "ImTiara";
        public const string Company = null;
        public const string Version = "1.1.1";
        public const string DownloadLink = "https://github.com/ImTiara/AvatarHider/releases";
    }

    public class AvatarHider : MelonMod
    {

        private bool m_HideAvatars;
        private bool m_IgnoreFriends;
        private bool m_ExcludeShownAvatars;
        private bool m_DisableSpawnSound;

        private float m_Distance;

        public override void OnApplicationStart()
        {
            MelonPreferences.CreateCategory("AvatarHider", "Avatar Hider");
            MelonPreferences.CreateEntry("AvatarHider", "HideAvatars", false, "Hide Avatars");
            MelonPreferences.CreateEntry("AvatarHider", "IgnoreFriends", true, "Ignore Friends");
            MelonPreferences.CreateEntry("AvatarHider", "ExcludeShownAvatars", true, "Exclude Shown Avatars");
            MelonPreferences.CreateEntry("AvatarHider", "DisableSpawnSound", false, "Disable Spawn Sounds");
            MelonPreferences.CreateEntry("AvatarHider", "HideDistance", 7.0f, "Distance (meters)");

            OnPreferencesSaved();

            MelonCoroutines.Start(AvatarScanner());
        }

        private IEnumerator AvatarScanner()
        {
            while (true)
            {
                if (m_HideAvatars && Manager.GetLocalVRCPlayer() != null)
                {
                    foreach (VRC.Player player in Manager.GetAllPlayers().Where(p => p != null && !p.IsMe() && p.prop_APIUser_0 != null && !(m_IgnoreFriends && p.prop_APIUser_0.IsFriendsWith()) && !(m_ExcludeShownAvatars && p.prop_APIUser_0.IsShowingAvatar())))
                    {
                        try
                        {
                            APIUser apiUser = player.prop_APIUser_0;
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

                            if (m_DisableSpawnSound)
                                avtrObject.StopSpawnSounds();

                        }
                        catch (Exception e)
                        {
                            MelonLogger.Msg(ConsoleColor.Red, $"Failed to scan avatar: {e}");
                        }
                        yield return new WaitForSeconds(.19f);
                    }
                }
                yield return new WaitForSeconds(.5f);
            }
        }

        public override void OnPreferencesSaved()
        {
            m_HideAvatars = MelonPreferences.GetEntryValue<bool>("AvatarHider", "HideAvatars");
            m_IgnoreFriends = MelonPreferences.GetEntryValue<bool>("AvatarHider", "IgnoreFriends");
            m_ExcludeShownAvatars = MelonPreferences.GetEntryValue<bool>("AvatarHider", "ExcludeShownAvatars");
            m_DisableSpawnSound = MelonPreferences.GetEntryValue<bool>("AvatarHider", "DisableSpawnSound");
            m_Distance = MelonPreferences.GetEntryValue<float>("AvatarHider", "HideDistance");

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
                MelonLogger.Msg(ConsoleColor.Red, $"Failed to unhide avatar: {e}");
            }
        }
    }
}