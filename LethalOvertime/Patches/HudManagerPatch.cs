using System.Collections;
using System.Linq;
using HarmonyLib;
using LethalOvertime;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

namespace ShipLoot.Patches
{
    [HarmonyPatch]
    internal class HudManagerPatcher
    {
        private static GameObject _totalCounter;
        private static TextMeshProUGUI _textMesh;
        private static float _displayTimeLeft;
        private const float DisplayTime = 5f;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(HUDManager), "PingScan_performed")]
        private static void Start(HUDManager __instance, InputAction.CallbackContext context)
        {
            if (!StartOfRound.Instance.inShipPhase && !GameNetworkManager.Instance.localPlayerController.isInHangarShipRoom)
                return;

            if (!_totalCounter)
                CopyValueCounter();
            var tempbonus = TimeOfDay.Instance.quotaFulfilled - TimeOfDay.Instance.profitQuota;
            float value = tempbonus <= 0 ? 0 : (tempbonus / 5 + 15 * TimeOfDay.Instance.daysUntilDeadline);
            _textMesh.text = "Potential overtime bonus:" + value;
            _displayTimeLeft = DisplayTime;

            if (!_totalCounter.activeSelf)
                GameNetworkManager.Instance.StartCoroutine(ShipLootCoroutine());
        }

        private static IEnumerator ShipLootCoroutine()
        {
            _totalCounter.SetActive(true);

            while (_displayTimeLeft > 0f)
            {
                float time = _displayTimeLeft;
                _displayTimeLeft = 0f;
                yield return new WaitForSeconds(time);
            }

            _totalCounter.SetActive(false);
        }
        private static void CopyValueCounter()
        {
            GameObject valueCounter = GameObject.Find("/Systems/UI/Canvas/IngamePlayerHUD/BottomMiddle/ValueCounter");
            if (!(bool)(UnityEngine.Object)valueCounter)
                LethalOvertimeModBase.Instance.mls.LogError("Failed to find ValueCounter object to copy!");

            _totalCounter = UnityEngine.Object.Instantiate<GameObject>(valueCounter.gameObject, valueCounter.transform.parent, false);
            Vector3 localPosition = _totalCounter.transform.localPosition;

            // Adjust the position higher up and more to the right
            float adjustedX = Mathf.Clamp(localPosition.x + LethalOvertimeModBase.AdjustScreenPositionXaxis.Value + 80f, 0f, Screen.width);
            float adjustedY = Mathf.Clamp(localPosition.y + LethalOvertimeModBase.AdjustScreenPositionYaxis.Value - 150f, 0f, Screen.height);

            _totalCounter.transform.localPosition = new Vector3(adjustedX, adjustedY, localPosition.z);

            _textMesh = _totalCounter.GetComponentInChildren<TextMeshProUGUI>();
            _textMesh.fontSizeMin = 5f;
            _textMesh.fontSize = LethalOvertimeModBase.FontSize.Value;
            _textMesh.ForceMeshUpdate();

            // Set the anchor and pivot of the text's RectTransform to the top right of the parent
            _textMesh.alignment = TextAlignmentOptions.TopRight;
            RectTransform textRectTransform = _textMesh.rectTransform;
            textRectTransform.anchorMin = new Vector2(1f, 1f); // Change the anchorMin to (1, 1) to position the child at the top right of the parent
            textRectTransform.anchorMax = new Vector2(1f, 1f); // Change the anchorMax to (1, 1) to position the child at the top right of the parent
            textRectTransform.pivot = new Vector2(1f, 1f); // Change the pivot to (1, 1) to position the child at the top right of the parent

            // Adjust local position for top right corner
            Vector3 textLocalPosition = textRectTransform.localPosition;
            textRectTransform.localPosition = new Vector3(textLocalPosition.x - 20f, textLocalPosition.y, textLocalPosition.z);
        }


    }
}
