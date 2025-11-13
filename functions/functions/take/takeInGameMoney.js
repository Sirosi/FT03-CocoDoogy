const {onCall} = require("firebase-functions/v2/https");
const {fnConfig} = require("../../config");
const admin = require("../../admin");

exports.takeInGameMoney = onCall(fnConfig, async (request) => {
  const uid = request.auth.uid;
  const amount = request.data.amount;

  if (!uid) {
    return {success: false, reason: "로그인이 필요합니다."};
  }

  if (typeof amount !== "number" || amount <= 0) {
    return {success: false, reason: "유효하지 않은 획득 금액입니다."};
  }

  try {
    const userPrivateRef = admin.firestore().collection("users").doc(uid).collection("private").doc("data");

    const userSnap = await userPrivateRef.get();
    if (!userSnap.exists) {
      return {success: false, reason: "사용자를 찾을 수 없습니다."};
    }

    const currentMoney = userSnap.data().inGameMoney || 0;

    await userPrivateRef.update({
      inGameMoney: admin.firestore.FieldValue.increment(amount),
    });

    return {
      success: true,
      uid,
      gained: amount,
      newTotal: currentMoney + amount,
      message: `${amount} 재화를 획득했습니다!`,
    };
  } catch (err) {
    console.error("gainInGameMoney error:", err);
    return {success: false, reason: "재화 획득 중 오류가 발생했습니다."};
  }
});
