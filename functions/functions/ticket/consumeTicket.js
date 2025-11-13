const {onCall} = require("firebase-functions/v2/https");
const {fnConfig} = require("../../config");
const admin = require("../../admin");

const maxTicket = 5;

exports.consumeTicket = onCall(fnConfig, async (request) => {
  const uid = request.auth.uid;
  if (!uid) return {success: false, reason: "로그인이 필요합니다."};

  const userPrivateRef = admin.firestore().collection("users").doc(uid).collection("private").doc("data");
  const now = Date.now();

  try {
    const result = await admin.firestore().runTransaction(async (t) => {
      const userDoc = await t.get(userPrivateRef);
      if (!userDoc.exists) throw new Error("사용자를 찾을 수 없습니다.");

      const data = userDoc.data();

      let currentTickets = data.gameTicket || 0;
      let bonusTickets = data.bonusTicket || 0;
      let newLastTime = data.lastTicketTime;

      if (currentTickets + bonusTickets <= 0) {
        throw new Error("티켓이 부족합니다.");
      }

      if (bonusTickets > 0) {
        bonusTickets--;
      } else {
        currentTickets--;
        if (data.gameTicket === maxTicket || !data.lastTicketTime) {
          newLastTime = now;
        }
      }

      t.update(userPrivateRef, {
        gameTicket: currentTickets,
        bonusTicket: bonusTickets,
        lastTicketTime: newLastTime,
      });

      return {
        success: true,
        gameTicket: currentTickets,
        bonusTicket: bonusTickets,
        serverTime: now,
        lastTicketTime: newLastTime,
      };
    });

    return result;
  } catch (error) {
    console.error("consumeTicket 트랜잭션 실패:", error.message);
    return {success: false, reason: error.message || "티켓 사용 중 오류 발생"};
  }
});
