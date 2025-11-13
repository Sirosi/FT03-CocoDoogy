const {onCall} = require("firebase-functions/v2/https");
const {fnConfig} = require("../../config");
const purchaseService = require("../../services/purchaseService");

exports.purchaseWithCashMoney = onCall(fnConfig, async (request) => {
  const uid = request.auth.uid;
  const itemId = request.data.itemId;
  const itemQuantity = Number(request.data.itemQuantity) || 1;

  if (!uid) {
    return {success: false, reason: "로그인이 필요합니다."};
  }
  if (!itemId) {
    return {success: false, reason: "아이템 ID가 불일치하거나 존재하지 않습니다."};
  }

  try{
    const result = await purchaseService.processPurchase(uid, itemId, itemQuantity, "cashMoney");
    return result;
    } catch (err) {
    console.error("purchaseWithCashMoney error:", err);
    return {success: false, reason: "구매 중 오류가 발생했습니다."};
  }
});
