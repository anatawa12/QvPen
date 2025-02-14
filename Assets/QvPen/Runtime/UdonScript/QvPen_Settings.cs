﻿using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Data;

#pragma warning disable IDE0044
#pragma warning disable IDE0090, IDE1006

namespace QvPen.UdonScript
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class QvPen_Settings : UdonSharpBehaviour
    {
        [System.NonSerialized]
        public string version = string.Empty;

        [SerializeField]
        private TextAsset versionText;

        [SerializeField]
        private Text information;
        [SerializeField]
        private TextMeshPro informationTMP;
        [SerializeField]
        private TextMeshProUGUI informationTMPU;

        [SerializeField]
        private Transform pensParent;
        [SerializeField]
        private Transform erasersParent;

        [System.NonSerialized]
        public QvPen_PenManager[] penManagers = { };

        [System.NonSerialized]
        public QvPen_EraserManager[] eraserManagers = { };

        private void Start()
        {
            if (TryGetVersionFromPackageInfo(versionText, out var versionStr))
                version = versionStr;

#if !UNITY_EDITOR
            const string ureishi = nameof(ureishi);
            Log($"{nameof(QvPen)} {version} - {ureishi}");
#endif

            var infomationText =
                    $"<size=20></size>\n" +
                    $"<size=14>{version}</size>";

            if (information)
                information.text = infomationText;
            if (informationTMP)
                informationTMP.text = infomationText;
            if (informationTMPU)
                informationTMPU.text = infomationText;

            if (pensParent)
                penManagers = pensParent.GetComponentsInChildren<QvPen_PenManager>();
            if (erasersParent)
                eraserManagers = erasersParent.GetComponentsInChildren<QvPen_EraserManager>();
        }

        private bool TryGetVersionFromPackageInfo(TextAsset packageInfoTextAsset, out string version)
        {
            if (packageInfoTextAsset == null)
            {
                version = default;
                return false;
            }

            if (!VRCJson.TryDeserializeFromJson(packageInfoTextAsset.text, out var jsonToken))
            {
                version = default;
                return false;
            }

            var json = jsonToken.DataDictionary;

            if (!json.TryGetValue("version", TokenType.String, out var versionToken))
            {
                version = default;
                return false;
            }

            version = versionToken.String;
            return true;
        }

        #region Log

        private void Log(object o) => Debug.Log($"{logPrefix}{o}", this);
        private void Warning(object o) => Debug.LogWarning($"{logPrefix}{o}", this);
        private void Error(object o) => Debug.LogError($"{logPrefix}{o}", this);

        private readonly Color logColor = new Color(0xf2, 0x7d, 0x4a, 0xff) / 0xff;
        private string ColorBeginTag(Color c) => $"<color=\"#{ToHtmlStringRGB(c)}\">";
        private const string ColorEndTag = "</color>";

        private string _logPrefix;
        private string logPrefix
            => string.IsNullOrEmpty(_logPrefix)
                ? (_logPrefix = $"[{ColorBeginTag(logColor)}{nameof(QvPen)}.{nameof(QvPen.Udon)}.{nameof(QvPen_Settings)}{ColorEndTag}] ")
                : _logPrefix;

        private string ToHtmlStringRGB(Color c)
        {
            c *= 0xff;
            return $"{Mathf.RoundToInt(c.r):x2}{Mathf.RoundToInt(c.g):x2}{Mathf.RoundToInt(c.b):x2}";
        }

        #endregion
    }
}
