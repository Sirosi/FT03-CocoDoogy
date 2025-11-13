const {onCall} = require("firebase-functions/v2/https");
const {fnConfig} = require("../../config");
const purchaseService = require("../../services/purchaseService");

exports.purchaseWithInGameMoney = onCall(fnConfig, async (request) => {
  const uid = request.auth.uid;
  const itemId = request.data.itemId;

  if (!uid) {
    return {success: false, reason: "로그인이 필요합니다."};
  }
  if (!itemId) {
    return {success: false, reason: "아이템 ID가 불일치하거나 존재하지 않습니다."};
  }

  try{
    const result = await purchaseService.processPurchase(uid, itemId, "inGameMoney");
    return result;
  } catch (err) {
    console.error("purchaseWithInGameMoney error:", err);
    return {success: false, reason: "구매 중 오류가 발생했습니다."};
  }
});
